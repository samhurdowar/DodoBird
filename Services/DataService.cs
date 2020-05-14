using SourceControl.Common;
using SourceControl.Models.Db;
using SourceControl.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;

namespace SourceControl.Services
{
    public static class DataService
    {

        public static string GetPasswordModifiedDate()
        {
            try
            {
                using (TargetEntities targetDb = new TargetEntities("NetworkCafeEntities"))
                {

                    var sql = @"SELECT SettingValue FROM SiteSettings WHERE SettingName = 'PasswordModifiedDate'";

                    var rec = targetDb.Database.SqlQuery<string>(sql).FirstOrDefault();

                    return rec;
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message + "\r\nGetPasswordModifiedDate");
                return "Unable to process. Error - " + ex.Message;
            }
        }



        public static string Execute(string sql)
        {
            try
            {
                using (SourceControlEntities Db = new SourceControlEntities())
                {
                    Db.Database.ExecuteSqlCommand(sql);
                }
                return "T";
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message + "\r\nDataService.Execute - sql=" + sql);
                return "Unable to process. Error - " + ex.Message;
            }
        }

        public static bool Execute(string tableName, string columnName, object columnValue, string keyName, int keyValue, string columnType)
        {
            string sql = "";
            try
            {
                using (SourceControlEntities Db = new SourceControlEntities())
                {
                    string val = "";
                    if (columnType.ToUpper() == "TEXT")
                    {
                        val = "'" + ((columnValue != null) ? columnValue.ToString() : "").Replace("'", "''") + "'";
                    }
                    else
                    {
                        val = Helper.ToInt32(columnValue).ToString();
                    }

                    sql = string.Format("UPDATE {0} SET {1} = {2} WHERE {3} = {4}", tableName, columnName, val, keyName, keyValue);

                    Db.Database.ExecuteSqlCommand(sql);
                    Db.SaveChanges();
                }
                return true;
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message + "\r\nDataService.Execute - sql=" + sql);
                return false;
            }
        }

        public static string GetJsonStringValue(string json, string propertyName)
        {
            try
            {
                string strReturn = JObject.Parse(json)[propertyName].ToString();
                return strReturn;
            }
            catch (Exception ex)
            {
                Helper.LogError("Error - " + ex.Message + "\r\nDataService.GetJsonStringValue - json=" + json + "   propertyName=" + propertyName);
                return "";
            }

        }

        public static bool GetJsonBooleanValue(string json, string propertyName)
        {
            try
            {
                var str = JObject.Parse(json)[propertyName].ToString();
                bool ret = (str == "true" || str == "1");
                return ret;
            }
            catch (Exception ex)
            {
                Helper.LogError("Error - " + ex.Message + "\r\nDataService.GetJsonBooleanValue - json=" + json + "   propertyName=" + propertyName);
                return false;
            }

        }

        public static int GetJsonIntValue(string json, string propertyName)
        {
            try
            {
                int intReturn = Helper.ToInt32(JObject.Parse(json)[propertyName]);
                return intReturn;
            }
            catch (Exception ex)
            {
                Helper.LogError("Error - " + ex.Message + "\r\nDataService.GetJsonIntValue - json=" + json + "   propertyName=" + propertyName);
                return 0;
            }
        }

        public static string GetStringValue(string sql)
        {
            try
            {
                using (SourceControlEntities Db = new SourceControlEntities())
                {
                    var ret = Db.Database.SqlQuery<string>(sql).FirstOrDefault();
                    return ret;
                }
            }
            catch (Exception ex)
            {
                Helper.LogError("Error - " + ex.Message + "\r\nDataService.GetStringValue - sql=" + sql);
                return "";
            }
        }

        public static int GetIntValue(string sql)
        {
            try
            {
                using (SourceControlEntities Db = new SourceControlEntities())
                {
                    var ret = Db.Database.SqlQuery<int>(sql).FirstOrDefault();
                    return ret;
                }
            }
            catch (Exception ex)
            {
                Helper.LogError("Error - " + ex.Message + "\r\nDataService.GetIntValue - sql=" + sql);
                return 0;
            }
        }

        public static string GetJsonFromSQL(string jsonFld, string dbFld, string fromClause, string tableName, bool getFirstOrDefault, int pageTemplateId = 0, int dbEntityId = 0)
        {


            StringBuilder sb = new StringBuilder();
            string select = "";
            var finalExe = "";
            var finalString = "";
            var entityName = "";
            var reportGridColumns = "";
            try
            {
                if (dbEntityId > 0)
                {
                    var dbEntity = SessionService.DbEntity(dbEntityId);
                    entityName = dbEntity.EntityName;
                }
                else
                {
                    PageTemplate pageTemplate = SessionService.PageTemplate(pageTemplateId);
                    entityName = pageTemplate.DbEntity.EntityName;
                    reportGridColumns = pageTemplate.ReportGridColumns;
                }

                //Helper.LogError("GetJsonFromSQL()  entityName=" + entityName + "    dbFld=" + dbFld + "    fromClause=" + fromClause + "    pageTemplateId=" + pageTemplateId + "    tableName=" + tableName + "    getFirstOrDefault=" + getFirstOrDefault);
                using (TargetEntities Db = new TargetEntities(entityName))
                {

                    if (tableName == "PAGEDATA")
                    {

                        string[] jsonFlds = jsonFld.Split(new char[] { ',' });
                        string[] dbFlds = dbFld.Split(new char[] { ',' });
                        sb.Append(" '{ ");
                        for (int i = 0; i < jsonFlds.Length; i++)
                        {
                            var tName = "";
                            var fld = jsonFlds[i];
                            if (fld.Contains("[BYPASS_SELECT_FIELD]"))
                            {
                                string[] words = fld.Split(new char[] { '|' });

                                sb.Append("\"" + words[0] + "\": \"' + " + words[1].Replace("[BYPASS_SELECT_FIELD]", "") + " + '\", ");
                                continue;
                            }

                            if (fld.Contains("."))
                            {
                                string[] words = fld.Split(new char[] { '.' });
                                tName = words[0];
                                fld = words[1];
                            }


                            if (dbFlds[i].Contains("[DATE]"))
                            {
                                sb.Append("\"" + fld.Replace("[DATE]", "") + "\": \"' + ISNULL(CAST(FORMAT(" + dbFlds[i].Replace("[DATE]", "") + ",'MM/dd/yyyy') AS varchar(50)), '') + '\", ");
                            }
                            else if (dbFlds[i].Contains("[DATETIME]"))
                            {
                                sb.Append("\"" + fld.Replace("[DATETIME]", "") + "\": \"' + ISNULL(CAST(FORMAT(" + dbFlds[i].Replace("[DATETIME]", "") + ",'MM/dd/yyyy hh:mm tt') AS varchar(50)), '') + '\", ");
                            }
                            else
                            {
                                if (tName.Length > 0)
                                {
                                    if (SessionService.DataType(pageTemplateId, fld) == "TEXT")
                                    {
                                        var dLength = SessionService.DataLength(tName, fld);
                                        if (dLength > 0)
                                        {
                                            sb.Append("\"" + fld + "\": \"' + REPLACE(CAST(ISNULL(" + dbFlds[i] + ",'') AS varchar(" + dLength + ")), '\"', '') + '\", ");

                                        }
                                        else
                                        {
                                            sb.Append("\"" + fld + "\": \"' + REPLACE(CAST(ISNULL(" + dbFlds[i] + ",'') AS varchar(500)), '\"', '')   + '\", ");
                                        }

                                    }
                                    else
                                    {
                                        sb.Append("\"" + fld + "\": \"' + CAST(ISNULL(" + dbFlds[i] + ",'') AS varchar(50)) + '\", ");
                                    }
                                }
                                else
                                {
                                    sb.Append("\"" + fld + "\": \"' + CAST(ISNULL(" + dbFlds[i] + ",'') AS varchar(500)) + '\", ");
                                }
                            }
                        }

                        select = sb.ToString();
                        select = select.Substring(0, select.Length - 2) + " }, ' ";
                        finalExe = fromClause.Replace("[PAGEDATA]", select);


                        // save user query
                        if (pageTemplateId > 0 && reportGridColumns.Length > 0)
                        {
                            sb.Clear();
                            // get grid columns and add to table
                            PageTemplate pageTemplate = SessionService.PageTemplate(pageTemplateId);

                            string[] ids = reportGridColumns.Split(new char[] { ',' });
                            foreach (string id in ids)
                            {
                                var id_ = 0;
                                int.TryParse(id.Trim(), out id_);
                                var columnDef = SessionService.ColumnDef(id_);

                                if (columnDef == null) continue;

                                string lookUpField = "";

                                // look for lookup table
                                if (columnDef.ElementType == "DropdownCustomOption")
                                {
                                    sb.Append("(SELECT OptionText FROM CustomOption WHERE ColumnDefId = " + columnDef.ColumnDefId + " AND CAST(OptionValue AS varchar(150)) = CAST(" + pageTemplate.TableName + "." + columnDef.ColumnName + " AS varchar(150))) AS " + columnDef.ColumnName + ",");
                                }
                                else if (columnDef.LookupTable != null && columnDef.LookupTable.Length > 0 && columnDef.ElementType == "DropdownSimple")
                                {
                                    if (columnDef.TextField.Contains(","))
                                    {
                                        string[] fields = columnDef.TextField.Split(new char[] { ',' });
                                        lookUpField = columnDef.LookupTable + "." + fields[0];
                                    }
                                    else
                                    {
                                        lookUpField = columnDef.LookupTable + "." + columnDef.TextField;
                                    }
                                    sb.Append("(SELECT " + lookUpField + " FROM " + columnDef.LookupTable + " WHERE " + columnDef.ValueField + " = " + pageTemplate.TableName + "." + columnDef.ColumnName + ") AS " + columnDef.ColumnName + ",");
                                }
                                else
                                {
                                    sb.Append(columnDef.ColumnName + ",");
                                }

                            }


                            select = sb.ToString();
                            select = select.Substring(0, select.Length - 1);
                            fromClause = fromClause.Replace("[PAGEDATA]", select);

                            using (SourceControlEntities sourceDb = new SourceControlEntities())
                            {
                                sourceDb.Database.ExecuteSqlCommand("DELETE FROM UserQuery WHERE UserId = " + SessionService.UserId + " AND PageTemplateId = " + pageTemplateId);
                                sourceDb.UserQueries.Add(new UserQuery
                                {
                                    UserId = SessionService.UserId,
                                    PageTemplateId = pageTemplateId,
                                    IsGrid = true,
                                    CurrentQuery = fromClause
                                });
                                sourceDb.SaveChanges();
                            }


                        }


                    }
                    else if (tableName.Length > 0)
                    {
                        //sb.Append("SELECT '{ ");
                        //// get all columns for table   select '{ "ColumnName": "' + ColumnName + '", "DisplayName": "' + DisplayName + '" }, ' AS jsonRec from ColumnDef

                        //var columns = SessionService.ColumnDefs(tableName);
                        //foreach (var column in columns)
                        //{
                        //	if (column.DataType == "TEXT")
                        //	{
                        //		var dLength = SessionService.DataLength(tableName, column.ColumnName);
                        //		if (dLength > 0)
                        //		{
                        //			sb.Append("\"" + column.ColumnName + "\": \"' + CAST(ISNULL(" + column.ColumnName + ",'') AS varchar(" + dLength + ")) + '\", ");
                        //		}
                        //		else
                        //		{
                        //			sb.Append("\"" + column.ColumnName + "\": \"' + CAST(ISNULL(" + column.ColumnName + ",'') AS varchar(max)) + '\", ");
                        //		}
                        //	}
                        //	else
                        //	{
                        //		sb.Append("\"" + column.ColumnName + "\": \"' + CAST(ISNULL(" + column.ColumnName + ",'') AS varchar(50)) + '\", ");
                        //	}
                        //}

                        //select = sb.ToString();
                        //select = select.Substring(0, select.Length - 2) + " }, ' ";
                        //finalExe = select + " " + fromClause;
                    }
                    else  // from dynamic table and GetFormData
                    {
                        string[] jsonFlds = jsonFld.Split(new char[] { ',' });
                        string[] dbFlds = dbFld.Split(new char[] { ',' });
                        sb.Append("SELECT '{ ");
                        for (int i = 0; i < jsonFlds.Length; i++)
                        {
                            sb.Append("\"" + jsonFlds[i] + "\": \"' + CAST(ISNULL(" + dbFlds[i] + ",'') AS varchar(500)) + '\", ");
                        }

                        select = sb.ToString();
                        select = select.Substring(0, select.Length - 2) + " }, ' ";
                        finalExe = select + " " + fromClause;

                    }

                    sb.Clear();

                    if (!getFirstOrDefault)
                    {
                        sb.Append("[ ");
                    }

                    //Helper.LogError("GetJsonFromSQL()  finalExe=" + finalExe);
                    List<string> recs = Db.Database.SqlQuery<string>(finalExe).ToList();

                    foreach (var rec in recs)
                    {
                        sb.AppendLine(rec.Replace("\\", "\\\\").Replace("\t", "   "));
                        if (getFirstOrDefault)
                        {
                            break;
                        }
                    }
                    finalString = sb.ToString();

                    finalString = finalString.Substring(0, finalString.Length - 4);

                    if (!getFirstOrDefault)
                    {
                        finalString += " ]";
                    }
                    //Helper.LogError("GetJsonFromSQL()  finalString=" + finalString);
                    return finalString;

                }
            }
            catch (Exception ex)
            {
                Helper.LogError("Error - " + ex.Message + "\r\nDataService.GetJsonObject  finalString=" + finalString);
                return "";
            }
        }

        public static string GetJsonForObject(object objectType)
        {
            string json = "";
            try
            {
                json = Newtonsoft.Json.JsonConvert.SerializeObject(objectType);
                return json;
            }
            catch (Exception ex)
            {
                Helper.LogError("Error - " + ex.Message + "\r\nDataService.GetJsonObject - json=" + json);
                return "";
            }
        }

        public static string UpdateRecord(int pageTemplateId, string json, string oldJson)
        {
            try
            {
                using (SourceControlEntities Db = new SourceControlEntities())
                {
                    // check for history columns, 
                    var historyDef = Db.ColumnDefs.Where(w => w.ElementType == "ChangeHistory" && w.PageTemplateId == pageTemplateId).FirstOrDefault();
                    if (historyDef != null && oldJson.Length > 3)
                    {

                        // loop through columns and compare newJson and oldJson
                        StringBuilder sb = new StringBuilder();
                        dynamic newJson = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
                        var primaryKey = SessionService.PrimaryKey(pageTemplateId);
                        var recordId = Helper.ToInt32(newJson["P" + pageTemplateId + "_" + primaryKey]);

                        if (recordId > 0)
                        {

                            dynamic origJson = Newtonsoft.Json.JsonConvert.DeserializeObject(oldJson);

                            var columnDefs = SessionService.ColumnDefs(pageTemplateId);

                            foreach (var columnDef in columnDefs.Where(w => !(bool)w.IsComputed))
                            {
                                var formColumnName = "P" + pageTemplateId + "_" + columnDef.ColumnName;


                                // set value for column
                                if (columnDef.DataType == "BOOLEAN" && newJson[formColumnName] == null)
                                {
                                    int origVal = Helper.ToBoolean01(origJson[formColumnName]);
                                    int newVal = Helper.ToBoolean01(newJson[formColumnName]);

                                    var origVal_ = (origVal == 1) ? "true" : "false";
                                    var newVal_ = (newVal == 1) ? "true" : "false";


                                    if (origVal != newVal)
                                    {
                                        sb.Append("[Changed " + columnDef.ColumnName + " from " + origVal_ + " to " + newVal_ + "] ");
                                    }
                                }

                                if (newJson[formColumnName] != null)
                                {
                                    if ((columnDef.DataType == "TEXT" || columnDef.DataType == "MAXTEXT") && columnDef.ElementType != "MultiSelect")
                                    {
                                        var origVal = Helper.ToSafeString(origJson[formColumnName]);
                                        var newVal = Helper.ToSafeString(newJson[formColumnName]);
                                        if (origVal != newVal)
                                        {
                                            sb.Append("[Changed " + columnDef.ColumnName + " from " + origVal + " to " + newVal + "] ");
                                        }

                                    }
                                    else if (columnDef.DataType == "DATE")
                                    {
                                        string origVal = Helper.ToDbDateTime(origJson[formColumnName]);
                                        string newVal = Helper.ToDbDateTime(newJson[formColumnName]);

                                        if (origVal != newVal)
                                        {
                                            sb.Append("[Changed " + columnDef.ColumnName + " from " + origVal + " to " + newVal + "] ");
                                        }
                                    }
                                    else if (columnDef.DataType == "BOOLEAN")
                                    {
                                        int origVal = Helper.ToBoolean01(origJson[formColumnName]);
                                        int newVal = Helper.ToBoolean01(newJson[formColumnName]);
                                        var origVal_ = (origVal == 1) ? "true" : "false";
                                        var newVal_ = (newVal == 1) ? "true" : "false";

                                        if (origVal != newVal)
                                        {
                                            sb.Append("[Changed " + columnDef.ColumnName + " from " + origVal_ + " to " + newVal_ + "] ");
                                        }
                                    }
                                    else if (columnDef.ElementType == "MultiSelect")
                                    {
                                        string origVal = Helper.ToSafeString(origJson[formColumnName]);
                                        string newVal = Helper.ToSafeString(newJson[formColumnName]);
                                        if (origVal != newVal)
                                        {
                                            // lookup values for all ids
                                            var origVal_ = "";
                                            if (origVal.Length > 0)
                                            {
                                                origVal_ = DataService.GetStringValue("SELECT STUFF((SELECT ', ' + " + columnDef.TextField + " FROM (SELECT " + columnDef.TextField + " FROM " + columnDef.LookupTable + " WHERE " + columnDef.ValueField + " IN (" + origVal + ")) AS T FoR XML PATH('')), 1, 1, '')");
                                                origVal_ = origVal_.Replace("<" + columnDef.ColumnName + ">", "").Replace("</" + columnDef.ColumnName + ">", "");
                                            }

                                            var newVal_ = "";
                                            if (newVal.Length > 0)
                                            {
                                                newVal_ = DataService.GetStringValue("SELECT STUFF((SELECT ', ' + " + columnDef.TextField + " FROM (SELECT " + columnDef.TextField + " FROM " + columnDef.LookupTable + " WHERE " + columnDef.ValueField + " IN (" + newVal + ")) AS T FoR XML PATH('')), 1, 1, '')");
                                                newVal_ = newVal_.Replace("<" + columnDef.ColumnName + ">", "").Replace("</" + columnDef.ColumnName + ">", "");

                                            }

                                            // add field_Text to json for update  Manager":null,"AnsibleGroup":"3","ActiveI
                                            var replaceThis = "\"" + columnDef.ColumnName + "\":";
                                            var withThis = "\"" + columnDef.ColumnName + "_Text\":\"" + newVal_ + "\",\"" + columnDef.ColumnName + "\":";
                                            json = json.Replace(replaceThis, withThis);


                                            sb.Append("[Changed " + columnDef.ColumnName + " from " + origVal_ + " to " + newVal_ + "] ");
                                        }
                                    }
                                    else if (columnDef.ElementType == "DropdownCustomOption")
                                    {
                                        var origVal = Helper.ToInt32(origJson[formColumnName]);
                                        var newVal = Helper.ToInt32(newJson[formColumnName]);
                                        if (origVal != newVal)
                                        {

                                            string origVal_ = Helper.ToSafeString(DataService.GetStringValue("SELECT OptionText FROM CustomOption WHERE OptionValue = " + origVal));
                                            string newVal_ = Helper.ToSafeString(DataService.GetStringValue("SELECT OptionText FROM CustomOption WHERE OptionValue = " + newVal));
                                            if (origVal_ != newVal_)
                                            {
                                                if (origVal_.Length == 0) origVal_ = "''";
                                                if (newVal_.Length == 0) newVal_ = "''";
                                                sb.Append("[Changed " + columnDef.ColumnName + " from " + origVal_ + " to " + newVal_ + "] ");
                                            }

                                        }
                                    }
                                    else
                                    {
                                        var origVal = Helper.ToInt32(origJson[formColumnName]);
                                        var newVal = Helper.ToInt32(newJson[formColumnName]);
                                        if (origVal != newVal)
                                        {
                                            if (columnDef.LookupTable.Length > 0 && columnDef.ValueField.Length > 0)
                                            {
                                                var dbFld = columnDef.TextField;
                                                if (columnDef.TextField.Contains(","))
                                                {
                                                    var columns = columnDef.TextField.Split(new char[] { ',' });
                                                    dbFld = columns[0];
                                                }
                                                string origVal_ = Helper.ToSafeString(DataService.GetStringValue("SELECT " + dbFld + " FROM " + columnDef.LookupTable + " WHERE " + columnDef.ValueField + "=" + origVal));
                                                string newVal_ = Helper.ToSafeString(DataService.GetStringValue("SELECT " + dbFld + " FROM " + columnDef.LookupTable + " WHERE " + columnDef.ValueField + "=" + newVal));
                                                if (origVal_ != newVal_)
                                                {
                                                    if (origVal_.Length == 0) origVal_ = "''";
                                                    if (newVal_.Length == 0) newVal_ = "''";
                                                    sb.Append("[Changed " + columnDef.ColumnName + " from " + origVal_ + " to " + newVal_ + "] ");
                                                }
                                            }
                                            else
                                            {
                                                sb.Append("[Changed " + columnDef.ColumnName + " from " + origVal + " to " + newVal + "] ");
                                            }
                                        }
                                    }
                                }
                            }

                            //Changes were made   
                            if (sb.Length > 0)
                            {
                                Db.Database.ExecuteSqlCommand("INSERT INTO ChangeHistory(PageTemplateId,RecordId,UserId,ChangeHistoryText) VALUES(" + pageTemplateId + "," + recordId + "," + SessionService.UserId + ", '" + sb.ToString().Replace("'", "''") + "');");
                            }
                        }
                    }

                    return UpdateRecord(pageTemplateId, json);
                }
            }
            catch (Exception ex)
            {
                Helper.LogError("Error - " + ex.Message + "\r\nDataService.UpdateRecord");
                return "Unable to process UpdateRecord() - " + ex.Message;
            }
        }

        public static string UpdateRecordByTable(int dbEntityId, string tableName, string json)
        {
            try
            {
                return "xxx";
                //return UpdateRecord(pageTemplateId, json, false);
            }
            catch (Exception ex)
            {
                Helper.LogError("Error - " + ex.Message + "\r\nDataService.UpdateRecord");
                return "Unable to process UpdateRecord() - " + ex.Message;
            }

        }

        public static string UpdateRecord(int pageTemplateId, string json, bool includePrefix = true)
        {
            try
            {
                var pageTemplate = SessionService.PageTemplate(pageTemplateId);
                var columnDefs = SessionService.ColumnDefs(pageTemplateId);
                var tableName = pageTemplate.TableName;

                var recordId = "";
                using (TargetEntities Db = new TargetEntities())
                {
                    Db.Database.Connection.ConnectionString = pageTemplate.DbEntity.ConnectionString;

                    string wherePrimaryKey = "";
                    string primaryKeyType = "";
                    StringBuilder sbInsert = new StringBuilder();
                    StringBuilder sbValue = new StringBuilder();
                    StringBuilder sbUpdate = new StringBuilder();
                    StringBuilder sbLinkNew = new StringBuilder();
                    sbUpdate.Append("UPDATE " + tableName + " SET ");
                    sbInsert.Append("INSERT INTO " + tableName + "(");
                    sbValue.Append(" VALUES(");
                    string formColumnName = "";
                    Guid newGuid = Guid.NewGuid();

                    dynamic obj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
                    foreach (var columnDef in columnDefs.Where(w => !(bool)w.IsComputed))
                    {
                        formColumnName = (includePrefix) ? "P" + pageTemplateId + "_" + columnDef.ColumnName : columnDef.ColumnName;


                        if ((bool)columnDef.IsPrimaryKey)
                        {
                            primaryKeyType = SessionService.DataType(pageTemplateId, columnDef.ColumnName);

                            if (obj[formColumnName] != null)
                            {
                                recordId = obj[formColumnName].ToString();
                                wherePrimaryKey = " WHERE " + columnDef.ColumnName + " = '" + recordId + "'";
                            }

                            if (primaryKeyType == "TEXT")
                            {
                                sbInsert.Append(columnDef.ColumnName + ",");
                                sbValue.Append("'" + newGuid + "',");
                            }


                            continue;
                        }
                        else if (columnDef.ColumnName == "ChangeDate" || columnDef.ElementType == "DateChanged")
                        {
                            sbInsert.Append(columnDef.ColumnName + ",");
                            sbValue.Append("getdate(),");
                            sbUpdate.Append(columnDef.ColumnName + " = getdate(),");
                            continue;
                        }
                        else if (columnDef.ColumnName == "ChangeBy")
                        {
                            sbInsert.Append(columnDef.ColumnName + ",");
                            sbValue.Append(SessionService.UserId + ",");
                            sbUpdate.Append(columnDef.ColumnName + " = " + SessionService.UserId + ",");
                            continue;
                        }

                        // set value for column  
                        if (columnDef.ElementType == "Note" || columnDef.ElementType == "FileAttachment")
                        {
                            sbLinkNew.Append("UPDATE " + columnDef.ElementType + " SET RecordId = [RecordId] WHERE RecordId = -" + (columnDef.ColumnDefId + SessionService.UserId) + ";");
                        }
                        else if (columnDef.DataType == "BOOLEAN" && obj[formColumnName] == null)
                        {
                            sbInsert.Append(columnDef.ColumnName + ",");
                            sbUpdate.Append(columnDef.ColumnName + " = ");
                            sbValue.Append("0,");
                            sbUpdate.Append("0,");
                        }
                        else if (obj[formColumnName] != null)
                        {
                            sbInsert.Append(columnDef.ColumnName + ",");
                            sbUpdate.Append(columnDef.ColumnName + " = ");

                            if (columnDef.DataType == "TEXT" || columnDef.DataType == "MAXTEXT")
                            {
                                sbValue.Append("'" + Helper.ToSafeString(obj[formColumnName]).Replace("'", "''") + "',");
                                sbUpdate.Append("'" + Helper.ToSafeString(obj[formColumnName]).Replace("'", "''") + "',");
                            }
                            else if (columnDef.DataType == "DATE")
                            {
                                string dateTime = Helper.ToDbDateTime(obj[formColumnName]);
                                if (dateTime == "null" && (bool)columnDef.IsRequired)
                                {
                                    dateTime = "getdate()";
                                }
                                sbValue.Append(dateTime + ",");
                                sbUpdate.Append(dateTime + ",");
                            }
                            else if (columnDef.DataType == "BOOLEAN")
                            {
                                int val01 = Helper.ToBoolean01(obj[formColumnName]);
                                sbValue.Append(val01 + ",");
                                sbUpdate.Append(val01 + ",");
                            }
                            else
                            {
                                sbValue.Append(Helper.ToInt32(obj[formColumnName]) + ",");
                                sbUpdate.Append(Helper.ToInt32(obj[formColumnName]) + ",");
                            }
                        }
                    }
                    //Helper.LogError("UpdateRecord()  recordId= " + recordId);

                    if (recordId != "0" && recordId.Length > 0 && !recordId.Contains("newid"))
                    {
                        var sql = sbUpdate.ToString().Substring(0, sbUpdate.ToString().Length - 1) + wherePrimaryKey;
                        //Helper.LogError("UpdateRecord()  sql= " + sql);
                        Db.Database.ExecuteSqlCommand(sql);
                    }
                    else
                    {
                        var sql = sbInsert.ToString().Substring(0, sbInsert.ToString().Length - 1) + ") " + sbValue.ToString().Substring(0, sbValue.ToString().Length - 1) + "); SELECT CAST(@@IDENTITY AS varchar(250));";
                        //Helper.LogError("UpdateRecord()  sql= " + sql);
                        if (primaryKeyType == "TEXT") // is guid
                        {
                            sql = sbInsert.ToString().Substring(0, sbInsert.ToString().Length - 1) + ") " + sbValue.ToString().Substring(0, sbValue.ToString().Length - 1) + ");";
                            recordId = newGuid.ToString();
                            Db.Database.ExecuteSqlCommand(sql);
                        }
                        else
                        {
                            recordId = Db.Database.SqlQuery<string>(sql).FirstOrDefault();
                        }
                        Db.SaveChanges();

                        //custom code for templatid = 2079
                        if (obj["P2079_PortNum2"] != null)
                        {
                            var portNum1 = Helper.ToInt32(obj["P2079_PortNum"].ToString());
                            var portNum2 = Helper.ToInt32(obj["P2079_PortNum2"].ToString());
                            if (portNum1 > 0 && portNum2 > 0 && portNum2 > portNum1)
                            {
                                sql = sql.Replace(recordId, "[guid]").Replace("'" + portNum1 + "'", "[portNum]");
                                for (int i = portNum1 + 1; i <= portNum2; i++)
                                {
                                    newGuid = Guid.NewGuid();
                                    var exe = sql.Replace("[guid]", newGuid.ToString()).Replace("[portNum]", "'" + i + "'");
                                    Db.Database.ExecuteSqlCommand(exe);
                                    Db.SaveChanges();
                                }
                            }
                        }



                        // update new notes and link to new record.  Need to finish...not coded for guids
                        //if (sbLinkNew.Length > 0) {
                        //	var exe = sbLinkNew.ToString().Replace("[RecordId]", recordId.ToString());
                        //	Db.Database.ExecuteSqlCommand(exe);
                        //}
                    }

                    return recordId.ToString();
                }
            }
            catch (Exception ex)
            {
                Helper.LogError("Error - " + ex.Message + "\r\n" + ex.StackTrace + "\r\nDataService.UpdateRecord");
                return "Unable to process UpdateRecord() - " + ex.Message;
            }

        }


    }

}