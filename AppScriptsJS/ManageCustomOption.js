function ExpandCustomOptions(t) {
    t.parentElement.querySelector(".caret-tree-nested").classList.toggle("caret-tree-active");
    t.classList.toggle("caret-tree-down");
    var id = $(t).attr("id");
    // expand_Tables 
    if (id.indexOf("expand_CustomOptions") > -1 && $(t).hasClass("caret-tree-down")) {
        GetCustomOptionList(id.replace("expand_CustomOptions", ""));
    }
}
function GetCustomOptionList(appDatabaseId) {
    AppSpinner(true);
    setTimeout(function () {
        $.ajax({
            url: "./Database/GetCustomOptionList",
            type: "POST",
            data: { appDatabaseId: appDatabaseId },
            dataType: "json",
            success: function (clientResponse) {
                if (clientResponse.Successful) {
                    RandomRefreshObject = "";
                    var myVar = setInterval(function () {
                        if (RandomRefreshObject.length > 0) {
                            if ($("#" + RandomRefreshObject).length) {
                                $(".select_CustomOptionTable").click(function () {
                                    var id = $(this).attr("id");
                                    HighlightItem(id);
                                    //xxx SelectTable(appDatabaseId, id.replace("select_TableName" + appDatabaseId, ""));
                                });
                                AppSpinner(false);
                                clearInterval(myVar);
                            }
                        }
                    }, 400);
                    var obj = [];
                    var id = "";
                    // record rows - tables
                    var records = JSON.parse(clientResponse.JsonData);
                    for (var i = 0; i < records.length; i++) {
                        var row = records[i];
                        id = appDatabaseId + row.TableName;
                        obj.push("<li>");
                        obj.push("<span id='select_CustomOptionTable" + id + "' class='highlight-item select_CustomOptionTable'>" + row.TableName + "</span>");
                        obj.push("</li>");
                    }
                    $("#expand_CustomOptions" + appDatabaseId + "_ul").html(obj.join("") + AddAlive());
                }
                else {
                    MessageBox("Error", clientResponse.ErrorMessage, false);
                }
            }
        });
    }, 400);
}
//# sourceMappingURL=ManageCustomOption.js.map