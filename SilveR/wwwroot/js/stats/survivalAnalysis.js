
$(function () {
    $("#Response").kendoDropDownList({
        dataSource: theModel.availableVariablesAllowNull
    });

    $("#Grouping").kendoDropDownList({
        dataSource: theModel.availableVariablesAllowNull
    });

    $("#Censorship").kendoDropDownList({
        dataSource: theModel.availableVariablesAllowNull
    });

    $("#Significance").kendoDropDownList({
        dataSource: theModel.significancesList
    });
});