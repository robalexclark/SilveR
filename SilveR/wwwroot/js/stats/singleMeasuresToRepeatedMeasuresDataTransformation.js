
$(function () {

    $("#Responses").kendoMultiSelect({
        dataSource: theModel.availableVariables,
        value: theModel.responses
    });

    $("#SubjectFactor").kendoDropDownList({
        dataSource: theModel.availableVariablesAllowNull,
        value: theModel.subjectFactor
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