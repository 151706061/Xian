#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Web.Common;

namespace ClearCanvas.Web.Services
{
    public class ApplicationServiceHost:ServiceHost
    {
        public ApplicationServiceHost(params Uri[] addresses)
        {
            InitializeDescription(typeof(ApplicationService), 
                new UriSchemeKeyedCollection(addresses));
        }

		public Uri MexHttpUrl { get; set; }

        protected override void InitializeRuntime()
        {
            PerformanceMonitor.Initialize();


			// Define the binding and set time-outs
            
            ServiceThrottlingBehavior throttling = new ServiceThrottlingBehavior();

        	//TODO (CR May 2010): we're limiting connections, but not applications
            int maximumSimultaneousConnections = ApplicationServiceSettings.Default.MaximumSimultaneousApplications;

            // because InstanceContextMode is PerSession
            // MaxConcurrentCalls = MaxConcurrentInstances = number of sessions
            throttling.MaxConcurrentSessions = throttling.MaxConcurrentInstances = maximumSimultaneousConnections;


            throttling.MaxConcurrentCalls = maximumSimultaneousConnections;
            Description.Behaviors.Add(throttling);

            NetTcpBinding netTcpBinding = new NetTcpBinding(SecurityMode.None, false);
			//Give the individual apps a chance to timeout on their own before we fault the channel.
			if (ApplicationServiceSettings.Default.InactivityTimeoutMinutes > 0)
				netTcpBinding.ReceiveTimeout = TimeSpan.FromMinutes(ApplicationServiceSettings.Default.InactivityTimeoutMinutes + 1);

            netTcpBinding.MaxBufferPoolSize = ApplicationServiceSettings.Default.MaxBufferPoolSize;
            netTcpBinding.MaxReceivedMessageSize = ApplicationServiceSettings.Default.MaxReceivedMessageSize;

			if (MexHttpUrl != null)
			{
				ServiceMetadataBehavior mexBehaviour = new ServiceMetadataBehavior();
				mexBehaviour.HttpGetEnabled = true;
				mexBehaviour.HttpGetUrl = MexHttpUrl;
				Description.Behaviors.Add(mexBehaviour);
			}

			LogSettings(netTcpBinding);

            // Add an endpoint for the given service contract
            ServiceEndpoint sep = AddServiceEndpoint(typeof(IApplicationService), netTcpBinding, "ApplicationServices");
			sep.Behaviors.Add(new SilverlightFaultBehavior()); // override the default fault handling behaviour which does not work for Silverlight app.
            
            base.InitializeRuntime();
        }

        private void LogSettings(NetTcpBinding binding)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Web viewer service settings:");
            sb.AppendLine(String.Format("\t{0}\t: {1}", "Maximum Simultaneous Applications", ApplicationServiceSettings.Default.MaximumSimultaneousApplications));
            sb.AppendLine(String.Format("\t{0}\t: {1}", "Inactivity Timeout", binding.ReceiveTimeout));
            sb.AppendLine(String.Format("\t{0}\t: {1}", "MaxBufferPoolSize", binding.MaxBufferPoolSize));
            sb.AppendLine(String.Format("\t{0}\t: {1}", "MaxReceivedMessageSize", binding.MaxReceivedMessageSize));
            Platform.Log(LogLevel.Info, sb.ToString());
        }
    }
}
