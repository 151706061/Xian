#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace ClearCanvas.Utilities.Manifest
{
    /// <summary>
    /// Input file generated by <see cref="ManifestInputGenerationApplication"/> for <see cref="ManifestGenerationApplication"/>.
    /// </summary>
    [XmlRoot("ManifestInput", Namespace = "http://www.clearcanvas.ca")]
    public class ManifestInput
    {
        #region Class definitions

        [XmlRoot("File")]
        public class InputFile
        {
            [XmlAttribute(AttributeName = "checksum", DataType = "boolean")]
            [DefaultValue(false)]
            public bool Checksum { get; set; }

            [XmlAttribute(AttributeName = "ignore", DataType = "boolean")]
            [DefaultValue(false)]
            public bool Ignore { get; set; }

            [XmlAttribute(AttributeName = "config", DataType = "boolean")]
            [DefaultValue(false)]
            public bool Config { get; set; }

            [XmlAttribute(AttributeName = "name", DataType = "string")]
            public string Name;
        }

        #endregion Class definitions

        #region Private Members

        private List<InputFile> _files;

        #endregion Private Members

        #region Public Properties

        [XmlArray("Files")]
        [XmlArrayItem("File")]
        public List<InputFile> Files
        {
            get
            {
                if (_files == null)
                    _files = new List<InputFile>();
                return _files;
            }
            set { _files = value; }
        }

        #endregion Public Properties

        #region Public Static Methods

        public static ManifestInput Deserialize(string filename)
        {
            XmlSerializer theSerializer = new XmlSerializer(typeof (ManifestInput));

            using (FileStream fs = new FileStream(filename, FileMode.Open))
            {
                ManifestInput input = (ManifestInput) theSerializer.Deserialize(fs);

                return input;
            }
        }

        public static void Serialize(string filename, ManifestInput input)
        {
            using (FileStream fs = new FileStream(filename, FileMode.CreateNew))
            {
                XmlSerializer theSerializer = new XmlSerializer(typeof (ManifestInput));

                XmlWriterSettings settings = new XmlWriterSettings
                                                 {
                                                     Indent = true,
                                                     IndentChars = "  ",
                                                     Encoding = Encoding.UTF8,
                                                 };

                XmlWriter writer = XmlWriter.Create(fs, settings);
                if (writer != null)
                    theSerializer.Serialize(writer, input);

                fs.Flush();
                fs.Close();
            }
        }

        #endregion
    }
}
