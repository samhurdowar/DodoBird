
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

namespace DodoBird.Controllers
{
    public class FormController : Controller
    {
        [HttpPost]
        public string GetFormSchema(int formId)
        {
            FormSchema formSchema = DataService.GetFormSchema(formId);

            var clientResponse = HelperService.GetJsonData(formSchema.AppDatabaseId, "SELECT * FROM Form WHERE FormId = @FormId", new SqlParameter[] { new SqlParameter("@FormId", formId) } );
            return clientResponse.JsonData;
        }

        [HttpPost]
        public string GetFormData(string json)
        {
            var response = DataService.GetFormData(json);
            return response;
        }


        [HttpPost]
        [ValidateInput(false)]
        public string SaveFormData(string json)
        {
            try
            {
                dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);

                var primaryKeys_ = jsonObj["PrimaryKeys"].ToString();
                dynamic primaryKeys = Newtonsoft.Json.JsonConvert.DeserializeObject(primaryKeys_);

                var formId = Convert.ToInt32(primaryKeys["FormId"]);
                var formSchema = DataService.GetFormSchema(formId);

                ClientResponse clientResponse = DataService.SaveFormData(formSchema.AppDatabaseId, formSchema.TableName, json);

                var jsonClientResponse = JsonConvert.SerializeObject(clientResponse);
                return jsonClientResponse;
            }
            catch (System.Exception ex)
            {
                return JsonConvert.SerializeObject(new ClientResponse { Successful = false, Id = "", ActionExecuted = "ERROR-SaveFormData", ErrorMessage = ex.Message });
            }



        }

        /*
         
sb.AppendLine("<div class='command-bar-container'>");


        

         
         */

        [HttpPost]
        public string GetFormLayout(int formId)
        {
            try
            {

                FormSchema formSchema = DataService.GetFormSchema(formId);

                StringBuilder sb = new StringBuilder();


                sb.AppendLine("<div class='command-bar-container'>");
                sb.AppendLine("    <span id='cmd_GoBack_FormId" + formId + "' class='command-active-span'><span class='command-icon fas fa-arrow-left'>&nbsp;</span> Go Back</span>");
                sb.AppendLine("    <span id='cmd_Save_FormId" + formId + "' class='command-disabled-span'><span class='command-icon fas fa-save'>&nbsp;</span> Save</span>");
                sb.AppendLine("</div>");


                sb.AppendLine("<form id='FormId" + formId + "'>");
                sb.AppendLine("PrimaryKeys: <input type='text' id='PrimaryKeys' style='width:400px;' />");

                // loop sections 
                foreach (var formSection in formSchema.FormSections)
                {
                    sb.AppendLine("<div style='background-color:#ccc;padding:5px;' class='form-section-header'>");
                    sb.AppendLine(formSection.SectionHeader);
                    sb.AppendLine("</div>");

                    sb.AppendLine("<div class='form-section-content'>");

                    sb.AppendLine("<table>");
                    sb.AppendLine("<tr>");
                    for (int i = 1; i <= formSection.ColumnCount; i++)
                    {
                        sb.AppendLine("<td valign='top'>");

                        sb.AppendLine("<table>");
                        foreach (var formColumn in formSchema.FormColumns.Where(w => w.FormSectionId == formSection.FormSectionId && w.SectionColumn == i).OrderBy(o => o.ColumnOrder))
                        {
                            sb.AppendLine("<tr>");
                            sb.AppendLine("<td>");
                            sb.AppendLine(formColumn.ColumnName);
                            sb.AppendLine("</td>");

                            sb.AppendLine("<td>");

                            sb.AppendLine("<input type='text' id='" + formColumn.ColumnName + "' />");
                            sb.AppendLine("</td>");

                            sb.AppendLine("</tr>");
                        }

                        sb.AppendLine("</table>");


                        sb.AppendLine("</td>");
                    }

                    sb.AppendLine("</tr>");
                    sb.AppendLine("</table>");

                    sb.AppendLine("</div>");

                }
                sb.AppendLine("</form>");


                // javascript

                sb.AppendLine("<script>");
                sb.AppendLine("    function Save_FormId" + formId + "() {");
                sb.AppendLine("        AppSpinner(true);");
                sb.AppendLine("        var json = ToJsonString(\"FormId" + formId + "\");");
                sb.AppendLine("        setTimeout(function () {");
                sb.AppendLine("            $.ajax({");
                sb.AppendLine("                url: './Form/SaveFormData',");
                sb.AppendLine("                type: 'POST',");
                sb.AppendLine("                data: { json: json },");
                sb.AppendLine("                dataType: 'text',");
                sb.AppendLine("                success: function (clientResponse) {");
                sb.AppendLine("                    if (clientResponse.Successful) {");
                sb.AppendLine("                        DisableButton('cmd_Save_FormId" + formId + "');");
                sb.AppendLine("                    } else {");
                sb.AppendLine("                        MessageBox(\"Error\", clientResponse.ErrorMessage, false);");
                sb.AppendLine("                    }");
                sb.AppendLine("                },");
                sb.AppendLine("                complete: function () {");
                sb.AppendLine("                    AppSpinner(false);");
                sb.AppendLine("                }");
                sb.AppendLine("            });");
                sb.AppendLine("        }, 300);");
                sb.AppendLine("    }");
                sb.AppendLine("</script>");


                return JsonConvert.SerializeObject(new ClientResponse { Successful = true, Id = formId.ToString(), ActionExecuted = "GetFormLayout", ErrorMessage = "", JsonData = sb.ToString() });
            }
            catch (System.Exception ex)
            {
                return JsonConvert.SerializeObject(new ClientResponse { Successful = false, Id = formId.ToString(), ActionExecuted = "GetFormLayout", ErrorMessage = ex.Message });
            }



        }
    }
}