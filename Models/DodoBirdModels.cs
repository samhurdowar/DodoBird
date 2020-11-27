using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DodoBird.Models
{

    public class Client
    {
        public int ClientId { get; set; }
        public string ClientName { get; set; }
        public System.DateTime DateAdd { get; set; }
    }

    public class AppDatabase
    {
        public int AppDatabaseId { get; set; }
        public int ClientId { get; set; }
        public string DatabaseName { get; set; }
        public string ConnectionString { get; set; }
    }

    public class AppUser
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

    public class Menu
    {
        public int MenuId { get; set; }
        public int ParentId { get; set; }
        public int ClientId { get; set; }
        public string MenuTitle { get; set; }
        public string PageFile { get; set; }
        public int TargetId { get; set; }
        public string TargetType { get; set; }
        public int SortOrder { get; set; }
    }
}