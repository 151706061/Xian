using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Core
{
    /// <summary>
    /// Provides a basic implementation of <see cref="ISubSelect{T}"/>.  See <see cref="SearchCriteria"/> for 
    /// usage of this class.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SubSelect<T> : SearchConditionBase, ISubSelect<T>
        where T: SearchCriteria
    {
        public SubSelect()
        {
        }

        public SubSelect(string name)
            : base(name)
        {
        }

        #region ISubSelect<T> Members

        public void Exists(T val)
        {
            SetCondition(SearchConditionTest.Exists, val);
        }

        public void NotExists(T val)
        {
            SetCondition(SearchConditionTest.NotExists, val);
        }

        #endregion
    }
}
