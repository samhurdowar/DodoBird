using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DodoBird.Models
{
    public class TableSchema
    {
        public string TableName { get; set; }
        public List<PrimaryKey> PrimaryKeys = new List<PrimaryKey>();
        public List<Column> Columns = new List<Column>();
    }

    public class PrimaryKey
    {
        public string ColumnName { get; set; }
        public string DataType { get; set; }
    }

    public class Column 
    {
        public string ColumnName { get; set; }
        public string DataType { get; set; }
        public int DataLength { get; set; }
        public bool IsPrimaryKey { get; set; }
        public bool IsIdentity { get; set; }
        public bool IsRequired { get; set; }
        public bool IsComputed { get; set; }
    }




    public class GridSchema
    {
        public int GridId { get; set; }
        public int AppDatabaseId { get; set; }
        public string TableName { get; set; }
        public string GridName { get; set; }
        public string GridFilter { get; set; }
        public string GridSort { get; set; }
        public List<AvailableColumn> AvailableColumns = new List<AvailableColumn>();
        public List<GridColumn> GridColumns = new List<GridColumn>();

    }

    public class AvailableColumn
    {
        public string ColumnName { get; set; }
    }

    public class GridColumn
    {
        public string ColumnName { get; set; }
        public int ColumnOrder { get; set; }
    }



}

/*


 */