var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tableData').DataTable({
        "ajax": {
            "url": "/IT/TicketType/GetAll/"
        },
        "columns": [
            { "data": "id", "width": "15%" },
            { "data": "title", "width": "35%" },
            { "data": "description", "width": "50%" }
        ]
    });
}

