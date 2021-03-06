#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Xml;
using System.Xml.Schema;
using ClearCanvas.Common;
using ClearCanvas.Common.Actions;
using ClearCanvas.Common.Specifications;
using ClearCanvas.Dicom.Utilities.Rules;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Rules.StudyDeleteAction
{
	[ExtensionOf(typeof(XmlActionCompilerOperatorExtensionPoint<ServerActionContext, ServerRuleTypeEnum>))]
	public class StudyDeleteActionOperator : ActionOperatorCompilerBase, IXmlActionCompilerOperator<ServerActionContext, ServerRuleTypeEnum>
    {
        public StudyDeleteActionOperator() :
            base("study-delete")
        {
        }

        #region IXmlActionCompilerOperator<ServerActionContext> Members

        public IActionItem<ServerActionContext> Compile(XmlElement xmlNode)
        {
            if (xmlNode.Attributes["time"] == null)
                throw new XmlActionCompilerException("Unexpected missing time attribute for study-delete action");
            if (xmlNode.Attributes["unit"] == null)
                throw new XmlActionCompilerException("Unexpected missing unit attribute for study-delete action");

            int time;
            if (false == int.TryParse(xmlNode.Attributes["time"].Value, out time))
                throw new XmlActionCompilerException("Unable to parse time value for study-delete rule");

            string xmlUnit = xmlNode.Attributes["unit"].Value;
            TimeUnit unit = (TimeUnit) Enum.Parse(typeof (TimeUnit), xmlUnit, true);
            // this will throw exception if the unit is not defined

            string refValue = xmlNode.Attributes["refValue"] != null ? xmlNode.Attributes["refValue"].Value : null;

            if (!String.IsNullOrEmpty(refValue))
            {
                if (xmlNode["expressionLanguage"] != null)
                {
                    string language = xmlNode["expressionLanguage"].Value;
                    Expression scheduledTime = CreateExpression(refValue, language);
                    return new StudyDeleteActionItem(time, unit, scheduledTime);
                }
                else
                {
                    Expression scheduledTime = CreateExpression(refValue);
                    return new StudyDeleteActionItem(time, unit, scheduledTime);
                }
            }
            else
            {
                return new StudyDeleteActionItem(time, unit);
            }
        }

		public XmlSchemaElement GetSchema(ServerRuleTypeEnum ruleType)
        {
			if (!ruleType.Equals(ServerRuleTypeEnum.StudyDelete))
				return null;

            XmlSchemaSimpleType timeUnitType = new XmlSchemaSimpleType();

            XmlSchemaSimpleTypeRestriction restriction = new XmlSchemaSimpleTypeRestriction();
            restriction.BaseTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");

            XmlSchemaEnumerationFacet enumeration = new XmlSchemaEnumerationFacet();
            enumeration.Value = "minutes";
            restriction.Facets.Add(enumeration);

            enumeration = new XmlSchemaEnumerationFacet();
            enumeration.Value = "hours";
            restriction.Facets.Add(enumeration);

            enumeration = new XmlSchemaEnumerationFacet();
            enumeration.Value = "days";
            restriction.Facets.Add(enumeration);

            enumeration = new XmlSchemaEnumerationFacet();
            enumeration.Value = "weeks";
            restriction.Facets.Add(enumeration);

            enumeration = new XmlSchemaEnumerationFacet();
            enumeration.Value = "months";
            restriction.Facets.Add(enumeration);

            enumeration = new XmlSchemaEnumerationFacet();
            enumeration.Value = "years";
            restriction.Facets.Add(enumeration);

            timeUnitType.Content = restriction;

            XmlSchemaSimpleType languageType = new XmlSchemaSimpleType();
            XmlSchemaSimpleTypeRestriction languageEnum = new XmlSchemaSimpleTypeRestriction();
            languageEnum.BaseTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");
            enumeration = new XmlSchemaEnumerationFacet();
            enumeration.Value = "dicom";
            languageEnum.Facets.Add(enumeration);
            languageType.Content = languageEnum;

            XmlSchemaComplexType type = new XmlSchemaComplexType();

            XmlSchemaAttribute attrib = new XmlSchemaAttribute();
            attrib.Name = "time";
            attrib.Use = XmlSchemaUse.Required;
            attrib.SchemaTypeName = new XmlQualifiedName("integer", "http://www.w3.org/2001/XMLSchema");
            type.Attributes.Add(attrib);

            attrib = new XmlSchemaAttribute();
            attrib.Name = "unit";
            attrib.Use = XmlSchemaUse.Required;
            attrib.SchemaType = timeUnitType;
            type.Attributes.Add(attrib);

            attrib = new XmlSchemaAttribute();
            attrib.Name = "refValue";
            attrib.Use = XmlSchemaUse.Optional;
            attrib.SchemaTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");
            type.Attributes.Add(attrib);

            attrib = new XmlSchemaAttribute();
            attrib.Name = "expressionLanguage";
            attrib.Use = XmlSchemaUse.Optional;
            attrib.SchemaType = languageType;
            type.Attributes.Add(attrib);

            XmlSchemaElement element = new XmlSchemaElement();
            element.Name = OperatorTag;
            element.SchemaType = type;


            return element;
        }

        #endregion
    }
}