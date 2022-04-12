var locationTable;

$(document).ready(function () {
    RefreshLocations();
    //Khởi tạo table location
    //locationTable = $("#tableLocation").DataTable();

    
    $('#ckbAlarmSms').change(function () {
        var enableSMS = $('#ckbAlarmSms').prop('checked');
        if (enableSMS == true)
            $("#smsNumber").prop("disabled", false);
        else
            $("#smsNumber").prop("disabled", true);
    });

    $('#ckbAlarmEmail').change(function () {
        if ($('#ckbAlarmEmail').prop('checked') == true)
            $("#email").prop("disabled", false);
        else
            $("#email").prop("disabled", true);
    });

    //Lấy thông số modusRTU
    RefreshModbusRTUParameters();

    //Lấy thông số settings
    RefreshServerParameters();

    //Lấy thông số alarm
    RefreshAlarmParameters();

    $('#btnSaveModbusRTU').click(function () {

        var port = $('#portName').val();
        var baudrate = $('#baudrate').find(':selected').text();
        var databits = $('#databits').find(':selected').text();
        var parity = $('#parity').find(':selected').text();
        var stopbits = $('#stopbits').find(':selected').text();
        var timeout = $('#timeout').val();

        //Kiểm tra time out
        if (timeout < 100) {
            toastr.error('Time out must be larger than 100. Please enter time out again.');
            return;
        }

        var obj = {
            Port: port,
            Baudrate: baudrate,
            Databits: databits,
            Parity: parity,
            Stopbits: stopbits,
            Timeout: timeout
        };

        $.ajax({
            url: "/Home/SaveModbusRTU",
            data: JSON.stringify(obj),
            type: "POST",
            dataType: "json",
            contentType: "application/json;charset=utf-8",
            success: function (result) {
                if (result.Status == 'Ok')
                    toastr.success('Save successfully.');
                else {
                    toastr.error('Save failed');
                    RefreshModbusRTUParameters();
                }
            },
            error: function (errorMessage) {
                toastr.error('Save failed');
                RefreshModbusRTUParameters();
            }
        });

    });

    $('#btnSaveServer').click(function () {

        //var webLogId = $('#atWebLogId').val();
        //var serverIp = $('#serverIp').val();
        var timeRate = $('#timeRate').val();
        var logType = $('#logType').find(':selected').text();

        //Kiểm tra time out
        if (timeRate < 1) {
            toastr.error('Log time rate must be larger than 1. Please enter log time rate again.');
            return;
        }

        //if (ValidateIPaddress(serverIp) == false) {
        //    toastr.error('The server ip address is incorrect format. Please enter again.');
        //    return;
        //}

        var obj = {
            //WebLoggerId: webLogId,
            //ServerIp: serverIp,
            LogTimeRate: timeRate,
            LogType: logType
        };

        $.ajax({
            url: "/Home/SaveServerParameters",
            data: JSON.stringify(obj),
            type: "POST",
            dataType: "json",
            contentType: "application/json;charset=utf-8",
            success: function (result) {
                if (result.Status == 'Ok')
                    toastr.success('Save successfully.');
                else {
                    toastr.error('Save failed');
                    RefreshServerParameters();
                }
            },
            error: function (errorMessage) {
                toastr.error('Save failed');
                RefreshServerParameters();
            }
        });

    });

    $('#btnSaveAlarm').click(function () {
        var enableSMS = $('#ckbAlarmSms').prop('checked');
        var enableEmail = $('#ckbAlarmEmail').prop('checked');
        var sms = $("#smsNumber").val();
        var email = $("#email").val();
        var smsKDNT = $("#smsNumberKDNT").val();
        var emailKDNT = $("#emailKDNT").val();

        var obj = {
            EnableEmail: enableEmail,
            EnableSMS: enableSMS,
            Email: email,
            SMS: sms,
            SMSKDNT: smsKDNT,
            EmailKDNT: emailKDNT
        };

        $.ajax({
            url: "/Home/SaveAlarmParameters",
            data: JSON.stringify(obj),
            type: "POST",
            dataType: "json",
            contentType: "application/json;charset=utf-8",
            success: function (result) {
                if (result.Status == 'Ok')
                    toastr.success('Save successfully.');
                else {
                    toastr.error('Save failed');
                    RefreshAlarmParameters();
                }
            },
            error: function (errorMessage) {
                toastr.error('Save failed');
                RefreshAlarmParameters();
            }
        });

    });

    $('#btnReboot').click(function () {
        $.ajax({
            url: "/Home/RestartDevices",
            type: "POST",
            dataType: "json",
            contentType: "application/json;charset=utf-8",
            success: function (result) {
                if (result.Status == 'Ok')
                    toastr.success('Successfully.');
                else {
                    toastr.error('Failed');
                    RefreshAlarmParameters();
                }
            },
            error: function (errorMessage) {
                toastr.error('Failed');
                RefreshAlarmParameters();
            }
        });

    });
    
    $('#btnUpdatePassword').click(function () {
        var pass = $('#modal-password').val();
        if (pass == '' || pass == null) {
            toastr.error("Password can't be empty.");
            return;
        }
        var newPass = $('#modal-newpassword').val();
        if (newPass == '' || newPass == null) {
            toastr.error("New password can't be empty.");
            return;
        }
        else {
            var comfirmPass = $('#modal-comfirmpassword').val();

            if (comfirmPass != newPass) {
                toastr.error("New password and comfirm do not match !");
            }
            else {
                var obj = {
                    Password: pass,
                    NewPassword: newPass,
                }

                $.ajax({
                    url: "/Home/UpdatePassword",
                    data: JSON.stringify(obj),
                    type: "POST",
                    contentType: "application/json;charset=UTF-8",
                    dataType: "json",
                    success: function (result) {
                        if (result.Status == true) {
                            toastr.success('Change password successfully.');
                        }
                        else {
                            toastr.error('Change password failed.');
                        }
                    },
                    error: function (errormessage) {
                        toastr.error('Change password failed.');
                    }
                });
            }
        }

    });
});

