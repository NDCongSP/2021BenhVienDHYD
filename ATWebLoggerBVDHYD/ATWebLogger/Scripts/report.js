var tbLocation;
$(document).ready(function () {
    GetLocations();

    //Initialize Select2 Elements
    $('.multipleSelect').select2({
        theme: 'bootstrap4',
        multiple: true
    });

    $('.select2bs4').select2({
        theme: 'bootstrap4',
    });

    if ($('.select2bs4').not(':visible')) {
        $('.select2bs4').width("100%");
    };


    //Initialize Select2 Elements
    $('.multipleSelect').select2({
        multiple: true
    });

    $('.select2').select2({
    });

    //Chỉnh select 2 đúng vị trí
    $('.select2-selection').css('padding', '6px');
    $('.select2-selection').css('height', '38px');
    $('.select2-selection__arrow').css('margin-top', '6px');

    $('#fromLocation').datetimepicker({
        icons: {
            time: "fa fa-calendar-minus",
            date: "fa fa-calendar",
            up: "fa fa-arrow-up",
            down: "fa fa-arrow-down"
        },
        format: 'YYYY-MM-DD HH:mm'
    });
    $('#toLocation').datetimepicker({
        icons: {
            time: "fa fa-calendar-minus",
            date: "fa fa-calendar",
            up: "fa fa-arrow-up",
            down: "fa fa-arrow-down"
        },
        useCurrent: false,
        format: 'YYYY-MM-DD HH:mm'
    });
    $("#fromLocation").on("change.datetimepicker", function (e) {
        $('#toLocation').datetimepicker('minDate', e.date);
        console.log(this)
        $("#fromLocation").datetimepicker('hide');
        //var fromLocation = document.getElementById('fromLocation')
        //fromLocation.datetimepicker('hide');

        //$(this).datetimepicker('hide');
    });
    $("#toLocation").on("change.datetimepicker", function (e) {
        $('#fromLocation').datetimepicker('maxDate', e.date);
        $(this).datetimepicker('hide');
    });
    $('#fromLocation').datetimepicker('date', moment().format('YYYY-MM-DD 00:00'));
    $('#toLocation').datetimepicker('date', moment().format('YYYY-MM-DD 23:59'));

    $('#fromAlarm').datetimepicker({
        icons: {
            time: "fa fa-calendar-minus",
            date: "fa fa-calendar",
            up: "fa fa-arrow-up",
            down: "fa fa-arrow-down"
        },
        format: 'YYYY-MM-DD HH:mm'
    });
    $('#toAlarm').datetimepicker({
        icons: {
            time: "fa fa-calendar-minus",
            date: "fa fa-calendar",
            up: "fa fa-arrow-up",
            down: "fa fa-arrow-down"
        },
        useCurrent: false,
        format: 'YYYY-MM-DD HH:mm'
    });
    $("#fromAlarm").on("change.datetimepicker", function (e) {
        $('#toAlarm').datetimepicker('minDate', e.date);
        $(this).datetimepicker('hide');
    });
    $("#toAlarm").on("change.datetimepicker", function (e) {
        $('#fromAlarm').datetimepicker('maxDate', e.date);
        $(this).datetimepicker('hide');
    });
    $('#fromAlarm').datetimepicker('date', moment().format('YYYY-MM-DD 00:00'));
    $('#toAlarm').datetimepicker('date', moment().format('YYYY-MM-DD 23:59'));


    $('#btnPreviewLocation').click(function () {
        var from = $('#fromLocation').datetimepicker('viewDate');
        var to = $('#toLocation').datetimepicker('viewDate');
        var param = $("#selectLocation option:selected").val();
        var location = $("#selectLocation option:selected").text();
        var timeRange = {
            From: from,
            To: to,
            Parameter: param + '|' + location
        }
        $('#tbHeader').html('');
        $('#tbBody').html('');
        if (param != null && param != '') {
            $.ajax({
                url: '/Home/GetLocationData',
                type: 'POST',
                data: JSON.stringify(timeRange),
                contentType: 'application/json;charset=utf-8',
                dataType: 'json',
                success: function (result) {
                    var header = '';
                    header += '<th>#</th>'
                    header += '<th>DateTime</th>'
                    header += '<th>Value</th>'
                    var html = '';
                    $.each(result.Data, function (key, item) {
                        const datetime = moment();
                        console.log(ToJavaScriptDate(item.DateTime))
                        html += '<tr>';
                        html += '<td>' + (key + 1) + '</td>';
                        html += '<td>' + ToJavaScriptDate(item.DateTime) + '</td>';
                        html += '<td>' + item.Value + '</td>';
                        html += '</tr>';
                    });      

                    if (!$.fn.dataTable.isDataTable('#logTable')) {
                        $('#tbHeader').html(header);
                        $('#tbBody').html(html);
                        tbLocation = $('#logTable').DataTable({
                            ordering: true,
                        });
                    }
                    else {
                        tbLocation.destroy();
                        $('#tbHeader').html(header);
                        $('#tbBody').html(html);
                        tbLocation = $('#logTable').DataTable({
                            ordering: true,
                        });
                    }
                },
                error: function (errorMessage) {
                    alert(errorMessage.reponseText);
                }
            });
        }
    });

    $('#btnPreviewAlarm').click(function () {
        var from = $('#fromAlarm').datetimepicker('viewDate');
        var to = $('#toAlarm').datetimepicker('viewDate');
        var param = $("#selectLocationAlarm option:selected").text();
        var type = $("#selectAlarmType option:selected").text();
        var timeRange = {
            From: from,
            To: to,
            Parameter: param + '|' + type
        };

        $('#tbHeader').html('');
        $('#tbBody').html('');

        if (param != null && param != '') {
            $.ajax({
                url: '/Home/GetAlarmData',
                type: 'POST',
                data: JSON.stringify(timeRange),
                contentType: 'application/json;charset=utf-8',
                dataType: 'json',
                success: function (result) {
                    var header = '';
                    header += '<th>DateTime</th>'
                    header += '<th>Location</th>'
                    header += '<th>Status</th>'
                    header += '<th>Value</th>'
                    header += '<th>Low Level</th>'
                    header += '<th>High Level</th>'
                    header += '<th>Ack</th>'
                    var html = '';
                    $.each(result.Data, function (key, item) {
                        var classRow = '';
                        if (item.Ack == 'No')
                            classRow = 'class="bg-danger"';

                        html += '<tr ' + classRow + '>';
                        //html += '<td>' + ToJavaScriptDate(item.DateTime) + '</td>';                  
                        html += '<td>' + ToJavaScriptDate(item.DateTime) + '</td>';
                        html += '<td>' + item.LocationName + '</td>';
                        html += '<td>' + item.Type + '</td>';
                        html += '<td>' + item.Value + '</td>';
                        html += '<td>' + item.LowLevel + '</td>';
                        html += '<td>' + item.HighLevel + '</td>';
                        html += '<td>' + item.Ack + '</td>';
                        html += '</tr>';
                    });
                    
                    if (!$.fn.dataTable.isDataTable('#logTable')) {
                        $('#tbHeader').html(header);
                        $('#tbBody').html(html);
                        tbLocation = $('#logTable').DataTable({
                            ordering: true,
                        });
                    }
                    else {
                        tbLocation.destroy();
                        $('#tbHeader').html(header);
                        $('#tbBody').html(html);
                        tbLocation = $('#logTable').DataTable({
                            ordering: true,
                        });

                    }
                },
                error: function (errorMessage) {
                    alert(errorMessage.reponseText);
                }
            });
        }
    });

    $('#btnExportLocation').click(function () {
        var from = $('#fromLocation').datetimepicker('viewDate');
        var to = $('#toLocation').datetimepicker('viewDate');
        var param = $("#selectLocation option:selected").val();
        var location = $("#selectLocation option:selected").text();
        var timeRange = {
            From: from,
            To: to,
            Parameter: param + '|' + location
        }

        if (param != null && param != '') {
            $.ajax({
                url: '/Home/ExportLocationData',
                type: 'POST',
                data: JSON.stringify(timeRange),
                contentType: 'application/json;charset=utf-8',
                dataType: 'json',
                success: function (result) {
                    if (result.Status == 'Ok') {
                        window.location = "/Home/DowloadExportLocation";
                    }
                },
                error: function (errorMessage) {
                    alert(errorMessage.reponseText +" Loi");

                }
            });
        }
    });

    $('#btnExportAlarm').click(function () {
        var from = $('#fromAlarm').datetimepicker('viewDate');
        var to = $('#toAlarm').datetimepicker('viewDate');
        var param = $("#selectLocationAlarm option:selected").text();
        var type = $("#selectAlarmType option:selected").text();
        var timeRange = {
            From: from,
            To: to,
            Parameter: param + '|' + type
        };

        if (param != null && param != '') {
            $.ajax({
                url: '/Home/ExportAlarmData',
                type: 'POST',
                data: JSON.stringify(timeRange),
                contentType: 'application/json;charset=utf-8',
                dataType: 'json',
                success: function (result) {
                    if (result.Status = 'Ok') {
                        window.location = "/Home/DowloadExportAlarm";
                    }
                },
                error: function (errorMessage) {
                    alert(errorMessage.reponseText);
                }
            });
        }
    });
});

