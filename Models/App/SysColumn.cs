using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DodoBird.Models.App
{
    public class SysColumn
    {
        public string ColumnName { get; set; }
        public int ColumnOrder { get; set; }
        public int DataLength { get; set; }
        public bool IsIdentity { get; set; }
        public bool IsRequired { get; set; }
        public bool IsComputed { get; set; }

        public string DefaultValue { get; set; }
        public string DataType { get; set; }
        public bool IsPrimaryKey { get; set; }
        public int SystemTypeId { get; set; }

    }
}