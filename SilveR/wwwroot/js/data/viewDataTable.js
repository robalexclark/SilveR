$(function () {

    const spreadSheet = $("#spreadsheet").kendoSpreadsheet({
        toolbar: false,
        sheetsbar: false,
        sheets: [spreadsheet]
    });

    $("#save").click(function () {
        if (!$(this).hasClass("k-state-disabled")) {
            //dataSource.sync();
            const data = spreadSheet.data("kendoSpreadsheet").toJSON();

            $.ajax({
                type: "POST",
                url: "/data/UpdateDataSet",
                data: JSON.stringify(data),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    if (response === true) {
                        alert("Data saved successfully!");
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