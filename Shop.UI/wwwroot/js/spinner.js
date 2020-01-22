$(function () {
    //$('.spinner .btn:first-of-type').on('click', function () {
    $('body').on('click', '.spinner .btn:first-of-type', function () {
        var btn = $(this);
        var input = btn.closest('.spinner').find('input');

        var inputIsDisabled = input.prop("disabled");
        var inputIsReadonly = input.prop("readonly");

        if (!inputIsDisabled && !inputIsReadonly) {
            var step = parseInt(input.attr('step'));
            if (!step) {
                step = 1;
            }
            if (input.attr('max') === undefined || parseInt(input.val()) < parseInt(input.attr('max'))) {
                input.val(parseInt(input.val(), 10) + step).trigger("change");
            } else {
                btn.next("disabled", true);
            }
        }
    });
    //$('.spinner .btn:last-of-type').on('click', function () {
    $('body').on('click', '.spinner .btn:last-of-type', function () {
        var btn = $(this);
        var input = btn.closest('.spinner').find('input');

        var inputIsDisabled = input.prop("disabled");
        var inputIsReadonly = input.prop("readonly");

        if (!inputIsDisabled && !inputIsReadonly) {
            var step = parseInt(input.attr('step'));
            if (!step) {
                step = 1;
            }
            if (input.attr('min') === undefined || parseInt(input.val()) > parseInt(input.attr('min'))) {
                input.val(parseInt(input.val(), 10) - step).trigger("change");
            } else {
                btn.prev("disabled", true);
            }
        }
    });
});