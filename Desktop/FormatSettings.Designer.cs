﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.3634
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ClearCanvas.Desktop {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "9.0.0.0")]
    internal sealed partial class FormatSettings : global::System.Configuration.ApplicationSettingsBase {
        
        private static FormatSettings defaultInstance = ((FormatSettings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new FormatSettings())));
        
        public static FormatSettings Default {
            get {
                return defaultInstance;
            }
        }
        
        /// <summary>
        /// Date format string
        /// </summary>
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Configuration.SettingsDescriptionAttribute("Date format string")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("dd-MMM-yyyy")]
        public string DateFormat {
            get {
                return ((string)(this["DateFormat"]));
            }
            set {
                this["DateFormat"] = value;
            }
        }
        
        /// <summary>
        /// Time format string (use hh:mm tt for non-military)
        /// </summary>
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Configuration.SettingsDescriptionAttribute("Time format string (use hh:mm tt for non-military)")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("HH:mm")]
        public string TimeFormat {
            get {
                return ((string)(this["TimeFormat"]));
            }
            set {
                this["TimeFormat"] = value;
            }
        }
        
        /// <summary>
        /// Date and time format string (use hh:mm tt for non-military)
        /// </summary>
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Configuration.SettingsDescriptionAttribute("Date and time format string (use hh:mm tt for non-military)")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string DateTimeFormat {
            get {
                return ((string)(this["DateTimeFormat"]));
            }
            set {
                this["DateTimeFormat"] = value;
            }
        }
        
        /// <summary>
        /// Threshold for usage of relative datetime terms. A setting of 0 turns off relative terms.
        /// </summary>
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Configuration.SettingsDescriptionAttribute("Threshold for usage of relative datetime terms. A setting of 0 turns off relative" +
            " terms.")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1")]
        public int DescriptiveDateThresholdInDays {
            get {
                return ((int)(this["DescriptiveDateThresholdInDays"]));
            }
            set {
                this["DescriptiveDateThresholdInDays"] = value;
            }
        }
        
        /// <summary>
        /// Specifies whether descriptive date formatting (e.g. &quot;today&quot;, &quot;2 days ago&quot;, etc) is enabled.
        /// </summary>
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Configuration.SettingsDescriptionAttribute("Specifies whether descriptive date formatting (e.g. \"today\", \"2 days ago\", etc) i" +
            "s enabled.")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool DescriptiveFormattingEnabled {
            get {
                return ((bool)(this["DescriptiveFormattingEnabled"]));
            }
            set {
                this["DescriptiveFormattingEnabled"] = value;
            }
        }
    }
}
