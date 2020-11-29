using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DodoBird.Models.App
{

    //{ MenuLevelId: string, MenuTitle: string, GridId: number, CurrentPage: number, NumOfPages: number, RecordCount: number, PrimaryKey: string, OrderByColumn: string, SortDirection: string }[] = [];

    public class PageNavigation
    {
        public int MenuId { get; set; }
        public string MenuTitle { get; set; }
        public int GridId { get; set; }
        public int CurrentPage { get; set; }
        public int NumOfPages { get; set; }
        public int RecordCount { get; set; }
        public string PrimaryKey { get; set; }
        public string OrderByColumn { get; set; }
        public string SortDirection { get; set; }
    }

}