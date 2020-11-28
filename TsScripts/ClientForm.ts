
function ValidateForm(formName) {

    var returnStatus = false;
    var elementType = "";
    var elementValue = "";
    var o = {};

    $("#" + formName + " :input").each(function () {
 
        var id = $(this).attr("name");
        if (!id) {
            id = $(this).attr("id");
            elementType = (<HTMLInputElement>document.getElementById(id)).type;
            elementValue = (<HTMLInputElement>document.getElementById(id)).value;
        } else {
            var x = document.getElementsByName(id);
            var y = <HTMLInputElement>x[0];
            elementType = y.type;
            elementValue = y.value;
        }

        try {

            if (elementType == "checkbox" && !o[id]) {
                var checkedValues = "";
                var inputElements = document.getElementsByName(id);
                for (var i = 0; i < inputElements.length; ++i) {
                    var inputElement = <HTMLInputElement>inputElements[i];
                    if (inputElement.checked) {
                        if (checkedValues.length == 0) {
                            checkedValues = inputElement.value;
                        } else {
                            checkedValues += "," + inputElement.value;
                        }
                    }
                }
                elementValue = checkedValues;
            } else if (elementType == "radio" && !o[id]) {
                elementValue = "";
                var inputElements = document.getElementsByName(id);
                for (var i = 0; i < inputElements.length; ++i) {
                    var inputElement = <HTMLInputElement>inputElements[i];
                    if (inputElement.checked) {
                        elementValue = inputElement.value;
                        break;
                    }
                }
            }

            if (!o[id]) {
                
                o[id] = elementValue;
                console.log(id + "=" + elementType + "=" + elementValue);
                if (elementValue.length == 0 && $(this).hasClass("input-required")) {
                    $(this).closest('td').addClass("input-required-highlight");
                    if (!returnStatus) {
                        $(this).focus();
                    }
                    returnStatus = true;
                } else {
                    $(this).closest('td').removeClass("input-required-highlight");
                }
            }


        } catch (e) {
            console.log("ValidateForm Error=" + e);
        }

    });

    return returnStatus;
}

function ToJsonString(formName) {
    var elementType = "";
    var elementValue = "";
    var o = {};

    $("#" + formName + " :input").each(function () {

        var id = $(this).attr("name");
        if (!id) {
            id = $(this).attr("id");
            elementType = (<HTMLInputElement>document.getElementById(id)).type;
            elementValue = (<HTMLInputElement>document.getElementById(id)).value;
        } else {
            var x = document.getElementsByName(id);
            var y = <HTMLInputElement>x[0];
            elementType = y.type;
            elementValue = y.value;
        }

        try {

            if (elementType == "checkbox" && !o[id]) {
                var checkedValues = "";
                var inputElements = document.getElementsByName(id);
                for (var i = 0; i < inputElements.length; ++i) {
                    var inputElement = <HTMLInputElement>inputElements[i];
                    if (inputElement.checked) {
                        if (checkedValues.length == 0) {
                            checkedValues = inputElement.value;
                        } else {
                            checkedValues += "," + inputElement.value;
                        }
                    }
                }
                elementValue = checkedValues;
            } else if (elementType == "radio" && !o[id]) {
                elementValue = "";
                var inputElements = document.getElementsByName(id);
                for (var i = 0; i < inputElements.length; ++i) {
                    var inputElement = <HTMLInputElement>inputElements[i];
                    if (inputElement.checked) {
                        elementValue = inputElement.value;
                        break;
                    }
                }
            }

            if (!o[id]) {
                //console.log(id + "=" + elementType + "=" + elementValue);
                o[id] = elementValue;
            }


        } catch (e) { }

    });

    var json = JSON.stringify(o);
    return json;
}

function BindForm(formName, data) {

    var elementType = "";
    var dataValue = "";


    // get data properties
    var keys = "|";
    for (var key in data) {
        keys += key + "|";
    }

    $("#" + formName + " :input").each(function () {
        try {
            
            dataValue = "";

            var id = $(this).attr("name");
            if (!id) {
                id = $(this).attr("id");
                elementType = (<HTMLInputElement>document.getElementById(id)).type;
            } else {
                var x = document.getElementsByName(id);
                var y = <HTMLInputElement>x[0];
                elementType = y.type;
            }
            //console.log("BindData id=" + id + "  elementType=" + elementType + "    dataValue=" + data[id]);


            if (keys.indexOf("|" + id + "|") > -1) {
                dataValue = (data[id] != null) ? data[id] : "";

                //console.log("BindData id=" + id + "  elementType=" + elementType + "    dataValue=" + dataValue);

                if (elementType == "text") {
                    $(this).val(dataValue);

                    $(this).keydown(function () {
                        EnableButton("cmd_Save_" + formName);
                    });
                } else if (elementType == "checkbox") {
                    if (dataValue == "true" || dataValue == "TRUE" || dataValue == "1") {
                        $(this).prop('checked', true);
                    } else {
                        $(this).prop('checked', false);
                    }

                    $(this).click(function () {
                        EnableButton("cmd_Save_" + formName);
                    });
                } else if (elementType == "radio") {

                    var groupName = "input:radio[name=\"" + id + "\"]";
                    $(groupName).each(function () {
                        var radioVal = $(this).val()
                        //console.log("radioVal=" + radioVal);
                        if (radioVal == dataValue) {
                            $(this).prop('checked', true);
                        }
                    });

                    $(this).click(function () {
                        EnableButton("cmd_Save_" + formName);
                    });
                } else if (elementType == "select-one") {
                    $(this).val(dataValue);

                    $(this).change(function () {
                        //console.log("EnableButton select-one");
                        EnableButton("cmd_Save_" + formName);
                    });
                }

            }




        } catch (e) {
            console.log("BindForm error=" + e);
        }
    });
}


function EnableButton(id) {
    //console.log("EnableButton id=" + id);
    try {
        $("#" + id).unbind("click");
    } catch (e) { }

    $("#" + id).click(function () {
        var cmd = id.replace("cmd_", "") + "()";
        eval(cmd);
    });

    $("#" + id).removeClass("command-disabled-span").addClass("command-active-span");
}

function DisableButton(id) {
    try {
        $("#" + id).unbind("click");
    } catch (e) { }
    $("#" + id).removeClass("command-active-span").addClass("command-disabled-span");
}