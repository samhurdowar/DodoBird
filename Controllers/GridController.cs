
using Newtonsoft.Json;
using DodoBird.Models.App;
using DodoBird.Models.Db;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace DodoBird.Controllers
{
    public class GridController : Controller
    {
        [HttpPost]
        public string GetGrid(PageNavigation pageNavigation)
        {
            using (DodoBirdEntities Db = new DodoBirdEntities())
            {
                var grid = Db.Grids.Find(pageNavigation.GridId);
                var appTable = Db.AppTables.Find(grid.AppTableId);

                using (TargetEntities targetDb = new TargetEntities())
                {
                    targetDb.Database.Connection.ConnectionString = appTable.AppDatabase.ConnectionString;

                    StringBuilder sb = new StringBuilder();

                    var primaryKey = appTable.PrimaryKey;
                    var primaryKeyType = appTable.PrimaryKeyType;

                    sb.Append(primaryKey);

                    if (pageNavigation.SortDirection == null || pageNavigation.SortDirection.Length == 0) pageNavigation.SortDirection = "ASC";
                    foreach (var column in grid.GridColumns.Where(w => w.AppColumn.ColumnName != primaryKey && !w.AppColumn.IsPrimaryKey && !"|CompanyID|DateCreated|LastUpdated|UpdatedBy|".Contains(w.AppColumn.ColumnName) && w.IsDisplayed).OrderBy(o => o.SortOrder))
                    {
                        if (pageNavigation.OrderByColumn == null || pageNavigation.OrderByColumn.Length == 0) pageNavigation.OrderByColumn = column.AppColumn.ColumnName;
                        sb.Append("," + column.AppColumn.ColumnName);
                    }


                    // get page of records in json format
                    var numOfRecordsOnPage = 15;
                    var offSet = " OFFSET " + ((pageNavigation.CurrentPage - 1) * numOfRecordsOnPage) + " ROWS ";
                    var fetch = " FETCH NEXT " + numOfRecordsOnPage + " ROWS ONLY ";

                    var exe = "SELECT " + sb.ToString() + " FROM " + appTable.TableName + " ORDER BY " + pageNavigation.OrderByColumn + " " + pageNavigation.SortDirection + " " + offSet + fetch + " FOR JSON AUTO, INCLUDE_NULL_VALUES";

                    var recs = targetDb.Database.SqlQuery<string>(exe).ToList();  //, new SqlParameter("@CompanyId", companyId)
                    sb.Clear();
                    foreach (var rec in recs)
                    {
                        sb.Append(rec);
                    }

                    var exeCount = "SELECT COUNT(1) FROM " + appTable.TableName;
                    var recordCount = targetDb.Database.SqlQuery<int>(exeCount).FirstOrDefault();

                    var numOfPages = 0;
                    if (recordCount > 0)
                    {
                        double totalPage_ = Convert.ToDouble(recordCount) / Convert.ToDouble(numOfRecordsOnPage);
                        numOfPages = (int)Math.Ceiling(totalPage_);
                    }

                    return "{ \"PrimaryKey\" : \"" + primaryKey + "\", \"RecordCount\" : " + recordCount + ", \"NumOfPages\" : " + numOfPages + ", \"OrderByColumn\" : \"" + pageNavigation.OrderByColumn + "\", \"SortDirection\" : \"" + pageNavigation.SortDirection + "\", \"Records\" : " + sb.ToString() + " }";

                }




            }
        }
    }
}