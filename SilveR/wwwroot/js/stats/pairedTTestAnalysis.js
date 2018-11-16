
$(function () {
    $("#Response").kendoDropDownList({
        dataSource: theModel.availableVariablesAllowNull
    });

    $("#ResponseTransformation").kendoDropDownList({
        dataSource: theModel.transformationsList
    });

    $("#Treatment").kendoDropDownList({
        dataSource: theModel.availableVariablesAllowNull,
        change: treatmentChanged
    });

    $("#Subject").kendoDropDownList({
        dataSource: theModel.availableVariablesAllowNull
    });

    $("#OtherDesignFactors").kendoMultiSelect({
        dataSource: theModel.availableVariables,
        value: theModel.otherDesignFactors
    });

    $("#Covariates").kendoMultiSelect({
        dataSource: theModel.availableVariables,
        value: theModel.covariates,
        change: covariateBlockEnableDisable
    });

    $("#CovariateTransformation").kendoDropDownList({
        dataSource: theModel.transformationsList
    });

    $("#Covariance").kendoDropDownList({
        dataSource: theModel.covariancesList
    });

    const controlGroup = $("#ControlGroup").kendoDropDownList({
        autoBind: false,
        dataSource: {
            transport: {
                read: {
                    url: "/Values/GetLevels"
                }
            }
        }
    }).data("kendoDropDownList");
    controlGroup.bind("dataBound", function (e) {
        controlGroup.value(theModel.controlGroup);
    });


    $("#Significance").kendoDropDownList({
        dataSource: theModel.significancesList
    });

    treatmentChanged();

    covariateBlockEnableDisable();
});


function treatmentChanged() {

    const controlGroup = $("#ControlGroup").data("kendoDropDownList");

    controlGroup.value(null);
    controlGroup.dataSource.read({ treatment: $("#Treatment").val(), datasetID: $("#DatasetID").val(), includeNull: true });
}


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