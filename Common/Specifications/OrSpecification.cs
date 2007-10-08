using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Common.Specifications
{
    public class OrSpecification : CompositeSpecification
    {
        public OrSpecification()
        {

        }

        protected override TestResult InnerTest(object exp, object root)
        {
            TestResult r;

            foreach (ISpecification subSpec in this.SubSpecs)
            {
                r = subSpec.Test(exp);
                if (r.Success)
                    return new TestResult(true);
            }

            // note that we can only return the immediate reason - there is no sensible way to return sub-reasons 
            return new TestResult(false, new TestResultReason(this.FailureMessage));
        }
    }
}
