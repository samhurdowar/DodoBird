function ExpandGrids(appDatabaseId, t) {

    t.parentElement.querySelector(".caret-tree-nested").classList.toggle("caret-tree-active");
    t.classList.toggle("caret-tree-down");

    var id = $(t).attr("id");

    // expand_Grids
    if (id.indexOf("expand_Grids") > -1 && $(t).hasClass("caret-tree-down")) {
        GetGridList(appDatabaseId, id.replace("expand_Grids" + appDatabaseId, ""));
    }
}


function GetGridList(appDatabaseId, tableName) {
    AppSpinner(true);
    setTimeout(function () {

        $.ajax({
            url: "./Database/GetTableOjects",
            type: "POST",
            data: { appDatabaseId: appDatabaseId, tableName: tableName },
            dataType: "json",
            success: function (data) {

                var grids = data.Grids;
                var obj = [];

                for (var i = 0; i < grids.length; i++) {
                    var row = grids[i];

                    obj.push("<li>");
                    obj.push("<span id='HighlightGridId" + row.GridId + "' gridId='" + row.GridId + "' appDatabaseId='" + appDatabaseId + "' tableName='" + tableName + "' onclick='SelectGrid(this)' class='highlight-item'>" + row.GridName + "</span>");
                    obj.push("</li>");
                }

                $("#expand_Grids" + appDatabaseId + tableName + "_ul").html(obj.join(""));

            },
            complete: function () {
                AppSpinner(false);
            }
        });
    }, 300);
}

function GetToFormOptions(appDatabaseId, tableName) {
    var data = Lookups["Forms"].filter(w => w.TableName == tableName && w.AppDatabaseId == appDatabaseId);
    var obj = [];
    obj.push("<option value=\"0\">None</option>");
    for (var i in data) {
        var row = data[i];
        obj.push("<option value=\"" + row.FormId + "\">" + row.FormName + "</option>");
    }
    var str = obj.join("");
    $("#EditGrid #ToFormId").html(str);
}


function SelectGrid(t) {
    var gridId = $(t).attr("gridId");
    var appDatabaseId = $(t).attr("appDatabaseId");
    var tableName = $(t).attr("tableName");

    AppSpinner(true);
    setTimeout(function () {
        $.ajax({
            url: "./Home/GetPage",
            type: "POST",
            data: { id: 0, targetType: "", pageFile: "~/Views/App/EditGrid.cshtml" },
            dataType: "text",
            success: function (response) {


                // Set before object is created 
                RandomRefreshObject = "";
                var interval1 = setInterval(function () {
                    console.log("SelectGrid - Looking for RandomRefreshObject=" + RandomRefreshObject);


                    if (RandomRefreshObject.length > 0) {
                        if ($("#" + RandomRefreshObject).length) {
                            console.log("SelectGrid - Found RandomRefreshObject=" + RandomRefreshObject);


                            GetGridSchema(gridId);
                            HighlightItem("HighlightGridId" + gridId);
                            GetToFormOptions(appDatabaseId, tableName)

                            clearInterval(interval1);
                        }
                    }

                }, 300);


                $("#divProperties").html(response + AddAlive());

            },
            complete: function () {
                AppSpinner(false);
            }
        });

    }, 300);
}

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
                    var id = $(ui.item).attr("id").replace("GridColumn_", "");
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
