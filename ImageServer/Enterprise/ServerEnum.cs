#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;

namespace ClearCanvas.ImageServer.Enterprise
{
    /// <summary>
    /// A specialized <see cref="ServerEntity"/> that represents an enumerated value.
    /// </summary>
    [Serializable]
    public abstract class ServerEnum : ServerEntity
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">The name of the <see cref="ServerEntity"/>.</param>
        public ServerEnum(String name)
            : base(name)
        {
        }

        #endregion

        #region Private Members

        private short _enumValue;
        private string _lookup;
        private string _description;
        private string _longDescription;

        #endregion

        #region Public Properties

        /// <summary>
        /// The enumerated value itself.
        /// </summary>
		[EntityFieldDatabaseMappingAttribute(TableName = "", ColumnName = "Enum")]
		public short Enum
        {
            get { return _enumValue; }
            set { _enumValue = value; }
        }

        /// <summary>
        /// A lookup string.
        /// </summary>
		[EntityFieldDatabaseMappingAttribute(TableName = "", ColumnName = "Lookup")]
        public string Lookup
        {
            get { return _lookup; }
            set { _lookup = value; }
        }

        /// <summary>
        /// A short description of the enumerated value.
        /// </summary>
		[EntityFieldDatabaseMappingAttribute(TableName = "", ColumnName = "Description")]
		public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        /// <summary>
        /// A long description of the enumerated value.
        /// </summary>
		[EntityFieldDatabaseMappingAttribute(TableName = "", ColumnName = "LongDescription")]
		public string LongDescription
        {
            get { return _longDescription; }
            set { _longDescription = value; }
        }

        #endregion

        #region Public Abstract Methods

        public abstract void SetEnum(short val);

        #endregion

        #region Public Overrides

        public override int GetHashCode()
        {
            return Enum;
        }

        public override bool Equals(object obj)
        {
            ServerEnum e = obj as ServerEnum;
            if (e == null)
                return false;

            // Must be in the inheritance hierarchy of each other to be equal
            // eg, Status enum can't be equal to Type enum.
            if (GetType().IsAssignableFrom(obj.GetType()) ||
                obj.GetType().IsAssignableFrom(GetType()))
            {
                return e.Enum == Enum;
            }
            else
                return false;
        }

		public override string ToString()
		{
			return Description;
		}
        #endregion

        #region Operators

        /// <summary>
        /// Equality operator.
        /// </summary>
        public static bool operator ==(ServerEnum t1, ServerEnum t2)
        {
            if ((object) t1 == null && (object) t2 == null)
                return true;
            if ((object) t1 == null || (object) t2 == null)
                return false;
            return t1.Equals(t2);
        }

        /// <summary>
        /// Inequality operator.
        /// </summary>
        public static bool operator !=(ServerEnum t1, ServerEnum t2)
        {
            if ((object) t1 == null && (object) t2 == null)
                return false;
            if ((object) t1 == null || (object) t2 == null)
                return true;
            return !t1.Equals(t2);
        }

        #endregion
    }
}
