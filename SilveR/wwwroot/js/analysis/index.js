$(function () {

    $("#grid").kendoGrid({
        sortable: true,
        columns: [
            { selectable: true, width: "40px" },
            { title: "Analysis Type", field: "scriptDisplayName" },
            { title: "Dataset Name", field: "datasetName" },
            { title: "Date Analysed", field: "dateAnalysed", format: "{0:dd MMM yy HH:mm}" },

            { command: { text: "View Results", click: viewResults }, title: " " },
            { command: { text: "Re-Analyse", click: reAnalyse }, title: " " },
            {
                command: [{ text: "Delete", name: "destroy" }]
            }
        ],
        scrollable: true,
        editable: {
            confirmation: "Are you sure you want to delete this record?", confirmDelete: "Delete", cancelDelete: "Cancel", mode: "inline", destroy: true
        },
        dataSource: {
            type: "GET",
            transport: {
                read: {
                    url: "/Analyses/GetAnalyses"
                },
                destroy: {
                    url: "/Analyses/Destroy",
                    type: "DELETE"
                }
            },
            schema: {
                model: {
                    id: "analysisID",
                    fields: {
                        analysisGuid: { type: "string" },
                        datasetID: { type: "number" },
                        datasetName: { type: "string" },
                        aspNetUserID: { type: "string" },
                        scriptID: { type: "number" },
                        scriptDisplayName: { type: "string" },
                        rProcessOutput: { type: "string" },
                        htmlOutput: { type: "string" },
                        dateAnalysed: { type: "date" }
                    }
                }
            }
        }
    });

    function viewResults(e) {
        const dataItem = this.dataItem($(e.currentTarget).closest("tr"));
        window.location.href = '/analyses/viewresults?analysisGuid=' + dataItem.analysisGuid;
    }

    function reAnalyse(e) {
        const dataItem = this.dataItem($(e.currentTarget).closest("tr"));
        window.location.href = '/analyses/reanalyse?analysisGuid=' + dataItem.analysisGuid;
    }

    $("#deleteSelected").click(function () {
        const grid = $("#grid").data("kendoGrid");
        // Get selected rows
        var sel = $("input:checked", grid.tbody).closest("tr");
        // Get data item for each
        var analysisIDs = [];
        $.each(sel, function (idx, row) {
            var item = grid.dataItem(row);
            analysisIDs.push(item.analysisID);
        });

        $.ajax({
            type: "POST",
            url: "/Analyses/DeleteSelected",
            data: { analysisIDs: analysisIDs },
            success: function () {
                grid.dataSource.read();
            }
        });
    });
});