function RefreshModbusRTUParameters() {
    $.ajax({
        url: "/Home/GetModbusRTUParameters",
        type: "POST",
        dataType: "json",
        contentType: "application/json;charset=utf-8",
        success: function (result) {
            $('#portName').val(result.Port);

            $('#baudrate').val(result.Baudrate); 
            $('#baudrate').trigger('change');

            $('#databits').val(result.DataBits);
            $('#databits').trigger('change');

            $('#parity').val(result.Parity);
            $('#parity').trigger('change');

            $('#stopbits').val(result.Stopbits);
            $('#stopbits').trigger('change');

            $('#timeout').val(result.Timeout);

        },
        error: function (errorMessage) {
        }
    });
}

function RefreshServerParameters() {
    $.ajax({
        url: "/Home/GetServerParameters",
        type: "POST",
        dataType: "json",
        contentType: "application/json;charset=utf-8",
        success: function (result) {
            $('#atWebLogId').val(result.WebLoggerId);
            $('#serverIp').val(result.ServerIpDisplay);
            $('#timeRate').val(result.LogTimeRate);

            $('#logType').val(result.LogType);
            $('#logType').trigger('change');
        },
        error: function (errorMessage) {
        }
    });
}

function RefreshAlarmParameters() {
    $.ajax({
        url: "/Home/GetAlarmParameters",
        type: "POST",
        dataType: "json",
        contentType: "application/json;charset=utf-8",
        success: function (result) {
            if (result.EnableEmail == true) {
                $('#ckbAlarmEmail').prop('checked', true);
            }
            else {
                $('#ckbAlarmEmail').prop('checked', false);
            }

            if (result.EnableSMS == true) {
                $('#ckbAlarmSms').prop('checked', true);
            }
            else {
                $('#ckbAlarmSms').prop('checked', false);
            }

            $('#smsNumber').val(result.SMS);
            $('#email').val(result.Email);
            $('#smsNumberKDNT').val(result.SMSKDNT);
            $('#emailKDNT').val(result.EmailKDNT);
        },
        error: function (errorMessage) {
        }
    });
}

var tableLocation;

