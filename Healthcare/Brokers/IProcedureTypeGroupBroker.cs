#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Healthcare.Brokers
{
    /// <summary>
    /// Defines the interface for a <see cref="ProcedureTypeGroup"/> broker
    /// </summary>
    public partial interface IProcedureTypeGroupBroker
    {
        /// <summary>
        /// Finds all <see cref="ProcedureTypeGroup"/> instances of the specified sub-class.
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="subClass"></param>
        /// <returns></returns>
        IList<ProcedureTypeGroup> Find(ProcedureTypeGroupSearchCriteria criteria, Type subClass);

		/// <summary>
		/// Finds all <see cref="ProcedureTypeGroup"/> instances of the specified sub-class.
		/// </summary>
		/// <param name="criteria"></param>
		/// <param name="subClass"></param>
		/// <param name="page"></param>
		/// <returns></returns>
		IList<ProcedureTypeGroup> Find(ProcedureTypeGroupSearchCriteria criteria, Type subClass, SearchResultPage page);
		
		/// <summary>
        /// Finds one instance of the specified subclass matching the specified criteria.
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="subClass"></param>
        /// <returns></returns>
        ProcedureTypeGroup FindOne(ProcedureTypeGroupSearchCriteria criteria, Type subClass);
    }
}
