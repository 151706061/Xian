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

using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common.BrowsePatientData;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client
{
	public class OrderListTable : Table<OrderListItem>
	{
		public OrderListTable()
			: base(3)
		{
			this.Columns.Add(new DateTableColumn<OrderListItem>(SR.ColumnCreatedOn, order => order.EnteredTime, 0.5f));
			this.Columns.Add(new DateTimeTableColumn<OrderListItem>(SR.ColumnScheduledFor, order => order.OrderScheduledStartTime, 0.75f));
			this.Columns.Add(new TableColumn<OrderListItem, string>(SR.ColumnImagingService, order => order.DiagnosticService.Name, 1.5f));

			this.Columns.Add(new TableColumn<OrderListItem, string>(
				SR.ColumnStatus,
				order => order.OrderStatus.Code == "SC" && order.OrderScheduledStartTime == null
					? SR.MessageUnscheduled
					: order.OrderStatus.Value,
				0.5f));

			this.Columns.Add(new TableColumn<OrderListItem, string>(
				SR.ColumnMoreInfo,
				order => string.Format(SR.FormatMoreInfo,
					AccessionFormat.Format(order.AccessionNumber),
					PersonNameFormat.Format(order.OrderingPractitioner.Name),
					order.OrderingFacility.Code),
				1));

			this.Columns.Add(new TableColumn<OrderListItem, string>(SR.ColumnIndication, order => string.Format(SR.FormatIndication, order.ReasonForStudy), 2));
		}
	}
}
