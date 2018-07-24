
$(function () {

    $("#grid").kendoGrid({
        sortable: true,
        columns: [
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
        var dataItem = this.dataItem($(e.currentTarget).closest("tr"));

        window.location.href = '/analyses/viewresults?analysisGuid=' + dataItem.analysisGuid;
    };

    function reAnalyse(e) {
        var dataItem = this.dataItem($(e.currentTarget).closest("tr"));

        window.location.href = '/analyses/reanalyse?analysisGuid=' + dataItem.analysisGuid;
    };
});