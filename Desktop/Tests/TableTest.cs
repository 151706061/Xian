#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

#if	UNIT_TESTS
#pragma warning disable 1591

using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace ClearCanvas.Desktop.Tests
{
	[TestFixture]
	public class TableTest
	{
		[Test]
		public void TestSortingItemCollection()
		{
			new ItemCollection<Item>().Sort(new ComparerA()); // sorting an empty collection should not fail

			ItemCollection<Item> coll1 = new ItemCollection<Item>();
			coll1.Add(new Item(1, 2, 3));
			coll1.Sort(new ComparerA());
			AssertEquals("(1,2,3)", coll1);

			// reset
			coll1.Sort(new Shuffle());

			coll1.Add(new Item(3, 1, 1));
			coll1.Add(new Item(1, 8, 3));
			coll1.Add(new Item(1, 3, 3));
			coll1.Add(new Item(2, 4, 2));
			coll1.Add(new Item(2, 5, 2));
			coll1.Add(new Item(3, 6, 1));
			coll1.Add(new Item(3, 7, 1));
			coll1.Sort(new ComparerC());
			coll1.Sort(new ComparerB());
			coll1.Sort(new ComparerA());
			AssertEquals("(1,2,3),(1,3,3),(1,8,3),(2,4,2),(2,5,2),(3,1,1),(3,6,1),(3,7,1)", coll1);

			// reset
			coll1.Sort(new Shuffle());

			coll1.Sort(new ComparerC());
			coll1.Sort(new RevComparerB());
			coll1.Sort(new RevComparerA());
			AssertEquals("(3,7,1),(3,6,1),(3,1,1),(2,5,2),(2,4,2),(1,8,3),(1,3,3),(1,2,3)", coll1);

			// reset
			coll1.Sort(new Shuffle());

			coll1.Sort(new ComparerC());
			coll1.Sort(new ComparerB());
			coll1.Sort(new RevComparerA());
			AssertEquals("(3,1,1),(3,6,1),(3,7,1),(2,4,2),(2,5,2),(1,2,3),(1,3,3),(1,8,3)", coll1);

			// reset
			coll1.Sort(new Shuffle());

			coll1.Sort(new RevComparerC());
			coll1.Sort(new ComparerB());
			coll1.Sort(new ComparerA());
			AssertEquals("(1,2,3),(1,3,3),(1,8,3),(2,4,2),(2,5,2),(3,1,1),(3,6,1),(3,7,1)", coll1);
		}

		private static void AssertEquals(string expected, IEnumerable<Item> actual)
		{
			StringBuilder sb = new StringBuilder();
			foreach (Item item in actual)
			{
				sb.Append(item.ToString() + ",");
			}

			if (sb.Length == 0)
			{
				Assert.AreEqual(expected, sb.ToString());
			}
			else
			{
				Assert.AreEqual(expected, sb.ToString(0, sb.Length - 1));
			}
		}

		private class RevComparerA : IComparer<Item>
		{
			public int Compare(Item x, Item y)
			{
				return -x.A.CompareTo(y.A);
			}
		}

		private class ComparerA : IComparer<Item>
		{
			public int Compare(Item x, Item y)
			{
				return x.A.CompareTo(y.A);
			}
		}

		private class RevComparerB : IComparer<Item> {
			public int Compare(Item x, Item y) {
				return -x.B.CompareTo(y.B);
			}
		}

		private class ComparerB : IComparer<Item>
		{
			public int Compare(Item x, Item y)
			{
				return x.B.CompareTo(y.B);
			}
		}

		private class RevComparerC : IComparer<Item> {
			public int Compare(Item x, Item y) {
				return -x.C.CompareTo(y.C);
			}
		}

		private class ComparerC : IComparer<Item>
		{
			public int Compare(Item x, Item y)
			{
				return x.C.CompareTo(y.C);
			}
		}

		private class Shuffle : IComparer<Item>
		{
			public int Compare(Item x, Item y)
			{
				return new Random().Next(-1, 2);
			}
		}

		private struct Item
		{
			public int A;
			public int B;
			public int C;

			public Item(int a, int b, int c)
			{
				A = a;
				B = b;
				C = c;
			}

			public override string ToString()
			{
				return string.Format("({0},{1},{2})", A, B, C);
			}
		}
	}
}

#endif