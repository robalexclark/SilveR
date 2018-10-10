
$(function () {

    $("#grid").kendoGrid({
        sortable: true,
        columns: [
            { title: "Name", field: "datasetName" },
            { title: "Version", field: "versionNo" },
            { title: "Date Uploaded", field: "dateUpdated", format: "{0:dd MMM yy HH:mm}" },
            { command: { text: "View Data", click: viewData }, title: " " },
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
                    url: "/Data/GetDatasets"
                },
                destroy: {
                    url: "/Data/Destroy",
                    type: "DELETE"
                }
            },
            schema: {
                model: {
                    id: "datasetID",
                    fields: {
                        datasetName: { type: "string" },
                        versionNo: { type: "number" },
                        dateUpdated: { type: "date" }
                    }
                }
            }
        }
    });

    function viewData(e) {
        const dataItem = this.dataItem($(e.currentTarget).closest("tr"));

        window.location.href = window.location.protocol + '/Data/ViewDataTable?datasetId=' + dataItem.datasetID, true;
    };
});