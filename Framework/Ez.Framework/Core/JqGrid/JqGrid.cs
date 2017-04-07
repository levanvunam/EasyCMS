using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.JqGrid.Models;
using Ez.Framework.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ez.Framework.Core.JqGrid
{
    public class JqSearchIn
    {
        public string Sidx { get; set; }
        public string Sord { get; set; }
        public int Page { get; set; }
        public int Rows { get; set; }
        public bool _Search { get; set; }
        public string SearchField { get; set; }
        public string SearchOper { get; set; }
        public string SearchString { get; set; }
        public string Filters { get; set; }

        public WhereClause GenerateWhereClause<T>()
        {
            return new WhereClauseGenerator().Generate(_Search, Filters, typeof(T));
        }

        /// <summary>
        /// Export grid
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public List<T> Export<T>(IQueryable<T> data, GridExportMode mode)
        {
            var exportData = new List<T>();
            switch (mode)
            {
                case GridExportMode.All:
                    {
                        exportData = data.ToList();
                        break;
                    }
                case GridExportMode.CurrentSearch:
                    {
                        exportData = Search(data, false).rows.Cast<T>().ToList();
                        break;
                    }
                case GridExportMode.CurrentPage:
                    {
                        exportData = Search(data).rows.Cast<T>().ToList();
                        break;
                    }
            }

            return exportData;
        }

        /// <summary>
        /// Search grid
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="searchPaging"></param>
        /// <returns></returns>
        public JqGridSearchOut Search<T>(IQueryable<T> data, bool searchPaging = true)
        {
            try
            {
                if (string.IsNullOrEmpty(Sidx))
                {
                    var defaultOrderProperty =
                        (typeof(T)).GetProperties().FirstOrDefault(
                            pi => pi.GetCustomAttributes(typeof(DefaultOrderAttribute), false).Length > 0);
                    if (defaultOrderProperty != null)
                    {
                        Sidx = defaultOrderProperty.Name;
                        var defaultOrderAttribute = (DefaultOrderAttribute)defaultOrderProperty.GetCustomAttributes(typeof(DefaultOrderAttribute), true).First();
                        if (defaultOrderAttribute.Order != 0)
                        {
                            Sord = defaultOrderAttribute.Order.GetEnumName().ToLower();
                        }
                    }
                    else
                    {
                        var firstProperty = (typeof(T)).GetProperties().FirstOrDefault();
                        if (firstProperty != null) Sidx = firstProperty.Name;
                        else throw new Exception("Object have no property.");
                    }
                }
                var order = string.Format("{0} {1}", Sidx, Sord);
                data = data.OrderBy(order);

                if (_Search && !string.IsNullOrEmpty(Filters))
                {
                    var wc = GenerateWhereClause<T>();

                    data = data.Where(wc.Clause, wc.FormatObjects);
                }

                //Sometime data is null after excute filter
                var totalRecords = 0;
                try
                {
                    totalRecords = data.Count();
                }
                catch { }

                if (totalRecords <= (Page - 1) * Rows)
                {
                    Page = Page - 1;
                }
                var skip = (Page > 0 ? Page - 1 : 0) * Rows;

                if (searchPaging)
                {
                    data = data
                    .Skip(skip)
                    .Take(Rows);
                }

                var totalPages = (int)Math.Ceiling((float)totalRecords / Rows);

                var grid = new JqGridSearchOut
                {
                    total = totalPages,
                    page = Page,
                    records = totalRecords,
                };

                //Sometime data is null after excute filter
                try
                {
                    grid.rows = data.ToArray();
                }
                catch { }

                return grid;
            }
            catch (Exception)
            {
                return new JqGridSearchOut
                {
                    total = 0,
                    page = 0,
                    records = 0,
                };
            }
        }
    }

    public class JqGridSearchOut
    {
        public int total { get; set; }
        public int page { get; set; }
        public int records { get; set; }
        public Array rows { get; set; }
        public List<ModelOptions> ModelOptions { get; set; }
    }

    public class JqGridFilter
    {
        public GroupOp GroupOp { get; set; }
        public List<JqGridRule> Rules { get; set; }
        public List<JqGridFilter> Groups { get; set; }
    }

    public class JqGridRule
    {
        public string Field { get; set; }
        public Operations Op { get; set; }
        public string Data { get; set; }
    }

    public class WhereClause
    {
        public string Clause { get; set; }
        public object[] FormatObjects { get; set; }
    }
}
