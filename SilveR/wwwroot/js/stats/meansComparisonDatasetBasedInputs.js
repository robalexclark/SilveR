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

//function controlGroupDataBound() {
//    const controlGroupDropDown = $("#ControlGroup").data("kendoDropDownList");
//    controlGroupDropDown.value(initialControlLevel);
//    initialControlLevel = null;
//    enableDisableControlLevels();
//}

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