using SourceControl.Models.Db;
using SourceControl.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using SourceControl.FormLayoutClasses;
using System.Reflection;

namespace SourceControl.Services
{
    public class FormLayoutService
    {

        public static string GetFormLayout(int pageTemplateId, bool layoutOnly)
        {
            FormLayout formLayout = new FormLayout(pageTemplateId);

            formLayout.DocumentReady.AppendLine("");
            formLayout.DocumentReady.AppendLine("<script>");
            formLayout.DocumentReady.AppendLine("[functs]");
            formLayout.DocumentReady.AppendLine("$(document).ready(function () {");


            var pageTemplate = SessionService.PageTemplate(pageTemplateId);

            var layOut = pageTemplate.Layout;
            var pageTemplateId2 = pageTemplate.PageTemplateId2;


            GetLayoutReplacements(pageTemplateId, ref layOut, ref formLayout);

            if (pageTemplateId2 > 0)
            {
                formLayout.PageTemplateId = pageTemplateId2;
                GetLayoutReplacements(pageTemplateId2, ref layOut, ref formLayout);
            }



            if (layoutOnly) return layOut;



            formLayout.DocumentReady.AppendLine("});");
            formLayout.DocumentReady.AppendLine("</script>");
            formLayout.DocumentReady.AppendLine("");

            var dReady = formLayout.DocumentReady.ToString().Replace("[functs]", formLayout.Functions.ToString());
            var frm = "<form id='Form_" + pageTemplateId + "'><input type='hidden' id='P" + pageTemplateId + "_" + SessionService.PrimaryKey(pageTemplateId) + "' />";

            return dReady + frm + layOut + "</form>" + formLayout.AfterForm.ToString();
        }



        private static void GetLayoutReplacements(int pageTemplateId, ref string layOut, ref FormLayout formLayout)
        {

            var elementObject = "";
            var elementLink = "";
            var recordId = "$('#InternalId_" + pageTemplateId + "').val()";
            var tableName = SessionService.TableName(pageTemplateId);


            var columnDefs = SessionService.ColumnDefs(pageTemplateId);
            var columnDefId = columnDefs[0].ColumnDefId.ToString();
            foreach (var columnDef in columnDefs)
            {
                if ((bool)columnDef.IsPrimaryKey)  // display only for Primary key   
                {
                    if (columnDef.ElementType == "DisplayOnly")
                    {
                        layOut = layOut.Replace("[P" + pageTemplateId + "_" + columnDef.ColumnName + "]", formLayout.DisplayOnly(columnDef));
                    }
                }
                else if (columnDef.ElementType == "Note")
                {
                    var linkUpload = "&nbsp;&nbsp;<img src='" + SessionService.VirtualDomain + "\\Images\\plus.png'><a href=\"javascript:AddNote(" + columnDef.PageTemplateId + ", " + columnDef.ColumnDefId + ")\">Add Note</a>";

                    layOut = layOut.Replace("[P" + pageTemplateId + "_" + columnDef.ColumnName + "]", formLayout.Note(columnDef));
                    layOut = layOut.Replace("[P" + pageTemplateId + "_" + columnDef.ColumnName + "LINK]", linkUpload);
                }
                else if (columnDef.ElementType == "Custom")
                {
                    elementObject = columnDef.ElementObject.Replace("[PageTemplateId]", pageTemplateId.ToString()).Replace("[ColumnDefId]", columnDefId).Replace("[RecordId]", recordId).Replace("[GT]", ">").Replace("[LT]", "<").Replace("[CL]", ";");
                    formLayout.DocumentReady.AppendLine(columnDef.ElementDocReady.Replace("[PageTemplateId]", pageTemplateId.ToString()).Replace("[ColumnDefId]", columnDefId).Replace("[RecordId]", recordId)).Replace("[GT]", ">").Replace("[LT]", "<").Replace("[CL]", ";");
                    elementLink = columnDef.ElementLabelLink.Replace("[PageTemplateId]", pageTemplateId.ToString()).Replace("[ColumnDefId]", columnDefId).Replace("[RecordId]", recordId).Replace("[GT]", ">").Replace("[LT]", "<").Replace("[CL]", ";");
                    formLayout.Functions.AppendLine(columnDef.ElementFunction);


                    layOut = layOut.Replace("[P" + pageTemplateId + "_" + columnDef.ColumnName + "LINK]", elementLink);
                    layOut = layOut.Replace("[P" + pageTemplateId + "_" + columnDef.ColumnName + "]", elementObject);

                }
                else if (columnDef.ElementType == "FileAttachment")
                {
                    string linkUpload = "&nbsp;&nbsp;<img src='" + SessionService.VirtualDomain + "\\Images\\paperclip.png'><a href=\"javascript:UploadFile1(" + columnDef.PageTemplateId + ", " + columnDef.ColumnDefId + ")\">Upload</a><span id='spanUpload" + columnDef.ColumnDefId + "'></span>";

                    layOut = layOut.Replace("[P" + pageTemplateId + "_" + columnDef.ColumnName + "]", formLayout.FileAttachment(columnDef));
                    layOut = layOut.Replace("[P" + pageTemplateId + "_" + columnDef.ColumnName + "LINK]", linkUpload);

                }
                else
                {
                    if (columnDef.ElementType.Length > 2)
                    {
                        Type type = typeof(FormLayout);

                        var replaceWith = (string)type.InvokeMember(columnDef.ElementType, BindingFlags.InvokeMethod, null, formLayout, new object[] { columnDef });
                        layOut = layOut.Replace("[P" + pageTemplateId + "_" + columnDef.ColumnName + "]", replaceWith);
                    }

                }
            }

        }



    }




}

/*  

	 
*/
