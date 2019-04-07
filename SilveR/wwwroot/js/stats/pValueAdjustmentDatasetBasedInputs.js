
$(function () {

    $("#PValues").kendoDropDownList({
        dataSource: theModel.availableVariablesAllowNull
    });

    $("#SelectedTest").kendoDropDownList({
        dataSource: theModel.multipleComparisonTests
    });

    $("#Significance").kendoDropDownList({
        dataSource: theModel.significancesList
    });
});