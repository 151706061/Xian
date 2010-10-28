#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare
{
	[ExtensionPoint]
	public class NoteboxExtensionPoint : ExtensionPoint<INotebox>
	{
	}

	/// <summary>
	/// Defines an interface to a notebox.
	/// </summary>
	public interface INotebox
	{
		/// <summary>
		/// Queries the notebox for its contents.
		/// </summary>
		/// <param name="nqc"></param>
		/// <returns></returns>
		IList GetItems(INoteboxQueryContext nqc);

		/// <summary>
		/// Queries the notebox for a count of its contents.
		/// </summary>
		/// <param name="nqc"></param>
		/// <returns></returns>
		int GetItemCount(INoteboxQueryContext nqc);
	}

	/// <summary>
	/// Defines an interface that provides a <see cref="Worklist"/> with information about the context
	/// in which it is executing.
	/// </summary>
	public interface INoteboxQueryContext
	{
		/// <summary>
		/// Gets the staff on whose behalf the notebox query is executing.
		/// </summary>
		Staff Staff { get; }

		/// <summary>
		/// For group-based noteboxes, gets the group for which the notebox query is executing.
		/// </summary>
		StaffGroup StaffGroup { get; }

		/// <summary>
		/// Gets the <see cref="SearchResultPage"/> that specifies which page of the notebox is requested.
		/// </summary>
		SearchResultPage Page { get; }

		/// <summary>
		/// Obtains an instance of the specified broker.
		/// </summary>
		/// <typeparam name="TBrokerInterface"></typeparam>
		/// <returns></returns>
		TBrokerInterface GetBroker<TBrokerInterface>() where TBrokerInterface : IPersistenceBroker;
	}
}
