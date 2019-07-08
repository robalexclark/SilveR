$(function () {
    jQuery.ajaxSettings.traditional = true

    $("#Response").kendoDropDownList({
        dataSource: theModel.availableVariablesAllowNull
    });

    $("#ResponseTransformation").kendoDropDownList({
        dataSource: theModel.transformationsList,
        change: function () {
            const covariateTransformation = $("#CovariateTransformation").data("kendoDropDownList");
            covariateTransformation.value($("#ResponseTransformation").val());
        }
    });

    $("#CategoricalFactors").kendoMultiSelect({
        dataSource: theModel.availableVariables,
        value: theModel.categoricalFactors,
        change: categoricalFactorsChanged
    });

    $("#OtherDesignFactors").kendoMultiSelect({
        dataSource: theModel.availableVariables,
        value: theModel.otherDesignFactors
    });

    $("#ContinuousFactors").kendoMultiSelect({
        dataSource: theModel.availableVariables,
        value: theModel.continuousFactors
    });

    $("#ContinuousFactorsTransformation").kendoDropDownList({
        dataSource: theModel.transformationsList
    });

    $("#Covariates").kendoMultiSelect({
        dataSource: theModel.availableVariables,
        value: theModel.covariates,
        change: covariateBlockEnableDisable
    });

    const primaryFactorDropdown = $("#PrimaryFactor").kendoDropDownList({
        dataSource: getCategoricalFactors(),
        value: theModel.primaryFactor
    }).data("kendoDropDownList");
    primaryFactorDropdown.bind("dataBound", function (e) {
        if (!this.value()) {
            this.select(0);
        }
    });

    $("#CovariateTransformation").kendoDropDownList({
        dataSource: theModel.transformationsList
    });

    $("#Significance").kendoDropDownList({
        dataSource: theModel.significancesList
    });


    categoricalFactorsChanged({ currentPrimaryFactor: theModel.primaryFactor });

    covariateBlockEnableDisable();
});


function categoricalFactorsChanged() {
    //categorical factors have changed so fill in the primary factor...
    const primaryFactorDropDown = $("#PrimaryFactor").data("kendoDropDownList");
    primaryFactorDropDown.setDataSource(getCategoricalFactors());
}

function getCategoricalFactors() {
    const catFactors = $("#CategoricalFactors").data("kendoMultiSelect").dataItems();
    //catFactors.unshift("");

    return catFactors;
}

function covariateBlockEnableDisable() {
    const covariateDropDown = $("#Covariates");
    const primaryFactorDropDown = $("#PrimaryFactor").data("kendoDropDownList");
    const covariateTransformationDropDown = $("#CovariateTransformation").data("kendoDropDownList");

    if (covariateDropDown.val().length > 0) {
        primaryFactorDropDown.enable(true);
        primaryFactorDropDown.select(0);
        covariateTransformationDropDown.enable(true);
    }
    else {
        primaryFactorDropDown.enable(false);
        covariateTransformationDropDown.enable(false);
        covariateTransformationDropDown.value("None");
        primaryFactorDropDown.value(null);
    }
}