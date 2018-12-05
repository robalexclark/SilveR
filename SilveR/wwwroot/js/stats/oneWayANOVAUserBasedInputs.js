
$(function () {

    $("#TreatmentMeanSquare").kendoNumericTextBox({
        format: '#.######',
        decimals: 6,
        spinners: false
    });

    $("#NoOfTreatmentGroups").kendoNumericTextBox({
        format: '#.######',
        decimals: 6,
        spinners: false
    });

    $("#ResidualMeanSquare").kendoNumericTextBox({
        format: '#.######',
        decimals: 6,
        spinners: false
    });

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

    $("#PercentChange").kendoNumericTextBox({
        spinners: false
    });

    $("#AbsoluteChange").kendoNumericTextBox({
        spinners: false
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


    enableDisableEffectSizeEstimate();
    enableDisableVariabilityEstimate();
    enableDisablePlottingRangeType();
});


function enableDisableEffectSizeEstimate() {

    const effectSizeEstimate = $('input:radio[name="EffectSizeEstimate"]:checked').val();
    const treatmentMeanSquare = $("#TreatmentMeanSquare");
    const noOfTreatmentGroups = $("#NoOfTreatmentGroups");
    const means = $("#Means");

    if (effectSizeEstimate === "TreatmentMeanSquare") {
        treatmentMeanSquare.prop("disabled", false);
        noOfTreatmentGroups.prop("disabled", false);

        means.prop("disabled", true);
        means.val("");
    }
    else {
        means.prop("disabled", false);

        treatmentMeanSquare.prop("disabled", true);
        treatmentMeanSquare.data("kendoNumericTextBox").value(null);
        noOfTreatmentGroups.prop("disabled", true);
        noOfTreatmentGroups.data("kendoNumericTextBox").value(null);
    }
}

function enableDisableVariabilityEstimate() {

    const variabilityEstimate = $('input:radio[name="VariabilityEstimate"]:checked').val();
    const residualMeanSquare = $("#ResidualMeanSquare");
    const variance = $("#Variance");
    const standardDeviation = $("#StandardDeviation");

    if (variabilityEstimate === "ResidualMeanSquare") {
        residualMeanSquare.prop("disabled", false);

        variance.prop("disabled", true);
        $("#Variance").data("kendoNumericTextBox").value(null);

        standardDeviation.prop("disabled", true);
        $("#StandardDeviation").data("kendoNumericTextBox").value(null);
    }
    else if (variabilityEstimate === "Variance") {
        variance.prop("disabled", false);

        residualMeanSquare.prop("disabled", true);
        $("#ResidualMeanSquare").data("kendoNumericTextBox").value(null);

        standardDeviation.prop("disabled", true);
        $("#StandardDeviation").data("kendoNumericTextBox").value(null);
    }
    else if (variabilityEstimate === "StandardDeviation") {
        standardDeviation.prop("disabled", false);

        residualMeanSquare.prop("disabled", true);
        $("#ResidualMeanSquare").data("kendoNumericTextBox").value(null);

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