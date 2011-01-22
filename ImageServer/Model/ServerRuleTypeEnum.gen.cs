#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0//

#endregion

// This file is auto-generated by the ClearCanvas.Model.SqlServer.CodeGenerator project.

namespace ClearCanvas.ImageServer.Model
{
    using System;
    using System.Collections.Generic;
    using ClearCanvas.ImageServer.Model.EntityBrokers;
    using ClearCanvas.ImageServer.Enterprise;
    using System.Reflection;

[Serializable]
public partial class ServerRuleTypeEnum : ServerEnum
{
      #region Private Static Members
      private static readonly ServerRuleTypeEnum _AutoRoute = GetEnum("AutoRoute");
      private static readonly ServerRuleTypeEnum _StudyDelete = GetEnum("StudyDelete");
      private static readonly ServerRuleTypeEnum _Tier1Retention = GetEnum("Tier1Retention");
      private static readonly ServerRuleTypeEnum _OnlineRetention = GetEnum("OnlineRetention");
      private static readonly ServerRuleTypeEnum _StudyCompress = GetEnum("StudyCompress");
      private static readonly ServerRuleTypeEnum _SopCompress = GetEnum("SopCompress");
      #endregion

      #region Public Static Properties
      /// <summary>
      /// A DICOM auto-routing rule
      /// </summary>
      public static ServerRuleTypeEnum AutoRoute
      {
          get { return _AutoRoute; }
      }
      /// <summary>
      /// A rule to specify when to delete a study
      /// </summary>
      public static ServerRuleTypeEnum StudyDelete
      {
          get { return _StudyDelete; }
      }
      /// <summary>
      /// A rule to specify how long a study will be retained on Tier1
      /// </summary>
      public static ServerRuleTypeEnum Tier1Retention
      {
          get { return _Tier1Retention; }
      }
      /// <summary>
      /// A rule to specify how long a study will be retained online
      /// </summary>
      public static ServerRuleTypeEnum OnlineRetention
      {
          get { return _OnlineRetention; }
      }
      /// <summary>
      /// A rule to specify when a study should be compressed
      /// </summary>
      public static ServerRuleTypeEnum StudyCompress
      {
          get { return _StudyCompress; }
      }
      /// <summary>
      /// A rule to specify when a SOP Instance should be compressed (during initial processing)
      /// </summary>
      public static ServerRuleTypeEnum SopCompress
      {
          get { return _SopCompress; }
      }

      #endregion

      #region Constructors
      public ServerRuleTypeEnum():base("ServerRuleTypeEnum")
      {}
      #endregion
      #region Public Members
      public override void SetEnum(short val)
      {
          ServerEnumHelper<ServerRuleTypeEnum, IServerRuleTypeEnumBroker>.SetEnum(this, val);
      }
      static public List<ServerRuleTypeEnum> GetAll()
      {
          return ServerEnumHelper<ServerRuleTypeEnum, IServerRuleTypeEnumBroker>.GetAll();
      }
      static public ServerRuleTypeEnum GetEnum(string lookup)
      {
          return ServerEnumHelper<ServerRuleTypeEnum, IServerRuleTypeEnumBroker>.GetEnum(lookup);
      }
      #endregion
}
}
