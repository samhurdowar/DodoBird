function ExpandTables(t) {

    t.parentElement.querySelector(".caret-tree-nested").classList.toggle("caret-tree-active");
    t.classList.toggle("caret-tree-down");

    var id = $(t).attr("id");

    // expand_Tables 
    if (id.indexOf("expand_Tables") > -1 && $(t).hasClass("caret-tree-down")) {
        GetTableList(id.replace("expand_Tables", ""));
    }
}


function ExpandTable(t) {
    t.parentElement.querySelector(".caret-tree-nested").classList.toggle("caret-tree-active");
    t.classList.toggle("caret-tree-down");
}

function GetTableList(appDatabaseId) {

    AppSpinner(true);
    setTimeout(function () {
        $.ajax({
            url: "./Database/GetTableList",
            type: "POST",
            data: { appDatabaseId: appDatabaseId },
            dataType: "json",
            success: function (records) {
                var obj = [];
                var id = "";
                // record rows - tables
                for (var i = 0; i < records.length; i++) {

                    var row = records[i];


                    id = appDatabaseId + row.TableName;

                    obj.push("<li>");
                    obj.push("<span id='expand_Table" + id + "' class='xxx' onclick='ExpandTable(this)'></span> <span id='select_TableName" + id + "' class='highlight-item select_TableName'>" + row.TableName + "</span>");
                    obj.push("<ul id='expand_Table" + id + "_ul' class='caret-tree-nested'>");

                    obj.push("<li>");
                    obj.push("<span id='expand_Fields" + id + "' class='xxx' onclick='ExpandColumns(" + appDatabaseId + ",this)'></span> <span style='margin-left:3px;'>Columns</span>");
                    obj.push("<ul id='expand_Fields" + id + "_ul' class='caret-tree-nested'>");
                    obj.push("</ul>");
                    obj.push("</li>");

                    obj.push("<li>");
                    obj.push("<span id='expand_Grids" + id + "' class='xxx' onclick='ExpandGrids(" + appDatabaseId + ",this)'></span> <span style='margin-left:3px;'>Grids</span>");
                    obj.push("<ul id='expand_Grids" + id + "_ul' class='caret-tree-nested'>");
                    obj.push("</ul>");
                    obj.push("</li>");

                    obj.push("<li>");
                    obj.push("<span id='expand_Forms" + id + "' class='caret-tree'></span> <span style='margin-left:3px;'>Forms</span>");
                    obj.push("<ul id='expand_Forms" + id + "_ul' class='caret-tree-nested'>");
                    obj.push("</ul>");
                    obj.push("</li>");


                    obj.push("<li>");
                    obj.push("<span id='expand_Views" + id + "' class='caret-tree'></span> <span style='margin-left:3px;'>Views</span>");
                    obj.push("<ul id='expand_Views" + id + "_ul' class='caret-tree-nested'>");
                    obj.push("</ul>");
                    obj.push("</li>");

                    obj.push("<li>");
                    obj.push("<span id='expand_Searches" + id + "' class='caret-tree'></span> <span style='margin-left:3px;'>Search Forms</span>");
                    obj.push("<ul id='expand_Searches" + id + "_ul' class='caret-tree-nested'>");
                    obj.push("</ul>");
                    obj.push("</li>");


                    obj.push("</ul>");
                    obj.push("</li>");

                }

                $("#expand_Tables" + appDatabaseId + "_ul").html(obj.join(""));

                // select_TableName
                $(".select_TableName").click(function () {
                    var id = $(this).attr("id");
                    HighlightItem(id);
                    SelectTable(appDatabaseId, id.replace("select_TableName" + appDatabaseId, ""));
                })

            },
            complete: function () {
                AppSpinner(false);
            }
        });
    }, 500);
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

                        EnableButton("cmd_Delete_EditTable");


                        // dependent tables
                        var obj = [];
                        if (data.TableSchema.DependentTables.length > 0) {
                            

                            var dependTables1 = data.TableSchema.DependentTables;
                            obj.push("<table class='table-padding-sm' style='width:100%;'>");
                            obj.push("<tr>");
                            obj.push("<td>ParentTableName</td>");
                            obj.push("<td>ParentKey</td>");
                            obj.push("<td>TableName</td>");
                            obj.push("<td>DependentKey</td>");
                            obj.push("<td>JoinType</td>");
                            obj.push("<td>Relation</td>");
                            obj.push("</tr>");



                            for (var i = 0; i < dependTables1.length; i++) {
                                var row1 = dependTables1[i];
                                obj.push("<tr>");
                                obj.push("<td >" + row1.ParentOwner + "." + row1.ParentTableName + "</td>");
                                obj.push("<td >" + row1.ParentKey + "</td>");
                                obj.push("<td >" + row1.Owner + "." + row1.TableName + "</td>");
                                obj.push("<td >" + row1.DependentKey + "</td>");
                                obj.push("<td >" + row1.JoinType + "</td>");
                                obj.push("<td >" + row1.Relation + "</td>");
                                obj.push("</tr>");


                                if (dependTables1[i].DependentTables.length > 0) {


                                    var dependTables2 = dependTables1[i].DependentTables;
                                    for (var x = 0; x < dependTables2.length; x++) {
                                        var row2 = dependTables2[x];
                                        obj.push("<tr>");
                                        obj.push("<td >" + row2.ParentOwner + "." + row2.ParentTableName + "</td>");
                                        obj.push("<td >" + row2.ParentKey + "</td>");
                                        obj.push("<td >" + row2.Owner + "." + row2.TableName + "</td>");
                                        obj.push("<td >" + row2.DependentKey + "</td>");
                                        obj.push("<td >" + row2.JoinType + "</td>");
                                        obj.push("<td >" + row2.Relation + "</td>");
                                        obj.push("</tr>");

                                    }

                                }
                            }
                            obj.push("</table>");
                            $("#divDependentTables").html(obj.join(""));
                        }
                    }
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


            BindForm("EditTable", { OldTableName: "NEWTABLE", TableName: "" })

            DisableButton("cmd_Delete_EditTable");
        }
    });
}