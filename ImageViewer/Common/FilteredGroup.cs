﻿#region License

// Copyright (c) 2009, ClearCanvas Inc.
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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using ClearCanvas.Common;
using ClearCanvas.Common.Specifications;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Common
{
	internal class SimpleSpecification<T> : ISpecification where T : class
	{
		private readonly Predicate<T> _test;

		public SimpleSpecification(Predicate<T> test)
		{
			Platform.CheckForNullReference(test, "test");
			_test = test;
		}

		#region ISpecification Members

		public TestResult Test(object obj)
		{
			if (obj is T && _test(obj as T))
				return new TestResult(true);
			else
				return new TestResult(false);
		}

		#endregion
	}

	internal class SimpleSpecification : SimpleSpecification<object>
	{
		public SimpleSpecification(Predicate<object> test)
			: base(test)
		{
		}
	}

	public class FilteredGroupList<T> : ObservableList<FilteredGroup<T>> where T : class
	{
		public FilteredGroupList()
		{
		}
	}

	public class RootFilteredGroup<T> : FilteredGroup<T> where T : class
	{
		public RootFilteredGroup()
			: this("Root", "All Items")
		{
		}

		public RootFilteredGroup(string name, string label)
			: this(name, label, ReturnTrue)
		{
		}

		public RootFilteredGroup(string name, string label, Predicate<T> test)
			: this(name, label, test, null)
		{
		}
		
		public RootFilteredGroup(string name, string label, ISpecification specification)
			: this(name, label, specification, null)
		{
		}

		public RootFilteredGroup(string name, string label, IFilteredGroupFactory<T> childGroupFactory)
			: this(name, label, new SimpleSpecification(ReturnTrue), childGroupFactory)
		{
		}

		public RootFilteredGroup(string name, string label, Predicate<T> test, IFilteredGroupFactory<T> childGroupFactory)
			: base(name, label, test, childGroupFactory)
		{
		}

		public RootFilteredGroup(string name, string label, ISpecification specification, IFilteredGroupFactory<T> childGroupFactory)
			: base(name, label, specification, childGroupFactory)
		{
		}

		private static bool ReturnTrue<U>(U item)
		{
			return true;
		}

		public void Add(T item)
		{
			base.AddItem(item);
		}

		public void Add(IEnumerable<T> items)
		{
			base.AddItems(items);
		}

		public void Remove(T item)
		{
			base.RemoveItem(item);
		}

		public new void Clear()
		{
			base.Clear();
		}
	}

	public interface IFilteredGroupFactory<T> where T: class
	{
		FilteredGroup<T> Create(T item);
	}

	public class FilteredGroup<T> where T : class 
	{
		private FilteredGroup<T> _parentGroup;
		private readonly string _name;
		private readonly string _label;
		private readonly ISpecification _specification;
		private readonly IFilteredGroupFactory<T> _childGroupFactory;
		private readonly ObservableList<T> _items;
		private readonly ReadOnlyCollection<T> _readOnlyItems;
		private readonly FilteredGroupList<T> _childGroups;

		//purposely not ListEventArgs
		private event EventHandler<ItemEventArgs<T>> _itemAdded;
		private event EventHandler<ItemEventArgs<T>> _itemRemoved;

		public FilteredGroup(string name, string label, Predicate<T> test)
			: this(name, label, test, null)
		{
		}
		
		public FilteredGroup(string name, string label, ISpecification specification)
			: this(name, label, specification, null)
		{
		}

		public FilteredGroup(string name, string label, Predicate<T> test, IFilteredGroupFactory<T> childGroupFactory)
			: this(name, label, new SimpleSpecification<T>(test), childGroupFactory)
		{
		}

		public FilteredGroup(string name, string label, ISpecification specification, IFilteredGroupFactory<T> childGroupFactory)
		{
			Platform.CheckForNullReference(specification, "specification");

			_name = name;
			_label = label;
			_specification = specification;
			_childGroupFactory = childGroupFactory;

			_childGroups = new FilteredGroupList<T>();
			_items = new ObservableList<T>();
			_readOnlyItems = new ReadOnlyCollection<T>(_items);

			_childGroups.ItemAdded += OnChildGroupAdded;
			_childGroups.ItemChanging += OnChildGroupChanging;
			_childGroups.ItemChanged += OnChildGroupChanged;
			_childGroups.ItemRemoved += OnChildGroupRemoved;

			_items.ItemAdded += OnItemAdded;
			_items.ItemRemoved += OnItemRemoved;
		}

		#region Public Events

		public event EventHandler<ItemEventArgs<T>> ItemAdded
		{
			add { _itemAdded += value; }
			remove { _itemAdded -= value; }
		}

		public event EventHandler<ItemEventArgs<T>> ItemRemoved
		{
			add { _itemRemoved += value; }
			remove { _itemRemoved -= value; }
		}

		#endregion

		#region Private Methods

		private void OnChildGroupAdded(object sender, ListEventArgs<FilteredGroup<T>> e)
		{
			e.Item.ParentGroup = this;
		}

		private void OnChildGroupChanging(object sender, ListEventArgs<FilteredGroup<T>> e)
		{
			e.Item.ParentGroup = null;
		}

		private void OnChildGroupChanged(object sender, ListEventArgs<FilteredGroup<T>> e)
		{
			e.Item.ParentGroup = this;
		}

		private void OnChildGroupRemoved(object sender, ListEventArgs<FilteredGroup<T>> e)
		{
			e.Item.ParentGroup = null;
		}

		private void OnEmpty()
		{
			if (ParentGroup != null)
				ParentGroup.OnChildGroupEmpty(this);
		}

		private void OnChildGroupEmpty(FilteredGroup<T> childGroup)
		{
			bool remove;
			OnChildGroupEmpty(childGroup, out remove);
			if (remove)
				ChildGroups.Remove(childGroup);
		}

		private void OnItemAdded(object sender, ListEventArgs<T> e)
		{
			OnItemAdded(e.Item);

			if (ParentGroup != null)
				ParentGroup.OnChildItemAdded(e.Item);
		}

		private void OnItemRemoved(object sender, ListEventArgs<T> e)
		{
			OnItemRemoved(e.Item);
			
			if (ParentGroup != null)
				ParentGroup.OnChildItemRemoved(e.Item);
		}

		private void OnChildItemAdded(T item)
		{
			_items.Remove(item);
		}

		private void OnChildItemRemoved(T item)
		{
			bool found = false;
			foreach (T childItem in GetAllChildItems())
			{
				if (childItem.Equals(item))
				{
					found = true;
					break;
				}
			}

			//when it no longer exists in any children, add it back to our list.
			if (!found)
				_items.Add(item);
		}

		protected virtual void OnChildGroupEmpty(FilteredGroup<T> childGroup, out bool remove)
		{
			//TODO (CR May09): make it so if there is a factory, you can't add your own groups.
			//NOTE: in the interest of time, deferring actually doing this since it involves no API changes.
			remove = _childGroupFactory != null;
		}

		private FilteredGroup<T> CreateNewGroup(T item)
		{
			if (_childGroupFactory != null)
				return _childGroupFactory.Create(item);
			else
				return null;
		}

		#endregion

		#region Public Properties

		public FilteredGroup<T> ParentGroup
		{
			get { return _parentGroup; }
			private set
			{
				Clear();
				_parentGroup = value;
				Refresh();
			}
		}

		public string Name
		{
			get { return _name; }
		}

		public string Label
		{
			get { return _label; }
		}

		public bool HasItems
		{
			get { return _items.Count > 0; }	
		}

		public ReadOnlyCollection<T> Items
		{
			get { return _readOnlyItems; }	
		}

		public FilteredGroupList<T> ChildGroups
		{
			get { return _childGroups; }
		}

		#endregion

		#region Public Methods

		public List<T> GetItems()
		{
			return new List<T>(_items);
		}

		public List<T> GetAllItems()
		{
			List<T> items = new List<T>();
			foreach (T item in Items)
			{
				if (!items.Contains(item))
					items.Add(item);
			}

			foreach (T item in GetAllChildItems())
			{
				if (!items.Contains(item))
					items.Add(item);
			}

			return items;
		}

		public List<T> GetAllChildItems()
		{
			List<T> items = new List<T>();
			foreach (FilteredGroup<T> child in ChildGroups)
			{
				foreach (T item in child.GetAllItems())
				{
					if (!items.Contains(item))
						items.Add(item);
				}
			}
			
			return items;
		}

		public override string ToString()
		{
			return this.Label;
		}

		#endregion

		#region Protected Methods

		#region Overridable

		protected virtual void OnItemAdded(T item)
		{
			EventsHelper.Fire(_itemAdded, this, new ItemEventArgs<T>(item));
		}

		protected virtual void OnItemRemoved(T item)
		{
			EventsHelper.Fire(_itemRemoved, this, new ItemEventArgs<T>(item));
		}

		#endregion

		protected virtual void Clear()
		{
			foreach (T item in GetItems())
				RemoveItem(item);
		}

		protected virtual void Refresh()
		{
			Clear();
			if (ParentGroup != null)
				AddItems(ParentGroup.GetItems());
		}

		protected virtual void AddItems(IEnumerable<T> items)
		{
			foreach (T item in items)
				AddItem(item);
		}

		protected virtual bool AddItem(T item)
		{
			if (!_specification.Test(item).Success)
				return false;

			if (!AddItemToChildren(item))
			{
				if (!_items.Contains(item))
					_items.Add(item);

				return true;
			}

			return false;
		}

		protected virtual bool AddItemToChildren(T item)
		{
			if (!_specification.Test(item).Success)
				return false;

			bool addedToChild = false;
			foreach (FilteredGroup<T> childGroup in _childGroups)
			{
				if (childGroup.AddItem(item))
					addedToChild = true;
			}

			if (!addedToChild)
			{
				FilteredGroup<T> newGroup = CreateNewGroup(item);
				if (newGroup != null)
				{
					ChildGroups.Add(newGroup);
					addedToChild = AddItemToChild(item, newGroup);
					Platform.CheckTrue(addedToChild, "Item should be guaranteed to have been inserted.");
				}
			}

			return addedToChild;
		}

		protected virtual bool AddItemToChild(T item, FilteredGroup<T> child)
		{
			Platform.CheckTrue(ChildGroups.Contains(child), "Group is not a child of this group.");

			if (!_specification.Test(item).Success)
				return false;

			return child.AddItem(item);
		}

		protected virtual void RemoveItem(T item)
		{
			foreach (FilteredGroup<T> group in ChildGroups)
				group.RemoveItem(item);

			_items.Remove(item);
			if (GetAllItems().Count == 0)
				OnEmpty();
		}

		#endregion
	}
}