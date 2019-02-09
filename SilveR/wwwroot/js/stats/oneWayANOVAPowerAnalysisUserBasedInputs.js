
$(function () {

    $("#Variance").kendoNumericTextBox({
        format: '#.######',
        decimals: 6,
        spinners: false
    });

    $("#StandardDeviation").kendoNumericTextBox({
        format: '#.######',
        decimals: 6,
        spinners: false
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

    enableDisableVariabilityEstimate();
    enableDisablePlottingRangeType();
});

function enableDisableVariabilityEstimate() {

    const variabilityEstimate = $('input:radio[name="VariabilityEstimate"]:checked').val();
    const variance = $("#Variance");
    const standardDeviation = $("#StandardDeviation");

    if (variabilityEstimate === "Variance") {
        variance.prop("disabled", false);

        standardDeviation.prop("disabled", true);
        $("#StandardDeviation").data("kendoNumericTextBox").value(null);
    }
    else if (variabilityEstimate === "StandardDeviation") {
        standardDeviation.prop("disabled", false);

        variance.prop("disabled", true);
        $("#Variance").data("kendoNumericTextBox").value(null);
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
        $("#PowerTo").data("kendoNumericTextBox").value(theModel.powerTo);

        sampleSizeFrom.prop("disabled", true);
        sampleSizeTo.prop("disabled", true);
        $("#SampleSizeFrom").data("kendoNumericTextBox").value(null);
        $("#SampleSizeTo").data("kendoNumericTextBox").value(null);
    }
}