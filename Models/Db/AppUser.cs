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
    
    public partial class AppUser
    {
        public int AppUserId { get; set; }
        public int ClientId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PrimaryPhone { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public System.DateTime DateAdd { get; set; }
    }
}
