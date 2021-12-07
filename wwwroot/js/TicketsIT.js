var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tableData').DataTable({
        "ajax": {
            "url": "/IT/Ticket/GetTickets/" + $("#userId").val()
        },
        "columns": [
            { "data": "ticket.id", "width": "5%" },
            { "data": "ticketTypeTitle", "width": "15%" },
            { "data": "requester.userName", "width": "15%" },
            { "data": "ticket.status", "width": "15%" },
            { "data": "ticket.response", "width": "35%" },
            {
                "data": "ticket.id",
                "render": function (data) {
                    return ` <div>
                        <a href="/IT/Ticket/Details/${data}"
                        class="btn btn-primary">Edit</a>
					    </div>
                        `
                },
                "width": "15%"
            }
        ]
    });
}