
$(function () {
    $("#Response").kendoDropDownList({
        dataSource: theModel.availableVariablesAllowNull
    });

    $("#GroupingFactor").kendoDropDownList({
        dataSource: theModel.availableVariablesAllowNull
    });

    $("#ResponseCategories").kendoDropDownList({
        dataSource: theModel.availableVariablesAllowNull
    });

    $("#Hypothesis").kendoDropDownList({
        dataSource: theModel.hypothesesList
    });

    $("#Significance").kendoDropDownList({
        dataSource: theModel.significancesList
    });
});