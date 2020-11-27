using DodoBird.Models;
using DodoBird.Models.App;
using DodoBird.Models.Db;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;

namespace DodoBird.Services
{
    public static class DataService
    {

        public static TableSchema GetTableSchema(int appDatabaseId, string tableName)
        {
            using (DodoBirdEntities Db = new DodoBirdEntities())
            {
                Db.Database.Connection.ConnectionString = SessionService.GetConnectionString(appDatabaseId);

                var sql = @"SELECT DISTINCT TABLE_NAME AS TableName FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE WHERE TABLE_NAME = @TableName";
                var tableSchema = Db.Database.SqlQuery<TableSchema>(sql, new SqlParameter("@TableName", tableName)).FirstOrDefault();

                // get primarykeys 
                sql = @"
                    SELECT DISTINCT c.name AS ColumnName, t.name AS DataType
                    FROM sys.columns c 
                    JOIN sys.objects o ON o.object_id = c.object_id AND o.type = 'U' 
                    JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE i ON i.TABLE_NAME = o.name 
                    JOIN sys.Types t ON t.system_type_id = c.system_type_id 
                    WHERE OBJECTPROPERTY(OBJECT_ID(CONSTRAINT_SCHEMA + '.' + CONSTRAINT_NAME), 'IsPrimaryKey') = 1  AND i.COLUMN_NAME = c.name AND o.name = @TableName
                ";

                var keys = Db.Database.SqlQuery<PrimaryKey>(sql, new SqlParameter("@TableName", tableName)).ToList();
                foreach (var key in keys)
                {
                    tableSchema.PrimaryKeys.Add(key);
                }

                // get columns   
                sql = @"
                    SELECT c.name AS ColumnName, ISNULL(c.column_id,0) AS ColumnOrder, CAST(ISNULL(c.max_length,0) AS int) AS DataLength,
                    CAST(0 AS Bit) AS IsPrimaryKey,
                    CAST(ISNULL(CASE c.is_identity WHEN 1 THEN 1 ELSE 0 END, 0) AS Bit) AS IsIdentity,
                    CAST(ISNULL(CASE c.is_nullable WHEN 1 THEN 0 ELSE 1 END, 0) AS Bit) AS IsRequired,
                    CAST(ISNULL(CASE c.is_computed WHEN 1 THEN 1 ELSE 0 END, 0) AS Bit) AS IsComputed,
                    t.name AS DataType
                    FROM sys.columns c 
                    JOIN sys.Types t ON t.system_type_id = c.system_type_id AND t.name <> 'sysname' 
                    JOIN sys.objects o ON o.object_id = c.object_id AND o.type = 'U' AND o.name = @TableName
               ";


                var columns = Db.Database.SqlQuery<Column>(sql, new SqlParameter("@TableName", tableName)).ToList();

                var columns_ = from a in columns
                           join b in keys on a.ColumnName equals b.ColumnName
                           into aa
                           from bb in aa.DefaultIfEmpty()
                           select new Column { ColumnName = a.ColumnName, DataLength = a.DataLength, IsPrimaryKey = (bb == null) ? false : true, IsIdentity = a.IsIdentity, IsRequired = a.IsRequired, IsComputed = a.IsComputed, DataType = a.DataType };

                foreach (var column in columns_)
                {
                    tableSchema.Columns.Add(column);
                }

                return tableSchema;

            }
        }

        public static GridSchema GetGridSchema(int gridId)
        {
            using (DodoBirdEntities Db = new DodoBirdEntities())
            {
                Db.Database.Connection.ConnectionString = SessionService.DodoBirdConnectionString;

                var sql = @"SELECT * FROM Grid WHERE GridId = @GridId";
                var gridSchema = Db.Database.SqlQuery<GridSchema>(sql, new SqlParameter("@GridId", gridId)).FirstOrDefault();

                // get grid columns 
                sql = @"SELECT ColumnName, ColumnOrder FROM GridColumn WHERE GridId = @GridId ORDER BY ColumnOrder";
                var gridColumns = Db.Database.SqlQuery<GridColumn>(sql, new SqlParameter("@GridId", gridId)).ToList();
                if (gridColumns.Count > 0)
                {
                    gridSchema.GridColumns.AddRange(gridColumns);
                }
                sql = @"SELECT ColumnName FROM GridColumn WHERE GridId = @GridId ORDER BY ColumnOrder";
                var gridColumns_ = Db.Database.SqlQuery<string>(sql, new SqlParameter("@GridId", gridId)).ToList();

                // get available columns 
                TableSchema tableSchema = DataService.GetTableSchema(gridSchema.AppDatabaseId, gridSchema.TableName);
                var availableColumns = tableSchema.Columns.Where(w => !gridColumns_.Contains(w.ColumnName)).Select( s => new AvailableColumn { ColumnName = s.ColumnName }).OrderBy(o => o.ColumnName).ToList();
                if (availableColumns.Count > 0)
                {
                    gridSchema.AvailableColumns.AddRange(availableColumns);
                }

                return gridSchema;

            }
        }

