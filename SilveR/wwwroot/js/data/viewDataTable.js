
$(function () {
    var crudServiceBaseUrl = "https://demos.telerik.com/kendo-ui/service";

    var dataSource = new kendo.data.DataSource({
        transport: {
            read: onRead
        },
        batch: true,
        change: function () {
            $("#cancel, #save").toggleClass("k-state-disabled", !this.hasChanges());
        }
    });

    $("#spreadsheet").kendoSpreadsheet({
        columns: 20,
        rows: 100,
        toolbar: false,
        sheetsbar: false,
        sheets: [{
            name: "Products",
            dataSource: dataSource
        }]
    });


    function onRead(options) {
        $.ajax({
            url: crudServiceBaseUrl + "/Products",
            dataType: "jsonp",
            success: function (result) {
                options.success(result);
            },
            error: function (result) {
                options.error(result);
            }
        });
    }

    $("#save").click(function () {
        if (!$(this).hasClass("k-state-disabled")) {
            dataSource.sync();
        }
    });

    $("#cancel").click(function () {
        if (!$(this).hasClass("k-state-disabled")) {
            dataSource.cancelChanges();
        }
    });
});
