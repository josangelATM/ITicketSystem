var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tableData').DataTable({
        "ajax": {
            "url": "/Employee/Ticket/GetTickets/" + $("#userId").val()
        },
        "columns": [
            { "data": "ticket.id", "width": "5%" },
            { "data": "ticketTypeTitle", "width": "15%" },
            { "data": "assignedTo.username", "width": "15%" },
            { "data": "ticket.status", "width": "15%" },
            { "data": "ticket.response", "width": "35%" },
            {
                "data": "ticket.id",
                "render": function(data) {
                    return ` <div>
                        <a href="/Employee/Ticket/Details/${data}"
                        class="btn btn-primary">Go to Ticket</a>
					    </div>
                        `
                },
                "width": "15%"
            }
        ]
    });
}