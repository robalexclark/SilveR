$(function () {
    jQuery.ajaxSettings.traditional = true

    $("#Response").kendoDropDownList({
        dataSource: theModel.availableVariablesAllowNull
    });

    $("#ResponseTransformation").kendoDropDownList({
        dataSource: theModel.transformationsList
    });

    $("#Treatments").kendoMultiSelect({
        dataSource: theModel.availableVariables,
        value: theModel.treatments
    });

    $("#OtherDesignFactors").kendoMultiSelect({
        dataSource: theModel.availableVariables,
        value: theModel.otherDesignFactors
    });

    $("#RandomFactor1").kendoDropDownList({
        dataSource: theModel.availableVariablesAllowNull
    });
    $("#RandomFactor2").kendoDropDownList({
        dataSource: theModel.availableVariablesAllowNull
    });
    $("#RandomFactor3").kendoDropDownList({
        dataSource: theModel.availableVariablesAllowNull
    });
    $("#RandomFactor4").kendoDropDownList({
        dataSource: theModel.availableVariablesAllowNull
    });

    $("#Covariates").kendoMultiSelect({
        dataSource: theModel.availableVariables,
        value: theModel.covariates,
        change: covariateBlockEnableDisable
    });

    $("#CovariateTransformation").kendoDropDownList({
        dataSource: theModel.transformationsList
    });

    $("#Significance").kendoDropDownList({
        dataSource: theModel.significancesList
    });

    covariateBlockEnableDisable();
});


function covariateBlockEnableDisable() {
    const covariateDropDown = $("#Covariates");
    const covariateTransformationDropDown = $("#CovariateTransformation").data("kendoDropDownList");

    if (covariateDropDown.val().length > 0) {
        covariateTransformationDropDown.enable(true);
    }
    else {
        covariateTransformationDropDown.enable(false);
        covariateTransformationDropDown.value("None");
    }
}