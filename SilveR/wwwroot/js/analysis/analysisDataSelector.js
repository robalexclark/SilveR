
$(function () {

    if (!selectedAnalysisType) {
        $("#AnalysisType").kendoDropDownList({
            change: analysisSelectionChanged,
            dataSource: scriptList
        });
    }

    $("#DatasetID").kendoDropDownList({
        dataTextField: "datasetNameVersion",
        dataValueField: "datasetID",
        dataSource: datasets
    });

    function analysisSelectionChanged() {
        const analysisSelector = $("#AnalysisType").data("kendoDropDownList");
        if (analysisSelector.value() != "P-value Adjustment") {
            $("#dataSelectionBlock").show();
        }
        else {
            $("#dataSelectionBlock").hide();
        }
    }
});