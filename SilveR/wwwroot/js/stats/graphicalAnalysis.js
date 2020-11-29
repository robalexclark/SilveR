$(function () {

    $("#Response").kendoDropDownList({
        dataSource: theModel.availableVariablesAllowNull
    });

    $("#ResponseTransformation").kendoDropDownList({
        dataSource: theModel.transformationsList
    });

    $("#XAxis").kendoDropDownList({
        dataSource: theModel.availableVariablesAllowNull
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

    $("#ReferenceLine").kendoNumericTextBox({
        format: '#.######',
        decimals: 6,
        spinners: false
    });

    $("#BoxplotOptions").kendoDropDownList({
        dataSource: theModel.boxplotOptionsList
    });

    enableDisableXAxisTransformation();
    enableDisableScatterplot();
    enableDisableBoxplot();
    enableDisableSEMplot();
    enableDisableHistogram();
    enableDisableCaseProfiles();


    const xAxis = $("#XAxis").data("kendoDropDownList");
    xAxis.bind("change", enableDisableXAxisTransformation);
});


function enableDisableXAxisTransformation() {
    const xAxisDropdown = $("#XAxis").data("kendoDropDownList");
    const xAxisTransformation = $("#XAxisTransformation").data("kendoDropDownList");

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
    const scatterplot = $("#ScatterplotSelected");
    const linearFit = $("#LinearFitSelected");
    const jitter = $("#JitterSelected");

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
    const boxplot = $("#BoxplotSelected");
    const outliers = $("#OutliersSelected");

    if (boxplot.prop('checked')) {
        outliers.prop('disabled', false);
    }
    else {
        outliers.prop('checked', false);
        outliers.prop('disabled', true);
    }
}

function enableDisableSEMplot() {
    const semplot = $("#SEMPlotSelected");
    const columnRadBtn = $("#Column");
    const lineRadBtn = $("#Line");
    const includeData = $("#SEMPlotIncludeData");

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
    const histogram = $("#HistogramSelected");
    const normalDist = $("#NormalDistSelected");

    if (histogram.prop('checked')) {
        normalDist.prop('disabled', false);
    }
    else {
        normalDist.prop('checked', false);
        normalDist.prop('disabled', true);
    }
}

function enableDisableCaseProfiles() {
    const caseProfiles = $("#CaseProfilesPlotSelected");
    const caseIDDropDown = $("#CaseIDFactor").data("kendoDropDownList");
    const showCaseIDsInLegend = $("#ShowCaseIDsInLegend");

    if (caseProfiles.prop('checked')) {
        caseIDDropDown.enable(true);
        showCaseIDsInLegend.prop('disabled', false);
    }
    else {
        caseIDDropDown.value(null);
        caseIDDropDown.enable(false);
        showCaseIDsInLegend.prop('disabled', true);
    }
}