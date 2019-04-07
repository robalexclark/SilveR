
$(function () {
    $("#SelectedTest").kendoDropDownList({
        dataSource: theModel.multipleComparisonTests
    });

    $("#Significance").kendoDropDownList({
        dataSource: theModel.significancesList
    });
});