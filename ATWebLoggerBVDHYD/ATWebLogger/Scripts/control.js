$(document).ready(function () {
    //$("#logTable").DataTable();
    RefreshLogTable();
    $('#btnWriteHolding').click(function () {
        var address = $('#addressHolding').val();
        var dataType = $('#dataTypeHolding').val();
        var value = $('#valueHolding').val();
        var deviceId = $('#deviceIdHolding').val();
        WriteValue(deviceId, address, dataType, value);
    });

    $('#btnWriteCoil').click(function () {
        var address = $('#addressCoil').val();
        var dataType = $('#dataTypeCoil').val();
        var value = $('#valueCoil').val();
        var deviceId = $('#deviceIdCoil').val();
        WriteValue(deviceId, address, dataType, value);
    });

    $('#btnWriteATMS').click(function () {
        var deviceId = $('#deviceIdATMS').val();
        var deadband = $('#deadbandATMS').val();
        var temphigh = $('#temphighATMS').val();
        var templow = $('#templowATMS').val();
        var offset = $('#offsetATMS').val();
        WriteValueATMS(deviceId, deadband, temphigh, templow, offset);
    });

    $('#btnReadHolding').click(function () {
        var address = $('#addressHolding').val();
        var dataType = $('#dataTypeHolding').val();
        var deviceId = $('#deviceIdHolding').val();
        ReadValue(deviceId, address, dataType, $('#valueHolding'));
    });

    $('#btnReadCoil').click(function () {
        var address = $('#addressCoil').val();
        var dataType = $('#dataTypeCoil').val();
        var deviceId = $('#deviceIdCoil').val();
        ReadValue(deviceId, address, dataType, $('#valueCoil'));
    });

    $('#btnReadATMS').click(function () {
        var deviceId = $('#deviceIdATMS').val();
        ReadValueATMS(deviceId, $('#deadbandATMS'), $('#temphighATMS'), $('#templowATMS'), $('#offsetATMS'));
    });
});

function WriteValueATMS(deviceId, deadband, temphigh, templow, offset) {
    var obj = {
        DeviceId: deviceId,
        Deadband: deadband,
        TempHigh: temphigh,
        TempLow: templow,
        Offset: offset
    };

    $.ajax({
        url: "/Home/WriteValueATMS/",
        data: JSON.stringify(obj),
        type: "POST",
        contentType: "application/json;charset=UTF-8",
        dataType: "json",
        success: function (result) {
            if (result.Status == 'Ok') {
                toastr.success('Write value successfully.');
            }
            else {
                toastr.error('Write value failed.');
            }
            RefreshLogTable();
        },
        error: function (errormessage) {
            toastr.error('Write value failed.');
        }
    });
}

function ReadValueATMS(deviceId, deadband, temphigh, templow, offset) {
    var obj = {
        DeviceId: deviceId,
    };
    deadband.val('');
    temphigh.val('');
    templow.val('');
    offset.val('');
    $.ajax({
        url: "/Home/ReadValueATMS/",
        data: JSON.stringify(obj),
        type: "POST",
        contentType: "application/json;charset=UTF-8",
        dataType: "json",
        success: function (result) {
            if (result.Status == 'Ok') {
                deadband.val(result.Deadband);
                temphigh.val(result.TempHigh);
                templow.val(result.TempLow);
                offset.val(result.Offset);
                toastr.success('Read value successfully.');
            }
            else {
                toastr.error('Read value failed.');
            }
            RefreshLogTable();
        },
        error: function (errormessage) {
            toastr.error('Read value failed.');
        }
    });
}

