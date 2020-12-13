using DodoBird.Models;
using DodoBird.Models.App;
using DodoBird.Models.Db;
using Newtonsoft.Json;
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

                var sql = @"SELECT DISTINCT " + appDatabaseId + " AS AppDatabaseId,TABLE_SCHEMA AS Owner, TABLE_NAME AS TableName FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE WHERE TABLE_NAME = @TableName";
                TableSchema tableSchema = Db.Database.SqlQuery<TableSchema>(sql, new SqlParameter("@TableName", tableName)).FirstOrDefault();

                // get primarykeys 
                var primaryKeys = GetPrimaryKeys(Db, tableName);
                tableSchema.PrimaryKeys.AddRange(primaryKeys);

                // get columns   
                var columns = GetTableColumns(Db, tableName);
                tableSchema.Columns.AddRange(columns);


                // get dependent tables

                List<DependentTable> dependentTables = GetDependentTables(appDatabaseId, tableSchema);
                tableSchema.DependentTables.AddRange(dependentTables);

                return tableSchema;
            }
        }


        private static List<DependentTable> GetDependentTables(int appDatabaseId, TableSchema parentTableSchema)
        {
            using (DodoBirdEntities Db = new DodoBirdEntities())
            {

                var sql = @"SELECT * FROM DependentTable WHERE AppDatabaseId = @AppDatabaseId AND ParentOwner = @ParentOwner AND ParentTableName = @ParentTableName ";
                var dependentTables = Db.Database.SqlQuery<DependentTable>(sql, new SqlParameter("@AppDatabaseId", appDatabaseId), new SqlParameter("@ParentOwner", parentTableSchema.Owner), new SqlParameter("@ParentTableName", parentTableSchema.TableName)).ToList();

                if (dependentTables.Count > 0)
                {
                    using (DodoBirdEntities userDb = new DodoBirdEntities())
                    {
                        userDb.Database.Connection.ConnectionString = SessionService.GetConnectionString(appDatabaseId);

                        // get info for each dependent table
                        var i = 0;
                        foreach (var dependentTable in dependentTables)
                        {
                            sql = @"SELECT DISTINCT " + appDatabaseId + " AS AppDatabaseId,TABLE_SCHEMA AS Owner, TABLE_NAME AS TableName FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA = @Owner AND TABLE_NAME = @TableName";
                            var dependentTableSchema = userDb.Database.SqlQuery<TableSchema>(sql, new SqlParameter("@Owner", dependentTable.Owner), new SqlParameter("@TableName", dependentTable.TableName)).FirstOrDefault();

                            if (dependentTableSchema != null)
                            {
                                // get primarykeys 
                                var primaryKeys = GetPrimaryKeys(userDb, dependentTable.TableName);
                                dependentTable.PrimaryKeys.AddRange(primaryKeys);

                                // get columns   
                                var columns = GetTableColumns(userDb, dependentTable.TableName);
                                dependentTable.Columns.AddRange(columns);


                                // recursive. 
                                sql = @"SELECT COUNT(1) FROM DependentTable WHERE AppDatabaseId = @AppDatabaseId AND ParentOwner = @ParentOwner AND ParentTableName = @ParentTableName";
                                var dependentCount = Db.Database.SqlQuery<int>(sql, new SqlParameter("@AppDatabaseId", appDatabaseId), new SqlParameter("@ParentOwner", dependentTableSchema.Owner), new SqlParameter("@ParentTableName", dependentTableSchema.TableName)).FirstOrDefault();

                                if (dependentCount > 0)
                                {
                                    var dependentTables_ = GetDependentTables(appDatabaseId, dependentTableSchema);
                                    dependentTables[i].DependentTables.AddRange(dependentTables_);
                                }
                            }
                            i++;
                        }
                    }
  
                }

                return dependentTables;

            }
        }










        private static List<Column> GetPrimaryKeys(DodoBirdEntities Db, string tableName)
        {
            var sql = @"

                WITH T1 AS
                (	
	                SELECT c.name AS ColumnName, t.name AS DataType 
	                FROM sys.columns c 
	                JOIN sys.objects o ON o.object_id = c.object_id AND o.type = 'U' 
	                JOIN sys.Types t ON t.system_type_id = c.system_type_id AND t.system_type_id = t.user_type_id 
	                WHERE o.name = @TableName1 AND c.name IN (
	                SELECT column_name 
	                FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS AS TC 
	                INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE AS KU
	                ON TC.CONSTRAINT_TYPE = 'PRIMARY KEY' AND TC.CONSTRAINT_NAME = KU.CONSTRAINT_NAME AND KU.table_name = @TableName2 )
                ),
                T2 AS

                (
                    SELECT c.name AS ColumnName, ISNULL(c.column_id,0) AS ColumnOrder, CAST(ISNULL(c.max_length,0) AS int) AS DataLength,
                    REPLACE(REPLACE(REPLACE(ISNULL(object_definition(c.default_object_id),''), '(', ''), ')', ''), '''', '')   AS DefaultValue,
                    CAST(1 AS Bit) AS IsPrimaryKey,
                    CAST(ISNULL(CASE c.is_identity WHEN 1 THEN 1 ELSE 0 END, 0) AS Bit) AS IsIdentity,
                    CAST(ISNULL(CASE c.is_nullable WHEN 1 THEN 0 ELSE 1 END, 0) AS Bit) AS IsRequired,
                    CAST(ISNULL(CASE c.is_computed WHEN 1 THEN 1 ELSE 0 END, 0) AS Bit) AS IsComputed,
                    t.name AS DataType
                    FROM sys.columns c 
                    JOIN sys.Types t ON t.system_type_id = c.system_type_id AND t.system_type_id = t.user_type_id AND t.name <> 'sysname' 
                    JOIN sys.objects o ON o.object_id = c.object_id AND o.type = 'U' AND o.name = @TableName3
                ) 

                SELECT T2.* FROM T2 JOIN T1 ON T2.ColumnName = T1.ColumnName

                ";

            var keys = Db.Database.SqlQuery<Column>(sql, new SqlParameter("@TableName1", tableName), new SqlParameter("@TableName2", tableName), new SqlParameter("@TableName3", tableName)).ToList();
            return keys;
        }


        private static List<Column> GetTableColumns(DodoBirdEntities Db, string tableName)
        {

            // get columns   
            var sql = @"

                WITH T1 AS
                (	
	                SELECT c.name AS ColumnName, 1 AS PrimaryKey 
	                FROM sys.columns c 
	                JOIN sys.objects o ON o.object_id = c.object_id AND o.type = 'U' 
	                JOIN sys.Types t ON t.system_type_id = c.system_type_id AND t.system_type_id = t.user_type_id 
	                WHERE o.name = @TableName1 AND c.name IN (
	                SELECT column_name 
	                FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS AS TC 
	                INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE AS KU
	                ON TC.CONSTRAINT_TYPE = 'PRIMARY KEY' AND TC.CONSTRAINT_NAME = KU.CONSTRAINT_NAME AND KU.table_name = @TableName2 )
                ),
                T2 AS

                (
                    SELECT c.name AS ColumnName, ISNULL(c.column_id,0) AS ColumnOrder, CAST(ISNULL(c.max_length,0) AS int) AS DataLength,
                    REPLACE(REPLACE(REPLACE(ISNULL(object_definition(c.default_object_id),''), '(', ''), ')', ''), '''', '')   AS DefaultValue,
                    CAST(ISNULL(CASE c.is_identity WHEN 1 THEN 1 ELSE 0 END, 0) AS Bit) AS IsIdentity,
                    CAST(ISNULL(CASE c.is_nullable WHEN 1 THEN 0 ELSE 1 END, 0) AS Bit) AS IsRequired,
                    CAST(ISNULL(CASE c.is_computed WHEN 1 THEN 1 ELSE 0 END, 0) AS Bit) AS IsComputed,
                    t.name AS DataType
                    FROM sys.columns c 
                    JOIN sys.Types t ON t.system_type_id = c.system_type_id AND t.system_type_id = t.user_type_id AND t.name <> 'sysname' 
                    JOIN sys.objects o ON o.object_id = c.object_id AND o.type = 'U' AND o.name = @TableName3
                ) 

                SELECT CAST(ISNULL(T1.PrimaryKey, 0) AS Bit) AS IsPrimaryKey, T2.* FROM T2 LEFT JOIN T1 ON T2.ColumnName = T1.ColumnName


               ";


            var columns = Db.Database.SqlQuery<Column>(sql, new SqlParameter("@TableName1", tableName), new SqlParameter("@TableName2", tableName), new SqlParameter("@TableName3", tableName)).ToList();
            return columns;
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
                var gridColumns = Db.Database.SqlQuery<Column>(sql, new SqlParameter("@GridId", gridId)).ToList();
                if (gridColumns.Count > 0)
                {
                    gridSchema.GridColumns.AddRange(gridColumns);
                }
                sql = @"SELECT ColumnName FROM GridColumn WHERE GridId = @GridId ORDER BY ColumnOrder";
                var gridColumns_ = Db.Database.SqlQuery<string>(sql, new SqlParameter("@GridId", gridId)).ToList();

                // get available columns 
                TableSchema tableSchema = DataService.GetTableSchema(gridSchema.AppDatabaseId, gridSchema.TableName);
                var availableColumns = tableSchema.Columns.Where(w => !gridColumns_.Contains(w.ColumnName)).Select( s => new Column { ColumnName = s.ColumnName }).OrderBy(o => o.ColumnName).ToList();
                if (availableColumns.Count > 0)
                {
                    gridSchema.AvailableColumns.AddRange(availableColumns);
                }

                return gridSchema;

            }
        }



        public static FormSchema GetFormSchema(int formId)
        {
            using (DodoBirdEntities Db = new DodoBirdEntities())
            {
                Db.Database.Connection.ConnectionString = SessionService.DodoBirdConnectionString;

                var sql = @"SELECT * FROM Form WHERE FormId = @FormId";
                var formSchema = Db.Database.SqlQuery<FormSchema>(sql, new SqlParameter("@FormId", formId)).FirstOrDefault();

                // get form sections 
                sql = @"SELECT * FROM FormSection WHERE FormId = @FormId ORDER BY SectionOrder";
                var formSections = Db.Database.SqlQuery<FormSection>(sql, new SqlParameter("@FormId", formId)).ToList();
                if (formSections.Count > 0)
                {
                    formSchema.FormSections.AddRange(formSections);
                }

                // get form columns 
                sql = @"SELECT * FROM FormColumn WHERE FormId = @FormId ORDER BY ColumnOrder";
                var formColumns = Db.Database.SqlQuery<FormColumn>(sql, new SqlParameter("@FormId", formId)).ToList();
                if (formColumns.Count > 0)
                {
                    formSchema.FormColumns.AddRange(formColumns);
                }

                sql = @"SELECT ColumnName FROM FormColumn WHERE FormId = @FormId ORDER BY ColumnOrder";
                var formColumns_ = Db.Database.SqlQuery<string>(sql, new SqlParameter("@FormId", formId)).ToList();

                // get available columns 
                TableSchema tableSchema = DataService.GetTableSchema(formSchema.AppDatabaseId, formSchema.TableName);
                var availableColumns = tableSchema.Columns.Where(w => !formColumns_.Contains(w.ColumnName)).Select(s => new Column { ColumnName = s.ColumnName }).OrderBy(o => o.ColumnName).ToList();
                if (availableColumns.Count > 0)
                {
                    formSchema.AvailableColumns.AddRange(availableColumns);
                }

                return formSchema;

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



        public static string GetFormData(string json)
        {
            dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);

            var formId = Convert.ToInt32(jsonObj["FormId"]);

            FormSchema formSchema = DataService.GetFormSchema(formId);
            TableSchema tableSchema = DataService.GetTableSchema(formSchema.AppDatabaseId, formSchema.TableName);

            // generate sql statement
            List<SqlParameter> sqlParameters = new List<SqlParameter>();

            StringBuilder sb = new StringBuilder();
            sb.Append("FROM " + formSchema.TableName + " WHERE ");

            // primary keys
            string primaryKeys = "";
            primaryKeys = "'{ \"FormId\" : \"" + formId + "\"";
            var isNewRecord = false;
            foreach (var key in tableSchema.PrimaryKeys)
            {
                if (jsonObj[key.ColumnName] != null)
                {
                    var keyValue = jsonObj[key.ColumnName].ToString();
                    if (keyValue == "" || keyValue == "0")
                    {
                        isNewRecord = true;
                        break;
                    }


                    sb.Append(key.ColumnName + "= @" + key.ColumnName + " AND ");

                    sqlParameters.Add(new SqlParameter("@" + key.ColumnName, keyValue));
                }
                else
                {
                    isNewRecord = true;
                }

                primaryKeys += ", \"" + key.ColumnName + "\" : \"' + CAST(" + key.ColumnName + " AS varchar(250) ) + '\"";
            }
            primaryKeys += " }' AS PrimaryKeys, ";

            if (isNewRecord)  // get empty template with defaults
            {
                var defaultValue = "";
                sb.Clear();
                sb.Append("[ { \"PrimaryKeys\": \"{ 'FormId' : " + formId + " }\"");
                foreach (var column in tableSchema.Columns)
                {
                    defaultValue = column.DefaultValue;
                    if (defaultValue == "getdate")
                    {
                        defaultValue = DateTime.Now.ToString();
                    }
                    sb.Append(", \"" + column.ColumnName + "\":\"" + defaultValue + "\" ");
                }
                var jsonData = sb.ToString() + ", \"IsNewRecord\": \"" + isNewRecord + "\" } ]";

                var clientResponse = new ClientResponse { Successful = true, ActionExecuted = "GetFormData", JsonData = jsonData };
                var jsonClientResponse = JsonConvert.SerializeObject(clientResponse);
                return jsonClientResponse;
            }
            else
            {
                var sql = sb.ToString();
                sql = "SELECT 'False' AS IsNewRecord, " + primaryKeys + " * " + sql.Substring(0, sql.Length - 4);

                var clientResponse = HelperService.GetJsonData(formSchema.AppDatabaseId, sql, sqlParameters.ToArray());
                var jsonClientResponse = JsonConvert.SerializeObject(clientResponse);
                return jsonClientResponse;
            }

        }





        public static ClientResponse SaveFormData(int appDatabaseId, string tableName, string json)
        {
            try
            {

                dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);

                var primaryKeys_ = jsonObj["PrimaryKeys"].ToString();
                dynamic primaryKeys = Newtonsoft.Json.JsonConvert.DeserializeObject(primaryKeys_);

                var formId = primaryKeys["FormId"].ToString();

                var tableSchema = GetTableSchema(appDatabaseId, tableName);


                var recordId = "";
                using (DodoBirdEntities Db = new DodoBirdEntities())
                {
                    Db.Database.Connection.ConnectionString = SessionService.GetConnectionString(appDatabaseId);


                    string wherePrimaryKey = "";
                    string recordValue = "";
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


                    foreach (var column in tableSchema.Columns.Where(w => !(bool)w.IsComputed && !w.IsPrimaryKey))
                    {

                        if (jsonObj[column.ColumnName] != null)
                        {
                            recordValue = jsonObj[column.ColumnName].ToString();

                            sbInsert.Append(column.ColumnName + ",");
                            sbValue.Append("@" + column.ColumnName + ",");
                            insertParams.Add(new SqlParameter("@" + column.ColumnName, recordValue));


                            sbUpdate.Append(column.ColumnName + " = @" + column.ColumnName + ",");
                            updateParams.Add(new SqlParameter("@" + column.ColumnName, recordValue));
                        }
                    }



                    // set primary keys
                    var newPrimaryKeys = "{ 'FormId' : " + formId + "";

                    foreach (var column in tableSchema.PrimaryKeys)
                    {
                        if (primaryKeys[column.ColumnName] != null)
                        {
                            recordId = primaryKeys[column.ColumnName].ToString();
                            if (recordId.Length > 0 && recordId != "0")
                            {
                                wherePrimaryKey += " " + column.ColumnName + " = '" + recordId + "' AND ";
                            }
                        } else if (jsonObj[column.ColumnName] != null)
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

                            newPrimaryKeys = ", '" + column.ColumnName + "' : '" + newGuid_ + "'";
                        } else if (column.IsIdentity)
                        {
                            newPrimaryKeys += ", '" + column.ColumnName + "' : '[IDENTITY]'";
                        }
                    }


                    newPrimaryKeys += " }";

                    string jsonData = "";
                    var actionExecuted = "update";
                    if (wherePrimaryKey.Length > 0)  // update
                    {
                        var sql = sbUpdate.ToString().Substring(0, sbUpdate.ToString().Length - 1) + " WHERE " + wherePrimaryKey.Substring(0, wherePrimaryKey.Length - 4);
                        SqlParameter[] sqlParameters = updateParams.ToArray();
                        Db.Database.ExecuteSqlCommand(sql, sqlParameters);
                    }
                    else
                    {
                        SqlParameter[] sqlParameters = insertParams.ToArray();

                        var sql = sbInsert.ToString().Substring(0, sbInsert.ToString().Length - 1) + ") " + sbValue.ToString().Substring(0, sbValue.ToString().Length - 1) + ");";



                        if (newGuid_.Length > 0) // is guid
                        {
                            sql = sbInsert.ToString().Substring(0, sbInsert.ToString().Length - 1) + ") " + sbValue.ToString().Substring(0, sbValue.ToString().Length - 1) + ");";
                            recordId = newGuid.ToString();
                            Db.Database.ExecuteSqlCommand(sql, sqlParameters);
                        }
                        else
                        {
                            sql += "SELECT CAST(@@IDENTITY AS varchar(250));";
                            recordId = Db.Database.SqlQuery<string>(sql, sqlParameters).FirstOrDefault();
                            newPrimaryKeys = newPrimaryKeys.Replace("[IDENTITY]", recordId);
                            var jsonData_ = GetFormData(newPrimaryKeys);
                            json
                        }
                        Db.SaveChanges();

                        actionExecuted = "insert";

                    }

                    return new ClientResponse { Successful = true, Id = recordId.ToString(), ActionExecuted = actionExecuted, ErrorMessage = "", JsonData = jsonData };
                }
            }
            catch (Exception ex)
            {
                return new ClientResponse { Successful = false, Id = "", ActionExecuted = "SaveFormData", ErrorMessage = ex.Message };
            }
        }










    }
}

/*
             try
            {
                using (DodoBirdEntities Db = new DodoBirdEntities())
                {
                    Db.Database.ExecuteSqlCommand("dbo.SortFormSection @formId, @formSectionId, @newOrder", new[] { new SqlParameter("@formId", formId), new SqlParameter("@formSectionId", formSectionId), new SqlParameter("@newOrder", newOrder) });
                    return JsonConvert.SerializeObject(new ClientResponse { Successful = true, Id = formSectionId.ToString(), ActionExecuted = "SortFormSection", ErrorMessage = "" });
                }
            }
            catch (System.Exception ex)
            {
                return JsonConvert.SerializeObject(new ClientResponse { Successful = false, Id = formSectionId.ToString(), ActionExecuted = "SortFormSection", ErrorMessage = ex.Message });
            }

 */