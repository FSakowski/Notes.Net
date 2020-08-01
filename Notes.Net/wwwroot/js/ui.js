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