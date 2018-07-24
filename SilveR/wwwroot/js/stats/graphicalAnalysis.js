
$(function () {

    $("#Response").kendoDropDownList({
        dataSource: theModel.availableVariablesAllowNull
    });

    $("#ResponseTransformation").kendoDropDownList({
        dataSource: theModel.transformationsList
    });

    $("#XAxis").kendoDropDownList({
        dataSource: theModel.availableVariablesAllowNull,
        change: enableDisableXAxisTransformation()
    });

    $("#XAxisTransformation").kendoDropDownList({
        dataSource: theModel.transformationsList
    });

    $("#FirstCatFactor").kendoDropDownList({
        dataSource: theModel.availableVariablesAllowNull
    });

    $("#SecondCatFactor").kendoDropDownList({
        dataSource: theModel.availableVariablesAllowNull
    });

    $("#CaseIDFactor").kendoDropDownList({
        dataSource: theModel.availableVariablesAllowNull
    });

    enableDisableXAxisTransformation();
    enableDisableScatterplot();
    enableDisableBoxplot();
    enableDisableSEMplot();
    enableDisableHistogram();
    enableDisableCaseProfiles();
});


function enableDisableXAxisTransformation() {
    var xAxisDropdown = $("#XAxis").data("kendoDropDownList");
    var xAxisTransformation = $("#XAxisTransformation").data("kendoDropDownList");

    if (xAxisDropdown && xAxisTransformation) { //need check because of change event...
        if (xAxisDropdown.select() > 0) {
            xAxisTransformation.enable(true);
        }
        else {
            xAxisTransformation.value("None");
            xAxisTransformation.enable(false);
        }
    }
}

function enableDisableScatterplot() {
    var scatterplot = $("#ScatterplotSelected");
    var linearFit = $("#LinearFitSelected");
    var jitter = $("#JitterSelected");

    if (scatterplot.prop('checked')) {
        linearFit.prop('disabled', false);
        jitter.prop('disabled', false);
    }
    else {
        linearFit.prop('checked', false);
        linearFit.prop('disabled', true);
        jitter.prop('checked', false);
        jitter.prop('disabled', true);
    }
}

function enableDisableBoxplot() {
    var boxplot = $("#BoxplotSelected");
    var outliers = $("#OutliersSelected");
    var includeData = $("#BoxPlotIncludeData");

    if (boxplot.prop('checked')) {
        outliers.prop('disabled', false);
        includeData.prop('disabled', false);
    }
    else {
        outliers.prop('checked', false);
        outliers.prop('disabled', true);
        includeData.prop('checked', false);
        includeData.prop('disabled', true);
    }
}

function enableDisableSEMplot() {
    var semplot = $("#SEMPlotSelected");
    var columnRadBtn = $("#Column");
    var lineRadBtn = $("#Line");
    var includeData = $("#SEMPlotIncludeData");

    if (semplot.prop('checked')) {
        columnRadBtn.prop('disabled', false);
        lineRadBtn.prop('disabled', false);
        includeData.prop('disabled', false);
    }
    else {
        includeData.prop('checked', false);
        columnRadBtn.prop('disabled', true);
        lineRadBtn.prop('disabled', true);
        includeData.prop('disabled', true);
    }
}

function enableDisableHistogram() {
    var histogram = $("#HistogramSelected");
    var normalDist = $("#NormalDistSelected");

    if (histogram.prop('checked')) {
        normalDist.prop('disabled', false);
    }
    else {
        normalDist.prop('checked', false);
        normalDist.prop('disabled', true);
    }
}

function enableDisableCaseProfiles() {
    var caseProfiles = $("#CaseProfilesPlotSelected");
    var caseIDDropDown = $("#CaseIDFactor").data("kendoDropDownList");
    var refLine = $("#ReferenceLine");

    if (caseProfiles.prop('checked')) {
        caseIDDropDown.enable(true);
        refLine.prop('disabled', false);
    }
    else {
        caseIDDropDown.value(null);
        caseIDDropDown.enable(false);
        refLine.val("");
        refLine.prop('disabled', true);
    }
}