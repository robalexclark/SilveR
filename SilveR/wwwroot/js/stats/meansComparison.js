
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

    enableDisableValueType();
    enableDisableControlLevels();
    enableDisableChangeType();
    enableDisablePlottingRangeType();
});


function enableDisableValueType() {

    var valueType = $('input:radio[name="ValueType"]:checked').val()

    var groupMean = $("#GroupMean");
    var standardDeviation = $("#StandardDeviation");
    var variance = $("#Variance");
    var response = $("#Response").data("kendoDropDownList");
    var treatment = $("#Treatment").data("kendoDropDownList");
    var controlGroup = $("#ControlGroup").data("kendoDropDownList");

    if (valueType === "Supplied") {
        groupMean.prop("disabled", false);
        standardDeviation.prop("disabled", false);
        variance.prop("disabled", false);
        response.select(-1);
        treatment.select(-1);
        controlGroup.select(-1);
        response.value(null);
        treatment.value(null);
        controlGroup.value(null);
        response.enable(false);
        treatment.enable(false);
        controlGroup.enable(false);
    }
    else {
        groupMean.prop("disabled", true);
        standardDeviation.prop("disabled", true);
        variance.prop("disabled", true);
        response.enable(true);
        treatment.enable(true);
        controlGroup.enable(true);
        groupMean.val(null);
        standardDeviation.val(null);
        variance.val(null);
    }
}


function treatmentInfo() {
    return {
        selectedGroupingVariable: $("#Treatment").val(),
        datasetID: $("#DatasetID").val(),
        includeNull: true
    };
}

function enableDisableControlLevels() {
    var treatmentDropDown = $("#Treatment").data("kendoDropDownList");
    var controlDropDown = $("#ControlGroup").data("kendoDropDownList");

    if (treatmentDropDown.select() > 0) {
        controlDropDown.enable(true);
    }
    else {
        controlDropDown.select(-1);
        controlDropDown.value(null);
        controlDropDown.enable(false);
    }
}

//function controlGroupDataBound() {
//    var controlGroupDropDown = $("#ControlGroup").data("kendoDropDownList");
//    controlGroupDropDown.value(initialControlLevel);
//    initialControlLevel = null;
//    enableDisableControlLevels();
//}

function enableDisableChangeType() {

    var changeType = $('input:radio[name="ChangeType"]:checked').val();
    var percentChange = $("#PercentChange");
    var absoluteChange = $("#AbsoluteChange");

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

    var plottingRangeType = $('input:radio[name="PlottingRangeType"]:checked').val();

    var radioSampleSize = $("#SampleSize")[0];
    var sampleSizeFrom = $("#SampleSizeFrom");
    var sampleSizeTo = $("#SampleSizeTo");
    var powerFrom = $("#PowerFrom");
    var powerTo = $("#PowerTo");

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