
$(function () {

    $("#Responses").kendoMultiSelect({
        dataSource: theModel.availableVariables,
        value: theModel.responses
    });

    $("#SelectedVariables").kendoMultiSelect({
        dataSource: theModel.availableVariables,
        value: theModel.selectedVariables
    });
});