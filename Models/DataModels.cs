using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DodoBird.Models
{
    public class TableSchema
    {
        public int AppDatabaseId { get; set; }
        public string Owner { get; set; }
        public string TableName { get; set; }
        public List<Column> PrimaryKeys = new List<Column>();
        public List<Column> Columns = new List<Column>();
        public List<DependentTable> DependentTables = new List<DependentTable>();
    }



    public class DependentTable
    {
        public int DependentTableId { get; set; }
        public int AppDatabaseId { get; set; }
        public string ParentOwner { get; set; }
        public string ParentTableName { get; set; }
        public string ParentKey { get; set; }
        public string DependentKey { get; set; }
        public string JoinType { get; set; }
        public string Relation { get; set; }
        public string Owner { get; set; }
        public string TableName { get; set; }
        public List<Column> PrimaryKeys = new List<Column>();
        public List<Column> Columns = new List<Column>();
        public List<DependentTable> DependentTables = new List<DependentTable>();
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
        public int ToFormId { get; set; }
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
        public string FormType { get; set; }
        public string PageFile { get; set; }
        public string FormLayout { get; set; }
        public List<Column> AvailableColumns = new List<Column>();
        public List<FormSection> FormSections = new List<FormSection>();
        public List<FormColumn> FormColumns = new List<FormColumn>();
    }


    public class FormSection
    {
        public int FormSectionId { get; set; }
        public int FormId { get; set; }
        public int ColumnCount { get; set; }
        public int SectionOrder { get; set; }
        public string SectionHeader { get; set; }
        public DateTime DateAdd { get; set; }
    }

    public class FormColumn
    {
        public int FormId { get; set; }
        public int FormColumnId { get; set; }
        public int FormSectionId { get; set; }
        public int SectionColumn { get; set; }
        public int ColumnOrder { get; set; }
        public string ColumnName { get; set; }
        public string ElementType { get; set; }
        public DateTime DateAdd { get; set; }
    }

}

/*
 









 */