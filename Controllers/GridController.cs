
using Newtonsoft.Json;
using DodoBird.Models.App;
using DodoBird.Models.Db;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using DodoBird.Services;

namespace DodoBird.Controllers
{
    public class GridController : Controller
    {
        [HttpPost]
        public string GetGrid(PageNavigation pageNavigation)
        {
            using (DodoBirdEntities Db = new DodoBirdEntities())
            {
                var gridSchema = DataService.GetGridSchema(pageNavigation.GridId);
                var tableSchema = DataService.GetTableSchema(gridSchema.AppDatabaseId, gridSchema.TableName);

                Db.Database.Connection.ConnectionString = SessionService.GetConnectionString(gridSchema.AppDatabaseId);

                StringBuilder sb = new StringBuilder();
                StringBuilder sbRecords = new StringBuilder();
                // add primary key columns
                foreach (var key in tableSchema.PrimaryKeys)
                {
                    sb.Append(key.ColumnName + ",");
                }
                
                // set sort and direction 
                if (pageNavigation.SortDirection == null || pageNavigation.SortDirection.Length == 0) pageNavigation.SortDirection = "ASC";


                // select columns in grid
                var gridColumns = gridSchema.GridColumns;
                var tableColumns = tableSchema.Columns;

                var columns = from a in gridColumns
                              join b in tableColumns.Where(w => !w.IsPrimaryKey) on a.ColumnName equals b.ColumnName
                              orderby a.ColumnOrder
                              select b;
                foreach (var column in columns)
                {
                    if (pageNavigation.OrderByColumn == null || pageNavigation.OrderByColumn.Length == 0) pageNavigation.OrderByColumn = column.ColumnName;  // default sort

                    sb.Append(column.ColumnName + ",");
                }


                // set select statement get page of records in json format
                var numOfRecordsOnPage = 15;
                var offSet = " OFFSET " + ((pageNavigation.CurrentPage - 1) * numOfRecordsOnPage) + " ROWS ";
                var fetch = " FETCH NEXT " + numOfRecordsOnPage + " ROWS ONLY ";

                var selectColumns = sb.ToString();
                selectColumns = selectColumns.Substring(0, selectColumns.Length - 1);
                var exe = "SELECT " + selectColumns + " FROM " + tableSchema.TableName + " ORDER BY " + pageNavigation.OrderByColumn + " " + pageNavigation.SortDirection + " " + offSet + fetch + " FOR JSON AUTO, INCLUDE_NULL_VALUES";



                // loop for records
                var recs = Db.Database.SqlQuery<string>(exe).ToList();  //, new SqlParameter("@CompanyId", companyId)
                foreach (var rec in recs)
                {
                    sbRecords.Append(rec);
                }


                // get total count
                var exeCount = "SELECT COUNT(1) FROM " + tableSchema.TableName;
                var recordCount = Db.Database.SqlQuery<int>(exeCount).FirstOrDefault();

                var numOfPages = 0;
                if (recordCount > 0)
                {
                    double totalPage_ = Convert.ToDouble(recordCount) / Convert.ToDouble(numOfRecordsOnPage);
                    numOfPages = (int)Math.Ceiling(totalPage_);
                }

                return "{ \"PrimaryKey\" : \"xxx\", \"RecordCount\" : " + recordCount + ", \"NumOfPages\" : " + numOfPages + ", \"OrderByColumn\" : \"" + pageNavigation.OrderByColumn + "\", \"SortDirection\" : \"" + pageNavigation.SortDirection + "\", \"Records\" : " + sbRecords.ToString() + " }";

                

            }
        }
    }
}