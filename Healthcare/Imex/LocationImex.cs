﻿#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Core.Imex;
using ClearCanvas.Healthcare.Brokers;

namespace ClearCanvas.Healthcare.Imex
{
	[ExtensionOf(typeof(XmlDataImexExtensionPoint))]
	[ImexDataClass("Location")]
	public class LocationImex : XmlEntityImex<Location, LocationImex.LocationData>
	{
		[DataContract]
		public class LocationData : ReferenceEntityDataBase
		{
			[DataMember]
			public string Id;

			[DataMember]
			public string Name;

			[DataMember]
			public string Description;

			[DataMember]
			public string FacilityCode;

			[DataMember]
			public string Building;

			[DataMember]
			public string Floor;

			[DataMember]
			public string PointOfCare;
		}

		#region Overrides

		protected override IList<Location> GetItemsForExport(IReadContext context, int firstRow, int maxRows)
		{
			var where = new LocationSearchCriteria();
			where.Id.SortAsc(0);

			return context.GetBroker<ILocationBroker>().Find(where, new SearchResultPage(firstRow, maxRows));
		}

		protected override LocationData Export(Location entity, IReadContext context)
		{
			var data = new LocationData
						{
							Deactivated = entity.Deactivated,
							Id = entity.Id,
							Name = entity.Name,
							Description = entity.Description,
							FacilityCode = entity.Facility.Code,
							Building = entity.Building,
							Floor = entity.Floor,
							PointOfCare = entity.PointOfCare
						};

			return data;
		}

		protected override void Import(LocationData data, IUpdateContext context)
		{
			var facilityCriteria = new FacilitySearchCriteria();
			facilityCriteria.Code.EqualTo(data.FacilityCode);
			var facility = context.GetBroker<IFacilityBroker>().FindOne(facilityCriteria);

			var location = LoadOrCreateLocation(data.Id, data.Name, facility, context);
			location.Deactivated = data.Deactivated;
			location.Description = data.Description;
			location.Building = data.Building;
			location.Floor = data.Floor;
			location.PointOfCare = data.PointOfCare;
		}

		#endregion

		private Location LoadOrCreateLocation(string id, string name, Facility facility, IPersistenceContext context)
		{
			Location l;
			try
			{
				// see if already exists in db
				var where = new LocationSearchCriteria();
				where.Id.EqualTo(id);
				l = context.GetBroker<ILocationBroker>().FindOne(where);
			}
			catch (EntityNotFoundException)
			{
				// create it
				l = new Location(id, name, null, facility, null, null, null);
				context.Lock(l, DirtyState.New);
			}

			return l;
		}
	}
}
