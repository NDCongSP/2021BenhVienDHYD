var alarmTable;
$(document).ready(function () {
    RefreshAlarms();
});

function RefreshAlarms() {
    $.ajax({
        url: "/Home/GetAlarms/",
        type: "GET",
        contentType: "application/json;charset=UTF-8",
        dataType: "json",
        success: function (result) {
            var html = '';
            $.each(result, function (key, item) {

                var classRow = '';
                if (item.Ack == 'No')
                    classRow = 'class="bg-danger"';

                html += '<tr ' + classRow + '>';
                html += '<td>' + (key + 1) + '</td>';
                html += '<td>' + ToJavaScriptDate(item.DateTime) + '</td>';
                html += '<td>' + item.LocationName + '</td>';
                html += '<td>' + item.Type + '</td>';
                html += '<td>' + item.Value + '</td>';
                html += '<td>' + item.LowLevel + '</td>';
                html += '<td>' + item.HighLevel + '</td>';
                html += '<td>' + item.Ack + '</td>';
                html += '</tr>';
            });

            

            if (!$.fn.dataTable.isDataTable('#alarmTable')) {
                $('#alarmTableBody').html(html);
                alarmTable = $('#alarmTable').DataTable({
                    ordering: false
                });
            }
            else {
                alarmTable.destroy();
                $('#alarmTableBody').html(html);
                alarmTable = $('#alarmTable').DataTable({
                    ordering: false
                });
            }
        },
        error: function (errormessage) {
        }
    })
}

function AckAlarms() {
    $.ajax({
        url: "/Home/AckAlarms/",
        type: "POST",
        contentType: "application/json;charset=UTF-8",
        dataType: "json",
        success: function (result) {
            if (result.Status == 'Ok') {
                RefreshAlarms();
            }
        },
        error: function (errormessage) {
        }
    })
}

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

//<tr class="bg-danger">
//    <td>1</td>
//    <td>2019-12-16 10:23:37</td>
//    <td>
//        Location 1
//    </td>
//    <td>High Alarm</td>
//    <td>100</td>
//    <td>0</td>
//    <td>90</td>
//    <td>No</td>
//</tr>
