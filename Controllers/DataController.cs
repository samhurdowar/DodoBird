
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
//using SourceControl.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Linq.Dynamic;
using System.Reflection;
using System.Data.Entity.Core.Objects.DataClasses;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Linq.Expressions;
using SourceControl.Models.Db;
using SourceControl.Services;
using SourceControl.Models;

namespace SourceControl.Controllers
{
	public class DataController : Controller
	{


		public string GetMultiSelect(int pageTemplateId, int columnDefId)
		{
			try
			{
				var pageTemplate = SessionService.PageTemplate(pageTemplateId);
				var columnDef = SessionService.ColumnDef(columnDefId);

				using (TargetEntities Db = new TargetEntities())
				{
                    Db.Database.Connection.ConnectionString = pageTemplate.DbEntity.ConnectionString;

                    var columnValue = columnDef.ColumnName;
					var columnText = columnDef.ColumnName;
					var tableName = pageTemplate.TableName;
					var sql = "SELECT DISTINCT ISNULL(CAST(" + columnValue + " AS varchar(500)),'') AS ValueField, ISNULL(CAST(" + columnText + " AS varchar(500)),'') AS TextField FROM  " + tableName + " ORDER BY TextField";

					if (columnDef.ElementType == "DropdownCustomOption")
					{
						sql = "SELECT OptionValue AS ValueField, OptionText AS TextField FROM CustomOption WHERE ColumnDefId = " + columnDefId + " ORDER BY TextField";
					}
					else if (columnDef.ElementType == "DropdownSimple")
					{
						sql = "SELECT " + columnDef.ValueField + " AS ValueField, " + columnDef.TextField.Replace(",", " + ' - ' + ") + " AS TextField FROM " + columnDef.LookupTable + " ORDER BY TextField";
					}

					var recs = Db.Database.SqlQuery<ValueText>(sql).ToList();
					var json = DataService.GetJsonForObject(recs);
					return json;
				}
				
			}
			catch (Exception ex)
			{
				return ex.Message;
			}

		}


		[HttpPost]
		public string UpdateInline(int pageTemplateId, string recordId, string columnName, string columnValue)
		{
			try
			{
				string exe = "";
	
				var pageTemplate = SessionService.PageTemplate(pageTemplateId);

				var primaryKeyValue = recordId;
				var primaryKey = pageTemplate.PrimaryKey;
				var tableName = pageTemplate.TableName;
				var dbEntity = SessionService.DbEntity(pageTemplate.DbEntityId);
				var entityName = dbEntity.EntityName;

				using (TargetEntities Db = new TargetEntities(entityName))
				{

					string dataType = SessionService.DataType(pageTemplateId, columnName);

					int outInt = 0;
					decimal outDec = 0;
					if (dataType == "NUMBER" || dataType == "DECIMAL" || dataType == "CURRENCY")
					{
						if (dataType == "NUMBER")
						{

							if (int.TryParse(columnValue, out outInt))
							{
								exe = "UPDATE " + tableName + " SET " + columnName + " = " + columnValue + " WHERE " + primaryKey + " = '" + primaryKeyValue + "'";
							}
							else
							{
								exe = "UPDATE " + tableName + " SET " + columnName + " = 0 WHERE " + primaryKey + " = '" + primaryKeyValue + "'";
							}
						}
						else
						{
							if (decimal.TryParse(columnValue, out outDec) || int.TryParse(columnValue, out outInt))
							{
								exe = "UPDATE " + tableName + " SET " + columnName + " = " + columnValue + " WHERE " + primaryKey + " = '" + primaryKeyValue + "'";
							}
							else
							{
								exe = "UPDATE " + tableName + " SET " + columnName + " = 0 WHERE " + primaryKey + " = '" + primaryKeyValue + "'";
							}
						}

					}
					else if (dataType == "DATE")
					{
						DateTime dateTime = new DateTime();
						if (DateTime.TryParse(columnValue, out dateTime))
						{
							exe = "UPDATE " + tableName + " SET " + columnName + " = '" + columnValue + "' WHERE " + primaryKey + " = '" + primaryKeyValue + "'";
						}
						else
						{
							exe = "UPDATE " + tableName + " SET " + columnName + " = null WHERE " + primaryKey + " = '" + primaryKeyValue + "'";
						}

					}
					else if (dataType == "BOOLEAN")
					{
						if (columnValue == "true" || columnValue == "1")
						{
							exe = "UPDATE " + tableName + " SET " + columnName + " = 1 WHERE " + primaryKey + " = '" + primaryKeyValue + "'";
						}
						else
						{
							exe = "UPDATE " + tableName + " SET " + columnName + " = 0 WHERE " + primaryKey + " = '" + primaryKeyValue + "'";
						}
					}
					else
					{
						exe = "UPDATE " + tableName + " SET " + columnName + " = '" + columnValue.Replace("'", "''") + "' WHERE " + primaryKey + " = '" + primaryKeyValue + "'";
					}

					Db.Database.ExecuteSqlCommand(exe);
					Db.SaveChanges();
				}
				return "";
			}
			catch (Exception ex)
			{
				return "Unable to process UpdateInline() - " + ex.Message;
			}

		}

		[HttpPost]
		public string DeleteRecord(string ids_, int pageTemplateId)
		{
			try
			{
				var pageTemplate = SessionService.PageTemplate(pageTemplateId);

				var primaryKey = pageTemplate.PrimaryKey;
				var tableName = pageTemplate.TableName;
				var dbEntity = SessionService.DbEntity(pageTemplate.DbEntityId);
				var entityName = dbEntity.EntityName;

				using (TargetEntities Db = new TargetEntities(entityName))
				{

					Db.Database.ExecuteSqlCommand("DELETE FROM " + tableName + " WHERE " + primaryKey + " IN (" + ids_ + ")");
					Db.SaveChanges();
				}
				return "";
			}
			catch (Exception ex)
			{
				return ex.Message;
			}

		}

	}
}