//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DodoBird.Models.Db
{
    using System;
    using System.Collections.Generic;
    
    public partial class GridColumn
    {
        public int GridColumnId { get; set; }
        public int GridId { get; set; }
        public int AppColumnId { get; set; }
        public bool IsDisplayed { get; set; }
        public int SortOrder { get; set; }
    
        public virtual AppColumn AppColumn { get; set; }
    }
}
