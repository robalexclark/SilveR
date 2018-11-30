$(function () {

    if (!selectedAnalysisName) {
        $("#AnalysisName").kendoDropDownList({
            change: analysisSelectionChanged,
            dataSource: scriptsList,
            dataTextField: "scriptDisplayName",
            dataValueField: "scriptFileName"
        });
    }

    $("#SelectedDatasetID").kendoDropDownList({
        dataSource: datasets,
        dataTextField: "datasetNameVersion",
        dataValueField: "datasetID"
    });

    function analysisSelectionChanged() {
        const analysisSelector = $("#AnalysisName").data("kendoDropDownList");
        if (analysisSelector.value() != "P-value Adjustment") {
            $("#dataSelectionBlock").show();
        }
        else {
            $("#dataSelectionBlock").hide();
        }
    }
});