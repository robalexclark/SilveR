
$(function () {

    $("#Response").kendoDropDownList({
        dataSource: theModel.availableVariablesAllowNull
    });

    $("#Treatment").kendoDropDownList({
        dataSource: theModel.availableVariablesAllowNull,
        change: enableDisableControlLevels()
    });

    $("#OtherDesignFactor").kendoDropDownList({
        dataSource: theModel.availableVariablesAllowNull
    });

    $("#Significance").kendoDropDownList({
        dataSource: theModel.significancesList
    });

    $("#Control").kendoDropDownList({
        cascadeFrom: "Treatment",
        dataSource: {
            transport: {
                read: {
                    url: "/Values/GetLevels",
                    data: function () {
                        return {
                            treatment: $("#Treatment").val(),
                            datasetID: $("#DatasetID").val(),
                            includeNull: false
                        }
                    }
                }
            },
            serverFiltering: true,
            schema: { "errors": "Errors" }
        }
    });

    enableDisableControlLevels();
});

function enableDisableControlLevels() {

    var analysisType = $('input:radio[name="AnalysisType"]:checked').val();
    var treatmentDropDown = $("#Treatment").data("kendoDropDownList");
    var controlDropDown = $("#Control").data("kendoDropDownList");

    if (treatmentDropDown && controlDropDown) {
        if (treatmentDropDown.select() > 0 && analysisType === "CompareToControl") {
            controlDropDown.enable(true);
        }
        else {
            controlDropDown.enable(false);
            controlDropDown.value(null);
        }
    }
};