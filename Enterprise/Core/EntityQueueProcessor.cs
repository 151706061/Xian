#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using ClearCanvas.Common.Shreds;

namespace ClearCanvas.Enterprise.Core
{
	/// <summary>
	/// A specialization of <see cref="QueueProcessor{TItem}"/> that is designed to process Entity
	/// </summary>
	/// <typeparam name="TItem"></typeparam>
	public abstract class EntityQueueProcessor<TItem> : QueueProcessor<TItem>
		where TItem : Entity
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="batchSize">Max number of items to pull off queue for processing.</param>
		/// <param name="sleepTime"></param>
		protected EntityQueueProcessor(int batchSize, TimeSpan sleepTime)
			: base(batchSize, sleepTime)
		{
		}

		/// <summary>
		/// Gets the next batch of entities from the queue.
		/// </summary>
		/// <remarks>
		/// Subclasses can assume that a read-scope has been established.
		/// </remarks>
		/// <param name="batchSize"></param>
		/// <returns></returns>
		protected abstract IList<TItem> GetNextEntityBatch(int batchSize);

		/// <summary>
		/// Called to act on a queue item.
		/// </summary>
		/// <remarks>
		/// This method is intended to take actions based on the item, but must not modify the item.  If
		/// the item cannot be fully acted on, an exception should be thrown.
		/// </remarks>
		/// <param name="item"></param>
		protected abstract void ActOnItem(TItem item);

		/// <summary>
		/// Called when <see cref="ActOnItem"/> succeeds.
		/// </summary>
		/// <remarks>
		/// This method is intended to update the item to indicate that the actions succeeded.
		/// Subclasses can assume that an update context has been established.
		/// </remarks>
		/// <param name="item"></param>
		protected abstract void OnItemSucceeded(TItem item);

		/// <summary>
		/// Called when <see cref="ActOnItem"/> throws an exception.
		/// </summary>
		/// <remarks>
		/// This method is intended to update the time to indicate that processing failed, with the 
		/// specified error.  Subclasses can assume that an update context has been established.
		/// </remarks>
		/// <param name="item"></param>
		/// <param name="error"></param>
		protected abstract void OnItemFailed(TItem item, Exception error);

		#region Override Methods

		protected override IList<TItem> GetNextBatch(int batchSize)
		{
			using (PersistenceScope scope = new PersistenceScope(PersistenceContextType.Read))
			{
				IList<TItem> items = GetNextEntityBatch(batchSize);

				scope.Complete();
				return items;
			}
		}

		protected override void ProcessItem(TItem item)
		{
			Exception error = null;
			using (PersistenceScope scope = new PersistenceScope(PersistenceContextType.Update))
			{
				IUpdateContext context = (IUpdateContext)PersistenceScope.CurrentContext;
				context.ChangeSetRecorder.OperationName = this.GetType().FullName;

				// need to lock the item in context, to allow loading of extended properties collection by subclass
				context.Lock(item);

				try
				{
					// take action base on item
					ActOnItem(item);

					// ensure that the commit will ultimately succeed
					context.SynchState();

					// success callback
					OnItemSucceeded(item);

					// complete the transaction
					scope.Complete();
				}
				catch (Exception e)
				{
					// one of the actions failed
					ExceptionLogger.Log(this.GetType().FullName + ".ProcessItem", e);
					error = e;
				}
			}

			if (error != null)
			{
				// use a new scope to mark the item as failed
				using (PersistenceScope scope = new PersistenceScope(PersistenceContextType.Update, PersistenceScopeOption.RequiresNew))
				{
					IUpdateContext failContext = (IUpdateContext)PersistenceScope.CurrentContext;
					failContext.ChangeSetRecorder.OperationName = this.GetType().FullName;
					
					// Reload the TItem in this scope;  using the existing item results in NHibernate throwing a lazy loading exception for 
					// some TItem.  See: ticket #7191 
					var itemForThisScope = failContext.Load<TItem>(item.GetRef());

					// lock item into this context
					failContext.Lock(itemForThisScope);

					// failure callback
					OnItemFailed(itemForThisScope, error);

					// complete the transaction
					scope.Complete();
				}
			}
		}

		#endregion

	}
}
