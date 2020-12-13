
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
using DodoBird.Models;
using System.IO;
using System.Drawing;

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

                StringBuilder sbRecords = new StringBuilder();
                var selectColumns = "";

                // Standard grid
                if (gridSchema.GridType == 0)
                {
                    selectColumns = GetStandardGridSelect(gridSchema, tableSchema, ref pageNavigation);
                } else
                {
                    selectColumns = GetCustomGridSelect(gridSchema, tableSchema, ref pageNavigation);
                }

                // set select statement get page of records in json format
                var selectStatment = "SELECT " + selectColumns + " FROM " + tableSchema.Owner + "." + tableSchema.TableName + " ORDER BY " + pageNavigation.OrderByColumn + " " + pageNavigation.SortDirection;


                // set paging in json format
                var numOfRecordsOnPage = 15;
                var offSet = " OFFSET " + ((pageNavigation.CurrentPage - 1) * numOfRecordsOnPage) + " ROWS ";
                var fetch = " FETCH NEXT " + numOfRecordsOnPage + " ROWS ONLY ";

                var exe = selectStatment + " " + offSet + fetch + " FOR JSON AUTO, INCLUDE_NULL_VALUES";

                // loop for records
                var recs = Db.Database.SqlQuery<string>(exe).ToList();  
                foreach (var rec in recs)
                {
                    sbRecords.Append(rec);
                }

                // get total count
                var exeCount = "SELECT COUNT(1) FROM " + tableSchema.Owner + "." + tableSchema.TableName;
                var recordCount = Db.Database.SqlQuery<int>(exeCount).FirstOrDefault();

                var numOfPages = 0;
                if (recordCount > 0)
                {
                    double totalPage_ = Convert.ToDouble(recordCount) / Convert.ToDouble(numOfRecordsOnPage);
                    numOfPages = (int)Math.Ceiling(totalPage_);
                }


                string json = "{ \"ToFormId\" : \"" + gridSchema.ToFormId + "\", \"RecordCount\" : " + recordCount + ", \"NumOfPages\" : " + numOfPages + ", \"OrderByColumn\" : \"" + pageNavigation.OrderByColumn + "\", \"SortDirection\" : \"" + pageNavigation.SortDirection + "\", \"Records\" : " + sbRecords.ToString() + " }";

                return json;

            }
        }


        private string GetStandardGridSelect(GridSchema gridSchema, TableSchema tableSchema, ref PageNavigation pageNavigation)
        {


            // set sort and direction 
            if (pageNavigation.SortDirection == null || pageNavigation.SortDirection.Length == 0) pageNavigation.SortDirection = "ASC";


            StringBuilder sb = new StringBuilder();

            // add formId and primary key columns
            var formId = 6;  //xxx
            string primaryKeys = "";
            primaryKeys = "'{ \"FormId\" : \"" + formId  + "\"";

            foreach (var key in tableSchema.PrimaryKeys)
            {
                primaryKeys += ", \"" + key.ColumnName + "\" : \"' + CAST(" + key.ColumnName + " AS varchar(250) ) + '\"";
            }

            primaryKeys += " }' AS PrimaryKeys";

            sb.Append(primaryKeys);

            // select columns in grid
            var gridColumns = gridSchema.GridColumns;
            var tableColumns = tableSchema.Columns;

            var columns = from a in gridColumns
                            join b in tableColumns on a.ColumnName equals b.ColumnName
                            orderby a.ColumnOrder
                            select b;
            foreach (var column in columns)
            {
                sb.Append(", " + column.ColumnName);

                if (pageNavigation.OrderByColumn == null || pageNavigation.OrderByColumn.Length == 0) pageNavigation.OrderByColumn = column.ColumnName;  // default sort
            }

            var selectColumns = sb.ToString();

            return selectColumns;

            
        }

        private string GetCustomGridSelect(GridSchema gridSchema, TableSchema tableSchema, ref PageNavigation pageNavigation)
        {
            // set sort and direction 
            if (pageNavigation.SortDirection == null || pageNavigation.SortDirection.Length == 0) pageNavigation.SortDirection = "ASC";


            StringBuilder sb = new StringBuilder();

            // add primary key columns
            foreach (var key in tableSchema.PrimaryKeys)
            {
                sb.Append(key.ColumnName + ",");
            }

            // replace tokens
            var layout = gridSchema.Layout.Replace("'","''").Replace("\"", "\\\"");
            var columns = tableSchema.Columns;
            foreach (var column in columns)
            {
                layout = layout.Replace("[" + column.ColumnName + "]", "' + ISNULL(" + column.ColumnName + ",'') + '");

                if (pageNavigation.OrderByColumn == null || pageNavigation.OrderByColumn.Length == 0) pageNavigation.OrderByColumn = column.ColumnName;  // default sort
            }

            var selectColumns = sb.ToString() + "'" + layout + "' AS GridRecord ";


            return selectColumns;


        }


    }
}