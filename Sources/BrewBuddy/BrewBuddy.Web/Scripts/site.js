$(function () {
    $('input[type=text]:first').focus();

    var styleErrorElements = function() {
        $('.field-validation-error').each(function (i, item) {
            $(item).parent('.control-group').addClass('error');
            $(item).addClass('help-inline');
        });
        $('.validation-summary-errors').addClass('alert alert-error');
    };
    styleErrorElements();
    
    $('form').submit(function () {
        if (!$(this).valid()) {
            styleErrorElements();
        }
    });

    $('.btn-danger').click(function(e) {
        if (!confirm('Are you sure?')) {
            e.preventDefault();
        }
    });
});