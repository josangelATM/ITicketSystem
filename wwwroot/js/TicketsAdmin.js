var dataTable;
$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tableData').DataTable({
        "ajax": {
            "url": "/Admin/Ticket/GetAll/"
        },
        "columns": [
            { "data": "ticket.id", "width": "5%" },
            { "data": "ticketTypeTitle", "width": "10%" },
            { "data": "assignedTo.userName", "width": "10%" },
            { "data": "requester.userName", "width": "10%" },
            { "data": "ticket.status", "width": "15%" },
            { "data": "ticket.response", "width": "35%" },
            {
                "data": "ticket.id",
                "render": function (data) {
                    return ` <div class="d-flex justify-content-evenly">
                        <button onClick=Delete('/Admin/Ticket/Delete/${data}')
                        class="btn btn-danger btn-sm">Delete</button>
                        <a href="/IT/Ticket/Details/${data}"
                        class="btn btn-primary btn-sm">Edit</a>
					    </div>
                        `
                },
                "width": "15%"
            }
        ]
    });
}

function Delete(url) {
    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: 'DELETE',
                success: function (data) {
                    if (data.success) {
                        dataTable.ajax.reload();
                        notyf.success(data.message)
                    } else {
                        notyf.error(data.message)
                    }
                }
            })
        }
    })
}

