#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core.Imex;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Healthcare.Imex
{
    [ExtensionOf(typeof(XmlDataImexExtensionPoint))]
    [ImexDataClass("ProcedureTypeGroup")]
    public class ProcedureTypeGroupImex : XmlEntityImex<ProcedureTypeGroup, ProcedureTypeGroupImex.ProcedureTypeGroupData>
    {
        [DataContract]
        public class ProcedureTypeGroupData
        {
            [DataMember]
            public string Name;

            [DataMember]
            public string Class;

            [DataMember]
            public string Description;

            [DataMember]
            public List<ProcedureTypeData> ProcedureTypes;

        }

        [DataContract]
        public class ProcedureTypeData
        {
            [DataMember]
            public string Id;
        }

        #region Overrides

        protected override IList<ProcedureTypeGroup> GetItemsForExport(IReadContext context, int firstRow, int maxRows)
        {
            ProcedureTypeGroupSearchCriteria where = new ProcedureTypeGroupSearchCriteria();
            where.Name.SortAsc(0);
            return context.GetBroker<IProcedureTypeGroupBroker>().Find(where, new SearchResultPage(firstRow, maxRows));
        }

        protected override ProcedureTypeGroupData Export(ProcedureTypeGroup entity, IReadContext context)
        {
            ProcedureTypeGroupData data = new ProcedureTypeGroupData();
            data.Name = entity.Name;
            data.Class = entity.GetClass().FullName;
            data.Description = entity.Description;

            data.ProcedureTypes = CollectionUtils.Map<ProcedureType, ProcedureTypeData>(
                entity.ProcedureTypes,
                delegate(ProcedureType pt)
                {
                    ProcedureTypeData ptdata = new ProcedureTypeData();
                    ptdata.Id = pt.Id;
                    return ptdata;
                });

            return data;
        }

        protected override void Import(ProcedureTypeGroupData data, IUpdateContext context)
        {
            ProcedureTypeGroup group = LoadOrCreateProcedureTypeGroup(data.Name, data.Class, context);
            group.Description = data.Description;

            if (data.ProcedureTypes != null)
            {
                foreach (ProcedureTypeData s in data.ProcedureTypes)
                {
                    ProcedureTypeSearchCriteria where = new ProcedureTypeSearchCriteria();
                    where.Id.EqualTo(s.Id);
                    ProcedureType pt = CollectionUtils.FirstElement(context.GetBroker<IProcedureTypeBroker>().Find(where));
                    if (pt != null)
                        group.ProcedureTypes.Add(pt);
                }
            }
        }

        #endregion


        private ProcedureTypeGroup LoadOrCreateProcedureTypeGroup(string name, string className, IPersistenceContext context)
        {
            Type groupClass = ProcedureTypeGroup.GetSubClass(className, context);
            ProcedureTypeGroupSearchCriteria criteria = new ProcedureTypeGroupSearchCriteria();
            criteria.Name.EqualTo(name);

            ProcedureTypeGroup group = null;
            try
            {
                group = context.GetBroker<IProcedureTypeGroupBroker>().FindOne(criteria, groupClass);
            }
            catch (EntityNotFoundException)
            {
                group = (ProcedureTypeGroup)Activator.CreateInstance(groupClass);
                group.Name = name;

                context.Lock(group, DirtyState.New);
            }

            return group;
        }
    }
}
