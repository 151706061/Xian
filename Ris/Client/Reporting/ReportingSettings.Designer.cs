﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.832
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ClearCanvas.Ris.Client.Reporting {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "8.0.0.0")]
    internal sealed partial class ReportingSettings : global::System.Configuration.ApplicationSettingsBase {
        
        private static ReportingSettings defaultInstance = ((ReportingSettings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new ReportingSettings())));
        
        public static ReportingSettings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool AllowMultipleReportingWorkspaces {
            get {
                return ((bool)(this["AllowMultipleReportingWorkspaces"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool EnableTranscriptionWorkflow {
            get {
                return ((bool)(this["EnableTranscriptionWorkflow"]));
            }
        }
        
        /// <summary>
        /// A comma separated list of staff type codes, used to filter the Supervisor lookup field
        /// </summary>
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Configuration.SettingsDescriptionAttribute("A comma separated list of staff type codes, used to filter the Supervisor lookup " +
            "field")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("PRAD")]
        public string SupervisorLookupStaffTypeFilters {
            get {
                return ((string)(this["SupervisorLookupStaffTypeFilters"]));
            }
        }
    }
}