function RefreshLocations() {
    $.ajax({
        url: "/Home/GetLocations",
        type: "POST",
        dataType: "json",
        contentType: "application/json;charset=utf-8",
        success: function (result) {
            var html = '';
            $.each(result, function (key, item) {
                html += '<tr>';
                html += '<td>' + (key + 1) + '</td>';
                html += '<td>' + item.Name + '</td>';
                html += '<td>' + item.DeviceId + '</td>';
                html += '<td>' + item.MemoryAddress + '</td>';
                html += '<td>' + item.DataType + '</td>';
                html += '<td>' + item.LowLevel + '</td>';
                html += '<td>' + item.HighLevel + '</td>';
                html += '<td>' + item.Gain + '</td>';
                html += '<td>' + item.Offset + '</td>';
                html += '<td>' + item.Deadband + '</td>';
                html += '<td>' + item.State + '</td>';
                var editStr = 'onclick="return getLocationById(' + item.Id + ')"';
                var deleteStr = 'onclick="deleteLocation(' + item.Id + ')"';
                html += '<td class="text-center"><a class="btn btn-info btn-sm" href="#" style="margin:5px;" ' + editStr + '><i class="fas fa-pencil-alt"></i></a><a class="btn btn-danger btn-sm" style="margin:5px;" href="#" ' + deleteStr + '><i class="fas fa-trash"></i></a>' + '</td>';
                html += '</tr>';
            });
            

            if (!$.fn.dataTable.isDataTable('#tableLocation')) {
                $('#bodyLocations').html(html);
                tableLocation = $('#tableLocation').DataTable();
            }
            else {
                tableLocation.destroy();
                $('#bodyLocations').html(html);
                tableLocation = $('#tableLocation').DataTable();
            }
        },
        error: function (errorMessage) {
        }
    });

    //$("#tableLocation").DataTable().rows().invalidate().draw();
}

function getLocationById(id) {
    $.ajax({
        url: "/Home/GetLocationById/" + id,
        type: "POST",
        contentType: "application/json;charset=UTF-8",
        dataType: "json",
        success: function (result) {
            $('#modal-id').val(result.Id);
            $('#modal-name').val(result.Name);
            $('#modal-deviceId').val(result.DeviceId);
            $('#modal-address').val(result.MemoryAddress);
            $('#modal-dataType').val(result.DataType);
            $('#modal-dataType').trigger('change')
            $('#modal-lowlevel').val(result.LowLevel);
            $('#modal-highlevel').val(result.HighLevel);
            $('#modal-gain').val(result.Gain);
            $('#modal-offset').val(result.Offset);
            $('#modal-deadband').val(result.Deadband);
            $('#modal-state').prop('checked', result.Status);

            $('#modalLocation').modal('show');
            $('#btnUpdateLocation').show();
            $('#btnAddLocation').hide();
            $('#modalLocation-label').html('Edit Location');
        },
        error: function (errormessage) {
        }
    });
}

var deleteId;

function deleteLocation(id) {
    $('#modal-question').modal('show');
    deleteId = id;
}

$('#btnYes').click(function () {
    $.ajax({
        url: "/Home/DeleteLocationById/" + deleteId,
        type: "POST",
        contentType: "application/json;charset=UTF-8",
        dataType: "json",
        success: function (result) {
            if (result.Status == 'Ok') {
                toastr.success('Delete location successfully.');
            }
            else {
                toastr.error('Delete location failed.');
            }
            RefreshLocations();
        },
        error: function (errormessage) {
            toastr.error('Delete location failed.');
        }
    });

    $('#modal-question').modal('hide');
});

function showAddLocation() {
    clearModalLocation();
    $('#btnUpdateLocation').hide();
    $('#btnAddLocation').show();
    $('#modalLocation-label').html('Add Location');
    $('#modalLocation').modal('show');
}

function addLocation() {
    if (validateLocation() == true) {
        var obj = {
            Name: $('#modal-name').val(),
            DeviceId: $('#modal-deviceId').val(),
            MemoryAddress: $('#modal-address').val(),
            DataType: $('#modal-dataType').val(),
            LowLevel: $('#modal-lowlevel').val(),
            HighLevel: $('#modal-highlevel').val(),
            Gain: $('#modal-gain').val(),
            Offset: $('#modal-offset').val(),
            Deadband: $('#modal-deadband').val(),
            State: $('#modal-state').val()
        }

        $.ajax({
            url: "/Home/AddLocation/",
            data: JSON.stringify(obj),
            type: "POST",
            contentType: "application/json;charset=UTF-8",
            dataType: "json",
            success: function (result) {
                if (result.Status == 'Ok') {
                    toastr.success('Add location successfully.');
                }
                else {
                    toastr.error('Add location failed.');
                }
                RefreshLocations();
            },
            error: function (errormessage) {
                toastr.error('Add location failed.');
            }
        });

        $('#modalLocation').modal('hide');
    }
}

