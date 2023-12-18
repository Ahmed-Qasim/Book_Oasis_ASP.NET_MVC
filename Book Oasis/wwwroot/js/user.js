﻿

var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": { url: '/admin/user/GetAll' },
        "columns": [
            { data: 'name', "width": "25%" },
            { data: 'email', "width": "15%" },
            { data: 'phoneNumber', "width": "10%" },
            { data: 'company.name', "width": "15%" },
            { data: 'role', "width": "10%" },
            {
                data: { id: "id", lockoutEnd:"lockoutEnd"},
                "render": function (data) {
                    var today = new Date().getTime();
                    var lockout = new Date(data.lockoutEnd).getTime();
                    if (lockout > today) {
                        return `<div class="text-center">
                             <a  onClick=lockUnlock('${data.id}') class="btn btn-danger text-white" style="cursor:pointer" width:150px;>
                                  <i class="bi bi-lock-fill"></i> unlock</a>
                              <a  href="/admin/user/RoleManagement?userId=${data.id}" class="btn btn-danger text-white" style="cursor:pointer" width:150px;>
                                  <i class="bi bi-pencil-square"></i> permission</a>
                    </div>`
                    } else {
                        return `<div class="text-center">
                             <a onClick=lockUnlock('${data.id}') class="btn btn-success text-white" style="cursor:pointer" width:150px;>
                             <i class="bi bi-unlock-fill"></i>
                             lock</a>

                               <a href="/admin/user/RoleManagement?userId=${data.id}" class="btn btn-danger text-white" style="cursor:pointer" width:150px;>
                                 <i class="bi bi-pencil-square"></i>
                                permission</a>
                        </div>`
                    }
                    
                },
                "width": "25%"
            }
        ]
    });
}

function lockUnlock(id) {
    $.ajax({
        type: "POST",
        url: "/admin/user/LockUnlock",
        data: JSON.stringify(id),
        contentType: "application/json",
        success: function (data) {
            if (data.success) {
                toastr.success(data.message);
                dataTable.ajax.reload();
            }
        }
    })
}



