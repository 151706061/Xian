using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Common.Specifications
{
    public class AndSpecification : CompositeSpecification
    {
        public AndSpecification()
        {
        }

        protected override TestResult InnerTest(object exp, object root)
        {
            foreach (ISpecification subSpec in this.SubSpecs)
            {
                TestResult r = subSpec.Test(exp);
                if (r.Fail)
                    return new TestResult(false, new TestResultReason(this.FailureMessage, r.Reasons));
            }
            return new TestResult(true);
        }
    }
}
