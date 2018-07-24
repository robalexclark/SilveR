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

    $("#RepeatedFactor").kendoDropDownList({
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

    $("#Covariance").kendoDropDownList({
        dataSource: theModel.covariancesList,
    });

    $("#Significance").kendoDropDownList({
        dataSource: theModel.significancesList
    });

    var selectedEffect = $("#SelectedEffect").kendoDropDownList({
        dataSource: {
            transport: {
                read: {
                    url: "/Values/GetSelectedEffectsList"
                }
            }
        }
    }).data("kendoDropDownList");
    selectedEffect.bind("dataBound", function (e) {
        if (selectedEffect.select() === -1) {
            if (theModel.selectedEffect) {
                selectedEffect.value(theModel.selectedEffect);
            }
            else {
                selectedEffect.select(0);
            }
        }

        selectedEffect.enable(true);
    });

    treatmentsChanged({ currentPrimaryFactor: theModel.primaryFactor, currentSelectedEffect: theModel.selectedEffect });

    covariateBlockEnableDisable();
});

function treatmentsChanged() {
    var treatmentMultiSelect = $("#Treatments").data("kendoMultiSelect");

    $.ajax({
        type: 'GET',
        url: "/Values/GetInteractions",
        data: { selectedTreatments: treatmentMultiSelect.dataItems() },
        success: function (data) {
            var markup = '';
            for (var x = 0; x < data.length; x++) {
                markup += '<option value="' + data[x] + '">' + data[x] + '</option>';
            }

            $('#Interactions').html(markup).show();
        }
    });


    var currentlySelectedTreatments = $("#Treatments").data("kendoMultiSelect").dataItems();

    //treatments have changed so fill in the primary factor and the selected effect
    var primaryFactorDropDown = $("#PrimaryFactor").data("kendoDropDownList");
    primaryFactorDropDown.dataSource.read({
        selectedTreatments: currentlySelectedTreatments
    });

    var selectedEffectDropDown = $("#SelectedEffect").data("kendoDropDownList");
    selectedEffectDropDown.dataSource.read({
        selectedTreatments: currentlySelectedTreatments
    });

    selectedEffectsBlockEnableDisable();
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
        primaryFactorDropDown.enable(false);
        covariateTransformationDropDown.value("None");
    }
}

function selectedEffectsBlockEnableDisable() {
    var treatmentMultiSelect = $("#Treatments").data("kendoMultiSelect");
    var selectedEffectDropDown = $("#SelectedEffect").data("kendoDropDownList");
    var lsMeansSelectedCheckBox = $("#LSMeansSelected");

    if (treatmentMultiSelect != null && treatmentMultiSelect.value().length > 0 && selectedEffectDropDown.value().indexOf("*") == -1) {
        selectedEffectDropDown.enable(true);
        lsMeansSelectedCheckBox.prop("disabled", false);
    }
    else {
        selectedEffectDropDown.enable(false);
        lsMeansSelectedCheckBox.prop("disabled", true);
    }
}