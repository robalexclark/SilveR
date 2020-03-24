$(function () {
    jQuery.ajaxSettings.traditional = true

    $("#Response").kendoDropDownList({
        dataSource: theModel.availableVariablesAllowNull
    });

    $("#PositiveResult").kendoDropDownList({
        cascadeFrom: "Response",
        dataSource: {
            transport: {
                read: {
                    url: "/Values/GetLevels",
                    data: function () {
                        return {
                            treatment: $("#Response").val(),
                            datasetID: $("#DatasetID").val(),
                            includeNull: false
                        }
                    }
                }
            },
            serverFiltering: true,
            schema: { "errors": "Errors" }
        }
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

    $("#CovariateTransformation").kendoDropDownList({
        dataSource: theModel.transformationsList
    });

    $("#Significance").kendoDropDownList({
        dataSource: theModel.significancesList
    });

    treatmentsChanged();

    covariateBlockEnableDisable();
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