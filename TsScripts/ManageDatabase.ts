
function GetDatabaseList() {
    $.ajax({
        url: "./Database/GetDatabaseList",
        type: "POST",
        dataType: "json",
        success: function (records) {
            var obj = [];
            obj.push("<table style='width:100%;'>");

            // record rows
            for (var i = 0; i < records.length; i++) {

                var row = records[i];
                obj.push("<tr onclick='SelectDatabase(" + row.AppDatabaseId + ")'>");
                obj.push("<td class='command-bar-select1'>" + row.DatabaseName + "</td>");
                obj.push("</tr>");
            }

            obj.push("</table>");

            $("#divAppDatabases").html(obj.join(""));

            SetCommandBarDOM();
        }
    });
}


//GetTableList
function SelectDatabase(appDatabaseId) {
    AppSpinner(true);
    setTimeout(function () {
        $.ajax({
            url: "./Database/GetTableList",
            type: "POST",
            data: { appDatabaseId: appDatabaseId },
            dataType: "json",
            success: function (records) {
                var obj = [];
                obj.push("<div class='scroll'><table style='width:100%;'>");

                // record rows - tables
                for (var i = 0; i < records.length; i++) {

                    var row = records[i];
                    obj.push("<tr onclick=\"SelectTable(" + appDatabaseId + ",'" + row.TableName + "')\">");
                    obj.push("<td class='command-bar-select2'>" + row.TableName + "</td>");
                    obj.push("</tr>");
                }

                obj.push("</table></div>");

                $("#divAppTables").html(obj.join(""));

                SetCommandBarDOM();

                // clear divAppColumns
                $("#divAppColumns").html("");
            },
            complete: function () {
                AppSpinner(false);
            }
        });
    }, 500);


}


var Columns;
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
                    objColumns.push("<td class='command-bar-select3'>" + row.ColumnName + "</td>");
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
                    objGrids.push("<td class='command-bar-select3'>" + row.GridName + "</td>");
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
                    objForms.push("<td class='command-bar-select3'>" + row.FormName + "</td>");
                    objForms.push("</tr>");
                }
                objForms.push("</table>");

                $("#divForms").html(objForms.join(""));

                $(".data-objects").show();
                

                SetCommandBarDOM();
            },
            complete: function () {
                AppSpinner(false);
            }
        });
    }, 500);

}


function SelectColumn(t) {
    AppSpinner(true);
    setTimeout(function () {
        $.ajax({
            url: "./Home/GetPage",
            type: "POST",
            data: { id: 0, targetType: "", pageFile: "~/Views/App/EditColumn.cshtml" },
            dataType: "text",
            success: function (response) {
                $("#divProperties").html(response);

                var columnName = $(t).attr("columnName");
                let columnData = Columns.filter(f => f.ColumnName == columnName);
                BindForm("EditColumn", columnData[0]);
            },
            complete: function () {
                AppSpinner(false);
            }
        });

    }, 500);
}

function SelectGrid(gridId) {

    AppSpinner(true);
    setTimeout(function () {
        $.ajax({
            url: "./Home/GetPage",
            type: "POST",
            data: { id: 0, targetType: "", pageFile: "~/Views/App/EditGrid.cshtml" },
            dataType: "text",
            success: function (response) {
                $("#divProperties").html(response);
                GetGridSchema(gridId);
            },
            complete: function () {
                AppSpinner(false);
            }
        });

    }, 500);


}

var ToColumnIndex = "";


function GetGridSchema(gridId) {
    $.ajax({
        url: "./Database/GetGridSchema",
        type: "POST",
        data: { gridId: gridId },
        dataType: "json",
        success: function (data) {

            BindForm("EditGrid", data);

            TabIt("gridTab", data.GridType);

            // set AvailableColumns
            var obj1 = [];
            obj1.push("<ul id='sortableGrid1' columnIndex='0' class='connectedSortable'>");
            for (var i = 0; i < data.AvailableColumns.length; i++) {

                var row = data.AvailableColumns[i];
                obj1.push("<li id='GridColumn_" + row.ColumnName + "' columnIndex='0'>" + row.ColumnName + "</li>");
            }
            obj1.push("</ul>");

            $("#availableColumns").html(obj1.join(""));


            // set gridColumns
            var obj2 = [];
            obj2.push("<ul id='sortableGrid2' columnIndex='1' class='connectedSortable'>");
            for (var i = 0; i < data.GridColumns.length; i++) {

                var row = data.GridColumns[i];
                obj2.push("<li id='GridColumn_" + row.ColumnName + "' columnIndex='1'>" + row.ColumnName + "</li>");
            }
            obj2.push("</ul>");

            $("#gridColumns").html(obj2.join(""));


            $("#sortableGrid1, #sortableGrid2").sortable({
                connectWith: ".connectedSortable",
                start: function (event, ui) {
                    ui.item.startPos = ui.item.index();
                    ui.placeholder.height(ui.item.height());
                },
                stop: function (event, ui) {
                    var id = $(ui.item).attr("id").replace("GridColumn_","");
                    var newOrder = ui.item.index() + 1;

                    var columnIndex = $(ui.item).attr("columnIndex");


                    if (ToColumnIndex == "") ToColumnIndex = columnIndex;
                    console.log("gridId=" + gridId + "    ColumnName=" + id + "    FromColumnIndex=" + columnIndex + "    ToColumnIndex=" + ToColumnIndex + "    newOrder=" + newOrder);

                    SortGridColumn(gridId, id, columnIndex, ToColumnIndex, newOrder);
                },
                receive: function (event, ui) {
                    try {
                        ToColumnIndex = $(event.target).attr("columnIndex");
                    } catch (e) {
                        ToColumnIndex = "";
                    }
                }
            });
        },
        complete: function () {
            AppSpinner(false);
        }
    });

}

function SortGridColumn(gridId, columnName, fromIndex, toIndex, newOrder) {
    $.ajax({
        url: "./Database/SortGridColumn",
        type: "POST",
        data: { gridId: gridId, columnName: columnName, fromIndex: fromIndex, toIndex: toIndex, newOrder: newOrder },
        dataType: "text",
        success: function (response) {
            GetGridSchema(gridId);
        }
    });
}



function SaveDatabase() {

    if (ValidateForm("FormDatabase")) {
        return;
    }

    var json = ToJsonString("FormDatabase");
    //console.log("json=" + json);

    $.ajax({
        url: "./Database/SortGridColumn",
        type: "POST",
        data: { json: json },
        dataType: "text",
        success: function (response) {

        }
    });

}

