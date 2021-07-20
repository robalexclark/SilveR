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
        format: '#.#',
        decimals: 1,
        spinners: true,
        step: 0.1
    });

    $("#UpperBoundAbsolute").kendoNumericTextBox({
        format: '#.#',
        decimals: 1,
        spinners: true,
        step: 0.1
    });

    $("#LowerBoundPercentage").kendoNumericTextBox({
        format: '#.#',
        decimals: 1,
        spinners: true,
        min: 0,
        max: 100
    });

    $("#UpperBoundPercentage").kendoNumericTextBox({
        format: '#.#',
        decimals: 1,
        spinners: true,
        min: 0,
        max: 100
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

    selectedEffectsBlockEnableDisable();
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

function selectedEffectsBlockEnableDisable() {
    const treatmentMultiSelect = $("#Treatments").data("kendoMultiSelect");
    const selectedEffectDropDown = $("#SelectedEffect").data("kendoDropDownList");
    const controlGroup = $('#ControlGroup').data("kendoDropDownList");

    if (treatmentMultiSelect != null && treatmentMultiSelect.value().length > 0 && selectedEffectDropDown.value().indexOf("*") == -1) {
        controlGroup.enable(true);
    }
    else {
        controlGroup.enable(false);
        controlGroup.value(null);
    }
}

function enableDisableEquivalenceType() {

    const analysisType = $('input:radio[name="EquivalenceBoundsType"]:checked').val();

    const lowerBoundAbsolute = $("#LowerBoundAbsolute").data("kendoNumericTextBox");
    const upperBoundAbsolute = $("#UpperBoundAbsolute").data("kendoNumericTextBox");
    const lowerBoundPercentage = $("#LowerBoundPercentage").data("kendoNumericTextBox");
    const upperBoundPercentage = $("#UpperBoundPercentage").data("kendoNumericTextBox");

    if (analysisType == "Absolute") {
        lowerBoundAbsolute.enable(true);
        upperBoundAbsolute.enable(true);

        lowerBoundPercentage.enable(false);
        upperBoundPercentage.enable(false);
    } else {
        lowerBoundAbsolute.enable(false);
        upperBoundAbsolute.enable(false);

        lowerBoundPercentage.enable(true);
        upperBoundPercentage.enable(true);
    }
}