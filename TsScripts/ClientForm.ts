
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