        public static void SetDefaultGridAndForm(int appDatabaseId, string tableName)
        {
            using (DodoBirdEntities Db = new DodoBirdEntities())
            {
                Db.Database.Connection.ConnectionString = SessionService.DodoBirdConnectionString;


                // set default grid
                var sql = "SELECT TableName FROM Grid WHERE AppDatabaseId = @AppDatabaseId AND TableName = @TableName";

                var grids = Db.Database.SqlQuery<string>(sql, new SqlParameter("@AppDatabaseId", appDatabaseId), new SqlParameter("@TableName", tableName)).ToList();
                if (grids.Count == 0)
                {
                    // create grid
                    var exe = "INSERT INTO Grid(AppDatabaseId,TableName,GridName,DateAdd) VALUES(@AppDatabaseId,@TableName,@GridName,getdate()); SELECT CAST(@@IDENTITY AS int);";
                    var gridId = Db.Database.SqlQuery<int>(exe, new object[] { new SqlParameter("@AppDatabaseId", appDatabaseId), new SqlParameter("@TableName", tableName), new SqlParameter("@GridName", "Default grid") }).FirstOrDefault();

                    // create first 6 columns
                    var columnOrder = 0;
                    TableSchema tableSchema = DataService.GetTableSchema(appDatabaseId, tableName);
                    foreach (var column in tableSchema.Columns.Where(w => !"'image','datetimeoffset','sql_variant','ntext','text','hierarchyid','geometry','geography','varbinary','binary','xml','sysname'".Contains("'" + w.DataType + "'") ))
                    {
                        columnOrder++;
                        exe = "INSERT INTO GridColumn(GridId,ColumnName,ColumnOrder,DateAdd) VALUES(@GridId,@ColumnName,@ColumnOrder,getdate());";
                        Db.Database.ExecuteSqlCommand(exe, new object[] { new SqlParameter("@GridId", gridId), new SqlParameter("@ColumnName", column.ColumnName), new SqlParameter("@ColumnOrder", columnOrder) });
                        if (columnOrder > 5) break;

                    }
                }


                // set default form
                sql = "SELECT TableName FROM Form WHERE AppDatabaseId = @AppDatabaseId AND TableName = @TableName";

                var forms = Db.Database.SqlQuery<string>(sql, new SqlParameter("@AppDatabaseId", appDatabaseId), new SqlParameter("@TableName", tableName)).ToList();
                if (forms.Count == 0)
                {
                    // create form
                    var exe = "INSERT INTO Form(AppDatabaseId,TableName,FormName,DateAdd) VALUES(@AppDatabaseId,@TableName,@FormName,getdate()); SELECT CAST(@@IDENTITY AS int);";
                    var formId = Db.Database.SqlQuery<int>(exe, new object[] { new SqlParameter("@AppDatabaseId", appDatabaseId), new SqlParameter("@TableName", tableName), new SqlParameter("@FormName", "Default form") }).FirstOrDefault();


                    // create section with 2 section columns   
                    exe = "INSERT INTO FormSection(FormId,ColumnCount,SectionHeader,SectionOrder,DateAdd) VALUES(@FormId,2,@SectionHeader,1,getdate()); SELECT CAST(@@IDENTITY AS int);";
                    var formSectionId = Db.Database.SqlQuery<int>(exe, new object[] { new SqlParameter("@FormId", formId), new SqlParameter("@SectionHeader", tableName) }).FirstOrDefault();



                    // create form columns
                    var columnOrder = 0;
                    var sectionColumn = 1;
                    TableSchema tableSchema = DataService.GetTableSchema(appDatabaseId, tableName);
                    var halfCount = tableSchema.Columns.Count / 2;
                    foreach (var column in tableSchema.Columns)
                    {
                        columnOrder++;
                        exe = "INSERT INTO FormColumn(FormId,FormSectionId,SectionColumn,ColumnOrder,ColumnName,ElementType,DateAdd) VALUES(@FormId,@FormSectionId,@SectionColumn,@ColumnOrder,@ColumnName,'Textbox',getdate())";
                        Db.Database.ExecuteSqlCommand(exe, new object[] { new SqlParameter("@FormId", formId), new SqlParameter("@FormSectionId", formSectionId), new SqlParameter("@SectionColumn", sectionColumn), new SqlParameter("@ColumnOrder", columnOrder), new SqlParameter("@ColumnName", column.ColumnName) });
                        
                        if (columnOrder > halfCount)
                        {
                            columnOrder = 0;
                            sectionColumn++;
                        }
                    }
                }

            }
        }


