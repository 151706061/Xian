using System.Collections;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare.Brokers;

namespace ClearCanvas.Healthcare {


    /// <summary>
    /// RegistrationCompletedProtocolWorklist entity
    /// </summary>
    [ExtensionOf(typeof(WorklistExtensionPoint), Name = "RegistrationCompletedProtocolWorklist")]
    public partial class RegistrationCompletedProtocolWorklist : Worklist
	{
	
		/// <summary>
		/// This method is called from the constructor.  Use this method to implement any custom
		/// object initialization.
		/// </summary>
		private void CustomInitialize()
		{
		}

        public override IList GetWorklist(Staff currentUserStaff, IPersistenceContext context)
        {
            return (IList) GetBroker<IRegistrationWorklistBroker>(context).GetCompletedProtocolWorklist(this);
        }

        public override int GetWorklistCount(Staff currentUserStaff, IPersistenceContext context)
        {
            return GetBroker<IRegistrationWorklistBroker>(context).GetCompletedProtocolWorklistCount(this);
        }

        public override string NameSuffix
        {
            get { return " - Completed Protocol"; }
        }

        #region Object overrides
		
		public override bool Equals(object that)
		{
			// TODO: implement a test for business-key equality
			return base.Equals(that);
		}
		
		public override int GetHashCode()
		{
			// TODO: implement a hash-code based on the business-key used in the Equals() method
			return base.GetHashCode();
		}
		
		#endregion
	}
}