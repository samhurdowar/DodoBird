function ExpandColumns(appDatabaseId, t) {
    t.parentElement.querySelector(".caret-tree-nested").classList.toggle("caret-tree-active");
    t.classList.toggle("caret-tree-down");
    var id = $(t).attr("id");
    // expand_Fields
    if (id.indexOf("expand_Fields") > -1 && $(t).hasClass("caret-tree-down")) {
        GetColumnList(appDatabaseId, id.replace("expand_Fields" + appDatabaseId, ""));
    }
}
function GetColumnList(appDatabaseId, tableName) {
    AppSpinner(true);
    setTimeout(function () {
        $.ajax({
            url: "./Database/GetTableOjects",
            type: "POST",
            data: { appDatabaseId: appDatabaseId, tableName: tableName },
            dataType: "json",
            success: function (data) {
                // TableSchema.Columns
                var columns = data.TableSchema.Columns;
                var obj = [];
                for (var i = 0; i < columns.length; i++) {
                    var row = columns[i];
                    obj.push("<li>");
                    obj.push("<span id='ColumnName" + row.ColumnName + "' tableName='" + tableName + "' onclick='SelectColumn(" + appDatabaseId + ",this)' class='highlight-item'>" + row.ColumnName + "</span>");
                    obj.push("</li>");
                }
                $("#expand_Fields" + appDatabaseId + tableName + "_ul").html(obj.join(""));
            },
            complete: function () {
                AppSpinner(false);
            }
        });
    }, 300);
}
function SelectColumn(appDatabaseId, t) {
    AppSpinner(true);
    setTimeout(function () {
        $.ajax({
            url: "./Home/GetPage",
            type: "POST",
            data: { id: 0, targetType: "", pageFile: "~/Views/App/EditColumn.cshtml" },
            dataType: "text",
            success: function (response) {
                var columnName = $(t).attr("id").replace("ColumnName", "");
                var tableName = $(t).attr("tableName");
                // get form data
                $.ajax({
                    url: "./Database/GetTableOjects",
                    type: "POST",
                    data: { appDatabaseId: appDatabaseId, tableName: tableName },
                    dataType: "json",
                    success: function (data) {
                        $("#divProperties").html(response);
                        // TableSchema.Columns
                        var columnData = data.TableSchema.Columns.find(function (w) { return w.ColumnName == columnName; });
                        BindForm("EditColumn", columnData);
                        HighlightItem($(t).attr("id"));
                    },
                    complete: function () {
                        AppSpinner(false);
                    }
                });
            },
            complete: function () {
                AppSpinner(false);
            }
        });
    }, 300);
}
//# sourceMappingURL=ManageColumn.js.map