using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DodoBird.Models
{
    public class TableSchema
    {
        public string Owner { get; set; }
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
        public string DefaultValue { get; set; }
        public int ColumnOrder { get; set; }
    }


    public class GridSchema
    {
        public int GridId { get; set; }
        public int AppDatabaseId { get; set; }
        public string TableName { get; set; }
        public string GridName { get; set; }
        public string GridFilter { get; set; }
        public string GridSort { get; set; }
        public int GridType { get; set; }
        public string Layout { get; set; }
        public List<Column> AvailableColumns = new List<Column>();
        public List<Column> GridColumns = new List<Column>();
    }



    public class FormSchema
    {
        public int FormId { get; set; }
        public int AppDatabaseId { get; set; }
        public string TableName { get; set; }
        public string FormName { get; set; }
        public string TargetType { get; set; }
        public string PageFile { get; set; }
        public List<Column> AvailableColumns = new List<Column>();
        public List<Column> FormColumns = new List<Column>();

    }

}

/*


 */