function ToJavaScriptDate(ms) {
    var pattern = /Date\(([^)]+)\)/;
    var results = pattern.exec(ms);
    var date = new Date(parseFloat(results[1]));
    var hour = date.getHours().toString();
    var mins = date.getMinutes().toString();
    var second = date.getSeconds().toString();

    // fix hour format
    if (hour.length == 1) {
        hour = "0" + hour;
    }

    // fix minutes format
    if (mins.length == 1) {
        mins = "0" + mins;
    }

    // fix second format
    if (second.length == 1) {
        second = "0" + second;
    }


    // return formatted date time string
    //return date.getFullYear() + 1 + "/" + (date.getMonth() + 1) + "/" + (date.getDate()) + " " + hour + ":" + mins + ":" + second;
    return date.getFullYear() + "/" + (date.getMonth() + 1) + "/" + (date.getDate()) + " " + hour + ":" + mins + ":" + second;
}

function GetLocations() {
    $.ajax({
        url: '/Home/GetLocationWebLog',
        type: 'GET',
        dataType: 'json',
        contentType: 'application/json;charset=utf-8',
        success: function (result) {
            var html = '';
            //html += '<option value="0">All</option>';
            $.each(result, function (key, item) {
                html += '<option value="' + item.Id + '">';
                html += item.Name;
                html += '</option>';
            });
            
            $('#selectLocationAlarm').html(html);
            $('#selectLocation').html(html);
        },
        error: function (error) {
            alert(error.responseText);
        }
    });
}

