$(function () {

    $("#Response").kendoDropDownList({
        dataSource: theModel.availableVariablesAllowNull
    });

    $("#Treatment").kendoDropDownList({
        dataSource: theModel.availableVariablesAllowNull
    });

    $("#Significance").kendoDropDownList({
        dataSource: theModel.significancesList
    });

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

    enableDisablePlottingRangeType();
});

function treatmentInfo() {
    return {
        selectedGroupingVariable: $("#Treatment").val(),
        datasetID: $("#DatasetID").val(),
        includeNull: true
    };
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
        $("#SampleSizeFrom").data("kendoNumericTextBox").value(6);
        $("#SampleSizeTo").data("kendoNumericTextBox").value(15);

        powerFrom.prop("disabled", true);
        powerTo.prop("disabled", true);
        $("#PowerFrom").data("kendoNumericTextBox").value(null);
        $("#PowerTo").data("kendoNumericTextBox").value(null);
    }
    else {
        powerFrom.prop("disabled", false);
        powerTo.prop("disabled", false);
        $("#PowerFrom").data("kendoNumericTextBox").value(70);
        $("#PowerTo").data("kendoNumericTextBox").value(90);

        sampleSizeFrom.prop("disabled", true);
        sampleSizeTo.prop("disabled", true);
        $("#SampleSizeFrom").data("kendoNumericTextBox").value(null);
        $("#SampleSizeTo").data("kendoNumericTextBox").value(null);
    }
}