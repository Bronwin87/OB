$(function () {
    console.log($('#DataTable').data("request-url"));

    $('#DataTable tfoot th').each(function () {
        if ($(this).data("searchable") !== false) {
            var title = $(this).text();
            $(this).html('<input type="text" class="input" placeholder="Search ' + title + '" />');
        }
    });

    //$('#DataTable thead th').each(function () {
    //    if ($(this).data("searchable") !== false) {
    //        var title = $(this).text();
    //        $(this).append('<input type="text" class="input" placeholder="Search ' + title + '" />');
    //    }
    //});

    var oTable = $('#DataTable').DataTable({
        "processing": true,
        "serverSide": true,
        "ajax": {
            "url": $('#DataTable').data("request-url"),
            "type": "POST"
        },
        "sorting": [[0, "desc"]],
        "initComplete": function () {
            $("#DataTable_length select").addClass("select");
            $("#DataTable_filter label").addClass("label");
            $("#DataTable_filter input").addClass("input");
        }
    });

    oTable.columns().every(function () {
        var that = this;
        $('input', this.footer()).on('keyup change', function () {
            if (that.search() !== this.value) {
                that
                    .search(this.value)
                    .draw();
            }
        });
    });
});