var dataTable;

$(function () {
    //debugger;
    const urlParams = new URLSearchParams(window.location.search);
    const status = urlParams.get('status');
    loadDataTable(status);
})

function loadDataTable(status) {
    dataTable = $('#tblData').DataTable({
        "order":[[0,'desc']],
        "ajax": {
            "url": "/order/GetAll?status=" + status
        },
        "columns": [
            { "data": "orderHeaderId", "width": "10%" },
            { "data": "name", "width": "15%" },
            { "data": "email", "width": "20%" },
            { "data": "phone", "width": "10%" },
            { "data": "orderStatus", "width": "10%" },
            { "data": "orderTotal", "width": "10%" },
            {
                "data": "orderHeaderId",
                "render": function (data) {
                    return `
                        <div class="text-center">
                            <a href="/Order/OrderDetails?orderId=${data}" class="border btn btn-success text-white" 
                            style="cursor:pointer; width:90%;" title="Order Details">
                                <i class="bi bi-pencil-square"></i>
                            </a>
                        </div>
                    `;
                }, "width": "5%"
            }
        ]
    });
}