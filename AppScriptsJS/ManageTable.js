function HighlightTable(id) {
    $(".highlight-table").removeClass("select-bar-active");
    $("#highlight-table" + id).addClass("select-bar-active");
}
function SelectTable(appDatabaseId, tableName) {
    AppSpinner(true);
    setTimeout(function () {
        $.ajax({
            url: "./Database/GetTableOjects",
            type: "POST",
            data: { appDatabaseId: appDatabaseId, tableName: tableName },
            dataType: "json",
            success: function (data) {
                // EditTable
                $.ajax({
                    url: "./Home/GetPage",
                    type: "POST",
                    data: { id: 0, targetType: "", pageFile: "~/Views/App/EditTable.cshtml" },
                    dataType: "text",
                    success: function (response) {
                        $("#divProperties").html(response);
                        BindForm("EditTable", data.TableSchema);
                        $("#EditTable #OldTableName").val(data.TableSchema.TableName);
                    }
                });
                // TableSchema.Columns
                var columns = data.TableSchema.Columns;
                Columns = data.TableSchema.Columns;
                var objColumns = [];
                if (columns.length > 20) {
                    objColumns.push("<div class='scroll'>");
                }
                objColumns.push("<table class='table-padding-sm' style='width:100%;'>");
                for (var i = 0; i < columns.length; i++) {
                    var row = columns[i];
                    objColumns.push("<tr id='ColumnName" + row.ColumnName + "' appDatabaseId='" + appDatabaseId + "' tableName='" + tableName + "' columnName='" + row.ColumnName + "' onclick='SelectColumn(this)'>");
                    objColumns.push("<td id='highlight-objectColumnName" + row.ColumnName + "' class='select-bar highlight-object'>" + row.ColumnName + "</td>");
                    objColumns.push("</tr>");
                }
                objColumns.push("</table>");
                if (columns.length > 20) {
                    objColumns.push("</div>");
                }
                $("#divColumns").html(objColumns.join(""));
                // Grids
                var grids = data.Grids;
                var objGrids = [];
                objGrids.push("<table style='width:100%;'>");
                for (var i = 0; i < grids.length; i++) {
                    var row = grids[i];
                    objGrids.push("<tr onclick='SelectGrid(" + row.GridId + ")'>");
                    objGrids.push("<td id='highlight-objectGridId" + row.GridId + "' class='select-bar highlight-object'>" + row.GridName + "</td>");
                    objGrids.push("</tr>");
                }
                objGrids.push("</table>");
                $("#divGrids").html(objGrids.join(""));
                // Forms
                var forms = data.Forms;
                var objForms = [];
                objForms.push("<table style='width:100%;'>");
                for (var i = 0; i < forms.length; i++) {
                    var row = forms[i];
                    objForms.push("<tr onclick='SelectForm(" + row.FormId + ")'>");
                    objForms.push("<td id='highlight-objectFormId" + row.FormId + "' class='select-bar highlight-object'>" + row.FormName + "</td>");
                    objForms.push("</tr>");
                }
                objForms.push("</table>");
                $("#divForms").html(objForms.join("") + AddAlive());
                $(".data-objects").show();
                // set hightlight click
                $(".highlight-object").click(function () {
                    var id = $(this).attr("id").replace("highlight-object", "");
                    HighlightObject(id);
                });
            },
            complete: function () {
                AppSpinner(false);
            }
        });
    }, 500);
}
function AddTable() {
    $.ajax({
        url: "./Home/GetPage",
        type: "POST",
        data: { id: 0, targetType: "", pageFile: "~/Views/App/EditTable.cshtml" },
        dataType: "text",
        success: function (response) {
            $("#divProperties").html(response);
            BindForm("EditTable", { OldTableName: "NEWTABLE", TableName: "" });
        }
    });
}
//# sourceMappingURL=ManageTable.js.map