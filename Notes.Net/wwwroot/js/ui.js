var deadManTimer;

$(document).ready(function () {
    $('#dismiss, .overlay').on('click', function () {
        $('#sidebar').removeClass('active');
        $('.overlay').removeClass('active');
    });

    $('#sidebarCollapse').on('click', function () {
        $('#sidebar').addClass('active');
        $('.overlay').addClass('active');
        $('.collapse.in').toggleClass('in');
        $('a[aria-expended=true]').attr('aria-expanded', 'false');
    });

    $('.dead-mans-handle').on('click', function (evt) {
        evt.preventDefault();
    });

    $('.dead-mans-handle').on('mousedown', function () {
        var button = $(this);
        var form = button[0].form;
        button.addClass("progress-animation");

        if (deadManTimer)
            clearTimeout(deadManTimer);

        deadManTimer = setTimeout(function () { form.submit(); }, 4000);
    });

    $('.dead-mans-handle').on('mouseup', function () {
        clearTimeout(deadManTimer);
        deadManTimer = null;
        $(this).removeClass("progress-animation");
    });
})

$(function () {

    $('.list-group-item').on('click', function () {
        if ($(this).hasClass('collapsed')) {
            $(this).find('svg').attr('data-icon', 'angle-right');
        } else {
            $(this).find('svg').attr('data-icon', 'angle-down');
        }
    });

});

function showToast(message) {
    var panel = $("#toast-panel");

    if (!panel)
        return;

    var template = $($('#toast-template').html());
    template.appendTo(panel);
    template.find(".toast-body").text(message);
    template.toast({ delay: 2000 });
    template.toast('show');
}
