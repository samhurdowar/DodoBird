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
    
    public partial class DebugLog
    {
        public int LogId { get; set; }
        public string Source { get; set; }
        public string LogContent { get; set; }
        public System.DateTime LogDate { get; set; }
    }
}
