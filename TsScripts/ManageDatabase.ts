var ToColumnIndex = "";
var InitDatabase = ":";
function HighlightItem(id) {
    $(".highlight-item").removeClass("select-bar-active");
    $("#" + id).addClass("select-bar-active");
}

function GetDatabaseList() {
    $.ajax({
        url: "./Database/GetDatabaseList",
        type: "POST",
        dataType: "json",
        success: function (clientResponse) {
            if (clientResponse.Successful) {


                RandomRefreshObject = "";
                var myVar = setInterval(function () {

                    if (RandomRefreshObject.length > 0) {
                        if ($("#" + RandomRefreshObject).length) {
                            $(".object-explorer-option").mouseover(function () {
                                $(this).closest('li').find(".object-explorer-add:first").show();
                            });
                            $(".object-explorer-option").mouseout(function () {
                                $(this).closest('li').find(".object-explorer-add:first").hide();
                            });


                            $(".select_AppDatabaseId").click(function () {
                                HighlightItem($(this).attr("id"));
                            })

                            clearInterval(myVar);
                        }
                    }

                }, 400);

                var obj = [];
                obj.push("<ul id='objectExplorer'>");

                // record rows
                var records = JSON.parse(clientResponse.JsonData);
                for (var i = 0; i < records.length; i++) {

                    var row = records[i];

                    obj.push("<li>");
                    obj.push("<span id='expand_Databases" + row.AppDatabaseId + "' class='xxx' onclick='ExpandDatabases(this)'></span> <span id='select_AppDatabaseId" + row.AppDatabaseId + "' class='highlight-item select_AppDatabaseId'>" + row.DatabaseName + "</span>");
                    obj.push("<ul id='expand_Databases" + row.AppDatabaseId + "_ul' class='caret-tree-nested'>");

                    //Tables
                    obj.push("<li>");
                    obj.push("<span id='expand_Tables" + row.AppDatabaseId + "' class='xxx' onclick='ExpandTables(this)'></span> <span>Tables</span>");
                    obj.push("<span class='object-explorer-add' style='float:right;font-size:1em;display:none;margin-right:10px;cursor:pointer;'> [+] </span>");
                    obj.push("<ul id='expand_Tables" + row.AppDatabaseId + "_ul' class='caret-tree-nested'></ul>");
                    obj.push("</li>");


                    //Custom Options
                    obj.push("<li class='object-explorer-option'>");
                    obj.push("<span id='expand_CustomOptions" + row.AppDatabaseId + "' class='xxx' onclick='ExpandCustomOptions(this)'></span> <span>Custom Options</span>");
                    obj.push("<span class='object-explorer-add' style='float:right;font-size:1em;display:none;margin-right:10px;cursor:pointer;' onclick='EditCustomOption(" + row.AppDatabaseId + ", 0)'> [+] </span>");
                    obj.push("<ul id='expand_CustomOptions" + row.AppDatabaseId + "_ul' class='caret-tree-nested'></ul>");
                    obj.push("</li>");

                    obj.push("</ul>");
                    obj.push("</li>");

                }

                obj.push("</ul>");

                $("#divObjectExplorer").html(obj.join("") + AddAlive());

            } else {
                MessageBox("Error", clientResponse.ErrorMessage, false);
            }
        }
    });
}


function ExpandDatabases(t) {
    t.parentElement.querySelector(".caret-tree-nested").classList.toggle("caret-tree-active");
    t.classList.toggle("caret-tree-down");


    var id = $(t).attr("id");

    // InitDatabase
    if (InitDatabase.indexOf(":" + id + ":") == -1) {
        InitDatabase += id + ":";

        $.ajax({
            url: "./Database/InitDatabase",
            type: "POST",
            data: { appDatabaseId: id.replace("expand_Databases","") },
            dataType: "json",
            success: function (clientResponse) {
                if (clientResponse.Successful) {
                } else {
                    MessageBox("Error", clientResponse.ErrorMessage, false);
                }
            }
        });
    }
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