function WriteValue(deviceId, address, dataType, value) {

    if (isNullOrEmpty(deviceId) == true || deviceId < 0 || deviceId > 255) {
        toastr.error('The Device Id is not in valid range. The valid range is 0 - 255');
        return false;
    }

    if (isNullOrEmpty(dataType) == true) {
        toastr.error('The data type can not be empty.');
        return;
    }

    if (isNullOrEmpty(address) == true) {
        toastr.error('The memory address is not in valid range.');
        return;
    }
    else {
        if (dataType == "Bool") {
            if (address < 1 || address > 9999) {
                toastr.error('The memory address is not in valid range.');
                return;
            }
        }
        else {
            if (address < 40001 || address > 49999) {
                toastr.error('The memory address is not in valid range.');
                return;
            }
        }
    }

    if (isNullOrEmpty(value)) {
        toastr.error('The value can not be empty.');
        return;
    }

    var obj = {
        DeviceId: deviceId,
        Address: address,
        DataType: dataType,
        Value: value
    };

    $.ajax({
        url: "/Home/WriteValue/",
        data: JSON.stringify(obj),
        type: "POST",
        contentType: "application/json;charset=UTF-8",
        dataType: "json",
        success: function (result) {
            if (result.Status == 'Ok') {
                toastr.success('Write value successfully.');
            }
            else {
                toastr.error('Write value failed.');
            }
            RefreshLogTable();
        },
        error: function (errormessage) {
            toastr.error('Write value failed.');
        }
    });
}

function ReadValue(deviceId, address, dataType, inputText) {
    if (isNullOrEmpty(deviceId) == true || deviceId < 0 || deviceId > 255) {
        toastr.error('The Device Id is not in valid range. The valid range is 0 - 255');
        return false;
    }

    if (isNullOrEmpty(dataType) == true) {
        toastr.error('The data type can not be empty.');
        return;
    }

    if (isNullOrEmpty(address) == true) {
        toastr.error('The memory address is not in valid range.');
        return;
    }
    else {
        if (dataType == "Bool") {
            if (address < 1 || address == 10000 || address > 19999) {
                toastr.error('The memory address is not in valid range.');
                return;
            }
        }
        else {
            if (address < 30001 || address == 40000 || address > 49999) {
                toastr.error('The memory address is not in valid range.');
                return;
            }
        }
    }

    var obj = {
        DeviceId: deviceId,
        Address: address,
        DataType: dataType
    };

    inputText.val('');

    $.ajax({
        url: "/Home/ReadValue/",
        data: JSON.stringify(obj),
        type: "POST",
        contentType: "application/json;charset=UTF-8",
        dataType: "json",
        success: function (result) {
            if (result.Status == 'Ok') {
                inputText.val(result.Value);
                toastr.success('Read value successfully.');
            }
            else {
                toastr.error('Read value failed.');
            }
            RefreshLogTable();
        },
        error: function (errormessage) {
            toastr.error('Read value failed.');
        }
    });
}

function isNullOrEmpty(text) {
    return (!text || text == undefined || text == "" || text.length == 0);
}
var logTable;
function RefreshLogTable() {
    $.ajax({
        url: "/Home/GetWriteLog",
        type: "POST",
        dataType: "json",
        contentType: "application/json;charset=utf-8",
        success: function (result) {
            var html = '';
            $.each(result, function (key, item) {
                html += '<tr>';
                html += '<td>' + (key + 1)+ '</td>';
                html += '<td>' + item.DateTime + '</td>';
                html += '<td>' + item.Action + '</td>';
                html += '<td>' + item.Result + '</td>';
                html += '</tr>';
            });

            if (!$.fn.dataTable.isDataTable('#logTable')) {
                $('#logTableBody').html(html);
                logTable = $('#logTable').DataTable();
            }
            else {
                logTable.destroy();
                $('#logTableBody').html(html);
                logTable = $('#logTable').DataTable();
            }
        },
        error: function (errorMessage) {
        }
    });

    //$("#tableLocation").DataTable().rows().invalidate().draw();
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
    return date.getFullYear() + 1 + "/" + (date.getMonth() + 1) + "/" + (date.getDate()) + " " + hour + ":" + mins + ":" + second;
}

//<tr>
//    <td>2019-12-16 10:23:37</td>
//    <td>
//        Write
//    </td>
//    <td>Ok</td>
//</tr>