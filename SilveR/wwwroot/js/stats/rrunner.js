
$(function () {

    $("#VariableA").kendoMultiSelect({
        dataSource: theModel.availableVariables,
        value: theModel.variableA
    });
    $("#VariableB").kendoMultiSelect({
        dataSource: theModel.availableVariables,
        value: theModel.variableB
    });
    $("#VariableC").kendoMultiSelect({
        dataSource: theModel.availableVariables,
        value: theModel.variableC
    });
    $("#VariableD").kendoMultiSelect({
        dataSource: theModel.availableVariables,
        value: theModel.variableD
    });

});