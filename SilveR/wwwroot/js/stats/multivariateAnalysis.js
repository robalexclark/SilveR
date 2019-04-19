$(function () {
    jQuery.ajaxSettings.traditional = true

    $("#Responses").kendoMultiSelect({
        dataSource: theModel.availableVariables,
        value: theModel.responses
    });

    $("#ResponseTransformation").kendoDropDownList({
        dataSource: theModel.transformationsList
    });

    $("#CategoricalPredictor").kendoDropDownList({
        dataSource: theModel.availableVariablesAllowNull
    });

    $("#ContinuousPredictors").kendoMultiSelect({
        dataSource: theModel.availableVariables,
        value: theModel.continuousPredictors
    });

    $("#CaseID").kendoDropDownList({
        dataSource: theModel.availableVariablesAllowNull
    });

    $("#NoOfClusters").kendoNumericTextBox({
        format: '#',
        decimals: 0,
        min: 2,
        spinners: true
    });


    $("#DistanceMethod").kendoDropDownList({
        dataSource: theModel.distanceMethodsList
    });

    $("#AgglomerationMethod").kendoDropDownList({
        dataSource: theModel.agglomerationMethodsList
    });

    $("#PlotLabel").kendoDropDownList({
        dataSource: theModel.plotLabelsList
    });


    $("#ResponseCentring").kendoDropDownList({
        dataSource: theModel.responseCentringList
    });

    $("#ResponseScale").kendoDropDownList({
        dataSource: theModel.responseScaleList
    });

    $("#NoOfComponents").kendoNumericTextBox({
        format: '#',
        decimals: 0,
        spinners: true
    });


    analysisTypeChanged($('input[name=AnalysisType]:checked').val());
});

function analysisTypeChanged(analysisRadioButton) {

    if (analysisRadioButton == "PrincipalComponentsAnalysis" || analysisRadioButton.value == "PrincipalComponentsAnalysis") {
        $("#NoOfClusters").data("kendoNumericTextBox").enable(false);
        $("#DistanceMethod").data("kendoDropDownList").enable(false);
        $("#AgglomerationMethod").data("kendoDropDownList").enable(false);
        $("#PlotLabel").data("kendoDropDownList").enable(false);

        $("#ResponseCentring").data("kendoDropDownList").enable(true);
        $("#ResponseScale").data("kendoDropDownList").enable(true);

        $("#NoOfComponents").data("kendoNumericTextBox").enable(false);

    } else if (analysisRadioButton == "ClusterAnalysis" || analysisRadioButton.value == "ClusterAnalysis") {
        $("#NoOfClusters").data("kendoNumericTextBox").enable(true);
        $("#DistanceMethod").data("kendoDropDownList").enable(true);
        $("#AgglomerationMethod").data("kendoDropDownList").enable(true);
        $("#PlotLabel").data("kendoDropDownList").enable(true);

        $("#ResponseCentring").data("kendoDropDownList").enable(false);
        $("#ResponseScale").data("kendoDropDownList").enable(false);

        $("#NoOfComponents").data("kendoNumericTextBox").enable(false);

    } else if (analysisRadioButton == "LinearDiscriminantAnalysis" || analysisRadioButton.value == "LinearDiscriminantAnalysis") {
        $("#NoOfClusters").data("kendoNumericTextBox").enable(false);
        $("#DistanceMethod").data("kendoDropDownList").enable(false);
        $("#AgglomerationMethod").data("kendoDropDownList").enable(false);
        $("#PlotLabel").data("kendoDropDownList").enable(false);

        $("#ResponseCentring").data("kendoDropDownList").enable(false);
        $("#ResponseScale").data("kendoDropDownList").enable(false);

        $("#NoOfComponents").data("kendoNumericTextBox").enable(false);
    }
    //} else if (analysisRadioButton == "PartialLeastSquares" || analysisRadioButton.value == "PartialLeastSquares") {
    //    $("#NoOfClusters").data("kendoNumericTextBox").enable(false);
    //    $("#DistanceMethod").data("kendoDropDownList").enable(false);
    //    $("#AgglomerationMethod").data("kendoDropDownList").enable(false);
    //    $("#PlotLabel").data("kendoDropDownList").enable(false);

    //    $("#ResponseCentring").data("kendoDropDownList").enable(false);
    //    $("#ResponseScale").data("kendoDropDownList").enable(false);

    //    $("#NoOfComponents").data("kendoNumericTextBox").enable(true);
    //}
}