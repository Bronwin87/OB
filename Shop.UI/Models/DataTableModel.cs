using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.UI.Models
{
    public class DataTablesPageRequest
    {
        /// <summary>
        /// Draw counter.
        /// This is used by DataTables to ensure that the Ajax returns from server-side processing requests are drawn in sequence by DataTables 
        /// (Ajax requests are asynchronous and thus can return out of sequence). This is used as part of the draw return parameter.
        /// </summary>
        public int draw { get; set; }

        /// <summary>
        /// Paging first record indicator. This is the start point in the current data set (0 index based - i.e. 0 is the first record).
        /// </summary>
        public int start { get; set; }

        /// <summary>
        /// Number of records that the table can display in the current draw.
        /// It is expected that the number of records returned will be equal to this number, 
        /// unless the server has fewer records to return. Note that this can be -1 to indicate that all records should be returned
        /// </summary>
        public int length { get; set; }

        /// <summary>
        /// Global search value. To be applied to all columns which have searchable as true.
        /// </summary>
        public Search search { get; set; }

        /// <summary>
        /// Column to which ordering should be applied. This is an index reference to the columns array of information that is also
        /// submitted to the server.
        /// </summary>
        public List<Order> order { get; set; }

        /// <summary>
        /// Columns defined in DataTable
        /// </summary>
        public List<Column> columns { get; set; }

    }

    public class Search
    {
        /// <summary>
        /// Global search value. To be applied to all columns which have searchable as true.
        /// </summary>
        public string value { get; set; }

        /// <summary>
        /// true if the global filter should be treated as a regular expression for advanced searching, false otherwise. 
        /// Note that normally server-side processing scripts will not perform regular expression searching for performance 
        /// reasons on large data sets, but it is technically possible and at the discretion of your script.
        /// </summary>
        public bool rejex { get; set; }
    }

    public class Column
    {
        /// <summary>
        /// Column's data source, as defined by columns.dataDT.
        /// </summary>
        public string data { get; set; }

        /// <summary>
        /// Column's name, as defined by columns.nameDT.
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// Flag to indicate if this column is searchable (true) or not (false).
        /// This is controlled by columns.searchableDT.
        /// </summary>
        public bool searchable { get; set; }

        /// <summary>
        /// Flag to indicate if this column is orderable (true) or not (false).
        /// This is controlled by columns.orderableDT.
        /// </summary>
        public bool orderable { get; set; }

        /// <summary>
        /// Search value to apply to this specific column.
        /// </summary>
        public Search search { get; set; }
    }

    public class Order
    {
        /// <summary>
        /// Column to which ordering should be applied. This is an index reference
        /// to thecolumns array ofinformation that is also submitted to the server.
        /// </summary>
        public int column { get; set; }

        /// <summary>
        /// Ordering direction for this column. It will be asc or desc to indicate
        /// ascending ordering or descending ordering, respectively.
        /// </summary>
        public string dir { get; set; }
    }

    public class Page<T>
    {
        public long CurrentPage { get; set; }
        public long TotalPages { get; set; }
        public long TotalItems { get; set; }
        public long TotalDisplayItems { get; set; }
        public long ItemsPerPage { get; set; }
        public IList<T> Items { get; set; }
        public object Context { get; set; }
    }
}
