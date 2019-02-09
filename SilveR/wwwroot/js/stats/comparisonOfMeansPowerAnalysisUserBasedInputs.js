$(function () {  

    $("#GroupMean").kendoNumericTextBox({
        format: '#.######',
        decimals: 6,
        spinners: false
    });

    $("#StandardDeviation").kendoNumericTextBox({
        format: '#.######',
        decimals: 6,
        spinners: false
    });

    $("#Variance").kendoNumericTextBox({
        format: '#.######',
        decimals: 6,
        spinners: false
    });

    $("#Significance").kendoDropDownList({
        dataSource: theModel.significancesList
    });

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

    enableDisableDeviationType();
    enableDisableChangeType();
    enableDisablePlottingRangeType();
});

function enableDisableDeviationType() {

    const deviationType = $('input:radio[name="DeviationType"]:checked').val();
    const standardDeviation = $("#StandardDeviation");
    const variance = $("#Variance");

    if (deviationType === "StandardDeviation") {
        standardDeviation.prop("disabled", false);
        variance.prop("disabled", true);
        $("#Variance").data("kendoNumericTextBox").value(null);
    }
    else {
        variance.prop("disabled", false);
        standardDeviation.prop("disabled", true);
        $("#StandardDeviation").data("kendoNumericTextBox").value(null);
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
        $("#PowerTo").data("kendoNumericTextBox").value(theModel.powerTo);

        sampleSizeFrom.prop("disabled", true);
        sampleSizeTo.prop("disabled", true);
        $("#SampleSizeFrom").data("kendoNumericTextBox").value(null);
        $("#SampleSizeTo").data("kendoNumericTextBox").value(null);
    }
}