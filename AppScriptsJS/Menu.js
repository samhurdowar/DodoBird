var SortableMenuItems = "";
var ObjMenu = [];
var RefreshItem = "RootMenus";
var HighlightedItem = "";
var ParentId = "0";
function GetAdminMenuList() {
    $.ajax({
        url: "./Menu/GetAdminMenuList",
        dataType: "json",
        success: function (data) {
            SortableMenuItems = "RootMenus";
            ObjMenu.length = 0;
            // get main menus
            var mainMenus;
            if (RefreshItem == "RootMenus") {
                mainMenus = data.filter(function (it) { return it.ParentId == 0; });
            }
            else {
                mainMenus = data.filter(function (it) { return it.MenuId == RefreshItem.replace("MenuId", ""); });
            }
            mainMenus = SortJson(mainMenus, "SortOrder");
            for (var i in mainMenus) {
                var row = mainMenus[i];
                var subMenus = data.filter(function (w) { return w.ParentId == row.MenuId; });
                subMenus = SortJson(subMenus, "SortOrder");
                if (subMenus.length > 0) {
                    ObjMenu.push("<li id='MenuId" + row.MenuId + "' class='refresh-item'>");
                    ObjMenu.push("<div class='highlight parent-menu-item'>");
                    ObjMenu.push("<span menuId='" + row.MenuId + "' class='menu-item-edit'>" + row.MenuId + "-" + row.MenuTitle + "</span>");
                    ObjMenu.push("<span class='menu-add-delete'>");
                    ObjMenu.push("      <span class='menu-item-add menu-item-command fas fa-plus'> &nbsp; </span>");
                    ObjMenu.push("      <span deleteId='" + row.MenuId + "' class='menu-item-delete menu-item-command fas fa-trash'>&nbsp;</span>");
                    ObjMenu.push("</span>");
                    ObjMenu.push("</div>");
                    GetAdminSubMenuList(row.MenuId, subMenus, data);
                    ObjMenu.push("</li>");
                }
                else {
                    ObjMenu.push("<li id='MenuId" + row.MenuId + "' class='highlight refresh-item menu-item'>");
                    ObjMenu.push("<span menuId='" + row.MenuId + "' class='menu-item-edit'>" + row.MenuId + "-" + row.MenuTitle + "</span>");
                    ObjMenu.push("<span class='menu-add-delete'>");
                    ObjMenu.push("      <span class='menu-item-add menu-item-command fas fa-plus'> &nbsp; </span>");
                    ObjMenu.push("      <span deleteId='" + row.MenuId + "' class='menu-item-delete menu-item-command fas fa-trash'>&nbsp;</span>");
                    ObjMenu.push("</span>");
                    ObjMenu.push("</li>");
                }
            }
            var r = Math.floor(Math.random() * 1001);
            var str = ObjMenu.join("") + "<span id='AmAlive" + r + "'></span>";
            $("#" + RefreshItem).html(str);
            RefreshDOM("RefreshMenuDOM()", "AmAlive" + r);
        }
    });
}
function RefreshDOM(functionToFire, amAlive) {
    var myVar = setInterval(function () {
        if ($("#" + amAlive).length) {
            eval(functionToFire);
            clearInterval(myVar);
        }
    }, 500);
}
function GetAdminSubMenuList(menuId, subMenus, data) {
    SortableMenuItems += ",Children" + menuId;
    ObjMenu.push("<ul id='Children" + menuId + "'>");
    // get sub menus
    for (var x in subMenus) {
        var row = subMenus[x];
        var subMenus_ = data.filter(function (w) { return w.ParentId == row.MenuId; });
        subMenus_ = SortJson(subMenus_, "SortOrder");
        if (subMenus_.length > 0) {
            ObjMenu.push("<li id='MenuId" + row.MenuId + "'>");
            ObjMenu.push("<div class='highlight parent-menu-item'>");
            ObjMenu.push("<span menuId='" + row.MenuId + "' class='menu-item-edit'>" + row.MenuId + "-" + row.MenuTitle + "</span>");
            ObjMenu.push("<span class='menu-add-delete'>");
            ObjMenu.push("      <span class='menu-item-add menu-item-command fas fa-plus'> &nbsp; </span>");
            ObjMenu.push("      <span deleteId='" + row.MenuId + "' class='menu-item-delete menu-item-command fas fa-trash'>&nbsp;</span>");
            ObjMenu.push("</span> ");
            ObjMenu.push("</div>");
            GetAdminSubMenuList(row.MenuId, subMenus_, data);
            ObjMenu.push("</li>");
        }
        else {
            ObjMenu.push("<li id='MenuId" + row.MenuId + "' class='highlight menu-item'>");
            ObjMenu.push("<span menuId='" + row.MenuId + "' class='menu-item-edit'> " + row.MenuId + "-" + row.MenuTitle + " </span>");
            ObjMenu.push("<span class='menu-add-delete'>");
            ObjMenu.push("      <span class='menu-item-add menu-item-command fas fa-plus'> &nbsp; </span>");
            ObjMenu.push("      <span deleteId='" + row.MenuId + "' class='menu-item-delete menu-item-command fas fa-trash'>&nbsp;</span>");
            ObjMenu.push("</span>");
            ObjMenu.push("</li>");
        }
    }
    ObjMenu.push("</ul>");
}
function RefreshMenuDOM() {
    var words = SortableMenuItems.split(",");
    for (var i = 0; i < words.length; i++) {
        $("#" + words[i]).sortable({
            stop: function (event, ui) {
                var menuId = $(ui.item).attr("id").replace("MenuId", "");
                var newOrder = ui.item.index() + 1;
                // get parent item to refresh 
                var obj = $(this).closest("li.refresh-item");
                RefreshItem = (obj.attr("id") == undefined) ? "RootMenus" : obj.attr("id");
                console.log("SortMenu    menuId=" + menuId + "    newOrder=" + newOrder + "    refreshItem=" + RefreshItem);
                SortMenu(menuId, newOrder);
            }
        });
    }
    // show add and delete
    $(".parent-menu-item").mouseover(function () {
        $(this).closest('div').find(".menu-add-delete:first").show();
    });
    $(".parent-menu-item").mouseout(function () {
        $(this).closest('div').find(".menu-add-delete:first").hide();
    });
    $(".menu-item").mouseover(function () {
        $(this).closest('li').find(".menu-add-delete:first").show();
    });
    $(".menu-item").mouseout(function () {
        $(this).closest('li').find(".menu-add-delete:first").hide();
    });
    // edit menu item
    $(".menu-item-edit").click(function () {
        var menuId = $(this).attr("menuId");
        // get parent item to refresh
        GetParentRefreshId(this);
        // highlight item
        HighlightMenuItem(this);
        SelectMenu(menuId);
        console.log("Edit menuId=" + menuId + "    RefreshItem=" + RefreshItem);
    });
    // add menu item
    $(".menu-item-add").click(function () {
        // get parent menuId
        var obj = $(this).closest("li");
        var parentId = obj.attr("id").replace("MenuId", "");
        ParentId = parentId;
        // get parent item to refresh
        GetParentRefreshId(this);
        // highlight item
        HighlightMenuItem(this);
        SelectMenu(0);
        console.log("Add ParentId=" + ParentId + "    RefreshItem=" + RefreshItem);
    });
    // delete menu item
    $(".menu-item-delete").click(function () {
        var deleteId = $(this).attr("deleteId");
        // get parent item to refresh
        GetParentRefreshId(this);
        // highlight item
        HighlightMenuItem(this);
        var menu = Menus.find(function (w) { return w.MenuId == deleteId; });
        ConfirmBox("Warning", "Delete " + menu.MenuTitle + "?", "DeleteMenu_(" + deleteId + ");");
        console.log("deleteId=" + deleteId + "    Title=" + menu.MenuTitle + "    RefreshItem=" + RefreshItem);
    });
}
function GetParentRefreshId(t) {
    var obj = $(t).closest("li.refresh-item");
    RefreshItem = obj.attr("id");
}
function HighlightMenuItem(t) {
    var objHighlight = $(t).closest(".highlight");
    $(".highlight").removeClass("menu-item-active");
    $(objHighlight).addClass("menu-item-active");
}
function SortMenu(menuId, newOrder) {
    $.ajax({
        url: "./Menu/SortMenu",
        data: { menuId: menuId, newOrder: newOrder },
        dataType: "text",
        success: function (data) {
            GetAdminMenuList();
        }
    });
}
function SelectMenu(menuId) {
    AppSpinner(true);
    setTimeout(function () {
        $("#EditMenu #MenuId").val(menuId);
        var json = ToJsonString("EditMenu");
        $.ajax({
            url: "./Data/GetFormData",
            data: { json: json },
            type: "POST",
            dataType: "json",
            success: function (data) {
                BindForm("EditMenu", data[0]);
                // set grid and form target type dropdowns
                var targetType = $("#EditMenu input[name='TargetType']:checked").val();
                if (targetType == "grid") {
                    $("#EditMenu #TargetGridId").val(data[0].TargetId);
                }
                else if (targetType == "form") {
                    $("#EditMenu #TargetFormId").val(data[0].TargetId);
                }
                // set display of TargetType options
                $(".targetType-class").hide();
                $("#span_" + data[0].TargetType).show();
                // set ParentId if new
                if (data[0].IsNewRecord == "True") {
                    $("#EditMenu #ParentId").val(ParentId);
                    $("#EditMenu #MenuId").val(menuId);
                }
                if (menuId == 0) {
                    DisableButton("cmd_Delete_EditMenu");
                }
                else {
                    EnableButton("cmd_Delete_EditMenu");
                }
                $("#editMenuBox").show();
            },
            complete: function () {
                AppSpinner(false);
            }
        });
    }, 500);
}
function GetMenuList() {
    $.ajax({
        url: "./Menu/GetMenuList",
        dataType: "json",
        success: function (data) {
            Menus = data;
            ObjMenu.length = 0;
            ObjMenu.push("<ul>");
            // get main menus
            var mainMenus = data.filter(function (it) { return it.ParentId == 0; });
            mainMenus = SortJson(mainMenus, "SortOrder");
            for (var i in mainMenus) {
                var row = mainMenus[i];
                var subMenus = data.filter(function (w) { return w.ParentId == row.MenuId; });
                if (subMenus.length > 0) {
                    ObjMenu.push("<li onclick='MenuClick(" + row.MenuId + ")'>");
                    ObjMenu.push("<span id='displayMenuId" + row.MenuId + "'>" + row.MenuTitle + "</span>");
                    GetSubMenuList(subMenus, data);
                    ObjMenu.push("</li>");
                }
                else {
                    ObjMenu.push("<li onclick='MenuClick(" + row.MenuId + ")'>");
                    ObjMenu.push("<span id='displayMenuId" + row.MenuId + "'>" + row.MenuTitle + "</span>");
                    ObjMenu.push("</li>");
                }
            }
            ObjMenu.push("</ul>");
            var str = ObjMenu.join("");
            $("#main_nav").html(str);
        }
    });
}
function GetSubMenuList(subMenus, data) {
    ObjMenu.push("<ul>");
    // get sub menus
    for (var x in subMenus) {
        var row = subMenus[x];
        var subMenus_ = data.filter(function (w) { return w.ParentId == row.MenuId; });
        subMenus_ = SortJson(subMenus_, "SortOrder");
        if (subMenus_.length > 0) {
            ObjMenu.push("<li onclick='MenuClick(" + row.MenuId + ")'>");
            ObjMenu.push("<span id='displayMenuId" + row.MenuId + "'>" + row.MenuTitle + "</span>&nbsp;&nbsp;<span class='menu-item-command fas fa-caret-right'>&nbsp;</span>");
            GetSubMenuList(subMenus_, data);
            ObjMenu.push("</li>");
        }
        else {
            ObjMenu.push("<li onclick='MenuClick(" + row.MenuId + ")'>");
            ObjMenu.push("<span id='displayMenuId" + row.MenuId + "'>" + row.MenuTitle + "</span>");
            ObjMenu.push("</li>");
        }
    }
    ObjMenu.push("</ul>");
}
//# sourceMappingURL=Menu.js.map