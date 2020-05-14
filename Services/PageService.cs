﻿using SourceControl.Common;
using SourceControl.Models;
using SourceControl.Models.Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace SourceControl.Services
{
	public class PageService
	{

		public static string GetServerSideRecords(int skip, int take, int page, int pageSize, List<GridSort> gridSorts = null, GridFilters filter = null, int pageTemplateId = 0, string tableNameOveride = "", string selectColumns = "", string sortOveride = "")
		{
            try
            {
                string tableName = "";
                string primaryKey = "";
                string innerSelect = "";
                string innerJoin = "";
                string outerSelect = "";
                string sortColumns = "";
                Dictionary<string, string> filterMap = new Dictionary<string, string>();
                Dictionary<string, string> sort1Map = new Dictionary<string, string>();
                Dictionary<string, string> sort2Map = new Dictionary<string, string>();

                if (tableNameOveride.Length > 0)
                {
                    tableName = tableNameOveride;
                    primaryKey = Helper.GetPrimaryKey(pageTemplateId, tableName);
                    innerSelect = tableName + "." + primaryKey;
                    outerSelect = tableName + "." + selectColumns;
                    sortColumns = sortOveride;
                    HttpContext.Current.Session["SortField1" + pageTemplateId] = null;
                    HttpContext.Current.Session["SortField2" + pageTemplateId] = null;
                }
                else
                {
                    tableName = SessionService.TableName(pageTemplateId);
                    primaryKey = SessionService.PrimaryKey(pageTemplateId);

                    TableFilterSort tfs = SessionService.TableFilterSort(pageTemplateId);
                    innerSelect = tfs.InnerSelect;
                    innerJoin = tfs.InnerJoin;
                    outerSelect = tfs.OuterSelect;
                    sortColumns = tfs.SortColumns;
                    filterMap = tfs.FilterMap;
                    sort1Map = tfs.Sort1Map;
                    sort2Map = tfs.Sort2Map;
                    selectColumns = tfs.GridColumns;

                }


                // WHERE
                string whereFilter = (HttpContext.Current.Session["WhereFilter" + pageTemplateId] != null) ? HttpContext.Current.Session["WhereFilter" + pageTemplateId].ToString() : "";

                if ((filter != null && filter.Filters != null))
                {
                    string buildFilter = "";
                    string buildFilter2 = "";
                    string fldName = "";
                    string oper = "";
                    string fldValue = "";

                    if ((filter != null && (filter.Filters != null && filter.Filters.Count > 0)))
                    {
                        HttpContext.Current.Session["totalCount" + pageTemplateId] = null;

                        var filters = filter.Filters;

                        string logic = filter.Logic;

                        for (var i = 0; i < filters.Count; i++)
                        {
                            if (filters[i].Field == null && filters[i].Filters != null)
                            {

                                if (buildFilter2.Length > 0)
                                {
                                    buildFilter2 += " AND  ";
                                }

                                buildFilter2 += "(";

                                var andOr = filters[i].Logic;
                                for (var x = 0; x < filters[i].Filters.Count; x++)
                                {
                                    fldName = filters[i].Filters[x].Field;
                                    oper = filters[i].Filters[x].Operator.ToLower();
                                    fldValue = filters[i].Filters[x].Value;
                                    buildFilter2 += BuildFilter(fldName, oper, fldValue, filterMap) + " " + andOr + " ";
                                }
                                buildFilter2 = buildFilter2.Substring(0, buildFilter2.Length - 4);
                                buildFilter2 += ") ";

                                continue;
                            }


                            fldName = filters[i].Field;
                            oper = filters[i].Operator.ToLower();
                            fldValue = filters[i].Value;

                            buildFilter += BuildFilter(fldName, oper, fldValue, filterMap) + " " + logic + " ";

                        }
                    }


                    if (buildFilter.Length > 0 || buildFilter2.Length > 0)
                    {
                        if (buildFilter.Length > 0)
                        {
                            buildFilter = buildFilter.Substring(0, buildFilter.Length - 4);

                            buildFilter = " (" + buildFilter + ") ";
                        }



                        if (buildFilter2.Length > 0)
                        {
                            if (buildFilter.Length > 0)
                            {
                                buildFilter += " AND  ";
                            }

                            buildFilter += " (" + buildFilter2 + ") ";
                        }
                    }

                    HttpContext.Current.Session["WhereFilter" + pageTemplateId] = buildFilter;

                }

                //--------- ORDER BY  
                if (gridSorts != null && gridSorts.Count > 0)
                {
                    HttpContext.Current.Session["SortField2" + pageTemplateId] = null;
                    foreach (var gridSort in gridSorts)
                    {
                        if ((gridSort.Field.Contains("_lco") || gridSort.Field.Contains("_tbl")) && sort1Map.ContainsKey(gridSort.Field))
                        {
                            HttpContext.Current.Session["SortField1" + pageTemplateId] = sort1Map[gridSort.Field];
                            HttpContext.Current.Session["SortField2" + pageTemplateId] = sort2Map[gridSort.Field];
                            HttpContext.Current.Session["SortDir" + pageTemplateId] = gridSort.Dir;
                            break;
                        }
                        else
                        {
                            HttpContext.Current.Session["SortField1" + pageTemplateId] = tableName + "." + gridSort.Field;
                            HttpContext.Current.Session["SortDir" + pageTemplateId] = gridSort.Dir;
                            break;
                        }
                    }
                }
                // set default sort
                if (HttpContext.Current.Session["SortField1" + pageTemplateId] == null)
                {
                    string[] columns = selectColumns.Split(new char[] { ',' });

                    HttpContext.Current.Session["SortDir" + pageTemplateId] = "ASC";

                    if (sortColumns.Length > 0)
                    {
                        HttpContext.Current.Session["SortField1" + pageTemplateId] = sortColumns;
                        HttpContext.Current.Session["SortDir" + pageTemplateId] = "";
                    }
                    else
                    {
                        if (columns.Count() > 1)
                        {
                            HttpContext.Current.Session["SortField1" + pageTemplateId] = columns[1];
                        }
                        else if (columns.Count() == 1)
                        {
                            HttpContext.Current.Session["SortField1" + pageTemplateId] = columns[0];
                        }

                    }
                }

                string orderBy1 = " ORDER BY " + HttpContext.Current.Session["SortField1" + pageTemplateId].ToString() + " " + HttpContext.Current.Session["SortDir" + pageTemplateId].ToString() + " ";

                if (HttpContext.Current.Session["SortField2" + pageTemplateId] == null)
                {
                    HttpContext.Current.Session["SortField2" + pageTemplateId] = HttpContext.Current.Session["SortField1" + pageTemplateId].ToString();
                }
                string orderBy2 = " ORDER BY " + HttpContext.Current.Session["SortField2" + pageTemplateId].ToString() + " " + HttpContext.Current.Session["SortDir" + pageTemplateId].ToString() + " ";

                if (orderBy1.Length < 14)
                {
                    orderBy1 = "";
                    orderBy2 = "";
                }

                // where clase
                string whereClause = "";
                if (HttpContext.Current.Session["WhereFilter" + pageTemplateId] != null && HttpContext.Current.Session["WhereFilter" + pageTemplateId].ToString().Length > 2)
                {
                    whereClause = " WHERE " + HttpContext.Current.Session["WhereFilter" + pageTemplateId].ToString() + " ";
                }


                //--------- COUNT()
                var totalCount = 0;
                if (HttpContext.Current.Session["totalCount" + pageTemplateId] == null)
                {
                    StringBuilder sbCount = new StringBuilder();
                    sbCount.Append("SELECT COUNT(1) ");
                    sbCount.Append("FROM " + tableName + " ");
                    sbCount.Append(innerJoin + " ");
                    sbCount.Append(whereClause + " ");

                    PageTemplate pageTemplate = SessionService.PageTemplate(pageTemplateId);
                    using (TargetEntities Db = new TargetEntities())
                    {
                        Db.Database.Connection.ConnectionString = pageTemplate.DbEntity.ConnectionString;

                        Helper.LogError("GetServerSideRecords=" + sbCount.ToString());
                        totalCount = Db.Database.SqlQuery<int>(sbCount.ToString()).FirstOrDefault();
                    }
                }
                else
                {
                    totalCount = Convert.ToInt32(HttpContext.Current.Session["totalCount" + pageTemplateId]);
                }

                //--------- Build SQL
                string pk = tableName + "." + primaryKey;

                StringBuilder sb = new StringBuilder();
                sb.Append("WITH MAIN AS (");

                // inner query
                sb.Append("SELECT " + innerSelect + " ");
                sb.Append("FROM " + tableName + " ");
                sb.Append(innerJoin + " ");
                sb.Append(whereClause + " ");
                sb.Append(orderBy1 + " ");


                if (take > 0)
                {
                    sb.Append("OFFSET " + skip + " ROWS FETCH NEXT " + take + " ROWS ONLY ");
                }
                else
                {
                    sb.Append("OFFSET 0 ROWS FETCH NEXT 1000000 ROWS ONLY ");
                }

                sb.Append(") ");

                // outer query
                sb.Append("SELECT [PAGEDATA] FROM " + tableName + " INNER JOIN MAIN ON " + pk + " = MAIN." + primaryKey + " ");
                sb.Append(orderBy2 + " ");

                string sql = sb.ToString();

                Helper.LogError("GetServerSideRecords:  outerSelect=" + outerSelect + "  sql=" + sql);

                if (totalCount == 0)
                {
                    return "{\"total\":0,\"data\":[] }";
                }
                var json = "";
                if (take > 0)
                {
                    json = "{\"total\":" + totalCount + ",\"data\":" + DataService.GetJsonFromSQL(outerSelect, outerSelect, sql, "PAGEDATA", false, pageTemplateId) + "}";
                }
                else
                {
                    json = DataService.GetJsonFromSQL(outerSelect, outerSelect, sql, "PAGEDATA", false, pageTemplateId);
                }
                Helper.LogError("PageService() GetServerSideRecords() after GetJsonFromSQL() json=" + json);
                json = json.Replace("\r", "").Replace("\n", "");
                return json;
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.StackTrace);
                return "";
            }

        }



		private static string BuildFilter(string fldName, string oper, string fldValue, Dictionary<string, string> filterMap)
		{
			if (oper.Contains("text") || oper == "eq")
			{
				fldValue = Helper.ToSafeString(fldValue);

				if (oper == "text_startswith")
				{
					if ((fldName.Contains("_lco") || fldName.Contains("_tbl")) && filterMap.ContainsKey(fldName))
					{
						return filterMap[fldName] + " LIKE '" + fldValue + "%' ";
					}
					else
					{
						return (fldName + " LIKE '" + fldValue + "%' ");
					}
				}
				else if (oper == "text_endswith")
				{
					if ((fldName.Contains("_lco") || fldName.Contains("_tbl")) && filterMap.ContainsKey(fldName))
					{
						return filterMap[fldName] + " LIKE '%" + fldValue + "' ";
					}
					else
					{
						return (fldName + " LIKE '%" + fldValue + "' ");
					}
				}
				else if (oper == "text_contains")
				{
					if ((fldName.Contains("_lco") || fldName.Contains("_tbl")) && filterMap.ContainsKey(fldName))
					{
						return filterMap[fldName] + " LIKE '%" + fldValue + "%' ";
					}
					else
					{
						return (fldName + " LIKE '%" + fldValue + "%' ");
					}
				}
				else if (oper == "text_eq" || oper == "eq")
				{
					if ((fldName.Contains("_lco") || fldName.Contains("_tbl")) && filterMap.ContainsKey(fldName))
					{
						return filterMap[fldName] + " = '" + fldValue + "' ";
					}
					else
					{
						return (fldName + " = '" + fldValue + "' ");
					}
				}
				else if (oper == "text_neq")
				{
					if ((fldName.Contains("_lco") || fldName.Contains("_tbl")) && filterMap.ContainsKey(fldName))
					{
						return filterMap[fldName] + " <> '" + fldValue + "' ";
					}
					else
					{
						return (fldName + " <> '" + fldValue + "' ");
					}
				}
				else if (oper == "text_doesnotcontain")
				{
					if ((fldName.Contains("_lco") || fldName.Contains("_tbl")) && filterMap.ContainsKey(fldName))
					{
						return filterMap[fldName] + " NOT LIKE '%" + fldValue + "%' ";
					}
					else
					{
						return (fldName + " NOT LIKE '%" + fldValue + "%' ");
					}
				}
			}
			else if (oper.Contains("number"))
			{
				var intValue = Helper.ToInt32(fldValue);

				if (oper == "number_eq")
				{
					return (fldName + " = " + intValue + " ");
				}
				else if (oper == "number_neq")
				{
					return (fldName + " <> " + intValue + " ");
				}
				else if (oper == "number_gt")
				{
					return (fldName + " > " + intValue + " ");
				}
				else if (oper == "number_lt")
				{
					return (fldName + " < " + intValue + " ");
				}
			}
			else if (oper.Contains("date"))
			{
				var dateValue = Helper.ToDbDateTime(fldValue);

				if (oper == "date_eq")
				{
					return (fldName + " = " + dateValue + " ");
				}
				else if (oper == "number_neq")
				{
					return (fldName + " <> " + dateValue + " ");
				}
				else if (oper == "date_lt")
				{
					return (fldName + " < " + dateValue + " ");
				}
				else if (oper == "date_gt")
				{
					return (fldName + " > " + dateValue + " ");
				}
			}

			return "";
		}



		public static GridSchemaColumns GetGridSchemaAndColumn(int pageTemplateId)
		{
			HttpContext.Current.Session["sec.GridSchemaColumns" + pageTemplateId] = null; //xxx


			if (HttpContext.Current.Session["sec.GridSchemaColumns" + pageTemplateId] == null)
			{

				var tableName = SessionService.TableName(pageTemplateId);
				var primaryKey = SessionService.PrimaryKey(pageTemplateId);

				StringBuilder sbSchema = new StringBuilder();
				StringBuilder sbColumns = new StringBuilder();
				StringBuilder sbGridScripts = new StringBuilder();
				
				if (SessionService.DataType(pageTemplateId, primaryKey) == "TEXT" || SessionService.DataType(pageTemplateId, primaryKey) == "GUID")
				{
					sbSchema.AppendLine(primaryKey + ": { type: \"string\", editable: false }");
				}
				else
				{
					sbSchema.AppendLine(primaryKey + ": { type: \"number\", editable: false }");
				}

				var pageTemplate = SessionService.PageTemplate(pageTemplateId);
				var columnDefs = SessionService.ColumnDefs(pageTemplateId);
				var gridWidth = "";

				// checkbox for delete
				if (pageTemplate.PageType != "gridonly")
				{
					sbColumns.AppendLine("{ title: \"<input type='checkbox' value='0' id='chkAll_" + pageTemplateId + "' />\", width: 25, template: '<input type=\"checkbox\" value=\"#= " + primaryKey + " #\" class=\"chk_" + pageTemplateId + "\" />' },");
				}


				using (SourceControlEntities Db = new SourceControlEntities())
				{

					int[] columnDefIds = Array.ConvertAll(pageTemplate.GridColumns.Split(new char[] { ',' }), s => int.Parse(s));

					foreach (var columnDefId in columnDefIds)
					{
						var columnDef = columnDefs.Where(w => w.ColumnDefId == columnDefId).FirstOrDefault();
						if (columnDef == null) continue;

						gridWidth = "";
						if (columnDef.GridWidth.Length > 0)
						{
							gridWidth = ", width: \"" + columnDef.GridWidth + "\"";
						}


                        if(columnDef.ElementType == "DropdownCustomOption") //xxx DropdownSimple
						{
							sbSchema.AppendLine(",\r\n" + columnDef.ColumnName + "_: { type: \"string\" }");
							sbColumns.AppendLine("{ field: \"" + columnDef.ColumnName + "_lco\", title: \"" + columnDef.DisplayName + "\", attributes: { \"style\": \"white-space:nowrap;\" } " + gridWidth + " ");
						}
						else if (columnDef.ElementType == "DropdownSimple")
						{
							sbSchema.AppendLine(",\r\n" + columnDef.ColumnName + "_: { type: \"string\" }");

							if (pageTemplate.PageType == "inline")
							{
								sbColumns.AppendLine("{ field: \"" + columnDef.ColumnName + "\", title: \"" + columnDef.DisplayName + "\", editor: editor_" + columnDef.ColumnName + ", values: lookup_" + columnDef.ColumnName + " " + gridWidth + " ");

								sbGridScripts.AppendLine("var lookup_" + columnDef.ColumnName + " = [");
								sbGridScripts.AppendLine("{ \"value\": \"0\", \"text\": \"\" }");

								using (TargetEntities targetDb = new TargetEntities())
								{
                                    targetDb.Database.Connection.ConnectionString = pageTemplate.DbEntity.ConnectionString;

                                    var sql = "SELECT ',{ \"value\": \"' + CAST(RoleId AS varchar(50)) + '\", \"text\": \"' + CAST(RoleName AS varchar(500)) + '\" }' FROM Role ORDER BY RoleName ASC";
									List<string> recs = targetDb.Database.SqlQuery<string>(sql).ToList();
									if (recs != null)
									{
										foreach (var rec in recs)
										{
											sbGridScripts.AppendLine(rec);
										}
									}
								}

								sbGridScripts.AppendLine("];");

								sbGridScripts.AppendLine("function editor_" + columnDef.ColumnName + "(container, options) {");
								sbGridScripts.AppendLine("CurrentColumnName = options.field;");
								sbGridScripts.AppendLine("$('<input required name=\"' + options.field + '\"  data-text-field=\"text\" data-value-field=\"value\" data-bind=\"value:' + options.field + '\"/>')");
								sbGridScripts.AppendLine(".appendTo(container)");
								sbGridScripts.AppendLine(".kendoDropDownList({");
								sbGridScripts.AppendLine("	autoBind: true,");
								sbGridScripts.AppendLine("	dataSource: lookup_" + columnDef.ColumnName + ",");
								sbGridScripts.AppendLine("	dataTextField: \"text\",");
								sbGridScripts.AppendLine("	dataValueField: \"value\"");
								sbGridScripts.AppendLine("});");
								sbGridScripts.AppendLine("}");
							}
							else
							{
								sbColumns.AppendLine("{ field: \"" + columnDef.ColumnName + "_\", title: \"" + columnDef.DisplayName + "\" " + gridWidth + " ");
							}
						}
						else if (columnDef.ElementType == "DisplayOnly")
						{
							sbColumns.AppendLine("{ template: \" #= " + columnDef.ColumnName + " #\", title: \"" + columnDef.DisplayName + "\", attributes: { \"style\": \"white-space:nowrap;\" } " + gridWidth + " ");
						}
						else if (columnDef.DataType == "DATE")
						{
							sbSchema.AppendLine(",\r\n" + columnDef.ColumnName + ": { type: \"date\", format: \"{0:MM/dd/yyyy}\" }");

							sbColumns.AppendLine("{ field: \"" + columnDef.ColumnName + "\", title: \"" + columnDef.DisplayName + "\", attributes: { \"style\": \"white-space:nowrap;\" }, type:\"date\", format:\"{0:MM/dd/yyyy}\" " + gridWidth + " ");
						}
						else if (columnDef.DataType == "DATETIME") 
						{
							sbSchema.AppendLine(",\r\n" + columnDef.ColumnName + ": { type: \"date\", format: \"{0:MM/dd/yyyy hh:mm tt }\" }");  

							sbColumns.AppendLine("{ field: \"" + columnDef.ColumnName + "\", title: \"" + columnDef.DisplayName + "\", attributes: { \"style\": \"white-space:nowrap;\" }, type:\"date\", format: \"{0:MM/dd/yyyy hh:mm tt }\" " + gridWidth + " ");
						}
						else if (columnDef.DataType == "NUMBER")
						{
							sbSchema.AppendLine(",\r\n" + columnDef.ColumnName + ": { type: \"number\" }");
							sbColumns.AppendLine("{ field: \"" + columnDef.ColumnName + "\", title: \"" + columnDef.DisplayName + "\", attributes: { \"style\": \"white-space:nowrap;\" } " + gridWidth + " ");
						}
						else if (columnDef.DataType == "BOOLEAN")
						{
							
							if (pageTemplate.PageType == "inline")
							{
								sbColumns.AppendLine("{ template: '<input type=\"checkbox\" #= " + columnDef.ColumnName + " ==\\'1\\' ? \\'checked=\"checked\"\\' : \"\" # onclick=\"CheckboxChangeColumnDef(this, \\'" + columnDef.ColumnName + "\\')\" class=\"checkbox_" + columnDef.ColumnName + "\" />', title: \"" + columnDef.DisplayName + "\"" + gridWidth + " ");

								sbGridScripts.AppendLine("$(\"#grid_" + pageTemplateId + " .k-grid-content\").on(\"change\", \"input.checkbox_" + columnDef.ColumnName + "\", function (e) {");
								sbGridScripts.AppendLine("var grid = $(\"#grid_" + pageTemplateId + "\").data(\"kendoGrid\");");
								sbGridScripts.AppendLine("var dataItem = grid.dataItem($(e.target).closest(\"tr\"));");

								sbGridScripts.AppendLine("dataItem.set(\"" + columnDef.ColumnName + "\", this.checked);");
								sbGridScripts.AppendLine("});");
							}
							else
							{
								sbSchema.AppendLine(",\r\n" + columnDef.ColumnName + ": { type: \"string\" }");

								if (columnDef.ElementType == "CheckboxYesNo")
								{
									sbColumns.AppendLine("{ field: \"" + columnDef.ColumnName + "\", title: \"" + columnDef.DisplayName + "\", template:\"#= GetBooleanYesNo(" + columnDef.ColumnName + ") #\" " + gridWidth + " ");
								}
								else
								{
									sbColumns.AppendLine("{ field: \"" + columnDef.ColumnName + "\", title: \"" + columnDef.DisplayName + "\", template:\"#= GetBooleanTrueFalse(" + columnDef.ColumnName + ") #\" " + gridWidth + " ");
								}
							}

						}
						else if (columnDef.ElementType == "HyperLink")
						{
							sbSchema.AppendLine(",\r\n" + columnDef.ColumnName + ": { type: \"string\" }");

							sbColumns.AppendLine("{ field: \"" + columnDef.ColumnName + "\", title: \"" + columnDef.DisplayName + "\", template:\"#= GetHyperLink(" + columnDef.ColumnName + ") #\", encoded: false, attributes: { \"style\": \"white-space:nowrap;\" } " + gridWidth + " ");
						}
                        else if(columnDef.ElementType == "Textarea")
                        {
                            sbSchema.AppendLine(",\r\n" + columnDef.ColumnName + ": { type: \"string\" }");

                            sbColumns.AppendLine("{ field: \"" + columnDef.ColumnName + "\", title: \"" + columnDef.DisplayName + "\", template:\"#= GetComment(" + columnDef.ColumnName + ") #\" " + gridWidth + " ");
                        }
                        else
						{

							if (pageTemplate.PageType == "inline")
							{
								sbColumns.AppendLine("{ field: \"" + columnDef.ColumnName + "\", title: \"" + columnDef.DisplayName + "\", editor: editor_Textbox, " + gridWidth + " ");

							}
							else
							{
								sbSchema.AppendLine(",\r\n" + columnDef.ColumnName + ": { type: \"string\" }");

								sbColumns.AppendLine("{ field: \"" + columnDef.ColumnName + "\", title: \"" + columnDef.DisplayName + "\", encoded: false, attributes: { \"style\": \"white-space:nowrap;\" } " + gridWidth + " ");
							}
						}


						// add multiple checkbox select  

						if ((bool)columnDef.IsMultiSelect)
						{
							sbColumns.AppendLine(",filterable: {");
							sbColumns.AppendLine("    multi: true,");
							sbColumns.AppendLine("    dataSource: {");
							sbColumns.AppendLine("        transport: {");
							sbColumns.AppendLine("            read: {");
							sbColumns.AppendLine("                url: \"Data/GetMultiSelect\",");
							sbColumns.AppendLine("                dataType: \"json\",");
							sbColumns.AppendLine("                data: {");
							sbColumns.AppendLine("                    pageTemplateId: \"" + columnDef.PageTemplateId + "\", columnDefId: " + columnDef.ColumnDefId + "");
							sbColumns.AppendLine("                }");
							sbColumns.AppendLine("            }");
							sbColumns.AppendLine("        }");
							sbColumns.AppendLine("    },");
							sbColumns.AppendLine("    itemTemplate: function(e) {");
							sbColumns.AppendLine("        if (e.field == \"all\") {");
							sbColumns.AppendLine("            return \"<div><label><strong><input type='checkbox' />Select All</strong></label></div>\";");
							sbColumns.AppendLine("        } else {");
							sbColumns.AppendLine("            return \"<div><input type='checkbox' name='\" + e.field + \"' value='#=ValueField#'/><span>#= TextField #</span></div>\"");
							sbColumns.AppendLine("        }");
							sbColumns.AppendLine("    }");
							sbColumns.AppendLine("}");
						}

						sbColumns.Append("},");

					}


				}

				var gridColumns = sbColumns.ToString();
				if (gridColumns.Length > 2)
				{
					gridColumns = gridColumns.Substring(0, gridColumns.Length - 1);
				}

				GridSchemaColumns gridSchemaColumns = new GridSchemaColumns { GridSchema = sbSchema.ToString(), GridColumns = gridColumns, GridScripts = sbGridScripts.ToString() };
				HttpContext.Current.Session["sec.GridSchemaColumns" + pageTemplateId] = gridSchemaColumns;

			}
			return (GridSchemaColumns)HttpContext.Current.Session["sec.GridSchemaColumns" + pageTemplateId];

		}




	}
}