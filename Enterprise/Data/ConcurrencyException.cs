using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Data
{
    /// <summary>
    /// Indicates that an entity's version has changed, and does not match the requested version
    /// </summary>
    public class ConcurrencyException : PersistenceException
    {
        public ConcurrencyException(Exception inner)
            : base(SR.ExceptionConcurrency, inner)
        {
        }
    }
}
