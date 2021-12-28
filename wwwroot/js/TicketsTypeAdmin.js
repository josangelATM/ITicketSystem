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
            { "data": "id", "width": "10%" },
            { "data": "title", "width": "25%" },
            { "data": "description", "width": "50%" },
            {
                "data": "id",
                "render": function (data) {
                    return ` <div class="d-flex justify-content-evenly">
                        <button onClick=Delete('/Admin/TicketType/Delete/${data}')
                        class="btn btn-danger btn-sm">Delete</button>
                        <a href="/Admin/TicketType/Edit/${data}"
                        class="btn btn-primary btn-sm" >Edit</a>
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

