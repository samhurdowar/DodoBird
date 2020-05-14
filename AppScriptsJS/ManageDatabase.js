function ManageDatabase(level_MenuId, menuTitle) {
    $.ajax({
        url: "./Home/GetPage",
        data: { pageFile: "~/Views/Home/ManageDatabase.cshtml" },
        type: "POST",
        dataType: "text",
        success: function (response) {
            response = response.replace("xxx", "You are the greatest");
            AddTab(level_MenuId, menuTitle, response);
        }
    });
}
function GetAppTable() {
    $.ajax({
        url: "./Entity/GetAppTable",
        type: "GET",
        dataType: "json",
        success: function (data) {
        }
    });
}
//# sourceMappingURL=ManageDatabase.js.map