function ExpandForms(appDatabaseId, t) {
    t.parentElement.querySelector(".caret-tree-nested").classList.toggle("caret-tree-active");
    t.classList.toggle("caret-tree-down");
    var id = $(t).attr("id");
    // expand_Forms
    if (id.indexOf("expand_Forms") > -1 && $(t).hasClass("caret-tree-down")) {
        GetFormList(appDatabaseId, id.replace("expand_Forms" + appDatabaseId, ""));
    }
}
function GetFormList(appDatabaseId, tableName) {
    AppSpinner(true);
    setTimeout(function () {
        $.ajax({
            url: "./Database/GetTableOjects",
            type: "POST",
            data: { appDatabaseId: appDatabaseId, tableName: tableName },
            dataType: "json",
            success: function (data) {
                var Forms = data.Forms;
                var obj = [];
                for (var i = 0; i < Forms.length; i++) {
                    var row = Forms[i];
                    obj.push("<li>");
                    obj.push("<span id='FormId" + row.FormId + "' onclick='SelectForm(" + row.FormId + ")' class='highlight-item'>" + row.FormName + "</span>");
                    obj.push("</li>");
                }
                $("#expand_Forms" + appDatabaseId + tableName + "_ul").html(obj.join(""));
            },
            complete: function () {
                AppSpinner(false);
            }
        });
    }, 300);
}
function SelectForm(FormId) {
    AppSpinner(true);
    setTimeout(function () {
        $.ajax({
            url: "./Home/GetPage",
            type: "POST",
            data: { id: 0, targetType: "", pageFile: "~/Views/App/EditForm.cshtml" },
            dataType: "text",
            success: function (response) {
                $("#divProperties").html(response);
                GetFormSchema(FormId);
                HighlightItem("FormId" + FormId);
            },
            complete: function () {
                AppSpinner(false);
            }
        });
    }, 500);
}
function GetFormSchema(formId) {
    $.ajax({
        url: "./Database/GetFormSchema",
        type: "POST",
        data: { formId: formId },
        dataType: "json",
        success: function (data) {
            BindForm("EditForm", data);
            // set FormType  0-Template Layout (template)   1-Custom Layout (custom)  2-Page File (page)
            var formType = 0;
            switch (data.FormType) {
                case "template":
                    formType = 0;
                    break;
                case "custom":
                    formType = 1;
                    break;
                case "page":
                    formType = 2;
                    break;
            }
            TabIt("FormTab", formType);
            SetFormLayout(formId);
        },
        complete: function () {
            AppSpinner(false);
        }
    });
}
function SetFormLayout(formId) {
    $.ajax({
        url: "./Database/GetFormSchema",
        type: "POST",
        data: { formId: formId },
        dataType: "json",
        success: function (data) {
            var obj = [];
            // FormAvailableColumns     
            var availColumns = data.AvailableColumns;
            obj.push("<ul id='availColumns_0_0' class='connectedFormColumns'>");
            obj.push("<li id='AvailColumn_EmptyColumn'>");
            obj.push("--- Drag here to make available ---");
            obj.push("</li>");
            for (var i = 0; i < availColumns.length; i++) {
                var row = availColumns[i];
                obj.push("<li id='AvailColumn_" + row.ColumnName + "'>");
                obj.push("" + row.ColumnName + "");
                obj.push("</li>");
            }
            obj.push("</ul>");
            $("#FormAvailableColumns").html(obj.join(""));
            var sortableSections = "availColumns_0_0";
            // FormSections
            obj.length = 0;
            var formSections = data.FormSections;
            obj.push("<ul id='formSections' class='form-section-header'>");
            for (var i = 0; i < formSections.length; i++) {
                var row = formSections[i];
                obj.push("<li id='FormSectionId" + row.FormSectionId + "'>");
                obj.push("<span class='edit-formlayout-section' formSectionId='" + row.FormSectionId + "'>" + row.FormSectionId + ": " + row.SectionHeader + "</span>");
                // loop for columns in section column
                obj.push("<table border='1'>");
                obj.push("<tr>");
                for (var x = 1; x <= row.ColumnCount; x++) {
                    sortableSections += ",FormSection_" + row.FormSectionId + "_" + x;
                    obj.push("<td valign='top'>");
                    obj.push("<ul id='FormSection_" + row.FormSectionId + "_" + x + "' class='connectedFormColumns'>");
                    var sectionColumns = data.FormColumns.filter(function (w) { return w.FormSectionId == row.FormSectionId && w.SectionColumn == x; });
                    if (sectionColumns.length > 0) {
                        for (var y = 0; y < sectionColumns.length; y++) {
                            obj.push("<li id='SetColumn_" + sectionColumns[y].ColumnName + "'>");
                            obj.push("<span class='edit-formlayout-column'>" + sectionColumns[y].ColumnName + "</span>");
                            obj.push("</li>");
                        }
                    }
                    else {
                        obj.push("<li id='SetColumn_EmptyColumn'>");
                        obj.push("--- Drag here ---");
                        obj.push("</li>");
                    }
                    obj.push("</ul>");
                    obj.push("</td>");
                }
                obj.push("</tr>");
                obj.push("</table>");
                obj.push("</li>");
            }
            obj.push("</ul>");
            $("#FormSections").html(obj.join(""));
            $("#formSections").sortable({
                stop: function (event, ui) {
                    var id = $(ui.item).attr("id").replace("FormSectionId", "");
                    var newOrder = ui.item.index() + 1;
                    console.log("Order Sections:  id=" + id + "    newOrder=" + newOrder);
                    SortFormSection(formId, id, newOrder);
                }
            });
            var ray = sortableSections.split(",");
            for (var i = 0; i < ray.length; i++) {
                $("#" + ray[i]).sortable({
                    connectWith: ".connectedFormColumns",
                    start: function (event, ui) {
                        ui.item.startPos = ui.item.index();
                        ui.placeholder.height(ui.item.height());
                        TargetId = $(this).attr("id");
                    },
                    stop: function (event, ui) {
                        var id = $(ui.item).attr("id").replace("xxx", "");
                        var newOrder = ui.item.index() + 1;
                        // null moving available only  
                        if (id.indexOf("AvailColumn_") > -1 && TargetId.indexOf("availColumns") > -1) {
                            return;
                        }
                        //console.log("Move column    id=" + id + "    TargetId=" + TargetId + "    newOrder=" + newOrder);
                        var columnName = id.replace("SetColumn_", "").replace("AvailColumn_", "");
                        var ray = TargetId.split("_");
                        var toFormSectionId = ray[1];
                        var toSectionColumn = ray[2];
                        console.log("Move column    formId=" + formId + "    columnName=" + columnName + "    toFormSectionId=" + toFormSectionId + "    toSectionColumn=" + toSectionColumn + "    newOrder=" + newOrder);
                        SortFormColumn(formId, columnName, toFormSectionId, toSectionColumn, newOrder);
                    },
                    receive: function (event, ui) {
                        try {
                            TargetId = $(event.target).attr("id");
                        }
                        catch (e) {
                            TargetId = "";
                        }
                    }
                });
            }
            // edit section
            $(".edit-formlayout-section").click(function () {
                console.log("edit section=" + $(this).attr("formSectionId"));
            });
            // edit column
            $(".edit-formlayout-column").click(function () {
                console.log("edit columnName=" + $(this).html());
            });
        },
        complete: function () {
            AppSpinner(false);
        }
    });
}
function SortFormSection(formId, formSectionId, newOrder) {
    $.ajax({
        url: "./Database/SortFormSection",
        type: "POST",
        data: { formId: formId, formSectionId: formSectionId, newOrder: newOrder },
        dataType: "json",
        success: function (clientResponse) {
            if (clientResponse.Successful) {
                SetFormLayout(formId);
            }
            else {
                MessageBox("Error", clientResponse.ErrorMessage, false);
            }
        }
    });
}
function SortFormColumn(formId, columnName, toFormSectionId, toSectionColumn, newOrder) {
    $.ajax({
        url: "./Database/SortFormColumn",
        type: "POST",
        data: { formId: formId, columnName: columnName, toFormSectionId: toFormSectionId, toSectionColumn: toSectionColumn, newOrder: newOrder },
        dataType: "json",
        success: function (clientResponse) {
            if (clientResponse.Successful) {
                SetFormLayout(formId);
            }
            else {
                MessageBox("Error", clientResponse.ErrorMessage, false);
            }
        }
    });
}
//# sourceMappingURL=ManageForm.js.map