#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using System.Text.RegularExpressions;
using System.Xml;
using NHibernate.Cfg;
using NHibernate.Mapping;

namespace ClearCanvas.Enterprise.Hibernate.Ddl
{
	/// <summary>
	/// Adds additional indexes to the Hibernate relational model, according to what is defined in *.dbi.xml files
	/// that are found in plugins.
	/// </summary>
	/// <remarks>
	/// This processor scans all plugins for *.dbi.xml resource files.  These files contain instructions for creating
	/// specific indexes in an XML format. See the file AdditionalIndexProcessor.dbi.xml.
	/// </remarks>
	class AdditionalIndexProcessor : IndexCreatorBase
	{
		public override void Process(Configuration config)
		{
			var tables = GetTables(config);

			// create a resource resolver that will scan all plugins
			// TODO: we should only scan plugins that are tied to the specified PersistentStore, but there is currently no way to know this
			IResourceResolver resolver = new ResourceResolver(
				CollectionUtils.Map(Platform.PluginManager.Plugins, (PluginInfo pi) => pi.Assembly).ToArray());

			// find all dbi resources
			var rx = new Regex("dbi.xml$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
			var dbiFiles = resolver.FindResources(rx);

			foreach (var dbiFile in dbiFiles)
			{
				using (var stream = resolver.OpenResource(dbiFile))
				{
					var xmlDoc = new XmlDocument();
					xmlDoc.Load(stream);
					var indexElements = xmlDoc.SelectNodes("indexes/index");
					if (indexElements == null)
						continue;

					foreach (XmlElement indexElement in indexElements)
					{
						ProcessIndex(indexElement, tables);
					}
				}
			}

		}

		private void ProcessIndex(XmlElement indexElement, IDictionary<string, Table> tables)
		{
			var tableName = indexElement.GetAttribute("table");
			var columnNames = CollectionUtils.Map(indexElement.GetAttribute("columns").Split(','), (string s) => s.Trim());

			if (string.IsNullOrEmpty(tableName) || columnNames.Count <= 0)
				return;

			// get table by name
			Table table;
			if (!tables.TryGetValue(tableName, out table))
				throw new DdlException(
					string.Format("An additional index refers to a table ({0}) that does not exist.", table.Name),
					null);

			// get columns by name
			var columns = new List<Column>();
			foreach (var columnName in columnNames)
			{
				var column = CollectionUtils.SelectFirst(table.ColumnIterator, col => col.Name == columnName);
				// bug #6994: could be that the index file specifies a column name that does not actually exist, so we need to check for nulls
				if (column == null)
					throw new DdlException(
						string.Format("An additional index on table {0} refers to a column ({1}) that does not exist.", table.Name, columnName),
						null);
				columns.Add(column);
			}

			// create index
			CreateIndex(table, columns);
		}


		private static Dictionary<string, Table> GetTables(Configuration config)
		{
			// build a set of all tables known to NH
			var tableSet = new Dictionary<string, Table>();
			foreach (var c in config.ClassMappings)
			{
				tableSet[c.Table.Name] = c.Table;
			}

			foreach (var mapping in config.CollectionMappings)
			{
				tableSet[mapping.CollectionTable.Name] = mapping.CollectionTable;
				tableSet[mapping.Table.Name] = mapping.Table;
			}
			return tableSet;
		}
	}
}
