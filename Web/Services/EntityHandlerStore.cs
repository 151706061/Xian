#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;

namespace ClearCanvas.Web.Services
{
	public class EntityHandlerStore
	{
		private readonly object _syncLock = new object();
		private readonly Dictionary<Guid, IEntityHandler> _handlers;

		internal EntityHandlerStore()
		{
			_handlers = new Dictionary<Guid, IEntityHandler>();
		}

		public void Add(IEntityHandler handler)
		{
			lock (_syncLock)
				_handlers.Add(handler.Identifier, handler);
		}

		public void Remove(IEntityHandler handler)
		{
			lock (_syncLock)
				_handlers.Remove(handler.Identifier);
		}

		public IEntityHandler this[Guid handlerId]
		{
			get
			{
				lock(_syncLock)
				{
					IEntityHandler handler;
					if (!_handlers.TryGetValue(handlerId, out handler))
						return null;

					return handler;
				}
			}
		}
	}
}
