var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tableData').DataTable({
        "ajax": {
            "url": "/Admin/User/All/"
        },
        "columns": [
            { "data": "user.employeeNumber", "width": "10%" },
            { "data": "user.userName", "width": "15%" },
            { "data": "user.email", "width": "15%" },
            { "data": "user.firstName", "width": "15%" },
            { "data": "user.lastname", "width": "15%" },
            { "data": "user.position", "width": "15%" },
            {
                "data": "user.id",
                "render": function (data) {
                    return ` <div class="d-flex justify-content-evenly">
                        <button onClick=Delete('/Admin/User/Delete/${data}')
                        class="btn btn-danger btn-sm">Delete</button>
                        <a href="/Admin/User/Edit/${data}"
                        class="btn btn-primary btn-sm">Edit</a>
                        </div>
                        `
                },
                "width": "20%"
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