        public static string SaveFormData(int appDatabaseId, string tableName, dynamic jsonObj)
        {
            try
            {
                var tableSchema = GetTableSchema(appDatabaseId, tableName);


                var recordId = "";
                using (DodoBirdEntities Db = new DodoBirdEntities())
                {
                    Db.Database.Connection.ConnectionString = SessionService.GetConnectionString(appDatabaseId);


                    string wherePrimaryKey = "";

                    List<SqlParameter> insertParams = new List<SqlParameter>();
                    List<SqlParameter> updateParams = new List<SqlParameter>();

                    StringBuilder sbInsert = new StringBuilder();
                    StringBuilder sbValue = new StringBuilder();
                    StringBuilder sbUpdate = new StringBuilder();

                    sbUpdate.Append("UPDATE " + tableName + " SET ");
                    sbInsert.Append("INSERT INTO " + tableName + "(");
                    sbValue.Append(" VALUES(");

                    Guid newGuid = Guid.NewGuid();
                    var newGuid_ = "";

                    // set primary keys
                    foreach (var column in tableSchema.PrimaryKeys)
                    {
                        if (jsonObj[column.ColumnName] != null)
                        {
                            recordId = jsonObj[column.ColumnName].ToString();
                            if (recordId.Length > 0 && recordId != "0")
                            {
                                wherePrimaryKey += " " + column.ColumnName + " = '" + recordId + "' AND ";
                            }
                        }

                        if (column.DataType == "uniqueidentifier")
                        {
                            sbInsert.Append(column.ColumnName + ",");
                            sbValue.Append("'" + newGuid + "',");
                            newGuid_ = newGuid.ToString();
                        }
                    }

                    foreach (var column in tableSchema.Columns.Where(w => !(bool)w.IsComputed && !w.IsPrimaryKey))
                    {

                        if (jsonObj[column.ColumnName] != null)
                        {
                            sbInsert.Append(column.ColumnName + ",");
                            sbValue.Append("@" + column.ColumnName + ",");
                            insertParams.Add(new SqlParameter("@" + column.ColumnName, jsonObj[column.ColumnName]));


                            sbUpdate.Append(column.ColumnName + " = @" + column.ColumnName + ",");
                            updateParams.Add(new SqlParameter("@" + column.ColumnName, jsonObj[column.ColumnName]));
                        }
                    }


                    if (wherePrimaryKey.Length > 0)  // update
                    {
                        var sql = sbUpdate.ToString().Substring(0, sbUpdate.ToString().Length - 1) + " WHERE " + wherePrimaryKey.Substring(0, wherePrimaryKey.Length - 4);
                        object[] obj = updateParams.ToArray();
                        Db.Database.ExecuteSqlCommand(sql, obj);
                    }
                    else
                    {
                        var sql = sbInsert.ToString().Substring(0, sbInsert.ToString().Length - 1) + ") " + sbValue.ToString().Substring(0, sbValue.ToString().Length - 1) + "); SELECT CAST(@@IDENTITY AS varchar(250));";
                        if (newGuid_.Length > 0) // is guid
                        {
                            sql = sbInsert.ToString().Substring(0, sbInsert.ToString().Length - 1) + ") " + sbValue.ToString().Substring(0, sbValue.ToString().Length - 1) + ");";
                            recordId = newGuid.ToString();
                            Db.Database.ExecuteSqlCommand(sql, insertParams);
                        }
                        else
                        {
                            recordId = Db.Database.SqlQuery<string>(sql, insertParams).FirstOrDefault();
                        }
                        Db.SaveChanges();


                    }

                    return recordId.ToString();
                }
            }
            catch (Exception ex)
            {
                return "Unable to process record - " + ex.Message;
            }
        }










    }
}

/*
 

 */