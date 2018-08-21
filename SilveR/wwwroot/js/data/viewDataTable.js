$(function () {

    //var dataSource = new kendo.data.DataSource({
    //    transport: {
    //        read: onRead
    //    },
    //    batch: true,
    //    change: function () {
    //        $("#cancel, #save").toggleClass("k-state-disabled", !this.hasChanges());
    //    }
    //});

    var spreadSheet = $("#spreadsheet").kendoSpreadsheet({
        //columns: 20,
        toolbar: false,
        sheetsbar: false,
        sheets: [spreadsheet]
    });

    //function onRead(options) {
    //    $.ajax({
    //        url: crudServiceBaseUrl,
    //        dataType: "json",
    //        success: function (result) {
    //            options.success(result);
    //        },
    //        error: function (result) {
    //            options.error(result);
    //        }
    //    });
    //}

    $("#save").click(function () {
        if (!$(this).hasClass("k-state-disabled")) {
            //dataSource.sync();
            var data = spreadSheet.data("kendoSpreadsheet").toJSON();

            $.ajax({
                type: "POST",
                url: "/data/UpdateDataSet",
                data: JSON.stringify(data),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    if (response === true) {
                        alert("Details saved successfully!!!");
                    }
                    else {
                        alert(response);
                    }
                }
            });
        }
    });

    $("#cancel").click(function () {
        if (!$(this).hasClass("k-state-disabled")) {
            location.reload(true)
        }
    });
});