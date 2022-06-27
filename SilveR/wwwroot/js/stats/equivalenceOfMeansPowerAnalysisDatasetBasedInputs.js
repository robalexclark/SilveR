$(function () {

    $("#Response").kendoDropDownList({
        dataSource: theModel.availableVariablesAllowNull
    });

    $("#Treatment").kendoDropDownList({
        dataSource: theModel.availableVariablesAllowNull,
        change: enableDisableControlLevels
    });

    $("#ControlGroup").kendoDropDownList({
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



    $("#LowerBoundAbsolute").kendoNumericTextBox({
        format: '#.##',
        decimals: 2,
        spinners: true,
        step: 0.01
    });

    $("#UpperBoundAbsolute").kendoNumericTextBox({
        format: '#.##',
        decimals: 2,
        spinners: true,
        step: 0.01
    });

    $("#LowerBoundPercentageChange").kendoNumericTextBox({
        format: '#.##',
        decimals: 2,
        spinners: true,
        min: 0,
        max: 1000
    });

    $("#UpperBoundPercentageChange").kendoNumericTextBox({
        format: '#.##',
        decimals: 2,
        spinners: true,
        min: 0,
        max: 1000
    });


    $("#Significance").kendoDropDownList({
        dataSource: theModel.significancesList
    });

    enableDisableControlLevels();
  
});

function treatmentInfo() {
    return {
        selectedGroupingVariable: $("#Treatment").val(),
        datasetID: $("#DatasetID").val(),
        includeNull: true
    };
}

function enableDisableControlLevels() {
    const treatmentDropDown = $("#Treatment").data("kendoDropDownList");
    const controlDropDown = $("#ControlGroup").data("kendoDropDownList");

    if (treatmentDropDown.select() > 0) {
        controlDropDown.enable(true);
    }
    else {
        controlDropDown.select(-1);
        controlDropDown.value(null);
        controlDropDown.enable(false);
    }
}