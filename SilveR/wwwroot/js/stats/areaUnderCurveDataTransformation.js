
$(function () {

    $("#Response").kendoDropDownList({
        dataSource: theModel.availableVariablesAllowNull,
        value: theModel.response
    });

    $("#SubjectFactor").kendoDropDownList({
        dataSource: theModel.availableVariablesAllowNull,
        value: theModel.subjectFactor
    });

    $("#TimeFactor").kendoDropDownList({
        dataSource: theModel.availableVariablesAllowNull,
        value: theModel.timeFactor
    });

    $("#Responses").kendoMultiSelect({
        dataSource: theModel.availableVariables,
        value: theModel.responses
    });

    $("#SelectedVariables").kendoMultiSelect({
        dataSource: theModel.availableVariables,
        value: theModel.selectedVariables
    });  


    const checkbox = $("#IncludeAllVariables");
    checkbox.change(function (event) {
        const cbx = event.target;
        checkBoxChanged(cbx);
    });

    function checkBoxChanged(cbx) {

        const selectedVariablesMultiSelect = $("#SelectedVariables").data("kendoMultiSelect");

        if (cbx.checked) {
            selectedVariablesMultiSelect.enable(false);
            selectedVariablesMultiSelect.value("");
        } else {
            selectedVariablesMultiSelect.enable(true);
        }
    }

    checkBoxChanged(checkbox);
});

function enableDisableMeasuresFormatType() {

    const selectedInputFormat = $('input:radio[name="SelectedInputFormat"]:checked').val();

    const response = $("#Response").data("kendoDropDownList");
    const subjectFactor = $("#SubjectFactor").data("kendoDropDownList");
    const timeFactor = $("#TimeFactor").data("kendoDropDownList");

    const responses = $("#Responses").data("kendoMultiSelect");
    const numericalTimePoints = $("#NumericalTimePoints");

    if (selectedInputFormat === "RepeatedMeasuresFormat") {
        response.enable(true);
        subjectFactor.enable(true);
        timeFactor.enable(true);

        responses.enable(false);
        responses.value("");

        numericalTimePoints.prop("disabled", true);
        numericalTimePoints.val("");
    }
    else if (selectedInputFormat === "SingleMeasuresFormat") {
        response.enable(false);
        response.select(0);

        subjectFactor.enable(false);
        subjectFactor.select(0);

        timeFactor.enable(false);
        timeFactor.select(0);

        responses.enable(true);
        numericalTimePoints.prop("disabled", false);
    }
}