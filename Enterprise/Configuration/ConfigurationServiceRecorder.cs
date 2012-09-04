#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.IO;
using System.Xml;
using ClearCanvas.Enterprise.Common.Configuration;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Enterprise.Configuration
{
	/// <summary>
	/// Records custom information about operations on <see cref="IConfigurationService"/>.
	/// </summary>
	public class ConfigurationServiceRecorder : ServiceOperationRecorderBase
	{
		/// <summary>
		/// Gets the category that should be assigned to the audit entries.
		/// </summary>
		protected override string Category
		{
			get { return "Configuration"; }
		}

		protected override bool GenerateMessage(ServiceOperationInvocationInfo info, out string message)
		{
			// don't bother logging failed attempts
			if (info.Exception != null)
			{
				message = null;
				return false;
			}

			var request = (ConfigurationDocumentRequestBase)info.Request;

			var sw = new StringWriter();
			using (var writer = new XmlTextWriter(sw))
			{
				writer.Formatting = Formatting.Indented;
				writer.WriteStartDocument();
				writer.WriteStartElement("action");
				writer.WriteAttributeString("type", info.OperationMethodInfo.Name);
				writer.WriteAttributeString("documentName", request.DocumentKey.DocumentName);
				writer.WriteAttributeString("documentVersion", request.DocumentKey.Version.ToString());
				writer.WriteAttributeString("documentUser", request.DocumentKey.User ?? "{application}");
				writer.WriteAttributeString("instanceKey", request.DocumentKey.InstanceKey ?? "{default}");
				writer.WriteEndElement();
				writer.WriteEndDocument();
			}

			message = sw.ToString();
			return true;
		}
	}
}
