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

    $("#RepeatedFactor").kendoDropDownList({
        dataSource: theModel.availableVariablesAllowNull,
        change: updateSelectedEffects
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

    $("#Covariance").kendoDropDownList({
        dataSource: theModel.covariancesList,
    });

    $("#Significance").kendoDropDownList({
        dataSource: theModel.significancesList
    });

    const selectedEffect = $("#SelectedEffect").kendoDropDownList({
        dataSource: {
            transport: {
                read: {
                    url: "/Values/GetRMPASelectedEffectsList"
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

    treatmentsChanged();

    covariateBlockEnableDisable();
});

function treatmentsChanged() {
    const treatmentMultiSelect = $("#Treatments").data("kendoMultiSelect");

    $.ajax({
        type: 'GET',
        url: "/Values/GetRMPAInteractions",
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
    updateSelectedEffects();
}

function updateSelectedEffects() {
    const selectedEffectDropDown = $("#SelectedEffect").data("kendoDropDownList");
    selectedEffectDropDown.dataSource.read({
        selectedTreatments: $("#Treatments").data("kendoMultiSelect").dataItems(),
        repeatedFactor: $("#RepeatedFactor").data("kendoDropDownList").value()
    });
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