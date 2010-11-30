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

using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Application.Services
{
	public class LocationAssembler
	{
		public LocationSummary CreateLocationSummary(Location location)
		{
			return new LocationSummary(
				location.GetRef(),
				location.Id,
				location.Name,
				new FacilityAssembler().CreateFacilitySummary(location.Facility),
				location.Building,
				location.Floor,
				location.PointOfCare,
				location.Deactivated);
		}

		public LocationDetail CreateLocationDetail(Location location)
		{
			return new LocationDetail(
				location.GetRef(),
				location.Id,
				location.Name,
				location.Description,
				new FacilityAssembler().CreateFacilitySummary(location.Facility),
				location.Building,
				location.Floor,
				location.PointOfCare,
				location.Deactivated);
		}

		public void UpdateLocation(LocationDetail detail, Location location, IPersistenceContext context)
		{
			location.Name = detail.Name;
			location.Id = detail.Id;
			location.Description = detail.Description;

			location.Facility = context.Load<Facility>(detail.Facility.FacilityRef, EntityLoadFlags.Proxy);
			location.Building = detail.Building;
			location.Floor = detail.Floor;
			location.PointOfCare = detail.PointOfCare;
			location.Deactivated = detail.Deactivated;
		}

	}
}
