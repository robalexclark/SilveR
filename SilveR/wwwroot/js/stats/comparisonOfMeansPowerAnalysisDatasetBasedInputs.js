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

    $("#Significance").kendoDropDownList({
        dataSource: theModel.significancesList
    });

    enableDisableControlLevels();

    //$("#PercentChange").kendoNumericTextBox({
    //    spinners: false
    //});

    //$("#AbsoluteChange").kendoNumericTextBox({
    //    spinners: false
    //});

    $("#SampleSizeFrom").kendoNumericTextBox({
        format: '#',
        decimals: 0,
        spinners: false
    });

    $("#SampleSizeTo").kendoNumericTextBox({
        format: '#',
        decimals: 0,
        spinners: false
    });

    $("#PowerFrom").kendoNumericTextBox({
        format: '#',
        decimals: 0,
        spinners: false
    });

    $("#PowerTo").kendoNumericTextBox({
        format: '#',
        decimals: 0,
        spinners: false
    });

    enableDisableChangeType();
    enableDisablePlottingRangeType();
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

function enableDisableChangeType() {

    const changeType = $('input:radio[name="ChangeType"]:checked').val();
    const percentChange = $("#PercentChange");
    const absoluteChange = $("#AbsoluteChange");

    if (changeType === "Percent") {
        percentChange.prop("disabled", false);
        absoluteChange.prop("disabled", true);
        absoluteChange.val(null);
    }
    else {
        percentChange.prop("disabled", true);
        absoluteChange.prop("disabled", false);
        percentChange.val(null);
    }
}

function enableDisablePlottingRangeType() {

    const plottingRangeType = $('input:radio[name="PlottingRangeType"]:checked').val();

    const sampleSizeFrom = $("#SampleSizeFrom");
    const sampleSizeTo = $("#SampleSizeTo");
    const powerFrom = $("#PowerFrom");
    const powerTo = $("#PowerTo");

    if (plottingRangeType === "SampleSize") {
        sampleSizeFrom.prop("disabled", false);
        sampleSizeTo.prop("disabled", false);
        $("#SampleSizeFrom").data("kendoNumericTextBox").value(theModel.sampleSizeFrom);
        $("#SampleSizeTo").data("kendoNumericTextBox").value(theModel.sampleSizeTo);

        powerFrom.prop("disabled", true);
        powerTo.prop("disabled", true);
        $("#PowerFrom").data("kendoNumericTextBox").value(null);
        $("#PowerTo").data("kendoNumericTextBox").value(null);
    }
    else {
        powerFrom.prop("disabled", false);
        powerTo.prop("disabled", false);
        $("#PowerFrom").data("kendoNumericTextBox").value(theModel.powerFrom);
        $("#PowerTo").data("kendoNumericTextBox").value(themodel.powerTo);

        sampleSizeFrom.prop("disabled", true);
        sampleSizeTo.prop("disabled", true);
        $("#SampleSizeFrom").data("kendoNumericTextBox").value(null);
        $("#SampleSizeTo").data("kendoNumericTextBox").value(null);
    }
}