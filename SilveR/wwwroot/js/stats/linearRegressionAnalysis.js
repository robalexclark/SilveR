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
        value: theModel.treatments,
        change: treatmentsChanged
    });

    $("#OtherDesignFactors").kendoMultiSelect({
        dataSource: theModel.availableVariables
        //value: theModel.otherDesignFactors
    });

    $("#ContinuousFactors").kendoMultiSelect({
        dataSource: theModel.availableVariables
        //value: theModel.otherDesignFactors
    });

    $("#ContinuousFactorsTransformation").kendoDropDownList({
        dataSource: theModel.transformationsList
    });

    $("#Covariate").kendoDropDownList({
        dataSource: theModel.availableVariablesAllowNull,
        change: covariateBlockEnableDisable
    });

    var primaryFactorDropdown = $("#PrimaryFactor").kendoDropDownList({
        dataSource: {
            transport: {
                read: {
                    url: "/Values/GetSelectedTreatments"
                }
            }
        }

    }).data("kendoDropDownList");
    primaryFactorDropdown.bind("dataBound", function (e) {
        primaryFactorDropdown.value(theModel.primaryFactor);
    });

    $("#CovariateTransformation").kendoDropDownList({
        dataSource: theModel.transformationsList
    });

    $("#Significance").kendoDropDownList({
        dataSource: theModel.significancesList
    });


    treatmentsChanged({ currentPrimaryFactor: theModel.primaryFactor });

    covariateBlockEnableDisable();
});


function treatmentsChanged() {
    var treatmentMultiSelect = $("#Treatments").data("kendoMultiSelect");

    var currentlySelectedTreatments = $("#Treatments").data("kendoMultiSelect").dataItems();

    //treatments have changed so fill in the primary factor and the selected effect
    var primaryFactorDropDown = $("#PrimaryFactor").data("kendoDropDownList");
    primaryFactorDropDown.dataSource.read({
        selectedTreatments: currentlySelectedTreatments
    });
}

function covariateBlockEnableDisable() {
    var covariateDropDown = $("#Covariate");
    var primaryFactorDropDown = $("#PrimaryFactor").data("kendoDropDownList");
    var covariateTransformationDropDown = $("#CovariateTransformation").data("kendoDropDownList");

    if (covariateDropDown.val().length > 0) {
        primaryFactorDropDown.enable(true);
        covariateTransformationDropDown.enable(true);
    }
    else {
        primaryFactorDropDown.enable(false);
        covariateTransformationDropDown.enable(false);
        covariateTransformationDropDown.value("None");
        primaryFactorDropDown.value(null);
    }
}