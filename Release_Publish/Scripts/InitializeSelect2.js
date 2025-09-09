$(document).ready(function () {
    //Initialize Select2 Elements
    $('.select2bs4').select2({
        theme: 'bootstrap4'
    })

    if ($('.select2bs4').not(':visible')) {
        $('.select2bs4').width("100%");
    }

    //Initialize Select2 Elements
    $('.select2').select2()

    //Chỉnh select 2 đúng vị trí
    $('.select2-selection').css('padding', '6px');
    $('.select2-selection').css('height', '38px');
    $('.select2-selection__arrow').css('margin-top', '6px');
});