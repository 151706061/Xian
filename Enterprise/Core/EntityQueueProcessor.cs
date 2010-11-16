﻿#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

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

		/// <summary>
		/// Called after the item processing transaction has committed.
		/// </summary>
		/// <remarks>
		/// This method is intended to allow subclasses to perform actions that should occur
		/// only after the transaction has been safely committed.
		/// Any exceptions thrown from this method will be logged and ignored.
		/// </remarks>
		/// <param name="item"></param>
		protected virtual void OnTransactionCommitted(TItem item)
		{
			// Do nothing by default
		}

		#region Override Methods

		protected override IList<TItem> GetNextBatch(int batchSize)
		{
			using (var scope = new PersistenceScope(PersistenceContextType.Read))
			{
				var items = GetNextEntityBatch(batchSize);

				scope.Complete();
				return items;
			}
		}

		protected override void ProcessItem(TItem item)
		{
			Exception error = null;
			using (var scope = new PersistenceScope(PersistenceContextType.Update))
			{
				var context = (IUpdateContext)PersistenceScope.CurrentContext;
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

			// exceptions thrown upon exiting the using block are intentionally not caught here,
			// allow them to be caught by the calling method

			// post-processing
			if(error == null)
			{
				AfterCommit(item);
			}
			else
			{
				UpdateQueueItemOnError(item, error);
			}
		}

		#endregion

		private void UpdateQueueItemOnError(TItem item, Exception error)
		{
			// use a new scope to mark the item as failed, because we don't want to commit any changes made in the outer scope
			using (var scope = new PersistenceScope(PersistenceContextType.Update, PersistenceScopeOption.RequiresNew))
			{
				var failContext = (IUpdateContext)PersistenceScope.CurrentContext;
				failContext.ChangeSetRecorder.OperationName = this.GetType().FullName;

				// bug #7191 : Reload the TItem in this scope;  using the existing item results in NHibernate throwing a lazy loading exception
				var itemForThisScope = failContext.Load<TItem>(item.GetRef(), EntityLoadFlags.None);

				// lock item into this context
				failContext.Lock(itemForThisScope);

				// failure callback
				OnItemFailed(itemForThisScope, error);

				// complete the transaction
				scope.Complete();
			}
		}

		private void AfterCommit(TItem item)
		{
			try
			{
				OnTransactionCommitted(item);
			}
			catch (Exception e)
			{
				// Swallow exception on purpose.  We don't care if the resources failed to get cleanup.
				ExceptionLogger.Log(this.GetType().FullName + ".ProcessItem", e);
			}
		}



	}
}
