
$(function () {

    $("#Responses").kendoMultiSelect({
        dataSource: theModel.availableVariables,
        value: theModel.responses
    });

    $("#ResponseTransformation").kendoDropDownList({
        dataSource: theModel.transformationsList
    });

    $("#TestValue").kendoNumericTextBox({
        format: '#.######',
        decimals: 6,
        spinners: false
    });

    $("#Significance").kendoDropDownList({
        dataSource: theModel.significancesList
    });
});