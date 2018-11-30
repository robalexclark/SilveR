//$.extend(window.kendo.ui.NumericTextBox.fn, {
//    clear: function () {
//        this._old = this._value;
//        this._value = null;
//        this._text.val(this._value);
//        this.element.val(this._value);
//    }
//});

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

    const radioSampleSize = $("#SampleSize")[0];
    const sampleSizeFrom = $("#SampleSizeFrom");
    const sampleSizeTo = $("#SampleSizeTo");
    const powerFrom = $("#PowerFrom");
    const powerTo = $("#PowerTo");

    if (plottingRangeType === "SampleSize") {
        sampleSizeFrom.prop("disabled", false);
        sampleSizeTo.prop("disabled", false);
        powerFrom.prop("disabled", true);
        powerTo.prop("disabled", true);
        powerFrom.val("70");
        powerTo.val("90");
    }
    else {
        sampleSizeFrom.prop("disabled", true);
        sampleSizeTo.prop("disabled", true);
        powerFrom.prop("disabled", false);
        powerTo.prop("disabled", false);
        sampleSizeFrom.val("6");
        sampleSizeTo.val("15");
    }
}