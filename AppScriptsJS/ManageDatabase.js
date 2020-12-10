var ToColumnIndex = "";
function HighlightItem(id) {
    $(".highlight-item").removeClass("select-bar-active");
    $("#" + id).addClass("select-bar-active");
}
function GetDatabaseList() {
    $.ajax({
        url: "./Database/GetDatabaseList",
        type: "POST",
        dataType: "json",
        success: function (records) {
            var obj = [];
            obj.push("<ul id='objectExplorer'>");
            // record rows
            for (var i = 0; i < records.length; i++) {
                var row = records[i];
                obj.push("<li>");
                obj.push("<span id='expand_Databases" + row.AppDatabaseId + "' class='xxx' onclick='ExpandDatabases(this)'></span> <span id='select_AppDatabaseId" + row.AppDatabaseId + "' class='highlight-item select_AppDatabaseId'>" + row.DatabaseName + "</span>");
                obj.push("<ul id='expand_Databases" + row.AppDatabaseId + "_ul' class='caret-tree-nested'>");
                obj.push("<li>");
                obj.push("<span id='expand_Tables" + row.AppDatabaseId + "' class='xxx' onclick='ExpandTables(this)'></span> <span>Tables</span>");
                obj.push("<ul id='expand_Tables" + row.AppDatabaseId + "_ul' class='caret-tree-nested'>");
                obj.push("</ul>");
                obj.push("</li>");
                obj.push("</ul>");
                obj.push("</li>");
            }
            obj.push("</ul>");
            $("#divObjectExplorer").html(obj.join(""));
            $(".select_AppDatabaseId").click(function () {
                //alert("Under construction");
                HighlightItem($(this).attr("id"));
            });
        }
    });
}
function ExpandDatabases(t) {
    t.parentElement.querySelector(".caret-tree-nested").classList.toggle("caret-tree-active");
    t.classList.toggle("caret-tree-down");
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
//# sourceMappingURL=ManageDatabase.js.map