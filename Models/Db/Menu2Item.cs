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
    
    public partial class Menu2Item
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Menu2Item()
        {
            this.Menu3Item = new HashSet<Menu3Item>();
        }
    
        public int Menu2ItemId { get; set; }
        public int Menu1ItemId { get; set; }
        public string MenuTitle { get; set; }
        public string PageFile { get; set; }
        public int TargetId { get; set; }
        public string TargetType { get; set; }
        public int SortOrder { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Menu3Item> Menu3Item { get; set; }
    }
}
