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

    $("#Treatments").kendoMultiSelect({
        dataSource: theModel.availableVariables,
        value: theModel.treatments,
        change: treatmentsChanged
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

    const primaryFactorDropdown = $("#PrimaryFactor").kendoDropDownList({
        dataSource: theModel.treatments,
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


    $("#LowerBoundAbsolute").kendoNumericTextBox({
        format: '#.##',
        decimals: 2,
        spinners: true,
        step: 0.01
    });

    $("#UpperBoundAbsolute").kendoNumericTextBox({
        format: '#.##',
        decimals: 2,
        spinners: true,
        step: 0.01
    });

    $("#LowerBoundPercentageChange").kendoNumericTextBox({
        format: '#.##',
        decimals: 2,
        spinners: true,
        min: 0,
        max: 1000
    });

    $("#UpperBoundPercentageChange").kendoNumericTextBox({
        format: '#.##',
        decimals: 2,
        spinners: true,
        min: 0,
        max: 1000
    });


    $("#Significance").kendoDropDownList({
        dataSource: theModel.significancesList
    });

    const selectedEffect = $("#SelectedEffect").kendoDropDownList({
        dataSource: {
            transport: {
                read: {
                    url: "/Values/GetSMPASelectedEffectsList"
                }
            }
        },
        change: selectedEffectChanged
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

        selectedEffectChanged();
    });

    $("#AllPairwise").kendoDropDownList({
        dataSource: theModel.pairwiseTestList
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

    treatmentsChanged();

    covariateBlockEnableDisable();

    enableDisableEquivalenceType();
});

function treatmentsChanged() {
    const treatmentMultiSelect = $("#Treatments").data("kendoMultiSelect");

    $.ajax({
        type: 'GET',
        url: "/Values/GetSMPAInteractions",
        data: { selectedTreatments: treatmentMultiSelect.dataItems() },
        success: function (data) {
            var markup = '';
            for (var x = 0; x < data.length; x++) {
                markup += '<option value="' + data[x] + '">' + data[x] + '</option>';
            }

            $('#Interactions').html(markup).show();
        }
    });


    //treatments have changed so fill in the primary factor...
    const primaryFactorDropDown = $("#PrimaryFactor").data("kendoDropDownList");
    primaryFactorDropDown.setDataSource($("#Treatments").data("kendoMultiSelect").dataItems());

    //...and the selected effect
    const selectedEffectDropDown = $("#SelectedEffect").data("kendoDropDownList");
    selectedEffectDropDown.dataSource.read({
        selectedTreatments: $("#Treatments").data("kendoMultiSelect").dataItems()
    });
}

//if the selected effect is changed then fill in the control group
function selectedEffectChanged() {

    const controlGroup = $("#ControlGroup").data("kendoDropDownList");

    controlGroup.value(null);
    controlGroup.dataSource.read({ treatment: $("#SelectedEffect").val(), datasetID: $("#DatasetID").val(), includeNull: true });

    enableDisableControlLevels();
}

function covariateBlockEnableDisable() {
    const covariateDropDown = $("#Covariates");
    const primaryFactorDropDown = $("#PrimaryFactor").data("kendoDropDownList");
    const covariateTransformationDropDown = $("#CovariateTransformation").data("kendoDropDownList");

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

//function selectedEffectsBlockEnableDisable() {
//    const treatmentMultiSelect = $("#Treatments").data("kendoMultiSelect");

//    const comparisonsBackToControl = $("[Value='ComparisonsToControl']");
//    const allPairwise = $("[Value='AllPairwise']");
//    const controlGroup = $('#ControlGroup').data("kendoDropDownList");

//    if (treatmentMultiSelect != null && treatmentMultiSelect.value().length > 1 && selectedEffectDropDown.value().indexOf("*") == -1) {
//        comparisonsBackToControl.attr('disabled', 'disabled');
//        comparisonsBackToControl.checked = false;
//        allPairwise.checked = true;

//        controlGroup.enable(false);
//        controlGroup.value(null);
//    }
//    else {
//        comparisonsBackToControl.removeAttr('disabled', 'disabled');
//        controlGroup.enable(true);
//    }

//    enableDisableControlLevels();
//}

function enableDisableEquivalenceType() {

    const analysisType = $('input:radio[name="EquivalenceBoundsType"]:checked').val();

    const lowerBoundAbsolute = $("#LowerBoundAbsolute").data("kendoNumericTextBox");
    const upperBoundAbsolute = $("#UpperBoundAbsolute").data("kendoNumericTextBox");
    const lowerBoundPercentage = $("#LowerBoundPercentageChange").data("kendoNumericTextBox");
    const upperBoundPercentage = $("#UpperBoundPercentageChange").data("kendoNumericTextBox");

    if (analysisType == "Absolute") {
        lowerBoundAbsolute.enable(true);
        upperBoundAbsolute.enable(true);

        lowerBoundPercentage.enable(false);
        upperBoundPercentage.enable(false);
        lowerBoundPercentage.value(null);
        upperBoundPercentage.value(null);
    } else {
        lowerBoundAbsolute.enable(false);
        upperBoundAbsolute.enable(false);
        lowerBoundAbsolute.value(null);
        upperBoundAbsolute.value(null);

        lowerBoundPercentage.enable(true);
        upperBoundPercentage.enable(true);
    }
}

function enableDisableControlLevels() {
    const treatmentMultiSelect = $("#Treatments").data("kendoMultiSelect");
    const selectedEffectDropDown = $("#SelectedEffect").data("kendoDropDownList");
    const comparisonsBackToControlChecked = $("[Value='ComparisonsToControl']:checked").val();

    const controlDropDown = $("#ControlGroup").data("kendoDropDownList");

    if (comparisonsBackToControlChecked != null && treatmentMultiSelect.value().length == 1 && selectedEffectDropDown.value().indexOf("*") == -1) {
        controlDropDown.enable(true);
    }
    else {
        controlDropDown.select(-1);
        controlDropDown.value(null);
        controlDropDown.enable(false);
    }
}