function clearModalLocation() {
    $('#modal-id').val('');
    $('#modal-name').val('Item 1');
    $('#modal-deviceId').val('1');
    $('#modal-address').val('40001');
    $('#modal-dataType').val('Word');
    $('#modal-lowlevel').val('-30');
    $('#modal-highlevel').val('100');
    $('#modal-gain').val('1');
    $('#modal-offset').val('0');
    $('#modal-deadband').val('0');
    $('#modal-state').prop('checked', true);
}

function validateLocation() {
    var Name = $('#modal-name').val(); 
    if (isNullOrEmpty(Name) == true) {
        toastr.error('Location name can not be empty.');
        return false;
    }

    var DeviceId = $('#modal-deviceId').val();
    if (isNullOrEmpty(DeviceId) == true || DeviceId < 0 || DeviceId > 255) {
        toastr.error('The Device Id is not in valid range. The valid range is 0 - 255');
        return false;
    }

    var DataType = $('#modal-dataType').val();
    if (isNullOrEmpty(DataType) == true) {
        toastr.error('The data type can not be empty');
        return false;
    }

    var MemoryAddress = $('#modal-address').val();
    if (isNullOrEmpty(MemoryAddress) == true || MemoryAddress <= 0) {
        toastr.error('The memory address is not in valid range.');
        return false;
    }
    else {
        if (DataType == "Bool") {
            if (MemoryAddress < 1 || MemoryAddress == 10000 || MemoryAddress > 19999) {
                toastr.error('The memory address is not in valid range.');
                return false;
            }
        }
        else {
            if (MemoryAddress < 30001 || MemoryAddress == 40000 || MemoryAddress > 49999) {
                toastr.error('The memory address is not in valid range.');
                return false;
            }
        }
    }

    var LowLevel = $('#modal-lowlevel').val();
    var HighLevel = $('#modal-highlevel').val();

    var Gain = $('#modal-gain').val();
    if (isNullOrEmpty(Gain) == true) {
        toastr.error('The Gain can not be empty');
        return false;
    }
    var Offset = $('#modal-offset').val();
    if (isNullOrEmpty(Gain) == true) {
        toastr.error('The Offset can not be empty');
        return false;
    }
    var Deadband = $('#modal-deadband').val();
    var State = $('#modal-state').val();

    if (isNullOrEmpty(State) == true) {
        toastr.error('The State can not be empty');
        return false;
    }

    return true;
}

function isNullOrEmpty(text)
{
    return (!text || text == undefined || text == "" || text.length == 0);
}

function updateLocation() {
    if (validateLocation() == true) {
        var obj = {
            Id: $('#modal-id').val(),
            Name: $('#modal-name').val(),
            DeviceId: $('#modal-deviceId').val(),
            MemoryAddress: $('#modal-address').val(),
            DataType: $('#modal-dataType').val(),
            LowLevel: $('#modal-lowlevel').val(),
            HighLevel: $('#modal-highlevel').val(),
            Gain: $('#modal-gain').val(),
            Offset: $('#modal-offset').val(),
            Deadband: $('#modal-deadband').val(),
            State: $('#modal-state').val(),
            Value: $('#modal-id').val()
        }

        $.ajax({
            url: "/Home/UpdateLocation/",
            data: JSON.stringify(obj),
            type: "POST",
            contentType: "application/json;charset=UTF-8",
            dataType: "json",
            success: function (result) {
                if (result.Status == 'Ok') {
                    toastr.success('Update location successfully.');
                }
                else {
                    toastr.error('Update location failed.');
                }
                RefreshLocations();
            },
            error: function (errormessage) {
                toastr.error('Update location failed.');
            }
        });

        $('#modalLocation').modal('hide');
    }
}

function ValidateIPaddress(ipaddress) {
    if (/^(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$/.test(ipaddress)) {
        return true;
    }
    return false;
}