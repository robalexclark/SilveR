
$(function () {
    $("#Response").kendoDropDownList({
        dataSource: theModel.availableVariablesAllowNull
    });

    $("#ResponseTransformation").kendoDropDownList({
        dataSource: theModel.transformationsList
    });

    $("#Treatment").kendoDropDownList({
        dataSource: theModel.availableVariablesAllowNull
    });

    $("#Subject").kendoDropDownList({
        dataSource: theModel.availableVariablesAllowNull
    });

    $("#OtherDesignFactors").kendoMultiSelect({
        dataSource: theModel.availableVariables,
        value: theModel.otherDesignFactors
    });

    $("#Covariate").kendoDropDownList({
        dataSource: theModel.availableVariablesAllowNull,
        change: covariateBlockEnableDisable()
    });

    $("#CovariateTransformation").kendoDropDownList({
        dataSource: theModel.transformationsList
    });

    $("#Covariance").kendoDropDownList({
        dataSource: theModel.covarianceList
    });

    $("#Significance").kendoDropDownList({
        dataSource: theModel.significancesList
    });

    covariateBlockEnableDisable();
});



function covariateBlockEnableDisable() {
    const covariateDropDown = $("#Covariate");
    const covariateTransformationDropDown = $("#CovariateTransformation").data("kendoDropDownList");

    if (covariateDropDown && covariateTransformationDropDown) {
        if (covariateDropDown.val().length > 0) {
            covariateTransformationDropDown.enable(true);
        }
        else {
            covariateTransformationDropDown.enable(false);
            covariateTransformationDropDown.value("None");
        }
    }
}