
$(function () {
    $("#Response").kendoDropDownList({
        dataSource: theModel.availableVariablesAllowNull
    });

    $("#ResponseTransformation").kendoDropDownList({
        dataSource: theModel.transformationsList
    });

    $("#Treatment").kendoDropDownList({
        dataSource: theModel.availableVariablesAllowNull,
        change: treatmentChanged
    });

    const controlGroup = $("#ControlGroup").kendoDropDownList({
        autoBind: false,
        dataSource: {
            transport: {
                read: {
                    url: "/Values/GetLevels"
                }
            }
        }
    }).data("kendoDropDownList");
    controlGroup.bind("dataBound", function (e) {
        controlGroup.value(theModel.controlGroup);
    });


    $("#Significance").kendoDropDownList({
        dataSource: theModel.significancesList
    });

    treatmentChanged();
});

function treatmentChanged() {

    const controlGroup = $("#ControlGroup").data("kendoDropDownList");

    controlGroup.value(null);
    controlGroup.dataSource.read({ treatment: $("#Treatment").val(), datasetID: $("#DatasetID").val(), includeNull: true });
}