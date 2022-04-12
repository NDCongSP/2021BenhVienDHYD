$(document).ready(function () {
    loadLocations();
});

function loadLocations() {
    $.ajax({
        url: '/Home/GetLocationWebLog',
        type: 'GET',
        dataType: 'json',
        contentType: 'application/json;charset=utf-8',

        success: function (result) {
            var html = '';

            $.each(result, function (key, item) {
                if (item.State == 'Enable') {

                    var valueId = 'locationValue' + item.Id;
                    var statusId = 'locationStatus' + item.Id;
                    html += '<div class="col-xl-4 col-lg-6 col-md-6 col-12">';
                    html += '<div class="info-box bg-gradient-navy">';
                    html += '<div class="info-box-content">';
                    html += '<span class="info-box-text text-center text-xl">' + item.Name + '</span>';
                    html += '<hr/>';
                    html += '<div class="row">';

                    html += '<div class="col">';
                    html += '<span class="text-lg">Value: </span><span id="' + valueId + '" class="text-lg text-success">' + item.Value + '</span>'
                    html += '</div>';

                    html += '<div class="col">';
                    if (item.Status == 'Good') {
                        html += '<span class="text-lg">Status: </span><span id="' + statusId + '"  class="text-lg text-success">' + item.Status + '</span>'
                    }
                    else {
                        html += '<span class="text-lg">Status: </span><span id="' + statusId + '"  class="text-lg text-danger">' + item.Status + '</span>'
                    }
                    html += '</div>';

                    html += '</div>';
                    html += '</div>';
                    html += '</div>';
                    html += '</div>';
                }
            });
            $('#locationContainer').html(html);
            refresh(); 
        },
        error: function (error) {
            alert(error.responseText);
        }

    });
}

function refresh() {
    $.ajax({
        url: '/Home/GetRealtimeLocations',
        type: 'GET',
        dataType: 'json',
        contentType: 'application/json;charset=utf-8',
        success: function (result) {
            $.each(result, function (key, item) {
                var valueId = '#locationValue' + item.Id;
                var statusId = '#locationStatus' + item.Id;
                //$('#locationContainer').find(valueId).html(item.Value);

                var status = $('#locationContainer').find(statusId);
                if (status) {
                    if (item.Status == 'Good') {
                        status.removeClass('text-danger');
                        status.addClass('text-success');
                    }
                    else {
                        status.removeClass('text-success');
                        status.addClass('text-danger');
                    }
                    status.html(item.Status);
                }

                var value = $('#locationContainer').find(valueId);
                if (value) {
                    if (item.IsHighAlarm == false && item.IsLowAlarm == false) {
                        value.removeClass('text-danger');
                        value.addClass('text-success');
                    }
                    else {
                        value.removeClass('text-success');
                        value.addClass('text-danger');
                    }
                    value.html(item.Value);
                }
            });
            setTimeout(refresh, 500);
        },
        error: function (error) {
            setTimeout(refresh, 500);
        }
    });  
}