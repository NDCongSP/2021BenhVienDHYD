using ATWebLogger.Core;
using ATWebLogger.Models;
using CoreData;
using MySql.Data.MySqlClient;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web.Mvc;

namespace ATWebLogger.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult Control()
        {
            return View();
        }
        [HttpGet]
        public ActionResult Alarm()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Settings()
        {
            return View();
        }
        [HttpGet]
        public ActionResult Trend()
        {
            return View();
        }
        [HttpGet]
        public ActionResult Report()
        {
            return View();
        }

        public ActionResult Logout()
        {
            SessionHelper.ClearSession();
            return RedirectToAction("Index", "Login");
        }

        public JsonResult GetRealtimeLocations()
        {
            List<object> lists = new List<object>();
            foreach (var item in DataController.WebLogger.GetLocations())
            {
                lists.Add(new
                {
                    item.Id,
                    item.Value,
                    item.Status,
                    item.IsHighAlarm,
                    item.IsLowAlarm
                });
            }
            return Json(lists, JsonRequestBehavior.AllowGet);
        }

        #region Alarms

        public JsonResult GetAlarms()
        {
            Alarms alarms = DataController.WebLogger.GetAlarms(100);
            return Json(alarms, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AckAlarms()
        {
            try
            {
                string cmdString = $"UPDATE {Universal.AlarmTableName} SET Ack = 'Yes' where Type != 'Normal Alarm'";

                using (MySqlConnection connection = new MySqlConnection(Universal.GetConnectionString()))
                {
                    using (MySqlCommand command = new MySqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandType = CommandType.Text;
                        command.CommandText = cmdString;

                        connection.Open();
                        var result = command.ExecuteNonQuery();
                        return Json(new { Status = "Ok" }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch { return Json(new { Status = "Failed" }, JsonRequestBehavior.AllowGet); }
        }

        #endregion

        #region Control

        public JsonResult WriteValue(WriteValueModel valueModel)
        {
            try
            {
                string result = DataController.WebLogger.WriteValue(valueModel.DeviceId, valueModel.Address, valueModel.DataType, valueModel.Value);
                //string result = "Ok";
                WriteLog writeLog = new WriteLog();
                writeLog.Action = $"Write - DeviceId:{valueModel.DeviceId}/Address:{valueModel.Address}/Value:{valueModel.Value}";
                writeLog.DateTime = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                writeLog.Result = result;

                if (WriteLogs.Count > 100)
                {
                    WriteLogs.RemoveAt(WriteLogs.Count - 1);
                }
                WriteLogs.Insert(0, writeLog);
                return Json(new { Status = result }, JsonRequestBehavior.AllowGet);
            }
            catch { return Json(new { Status = "Failed" }, JsonRequestBehavior.AllowGet); }
        }

        public JsonResult ReadValue(WriteValueModel valueModel)
        {
            try
            {
                string result = DataController.WebLogger.ReadValue(valueModel.DeviceId, valueModel.Address, valueModel.DataType, out double value);
                string logValue = result == "Ok" ? value.ToString() : "";
                WriteLog writeLog = new WriteLog();
                writeLog.Action = $"Read - DeviceId:{valueModel.DeviceId}/Address:{valueModel.Address}/Value:{logValue}";
                writeLog.DateTime = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                writeLog.Result = result;

                if (WriteLogs.Count > 100)
                {
                    WriteLogs.RemoveAt(WriteLogs.Count - 1);
                }
                WriteLogs.Insert(0, writeLog);
                return Json(new { Status = result, Value = logValue }, JsonRequestBehavior.AllowGet);
            }
            catch { return Json(new { Status = "Failed" }, JsonRequestBehavior.AllowGet); }
        }

        public JsonResult WriteValueATMS(WriteValueATMSModel valueATMSModel)
        {
            try
            {
                string result = DataController.WebLogger.WriteValueATMS(valueATMSModel.DeviceId, valueATMSModel.Deadband, valueATMSModel.TempHigh, valueATMSModel.TempLow, valueATMSModel.Offset);
                //string result = "Ok";
                WriteLog writeLog = new WriteLog();
                writeLog.Action = $"Write AT-TMS- DeviceId:{valueATMSModel.DeviceId}/Deadband:{valueATMSModel.Deadband}/TempHigh:{valueATMSModel.TempHigh}/TempLow:{valueATMSModel.TempLow}/Offset:{valueATMSModel.Offset}";
                writeLog.DateTime = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                writeLog.Result = result;

                if (WriteLogs.Count > 100)
                {
                    WriteLogs.RemoveAt(WriteLogs.Count - 1);
                }
                WriteLogs.Insert(0, writeLog);
                return Json(new { Status = result }, JsonRequestBehavior.AllowGet);
            }
            catch { return Json(new { Status = "Failed" }, JsonRequestBehavior.AllowGet); }
        }

        public JsonResult ReadValueATMS(WriteValueATMSModel valueATMSModel)
        {
            try
            {
                string result = DataController.WebLogger.ReadValueATMS(valueATMSModel.DeviceId, out double deadband, out double temphigh, out double templow, out double offset);
                //string result = "Ok";
                WriteLog writeLog = new WriteLog();
                writeLog.DateTime = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                writeLog.Result = result;

                string tempHighStr = result == "Ok" ? temphigh.ToString() : "";
                string tempLowStr = result == "Ok" ? templow.ToString() : "";
                string deadBandStr = result == "Ok" ? deadband.ToString() : "";
                string offsetStr = result == "Ok" ? offset.ToString() : "";

                writeLog.Action = $"Read AT-TMS- DeviceId:{valueATMSModel.DeviceId}/Deadband:{deadBandStr}/TempHigh:{tempHighStr}/TempLow:{tempLowStr}/Offset:{offsetStr}";

                if (WriteLogs.Count > 100)
                {
                    WriteLogs.RemoveAt(WriteLogs.Count - 1);
                }
                WriteLogs.Insert(0, writeLog);
                return Json(new { Status = result, Deadband = deadBandStr, TempHigh = tempHighStr, TempLow = tempLowStr, Offset = offsetStr }, JsonRequestBehavior.AllowGet);
            }
            catch { return Json(new { Status = "Failed" }, JsonRequestBehavior.AllowGet); }
        }

        public JsonResult GetWriteLog()
        {
            try
            {
                return Json(WriteLogs, JsonRequestBehavior.AllowGet);
            }
            catch { return Json(new { Status = "Failed" }, JsonRequestBehavior.AllowGet); }
        }

        public static List<WriteLog> WriteLogs { get; set; } = new List<WriteLog>();

        #endregion

        #region Settings

        public JsonResult GetLocations()
        {
            var x = DataController.WebLogger.GetLocations();
            var locations = new Locations();
            locations.GetAll();
            return Json(locations, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetLocationWebLog()
        {
            return Json(DataController.WebLogger.GetLocations(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveModbusRTU(ModbusRTUModel modbusRTU)
        {
            try
            {
                DataController.WebLogger.BaudRate = modbusRTU.Baudrate;
                DataController.WebLogger.DataBits = modbusRTU.DataBits;
                DataController.WebLogger.Parity = modbusRTU.Parity;
                DataController.WebLogger.Stopbits = modbusRTU.Stopbits;
                DataController.WebLogger.Timeout = modbusRTU.Timeout;

                DataController.WebLogger.SaveSettings();
                DataController.WebLogger.Restart();
                return Json(new { Status = "Ok" }, JsonRequestBehavior.AllowGet);
            }
            catch { return Json(new { Status = "Failed" }, JsonRequestBehavior.AllowGet); }
        }

        public JsonResult GetModbusRTUParameters()
        {
            ModbusRTUModel modbusRTU = new ModbusRTUModel();
            modbusRTU.Port = DataController.WebLogger.PortName;
            modbusRTU.Baudrate = DataController.WebLogger.BaudRate;
            modbusRTU.DataBits = DataController.WebLogger.DataBits;
            modbusRTU.Parity = DataController.WebLogger.Parity;
            modbusRTU.Stopbits = DataController.WebLogger.Stopbits;
            modbusRTU.Timeout = DataController.WebLogger.Timeout;

            return Json(modbusRTU, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveServerParameters(ServerModel serverModel)
        {
            try
            {
                //DataController.WebLogger.ServerIpAddress = serverModel.ServerIp;
                DataController.WebLogger.LogRate = serverModel.LogTimeRate;
                DataController.WebLogger.LogType = serverModel.LogType;

                DataController.WebLogger.SaveSettings();
                //DataController.WebLogger.Restart();
                return Json(new { Status = "Ok" }, JsonRequestBehavior.AllowGet);
            }
            catch { return Json(new { Status = "Failed" }, JsonRequestBehavior.AllowGet); }
        }

        public JsonResult GetServerParameters()
        {
            ServerModel serverModel = new ServerModel();
            serverModel.WebLoggerId = DataController.WebLogger.MACID;
            serverModel.ServerIp = DataController.WebLogger.ServerIpAddress;
            serverModel.LogTimeRate = DataController.WebLogger.LogRate;
            serverModel.LogType = DataController.WebLogger.LogType;
            serverModel.ServerIpDisplay = DataController.WebLogger.ServerIpAddress.Split(';')[3];
            return Json(serverModel, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveAlarmParameters(AlarmModel model)
        {
            try
            {

                DataController.WebLogger.EnableSMSAlarm = model.EnableSMS;
                DataController.WebLogger.EnableEmailAlarm = model.EnableEmail;

                if (model.EnableEmail)
                    DataController.WebLogger.EmailString = model.Email;
                    DataController.WebLogger.EmailStringKDNT = model.EmailKDNT;

                if (model.EnableSMS)
                    DataController.WebLogger.SMSString = model.SMS;
                    DataController.WebLogger.SMSStringKDNT = model.SMSKDNT;

                DataController.WebLogger.SaveSettings();
                //DataController.WebLogger.Restart();
                return Json(new { Status = "Ok" }, JsonRequestBehavior.AllowGet);
            }
            catch { return Json(new { Status = "Failed" }, JsonRequestBehavior.AllowGet); }

        }

        public JsonResult GetAlarmParameters()
        {
            AlarmModel alarmModel = new AlarmModel();
            alarmModel.EnableEmail = DataController.WebLogger.EnableEmailAlarm;
            alarmModel.EnableSMS = DataController.WebLogger.EnableSMSAlarm;
            alarmModel.SMS = DataController.WebLogger.SMSString;
            alarmModel.Email = DataController.WebLogger.EmailString;
            alarmModel.SMSKDNT = DataController.WebLogger.SMSStringKDNT;
            alarmModel.EmailKDNT = DataController.WebLogger.EmailStringKDNT;
            return Json(alarmModel, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetLocationById(int id)
        {
            Core.Location location = new Core.Location(id);
            return Json(location, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteLocationById(int id)
        {
            try
            {
                Core.Location location = new Core.Location(id);
                if (location != null)
                    location.Delete();
                return Json(new { Status = "Ok" }, JsonRequestBehavior.AllowGet);
            }
            catch { return Json(new { Status = "Failed" }, JsonRequestBehavior.AllowGet); }
        }

        public JsonResult AddLocation(Location location)
        {
            try
            {
                if (location != null)
                {
                    location.Save();
                    return Json(new { Status = "Ok" }, JsonRequestBehavior.AllowGet);
                }
                throw new ArgumentNullException();
            }
            catch { return Json(new { Status = "Failed" }, JsonRequestBehavior.AllowGet); }
        }

        public JsonResult UpdateLocation(Location location)
        {
            try
            {
                if (location != null)
                {
                    var saveLocation = new Location(Convert.ToInt32(location.Value));
                    saveLocation.Name = location.Name;
                    saveLocation.DeviceId = location.DeviceId;
                    saveLocation.MemoryAddress = location.MemoryAddress;
                    saveLocation.DataType = location.DataType;
                    saveLocation.LowLevel = location.LowLevel;
                    saveLocation.HighLevel = location.HighLevel;
                    saveLocation.Gain = location.Gain;
                    saveLocation.Offset = location.Offset;
                    saveLocation.Deadband = location.Deadband;
                    saveLocation.State = location.State;
                    saveLocation.Save();
                    return Json(new { Status = "Ok" }, JsonRequestBehavior.AllowGet);
                }
                throw new ArgumentNullException();
            }
            catch { return Json(new { Status = "Failed" }, JsonRequestBehavior.AllowGet); }
        }

        #endregion

        #region Report

        public JsonResult GetLocationData(TimeRange timeRange)
        {
            DateTime from = Convert.ToDateTime(timeRange.From);
            DateTime to = Convert.ToDateTime(timeRange.To);
            timeRange.From = from.ToString("yyyy/MM/dd HH:mm") + ":00";
            timeRange.To = to.ToString("yyyy/MM/dd HH:mm") + ":59";

            int locationId = Convert.ToInt32(timeRange.Parameter.Split('|')[0]);
            string locationName = timeRange.Parameter.Split('|')[1];
            Datas logData = new Datas();

            logData.GetBySqlStatement($"select * from {Universal.DataTableName} where DateTime >= '{timeRange.From}' and DateTime <= '{timeRange.To}' and LocationId = {locationId} order by DateTime asc");
            logData.ForEach(x => x.LocationName = locationName);
            return Json(new { Data = logData }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAlarmData(TimeRange timeRange)
        {
            string locationName = timeRange.Parameter.Split('|')[0];
            string type = timeRange.Parameter.Split('|')[1];

            DateTime from = Convert.ToDateTime(timeRange.From);
            DateTime to = Convert.ToDateTime(timeRange.To);
            timeRange.From = from.ToString("yyyy/MM/dd HH:mm") + ":00";
            timeRange.To = to.ToString("yyyy/MM/dd HH:mm") + ":59";

            string query = "";
            if (type == "All")
                query = $"select * from {Universal.AlarmTableName} where LocationName = '{locationName}' and DateTime >= '{timeRange.From}' and DateTime <= '{timeRange.To}'";
            else
                query = $"select * from {Universal.AlarmTableName} where LocationName = '{locationName}' and DateTime >= '{timeRange.From}' and DateTime <= '{timeRange.To}' and Type = '{type}'";

            Alarms alarms = new Alarms();
            alarms.GetBySqlStatement(query);

            return Json(new { Data = alarms }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ExportLocationData(TimeRange timeRange)
        {
            try
            {
                int locationId = Convert.ToInt32(timeRange.Parameter.Split('|')[0]);
                string locationName = timeRange.Parameter.Split('|')[1];

                DateTime from = Convert.ToDateTime(timeRange.From);
                DateTime to = Convert.ToDateTime(timeRange.To);
                timeRange.From = from.ToString("yyyy/MM/dd HH:mm") + ":00";
                timeRange.To = to.ToString("yyyy/MM/dd HH:mm") + ":59";

                DataTable dt = new DataTable();
                using (MySqlConnection connection = new MySqlConnection(Universal.GetConnectionString()))
                {
                    using (MySqlCommand command = new MySqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandType = CommandType.Text;
                        command.CommandText = $"select DateTime, Value from {Universal.DataTableName} where LocationId = {locationId} and DateTime >= '{timeRange.From}' and DateTime <= '{timeRange.To}' order by DateTime asc";
                        connection.Open();
                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
                        {
                            adapter.Fill(dt);
                        }
                    }
                }
                ExportLocation(dt, locationName, timeRange.From, timeRange.To, "Template");

                return Json(new { Status = "Ok" }, JsonRequestBehavior.AllowGet);
            }
            catch { return Json(new { Status = "Failed" }, JsonRequestBehavior.AllowGet); }
        }

        public JsonResult ExportAlarmData(TimeRange timeRange)
        {
            try
            {
                DateTime from = Convert.ToDateTime(timeRange.From);
                DateTime to = Convert.ToDateTime(timeRange.To);
                timeRange.From = from.ToString("yyyy/MM/dd HH:mm") + ":00";
                timeRange.To = to.ToString("yyyy/MM/dd HH:mm") + ":59";

                string locationName = timeRange.Parameter.Split('|')[0];
                string type = timeRange.Parameter.Split('|')[1];

                DataTable dt = new DataTable();
                using (MySqlConnection connection = new MySqlConnection(Universal.GetConnectionString()))
                {
                    using (MySqlCommand command = new MySqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandType = CommandType.Text;
                        string query = "";
                        if (type == "All")
                            query = $"select DateTime, LocationName, Type, Value, LowLevel, HighLevel, Ack from {Universal.AlarmTableName} where LocationName = '{locationName}' and DateTime >= '{timeRange.From}' and DateTime <= '{timeRange.To}'";
                        else
                            query = $"select DateTime, LocationName, Type, Value, LowLevel, HighLevel, Ack from {Universal.AlarmTableName} where LocationName = '{locationName}' and Type = '{type}' and DateTime >= '{timeRange.From}' and DateTime <= '{timeRange.To}'";
                        command.CommandText = query;
                        connection.Open();
                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
                        {
                            adapter.Fill(dt);
                        }
                    }
                }

                dt.Columns[0].Caption = "DateTime";
                dt.Columns[1].Caption = "Location Name";
                dt.Columns[2].Caption = "Status";
                dt.Columns[3].Caption = "Value";
                dt.Columns[4].Caption = "Low Level";
                dt.Columns[5].Caption = "High Level";
                dt.Columns[6].Caption = "Ack";

                ExportAlarm(dt, type, timeRange.From, timeRange.To);
                return Json(new { Status = "Ok" }, JsonRequestBehavior.AllowGet);
            }
            catch { return Json(new { Status = "Failed" }, JsonRequestBehavior.AllowGet); }
        }

        public FileResult DowloadExportLocation()
        {
            MemoryStream memoryStream = Session["ExportLocation"] as MemoryStream;

            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment: filename=" + "ReportLocation.xlsx");
            memoryStream.WriteTo(Response.OutputStream);
            Response.Flush();
            Response.End();

            return File(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ReportLocation");
        }

        public FileResult DowloadExportAlarm()
        {
            MemoryStream memoryStream = Session["ExportAlarm"] as MemoryStream;

            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment: filename=" + "ReportAlarm.xlsx");
            memoryStream.WriteTo(Response.OutputStream);
            Response.Flush();
            Response.End();

            return File(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ReportAlarm");
        }

        private void ExportLocation(DataTable dt, string locationName, string from, string to, string template)
        {
            if (dt != null)
            {
                FileStream fs = new FileStream(DataController.WebLogger.PathFile + template + ".xlsx", FileMode.OpenOrCreate);
                
                var memoryStream = new MemoryStream();
                using (var package = new ExcelPackage())
                {
                    package.Load(fs);

                    //Lấy excel work sheet đầu tiên
                    ExcelWorksheet sheet = package.Workbook.Worksheets["Data"];
                    ExcelWorksheet sheet1 = package.Workbook.Worksheets["Chart"];

                    sheet1.Cells["A1"].Value = $"BỆNH VIỆN ĐHYD TP HCM";
                    sheet1.Cells["A2"].Value = $"KHOA DƯỢC - {locationName}";
                    sheet1.Cells["P2"].Value = $"Từ ngày {from} - Đến ngày {to}";
                    //// Tạo phần đầu nếu muốn
                    sheet.Cells["A1"].Value = locationName;

                    // Tạo tiêu đề cột 
                    sheet.Cells["A2"].Value = $"Từ ngày {from} - đến ngày {to}";

                    //Thay doi ten cot gia tri theo du lieu truy van la nhiet do hay nhiet am
                    if (locationName.Contains("DA"))
                    {
                        sheet.Cells["B3"].Value = "Độ Ẩm (%)";
                    }
                    else
                    {
                        sheet.Cells["B3"].Value = "Nhiệt Độ (oC)";
                    }

                    // Tạo mẳng đối tượng để lưu dữ toàn bồ dữ liệu trong DataTable,

                    // vì dữ liệu được được gán vào các Cell trong Excel phải thông qua object thuần.

                    object[,] arr = new object[dt.Rows.Count, dt.Columns.Count];

                    //Chuyển dữ liệu từ DataTable vào mảng đối tượng
                    for (int r = 0; r < dt.Rows.Count; r++)
                    {
                        DataRow dr = dt.Rows[r];

                        for (int c = 0; c < dt.Columns.Count; c++)
                        {
                            if (c == 0)
                            {
                                arr[r, c] = "'" + dr[c].ToString();
                            }
                            else
                            {
                                arr[r, c] = double.Parse(dr[c].ToString());
                            }
                        }
                    }

                    //Thiết lập vùng điền dữ liệu
                    int rowStart = 4;

                    int columnStart = 1;

                    int rowEnd = rowStart + dt.Rows.Count - 1;

                    int columnEnd = dt.Columns.Count;

                    // Ô bắt đầu điền dữ liệu
                    var c1 = sheet.Cells[rowStart, columnStart];

                    // Ô kết thúc điền dữ liệu
                    var c2 = sheet.Cells[rowEnd, columnEnd];

                    // Lấy về vùng điền dữ liệu
                    var range = sheet.Cells[rowStart, columnStart, rowEnd, columnEnd];

                    //Kẻ viền
                    range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    //Điền dữ liệu vào vùng đã thiết lập
                    range.Value = arr;

                    // Căn giữa cột STT
                    var c3 = sheet.Cells[rowEnd, columnStart];

                    var c4 = sheet.Cells[rowStart, columnStart, rowEnd, columnStart];

                    c4.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    //luu
                    //sheet.Cells["A:AZ"].AutoFitColumns();

                    #region tự tạo file excel để xuất
                    ////Tạo 1 Sheet mới với tên Sheet là NewSheet1
                    //ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("LocationData");

                    //worksheet.Cells["A1"].Value = "REPORT";
                    //worksheet.Cells["A1"].Style.Font.Size = 22;
                    //worksheet.Cells["A1"].Style.Font.Bold = true;
                    //worksheet.Cells["A1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    //worksheet.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    //worksheet.Cells["A1:B1"].Merge = true;

                    //worksheet.Cells["A2"].Value = $"Từ ngày {from} - đến ngày {to}";
                    //worksheet.Cells["A2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    //worksheet.Cells["A2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    //worksheet.Cells["A2:B2"].Merge = true;


                    //worksheet.Cells["A3"].Value = $"Location: {locationName}";
                    //worksheet.Cells["A1"].Style.Font.Size = 16;
                    //worksheet.Cells["A1"].Style.Font.Bold = true;
                    //worksheet.Cells["A3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    //worksheet.Cells["A3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    //worksheet.Cells["A3:B3"].Merge = true;

                    //var cells = worksheet.Cells["A4"].LoadFromDataTable(dt, true, TableStyles.Light10);
                    //int rowEnd = cells.End.Row;

                    //var dateTimeCells = worksheet.Cells[$"A4:A{rowEnd}"];
                    //dateTimeCells.Style.Numberformat.Format = "dd/mm/yyyy hh:mm:ss";

                    //worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                    //worksheet.Cells[worksheet.Dimension.Address].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    //worksheet.Cells["A:AZ"].AutoFitColumns();

                    //package.Save();
                    //Session["ExportLocation"] = memoryStream;
                    #endregion

                    package.SaveAs(memoryStream);
                    fs.Dispose();

                    Session["ExportLocation"] = memoryStream;
                }
            }
        }

        private void ExportAlarm(DataTable dt, string type, string from, string to)
        {
            if (dt != null)
            {
                var memoryStream = new MemoryStream();

                using (var package = new ExcelPackage(memoryStream))
                {
                    //Tạo 1 Sheet mới với tên Sheet là NewSheet1
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("LocationData");

                    worksheet.Cells["A1"].Value = "REPORT ALARM";
                    worksheet.Cells["A1"].Style.Font.Size = 22;
                    worksheet.Cells["A1"].Style.Font.Bold = true;
                    worksheet.Cells["A1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheet.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["A1:G1"].Merge = true;

                    worksheet.Cells["A2"].Value = $"Từ ngày {from} - đến ngày {to}";
                    worksheet.Cells["A2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheet.Cells["A2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["A2:G2"].Merge = true;


                    worksheet.Cells["A3"].Value = $"Report Alarm Type: {type}";
                    worksheet.Cells["A1"].Style.Font.Size = 16;
                    worksheet.Cells["A1"].Style.Font.Bold = true;
                    worksheet.Cells["A3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheet.Cells["A3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksheet.Cells["A3:G3"].Merge = true;

                    var cells = worksheet.Cells["A4"].LoadFromDataTable(dt, true, TableStyles.Light10);
                    int rowEnd = cells.End.Row;

                    var dateTimeCells = worksheet.Cells[$"A4:A{rowEnd}"];
                    dateTimeCells.Style.Numberformat.Format = "dd/mm/yyyy hh:mm:ss";

                    worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                    worksheet.Cells[worksheet.Dimension.Address].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["A:AZ"].AutoFitColumns();

                    package.Save();

                    Session["ExportAlarm"] = memoryStream;
                }
            }
        }

        #endregion

        #region Trend

        public JsonResult GetRealtimeTrendConfig()
        {
            return Json(new { Interval = UpdateInterval, MaxElements = MaxRealtimeElements }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetRealtimeData(int id)
        {
            var location = DataController.WebLogger.Locations.FirstOrDefault(x => x.Id == id);
            string value = "";
            if (location != null)
            {
                value = location.Value;
            }

            return Json(new { Id = id, Value = value }, JsonRequestBehavior.AllowGet);
        }

        public static int MaxRealtimeElements = 60;
        public static int UpdateInterval = 1000;

        public JsonResult GetHistoricalData(TimeRange timeRange)
        {
            Datas result = new Datas();
            //if (int.TryParse(timeRange.Parameter, out int id))
            //{
            //    result.GetBySqlStatement("select * from ")
            //}
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDataLog(TimeRange timeRange)
        {
            List<DataModel> result = new List<DataModel>();

            DateTime from = Convert.ToDateTime(timeRange.From);
            DateTime to = Convert.ToDateTime(timeRange.To);
            timeRange.From = from.ToString("yyyy/MM/dd HH:mm") + ":00";
            timeRange.To = to.ToString("yyyy/MM/dd HH:mm") + ":59";
            int locationId = Convert.ToInt32(timeRange.Parameter.Split('|')[0]);
            string locationName = timeRange.Parameter.Split('|')[1];
            var dt = new DataTable();
            using (MySqlConnection connection = new MySqlConnection(CoreData.Universal.GetConnectionString()))
            {
                connection.Open();
                using (MySqlCommand command = new MySqlCommand())
                {
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = $"select DateTime, Value from {Universal.DataTableName} where DateTime >= '{timeRange.From}' and DateTime <= '{timeRange.To}' and LocationId = {locationId} order by DateTime asc";
                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
                    {
                        adapter.Fill(dt);
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            foreach (DataRow dtRow in dt.Rows)
                            {
                                var data = new DataModel()
                                {
                                    t = Convert.ToDateTime(dtRow[0]).ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz"),
                                    y = dtRow[1].ToString()
                                };
                                result.Add(data);
                            }
                        }
                    }
                }
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion

        public JsonResult RestartDevices()
        {
            try
            {
                DataController.WebLogger.Restart();
                return Json(new { Status = "Ok" }, JsonRequestBehavior.AllowGet);
            }
            catch { return Json(new { Status = "Failed" }, JsonRequestBehavior.AllowGet); }
        }

        public JsonResult UpdatePassword(ChangePasswordModel model)
        {
            bool result = false;

            if (!string.IsNullOrWhiteSpace(model.NewPassword))
            {
                result = DataController.WebLogger.SaveSettingsPass(model.NewPassword, model.Password);
            }

            return Json(new { Status = result }, JsonRequestBehavior.AllowGet);
        }
    }

    public class ChangePasswordModel
    {
        public string Password { get; set; }
        public string NewPassword { get; set; }
    }

    public class DataModel
    {
        public string t { get; set; }
        public string y { get; set; }
    }

    public class TimeRange
    {
        public string From { get; set; }
        public string To { get; set; }
        public string Parameter { get; set; }
    }
}