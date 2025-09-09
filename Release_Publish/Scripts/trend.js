
var updateInterval = 1000;
var maxElements = 50;
var updateCount = 0;
var locationId = 0;
var realTimeChartInstance;
var historicalChartInstance

var commonOptions2 = {
    maintainAspectRatio: false,
    responsive: true,
    legend: {
        display: false,
        position: 'top'
    },
    scales: {
        xAxes: [{
            type: 'time',
            time: {
                tooltipFormat: 'YYYY-MM-DD HH:mm:ss',

            },
            gridLines: {
                display: true,
            },
            distribution: 'linear',
            bounds: 'ticks',
        }],
        yAxes: [{
            gridLines: {
                display: true,
            },
            ticks: {
                beginAtZero: true
            }
        }]
    },
    tooltips: {
        enabled: true,
        mode: 'index',
        intersect: false
    }
}

$(document).ready(function () {

    GetLocations();

    GetRealtimeTrendConfig();

    if (locationId == null || locationId == undefined)
        locationId = 0;

    $('#selectLocation').on('select2:select', function (e) {
        locationId = e.params.data.id;

        historicalChartInstance.data.labels.pop();
        historicalChartInstance.data.datasets.forEach((dataset) => {
            dataset.data.pop();
        });

        historicalChartInstance.update();
    });

    var realTimeChart = $("#realTimeChart");

    var commonOptions = {
        maintainAspectRatio: false,
        responsive: true,
        legend: {
            display: true,
            fillStyle: 'Color',
        },
        scales: {
            xAxes: [{
                type: 'time',
                time: {
                    displayFormats: {
                        second: 'HH:mm:ss'
                    },
                    unit: 'second'
                },
                gridLines: {
                    display: true,
                },
            }],
            yAxes: [{
                gridLines: {
                    display: true,
                },
                ticks: {
                    beginAtZero: true
                }
            }]
        },
        tooltips: {
            enabled: false
        }
    }

    realTimeChartInstance = new Chart(realTimeChart, {
        type: 'line',
        data: {
            datasets: [{
                label: 'Location',
                borderColor: 'rgba(60,141,188,1)',
                backgroundColor: 'rgba(60,141,188,0.1)',
                pointRadius: '0',
                pointColor: '#3b8bba',
                pointStrokeColor: 'rgba(60,141,188,1)',
                pointHighlightFill: '#fff',
                pointHighlightStroke: 'rgba(60,141,188,1)',
                borderWidth: '2',
            }]
        },
        options: Object.assign({}, commonOptions, {
            title: {
                display: false,
                text: "Acceleration - Z",
                fontSize: 18
            }
        })
    });

    historicalChartInstance = new Chart($('#historicalChart'), {
        type: 'line',
        data: {
            datasets: [{
                label: '',
                borderColor: 'rgba(60,141,188,1)',
                backgroundColor: 'rgba(60,141,188,0.1)',
                pointRadius: true,
                pointColor: '#3b8bba',
                pointStrokeColor: 'rgba(60,141,188,1)',
                pointHighlightFill: '#fff',
                pointHighlightStroke: 'rgba(60,141,188,1)',
            }]
        },
        options: Object.assign({}, commonOptions2, {
            title: {
                display: false,
                text: "Acceleration - Z",
                fontSize: 18
            }
        })
    });

    updateData();

    $('#fromLocation').datetimepicker({
        icons: {
            time: "fa fa-calendar-minus",
            date: "fa fa-calendar",
            up: "fa fa-arrow-up",
            down: "fa fa-arrow-down"
        },
        format: 'YYYY-MM-DD HH:mm'
    });
    $('#fromLocation').datetimepicker('date', moment().format('YYYY-MM-DD 00:00'));
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
    $('#toLocation').datetimepicker('date', moment().format('YYYY-MM-DD 23:59'));

    $("#fromLocation").on("change.datetimepicker", function (e) {
        $('#toLocation').datetimepicker('minDate', e.date);
        $(this).datetimepicker('hide');
    });
    $("#toLocation").on("change.datetimepicker", function (e) {
        $('#fromLocation').datetimepicker('maxDate', e.date);
        $(this).datetimepicker('hide');
    });

    $('#btnPreview').click(function () {
        var from = $('#fromLocation').datetimepicker('viewDate');
        var to = $('#toLocation').datetimepicker('viewDate');
        var param = $('#selectLocation2').find(':selected').val();
        var timeRange = {
            From: from,
            To: to,
            Parameter: param + '|'
        }

        if (param != null && param != '') {
            $.ajax({
                url: '/Home/GetDataLog',
                type: 'POST',
                data: JSON.stringify(timeRange),
                contentType: 'application/json;charset=utf-8',
                dataType: 'json',   
                success: function (result) {
                    historicalChartInstance.destroy();
                    historicalChartInstance = new Chart($('#historicalChart'), {
                        type: 'line',
                        data: {
                            datasets: [{
                                label: '',
                                borderColor: 'rgba(60,141,188,1)',
                                backgroundColor: 'rgba(60,141,188,0.1)',
                                pointRadius: '0',
                                pointColor: '#3b8bba',
                                pointStrokeColor: 'rgba(60,141,188,1)',
                                pointHighlightFill: '#fff',
                                pointHighlightStroke: 'rgba(60,141,188,1)',
                                lineTension: '0',
                                borderWidth: '2',
                            }],
                        },
                        options: Object.assign({}, commonOptions2, {
                            title: {
                                display: false,
                                text: "Acceleration - Z",
                                fontSize: 18
                            }
                        })
                    });

                    $.each(result, function (key, item) {
                        historicalChartInstance.data.labels.push(item.t);
                        historicalChartInstance.data.datasets.forEach((dataset) => { dataset.data.push(item.y) });
                    });

                    historicalChartInstance.update();
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
    return date.getFullYear() + "/" + (date.getMonth() + 1) + "/" + (date.getDate()) + " " + hour + ":" + mins + ":" + second;
}


//Add data to realtime chart
function addData(data) {
    if (data) {
        realTimeChartInstance.data.labels.push(new Date());
        realTimeChartInstance.data.datasets.forEach((dataset) => { dataset.data.push(data) });

        if (updateCount > maxElements) {
            realTimeChartInstance.data.labels.shift();
            realTimeChartInstance.data.datasets[0].data.shift();
        }
        else updateCount++;
        realTimeChartInstance.update();
    }
};

function updateData() {

    $.ajax({
        url: '/Home/GetRealtimeData/' + locationId,
        type: 'GET',
        dataType: 'json',
        contentType: 'application/json;charset=utf-8',
        success: function (result) {
            addData(result.Value);
            setTimeout(updateData, updateInterval);
        },
        error: function (error) {
            addData(null);
            setTimeout(updateData, updateInterval);
        }
    });
} 

function GetLocations() {
    $.ajax({
        url: '/Home/GetLocationWebLog',
        type: 'GET',
        dataType: 'json',
        contentType: 'application/json;charset=utf-8',
        success: function (result) {
            var html = '';
            
            $.each(result, function (key, item) {
                if (locationId == 0)
                    locationId = item.Id;
                html += '<option value="' + item.Id + '">';
                html += item.Name;
                html += '</option>';
            });

            //$('#selectLocationAlarm').html(html);
            $('#selectLocation').html(html);
            $('#selectLocation2').html(html);
        },
        error: function (error) {
            alert(error.responseText);
        }
    });
}

function GetRealtimeTrendConfig() {
    $.ajax({
        url: '/Home/GetRealtimeTrendConfig',
        type: 'GET',
        dataType: 'json',
        contentType: 'application/json;charset=utf-8',
        success: function (result) {
            updateInterval = result.Interval;
            maxElements = result.MaxElements;
        },
        error: function (error) {

        }
    });
}


