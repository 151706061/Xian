#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

// This file is auto-generated by the ClearCanvas.Model.SqlServer2005.CodeGenerator project.

namespace ClearCanvas.ImageServer.Model
{
    using System;
    using System.Collections.Generic;
    using ClearCanvas.ImageServer.Model.EntityBrokers;
    using ClearCanvas.ImageServer.Enterprise;
    using System.Reflection;

[Serializable]
public partial class StudyStatusEnum : ServerEnum
{
      #region Private Static Members
      private static readonly StudyStatusEnum _Online = GetEnum("Online");
      private static readonly StudyStatusEnum _OnlineLossless = GetEnum("OnlineLossless");
      private static readonly StudyStatusEnum _OnlineLossy = GetEnum("OnlineLossy");
      private static readonly StudyStatusEnum _Nearline = GetEnum("Nearline");
      #endregion

      #region Public Static Properties
      /// <summary>
      /// Study is online
      /// </summary>
      public static StudyStatusEnum Online
      {
          get { return _Online; }
      }
      /// <summary>
      /// Study is online and lossless compressed
      /// </summary>
      public static StudyStatusEnum OnlineLossless
      {
          get { return _OnlineLossless; }
      }
      /// <summary>
      /// Study is online and lossy compressed
      /// </summary>
      public static StudyStatusEnum OnlineLossy
      {
          get { return _OnlineLossy; }
      }
      /// <summary>
      /// The study is nearline (in an automated library)
      /// </summary>
      public static StudyStatusEnum Nearline
      {
          get { return _Nearline; }
      }

      #endregion

      #region Constructors
      public StudyStatusEnum():base("StudyStatusEnum")
      {}
      #endregion
      #region Public Members
      public override void SetEnum(short val)
      {
          ServerEnumHelper<StudyStatusEnum, IStudyStatusEnumBroker>.SetEnum(this, val);
      }
      static public List<StudyStatusEnum> GetAll()
      {
          return ServerEnumHelper<StudyStatusEnum, IStudyStatusEnumBroker>.GetAll();
      }
      static public StudyStatusEnum GetEnum(string lookup)
      {
          return ServerEnumHelper<StudyStatusEnum, IStudyStatusEnumBroker>.GetEnum(lookup);
      }
      #endregion
}
}
