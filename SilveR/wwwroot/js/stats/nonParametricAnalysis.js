
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

    const analysisType = $('input:radio[name="AnalysisType"]:checked').val();
    const treatmentDropDown = $("#Treatment").data("kendoDropDownList");
    const controlDropDown = $("#Control").data("kendoDropDownList");

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