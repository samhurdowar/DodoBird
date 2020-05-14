using Newtonsoft.Json;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using DodoBird.Models.Db;
using System.Collections.Generic;
using System.Data.SqlClient;
using DodoBird.Models.App;

namespace DodoBird.Controllers
{
    public class DatabaseController : Controller
    {
        public string GetDatabaseList()
        {
            using (DodoBirdEntities Db = new DodoBirdEntities())
            {
                var recs = Db.AppDatabases.OrderBy(o => o.DatabaseName).ToList();
                var json = JsonConvert.SerializeObject(recs, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
                return json;
            }
        }

        public string GetTableList(int appDatabaseId, bool includeColumns = false)
        {
            using (DodoBirdEntities Db = new DodoBirdEntities())
            {
                if (includeColumns)
                {
                    var recs = Db.AppTables.Include(nameof(AppTable.AppColumns)).Where(w => w.AppDatabaseId == appDatabaseId).OrderBy(o => o.TableName);
                    var json = JsonConvert.SerializeObject(recs, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
                    return json;
                } 
                else
                {
                    var recs = Db.ViewAppTables.Where(w => w.AppDatabaseId == appDatabaseId).OrderBy(o => o.TableName);
                    var json = JsonConvert.SerializeObject(recs, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
                    return json;
                }
            }
        }

        public string GetTableOjects(int appTableId)
        {
            using (DodoBirdEntities Db = new DodoBirdEntities())
            {
                var grids = Db.Grids.Include(nameof(Grid.GridColumns)).Where(w => w.AppTableId == appTableId).ToList();

                // if no columns are displayed, set the first 8 columns as available
                foreach (var grid in grids)
                {
                    if (grid.GridColumns.Where(w => w.IsDisplayed).Count() == 0)
                    {
                        Db.Database.ExecuteSqlCommand("UPDATE GridColumn SET IsDisplayed = 1 WHERE GridColumnId IN (SELECT TOP 8 GridColumnId FROM GridColumn WHERE GridId = " + grid.GridId + " ORDER BY SortOrder)");
                    }
                }


                // reload grids
                grids = Db.Grids.Include(nameof(Grid.GridColumns)).Where(w => w.AppTableId == appTableId).ToList();

                var jsonGrids = JsonConvert.SerializeObject(grids, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

                var appColumns = Db.AppColumns.Where(w => w.AppTableId == appTableId);
                var jsonAppColumns = JsonConvert.SerializeObject(appColumns, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

                return "{ \"AppColumns\" : " + jsonAppColumns + ", \"Grids\" : " + jsonGrids + " }";
            }
        }

        public string GetGridColumnList(int gridId)
        {
            using (DodoBirdEntities Db = new DodoBirdEntities())
            {
                var recs = Db.GridColumns.Include(nameof(GridColumn.AppColumn)).Where(w => w.GridId == gridId);
                var json = JsonConvert.SerializeObject(recs, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
                return json;
            }
        }


        public string SortGridColumn(int gridId, int gridColumnId, int isDisplayed, int newOrder)
        {
            try
            {
                using (DodoBirdEntities Db = new DodoBirdEntities())
                {
                    Db.Database.ExecuteSqlCommand("dbo.SortGridColumn @gridId, @gridColumnId, @isDisplayed, @newOrder", new[] { new SqlParameter("@gridId", gridId), new SqlParameter("@gridColumnId", gridColumnId), new SqlParameter("@isDisplayed", isDisplayed), new SqlParameter("@newOrder", newOrder) });
                    return "";
                }
            }
            catch (System.Exception)
            {

                throw;
            }


        }





        public string SyncDatabases()
        {
            try
            {
                using (DodoBirdEntities Db = new DodoBirdEntities())
                {
                    var sql = @"
                    SELECT [AppDatabaseId] AS AppDatabaseId, o.name AS TableName, c.name AS PrimaryKey, 
                    CASE
	                    WHEN (system_type_id = 35)  THEN 'TEXT'
	                    WHEN (system_type_id = 36)  THEN 'TEXT'
	                    WHEN (system_type_id = 40)  THEN 'DATE'
	                    WHEN (system_type_id = 41)  THEN 'DATE'
	                    WHEN (system_type_id = 42)  THEN 'DATE'
	                    WHEN (system_type_id = 48)  THEN 'NUMBER'
	                    WHEN (system_type_id = 52)  THEN 'NUMBER'
	                    WHEN (system_type_id = 56)  THEN 'NUMBER'
	                    WHEN (system_type_id = 58)  THEN 'DATE'
	                    WHEN (system_type_id = 59)  THEN 'NUMBER'
	                    WHEN (system_type_id = 60)  THEN 'CURRENCY'
	                    WHEN (system_type_id = 61)  THEN 'DATETIME'
	                    WHEN (system_type_id = 62)  THEN 'NUMBER'
	                    WHEN (system_type_id = 98)  THEN 'TEXT'
	                    WHEN (system_type_id = 99)  THEN 'TEXT'
	                    WHEN (system_type_id = 104) THEN 'BOOLEAN'
	                    WHEN (system_type_id = 106) THEN 'DECIMAL'
	                    WHEN (system_type_id = 108) THEN 'NUMBER'
	                    WHEN (system_type_id = 122) THEN 'CURRENCY'
	                    WHEN (system_type_id = 127) THEN 'NUMBER'
	                    WHEN (system_type_id = 165) THEN 'TEXT'
	                    WHEN (system_type_id = 167) THEN 'TEXT'
	                    WHEN (system_type_id = 173) THEN ''
	                    WHEN (system_type_id = 175) THEN 'TEXT'
	                    WHEN (system_type_id = 189) THEN 'DATE'
	                    WHEN (system_type_id = 231) THEN 'TEXT'
	                    WHEN (system_type_id = 239) THEN 'TEXT'
	                    WHEN (system_type_id = 241) THEN 'TEXT'
                    END
                    AS PrimaryKeyType,
                    CAST(system_type_id AS int) AS SystemTypeId 
                    FROM sys.columns c JOIN sys.objects o ON o.object_id = c.object_id AND o.type = 'U' 
                    LEFT JOIN sys.default_constraints d ON d.object_id = c.default_object_id
                    LEFT JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE i ON i.TABLE_NAME = o.name 
                    WHERE i.COLUMN_NAME = c.name
                    ";

                    var appDatabases = Db.AppDatabases.ToList();
                    // load/sync tables and main grid for each database
                    foreach (var appDatabase in appDatabases)
                    {
                        var appTables = Db.AppTables.Where(w => w.AppDatabaseId == appDatabase.AppDatabaseId).ToList();
                        using (TargetEntities targetDb = new TargetEntities())
                        {
                            targetDb.Database.Connection.ConnectionString = appDatabase.ConnectionString;

                            var sysTables = targetDb.Database.SqlQuery<AppTable>(sql.Replace("[AppDatabaseId]", appDatabase.AppDatabaseId.ToString())).ToList();
                            foreach (var sysTable in sysTables)
                            {
                                var appTable = Db.AppTables.Where(w => w.AppDatabaseId == appDatabase.AppDatabaseId && w.TableName == sysTable.TableName).FirstOrDefault();
                                if (appTable == null)
                                {
                                    Db.AppTables.Add(sysTable);
                                    Db.SaveChanges();
                                    Db.Grids.Add(new Grid { AppTableId = sysTable.AppTableId, GridName = "Main grid for " + sysTable.TableName });
                                    Db.SaveChanges();
                                }
                                else
                                {
                                    appTable.PrimaryKey = sysTable.PrimaryKey;
                                    appTable.PrimaryKeyType = sysTable.PrimaryKeyType;
                                    appTable.SystemTypeId = sysTable.SystemTypeId;
                                    Db.Entry(appTable).State = System.Data.Entity.EntityState.Modified;
                                    Db.SaveChanges();
                                }
                            }
                        }
                    }

                    sql = @"
                    SELECT c.name AS ColumnName, ISNULL(c.column_id,0) AS ColumnOrder, CAST(ISNULL(c.max_length,0) AS int) AS DataLength,
                    CAST(ISNULL(CASE c.is_identity WHEN 1 THEN 1 ELSE 0 END, 0) AS Bit) AS IsIdentity,
                    CAST(ISNULL(CASE c.is_nullable WHEN 1 THEN 0 ELSE 1 END, 0) AS Bit) AS IsRequired,
                    CAST(ISNULL(CASE c.is_computed WHEN 1 THEN 1 ELSE 0 END, 0) AS Bit) AS IsComputed,
                    ISNULL(d.definition,'') AS DefaultValue,
                    CASE
	                    WHEN (system_type_id = 35)  THEN 'TEXT'
	                    WHEN (system_type_id = 36)  THEN 'TEXT'
	                    WHEN (system_type_id = 40)  THEN 'DATE'
	                    WHEN (system_type_id = 41)  THEN 'DATE'
	                    WHEN (system_type_id = 42)  THEN 'DATE'
	                    WHEN (system_type_id = 48)  THEN 'NUMBER'
	                    WHEN (system_type_id = 52)  THEN 'NUMBER'
	                    WHEN (system_type_id = 56)  THEN 'NUMBER'
	                    WHEN (system_type_id = 58)  THEN 'DATE'
	                    WHEN (system_type_id = 59)  THEN 'NUMBER'
	                    WHEN (system_type_id = 60)  THEN 'CURRENCY'
	                    WHEN (system_type_id = 61)  THEN 'DATETIME'
	                    WHEN (system_type_id = 62)  THEN 'NUMBER'
	                    WHEN (system_type_id = 98)  THEN 'TEXT'
	                    WHEN (system_type_id = 99)  THEN 'TEXT'
	                    WHEN (system_type_id = 104) THEN 'BOOLEAN'
	                    WHEN (system_type_id = 106) THEN 'DECIMAL'
	                    WHEN (system_type_id = 108) THEN 'NUMBER'
	                    WHEN (system_type_id = 122) THEN 'CURRENCY'
	                    WHEN (system_type_id = 127) THEN 'NUMBER'
	                    WHEN (system_type_id = 165) THEN 'TEXT'
	                    WHEN (system_type_id = 167) THEN 'TEXT'
	                    WHEN (system_type_id = 173) THEN ''
	                    WHEN (system_type_id = 175) THEN 'TEXT'
	                    WHEN (system_type_id = 189) THEN 'DATE'
	                    WHEN (system_type_id = 231) THEN 'TEXT'
	                    WHEN (system_type_id = 239) THEN 'TEXT'
	                    WHEN (system_type_id = 241) THEN 'TEXT'
                    END
                    AS DataType,
                    CAST(
                        CASE
	                        WHEN (i.COLUMN_NAME = c.name) THEN 1
	                        ELSE 0
                        END
                    AS Bit) 
                    AS IsPrimaryKey,
                    CAST(system_type_id AS int) AS SystemTypeId 
                    FROM sys.columns c JOIN sys.objects o ON o.object_id = c.object_id AND o.type = 'U' AND o.name = @TableName
                    LEFT JOIN sys.default_constraints d ON d.object_id = c.default_object_id
                    LEFT JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE i ON i.TABLE_NAME = o.name 
                    ";

                    // loop databases
                    appDatabases = Db.AppDatabases.ToList();
                    foreach (var appDatabase in appDatabases)
                    {
                        using (TargetEntities targetDb = new TargetEntities())
                        {
                            targetDb.Database.Connection.ConnectionString = appDatabase.ConnectionString;

                            // loop tables
                            var appTables = Db.AppTables.Where(w => w.AppDatabaseId == appDatabase.AppDatabaseId).ToList();
                            foreach (var appTable in appTables)
                            {
                                // loop syscolumns
                                var appColumns = appTable.AppColumns;
                                var sysColumns = targetDb.Database.SqlQuery<SysColumn>(sql, new SqlParameter("@TableName", appTable.TableName)).ToList();
                                foreach (var sysColumn in sysColumns)
                                {
                                    var appColumn = appColumns.Where(w => w.ColumnName == sysColumn.ColumnName).FirstOrDefault();
                                    if (appColumn == null)
                                    {
                                        var newAppColumn = new AppColumn
                                        {
                                            AppTableId = appTable.AppTableId,
                                            ColumnName = sysColumn.ColumnName,
                                            DisplayName = sysColumn.ColumnName,
                                            ElementType = "Textbox",
                                            ElementWidth = 300,
                                            ElementHeight = 0,

                                            ColumnOrder = sysColumn.ColumnOrder,
                                            IsIdentity = sysColumn.IsIdentity,
                                            IsRequired = sysColumn.IsRequired,
                                            IsComputed = sysColumn.IsComputed,
                                            IsPrimaryKey = sysColumn.IsPrimaryKey,
                                            DataLength = sysColumn.DataLength,
                                            DefaultValue = sysColumn.DefaultValue,
                                            DataType = sysColumn.DataType,
                                            SystemTypeId = sysColumn.SystemTypeId
                                        };
                                        Db.AppColumns.Add(newAppColumn);
                                        Db.SaveChanges();

                                        // add column to all grids for this table - display the first 8 columns
                                        var grids = Db.Grids.Where(w => w.AppTableId == appTable.AppTableId).ToList();
                                        foreach (var grid in grids)
                                        {
                                            Db.GridColumns.Add(new GridColumn { GridId = grid.GridId, AppColumnId = newAppColumn.AppColumnId, SortOrder = sysColumn.ColumnOrder, IsDisplayed = false });
                                            Db.SaveChanges();
                                        }
                                    }
                                    else
                                    {
                                        appColumn.IsIdentity = sysColumn.IsIdentity;
                                        appColumn.IsRequired = sysColumn.IsRequired;
                                        appColumn.IsComputed = sysColumn.IsComputed;
                                        appColumn.IsPrimaryKey = sysColumn.IsPrimaryKey;
                                        appColumn.DataLength = sysColumn.DataLength;
                                        appColumn.DefaultValue = sysColumn.DefaultValue;
                                        appColumn.DataType = sysColumn.DataType;
                                        appColumn.SystemTypeId = sysColumn.SystemTypeId;
                                        Db.Entry(appColumn).State = System.Data.Entity.EntityState.Modified;
                                        Db.SaveChanges();

                                    }
                                }
                            }
                        }
                    }
                }

                return "Database tables synced successfully.";
            }
            catch (System.Exception ex)
            {
                return ex.Message;
            }

        }

    }
}
