using MySql.Data.MySqlClient;
using PLCPiProject;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;

namespace ATWebLogger.Core
{
    public class WebLogger
    {
        #region Public members

        public string MACID { get; protected set; }
        public string Password { get; set; }
        public PLCPi GateWay { get; set; }

        #region ModbusRTU parameters

        public string PortName { get; set; }
        public int BaudRate { get; set; }
        public int DataBits { get; set; }
        public int Parity { get; set; }
        public int Stopbits { get; set; }
        public int Timeout { get; set; }

        #endregion

        #region DataLog parameters

        public int LogRate { get; set; } = 60;
        public string LogType { get; set; } = "Databases";

        #endregion

        #region Database parameters

        public string ServerIpAddress { get; set; } = "user id=root;password=100100;database=gateway;server=localhost;convertzerodatetime=True;port=3306";
        public const string DatabaseNane = "gateway";
        public string AlarmTableName { get { return MACID + "_alarm"; } }
        public string DataTableName { get { return MACID + "_data"; } }
        public string LocationTableName { get { return MACID + "_location"; } }
        public string ConnectionString
        {
            //get { return $"user id=customer_ttp;password=ThinhTamPhat!@#456&*(;database=gateway;server={ServerIpAddress};convertzerodatetime=True;port=3306"; }
            get { return ServerIpAddress; }
            //get { return $"{ServerIpAddress}"; }
        }

        #endregion

        #region Alarm parameters

        public bool EnableEmailAlarm { get; set; }
        public string EmailString { get; set; }
        public bool EnableSMSAlarm { get; set; }
        public string SMSString { get; set; }

        //Cong Update 09/01/2021
        public string SMSStringKDNT { get; set; }
        public string EmailStringKDNT { get; set; }

        #endregion

        public Locations Locations { get; set; }
        public Alarms Alarms { get; set; }

        //public string PathFile = $"D:\\ATPro\\CodeProject\\GatewayPi\\WeblogMVC\\SourceCode\\";
        //public string PathFile = @"C:\GatewayParametters\";
        public string PathFile = $"/home/pi/";

        public double VotLo = 5;

        #endregion

        #region Private members

        private readonly Thread readThread;
        private readonly System.Timers.Timer logTimer;
        private readonly object readLocker = new object();
        private readonly int countAlarm = 0;

        private double readValueNew = 0;//chứa giá trị mới đọc về từ modbus

        #endregion

        #region Constructors

        public WebLogger()
        {
            #region Read MACID & parameters of ModbusRTU comunication

            Console.WriteLine("Bắt đầu đọc thông số modbus");
            string dataParams = ReadText(PathFile + "Parametter.txt").Trim();
            //string dataParams = "MacAdd:" + "test" + "|Port:" + "COM2" + "|Baudrate:" + "9600"
            //    + "|DataBit:" + "8" + "|Parity:" + "0" + "|Stopbit:" + "1"
            //    + "|TimeOut:" + "1000";
            string[] prams = dataParams.Split('|');

            MACID = prams[0].Split(':')[1].Trim();
            PortName = prams[1].Split(':')[1].Trim();

            BaudRate = int.Parse(prams[2].Split(':')[1].Trim());
            DataBits = int.Parse(prams[3].Split(':')[1].Trim());
            Parity = int.Parse(prams[4].Split(':')[1].Trim());
            Stopbits = int.Parse(prams[5].Split(':')[1].Trim());
            Timeout = int.Parse(prams[6].Split(':')[1].Trim());

            Console.WriteLine($"MacId:{MACID}/Port:{PortName}/Baudrate:{BaudRate}/Databits:{DataBits}/Parity:{Parity}/StopBit:{Stopbits}/Timeout:{Timeout}");
            #endregion

            #region Read Password, Email, SMS settings

            Password = ReadText(PathFile + "password.txt").Trim();
            SMSString = ReadText(PathFile + "sms.txt").Trim().Split('|')[0];
            SMSStringKDNT = ReadText(PathFile + "sms.txt").Trim().Split('|')[1];
            EmailString = ReadText(PathFile + "email.txt").Trim().Split('|')[0];
            EmailStringKDNT = ReadText(PathFile + "email.txt").Trim().Split('|')[1];
            //Password = "admin";
            //SMSString = "0767397108,‭0795864635,0909167655";
            //SMSString = "0909167655,‭0909167655,0909167655";
            //EmailString = "ndcong08cddv02@gmail.com";
            Debug.WriteLine($"sms {SMSString} | Email {EmailString}");
            Console.WriteLine($"SMS: {SMSString}");
            Console.WriteLine($"Email: {EmailString}");

            string temp = ReadText(PathFile + "enableSMSEmail.txt");
            prams = temp.Split(',');
            //string temp = "True" + "," + "True" + "," + "Databases" + "," + "60" + "," + ServerIpAddress;
            //prams = temp.Split(',');

            Console.WriteLine($"EnableSMS/EnableEmail/LogType/LogRate/ServerIp: {temp}");

            EnableSMSAlarm = Convert.ToBoolean(prams[0]);
            EnableEmailAlarm = Convert.ToBoolean(prams[1]);
            LogType = prams[2].Trim();
            LogRate = Convert.ToInt32(prams[3].Trim());
            ServerIpAddress = prams[4].Trim();

            #endregion

            #region Read countAlarm setting
            Console.WriteLine("Bắt đầu đọc thông số countAlarm setting");

            countAlarm = Convert.ToInt16(ReadText(PathFile + "countAlarm.txt").Trim().Split('|')[0]);
            VotLo = Convert.ToDouble(ReadText(PathFile + "countAlarm.txt").Trim().Split('|')[1]);

            Console.WriteLine($"Count Alarm Settinf {countAlarm}|{VotLo}");
            //string dataParams = "MacAdd:" + "test" + "|Port:" + "COM2" + "|Baudrate:" + "9600"
            //    + "|DataBit:" + "8" + "|Parity:" + "0" + "|Stopbit:" + "1"
            //    + "|TimeOut:" + "1000";
            #endregion

            //Set connection string to use it globaly
            CoreData.Universal.ConnectionString = ConnectionString;
            CoreData.Universal.AlarmTableName = AlarmTableName;
            CoreData.Universal.DataTableName = DataTableName;
            CoreData.Universal.LocationTableName = LocationTableName;

            Console.WriteLine("Khởi tạo PLC Pi");
            try
            {
                GateWay = new PLCPi();
            }
            catch (Exception ex) { }
            Console.WriteLine("Khởi tạo PLC Pi thành công");

            #region Đồng bộ giờ

            try
            {
                MySqlConnection connection = new MySqlConnection(ConnectionString);
                connection.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT NOW()", connection);
                string TimeNow = cmd.ExecuteScalar().ToString();
                cmd.Dispose();
                connection.Close();
                connection.Dispose();
                if (TimeNow != "" && TimeNow != null)
                {
                    Console.WriteLine("Đồng bộ thời gian thành công" + Convert.ToDateTime(TimeNow).ToString("dd-MM-yyyy HH:mm:ss"));
                    GateWay.ThoiGian.CaiDat(Convert.ToDateTime(TimeNow).ToString("dd-MM-yyyy HH:mm:ss"));

                }
            }
            catch (Exception ex) { Console.WriteLine("Lỗi đồng bộ thời gian"); Console.WriteLine(ex.Message); }

            #endregion

            #region Create tables

            try
            {
                CreateTableIfNotExists();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi tạo bảng");
                Console.WriteLine(ex.Message);
            }

            #endregion

            //Khởi tạo 3G
            Console.WriteLine("Lấy danh sách location");
            Locations = new Locations();
            Locations.GetAll();
            Console.WriteLine($"Lấy danh sách location thành công: {Locations.Count}");

            Console.WriteLine("Khởi tạo USB3G");
            GateWay.SMS.Port_USB3G = ReadText(PathFile + "comSMS.txt");
            //GateWay.SMS.Port_USB3G = "COM12";
            //GateWay.SMS.Khoitao();
            Console.WriteLine($"Com SMS {GateWay.SMS.Port_USB3G}| Khoi Tao {GateWay.SMS.Khoitao()}");
            Console.WriteLine("Khởi tạo USB3G thành công");

            //GateWay.SMS.GuiSMS(SMSString, "Gui SMS test khi khoi tao");

            Console.WriteLine("Khởi tạo Modbus RTU Master");
            GateWay.ModbusRTUMaster.ResponseTimeout = 1000;
            //Khởi tạo modbus
            if (GateWay.ModbusRTUMaster.KetNoi(
                PortName,
                BaudRate,
                DataBits,
                (System.IO.Ports.Parity)(Parity),
                (System.IO.Ports.StopBits)(Stopbits)))
            {
                Console.WriteLine("Khởi tạo Modbus RTU Master thành công");
            }
            else
            {
                Console.WriteLine("Khởi tạo Modbus RTU Master thất bại");
            }

            //Mệt quá ngủ 1s dậy làm tiếp
            Thread.Sleep(1000);

            readThread = new Thread(Refresh);
            readThread.Start();

            //Khởi tạo timer dùng để kiểm tra alarm
            logTimer = new System.Timers.Timer(LogRate * 1000);
            logTimer.Elapsed += LogTimer_Elapsed;
            logTimer.Start();

            Console.WriteLine("Khởi tạo AT-Web/Logger hoàn tất");
        }

        #endregion

        #region Private methods

        private void Refresh()
        {
            Parallel.Invoke(
                () =>
                {
                    ReadValueLocations();
                },
                () =>
                {
                    ReadAlarmLocations();
                }
            );
        }

        private void LogTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            logTimer.Enabled = false;
            try
            {
                DateTime currentTime = DateTime.Now;
                foreach (var location in Locations)
                {
                    if (location.State == "Enable" && location.Status == "Good")
                    {
                        if (double.TryParse(location.Value, out double value))
                        {
                            switch (LogType.ToLower())
                            {
                                case "databases":
                                    {
                                        try
                                        {
                                            Data datalog = new Data
                                            {
                                                DateTime = currentTime,
                                                Value = value,
                                                LocationId = location.Id
                                            };
                                            datalog.Save();
                                        }
                                        catch { }
                                        break;
                                    }
                                case "usb":
                                    {
                                        break;
                                    }
                                case "all":
                                    {
                                        try
                                        {
                                            Data datalog = new Data
                                            {
                                                DateTime = currentTime,
                                                Value = value,
                                                LocationId = location.Id
                                            };
                                            datalog.Save();
                                        }
                                        catch { }
                                        break;
                                    }
                                default:
                                    break;
                            }

                        }
                    }
                }
            }
            catch { }
            finally { logTimer.Enabled = true; }
        }

        private void ReadValueLocations()
        {
            ReadValueFistTime();

            while (true)
            {
                foreach (var location in Locations)
                {
                    try
                    {
                        if (location.State == "Enable")
                        {
                            bool success = false;
                            int count = 0;
                            ushort address = (ushort)location.MemoryAddress;
                            byte deviceId = (byte)location.DeviceId;

                            #region Coil 1 - 9999
                            if (address >= 1 && address <= 9999)
                            {
                                bool[] data = { true };
                                lock (readLocker)
                                    success = GateWay.ModbusRTUMaster.ReadCoils(deviceId, address, 1, ref data);

                                if (!success)
                                {
                                    while (!success && count <= 4)
                                    {
                                        lock (readLocker)
                                            success = GateWay.ModbusRTUMaster.ReadCoils(deviceId, address, 1, ref data);
                                        if (count <= 2)
                                            Thread.Sleep(100);
                                        else Thread.Sleep(300);
                                        count++;
                                    }
                                }
                                count = 0;
                                if (success)
                                {
                                    if (data[0])
                                        location.Value = "1";
                                    else
                                        location.Value = "0";
                                }
                                else
                                {
                                    location.Value = "Null";
                                }
                            }
                            #endregion

                            #region Input 10001 - 19999
                            else if (address >= 10001 && address <= 19999)
                            {
                                bool[] data = { true };
                                address = (ushort)(address - 10001);
                                lock (readLocker)
                                    success = GateWay.ModbusRTUMaster.ReadDiscreteInputContact(deviceId, address, 1, ref data);

                                if (!success)
                                {
                                    while (!success && count <= 4)
                                    {
                                        lock (readLocker)
                                            success = GateWay.ModbusRTUMaster.ReadDiscreteInputContact(deviceId, address, 1, ref data);
                                        if (count <= 2)
                                            Thread.Sleep(100);
                                        else Thread.Sleep(300);
                                        count++;
                                    }
                                }
                                count = 0;
                                if (success)
                                {
                                    if (data[0])
                                        location.Value = "1";
                                    else
                                        location.Value = "0";
                                }
                                else
                                {
                                    location.Value = "Null";
                                }
                            }
                            #endregion

                            #region Input Register 30001 - 39999
                            else if (address >= 30001 && address <= 39999)
                            {
                                int size = GetBufferSize(location.DataType);
                                if (size >= 2)
                                {
                                    byte[] data = new byte[size];
                                    address = (ushort)(address - 30001);
                                    lock (readLocker)
                                        success = GateWay.ModbusRTUMaster.ReadInputRegisters(deviceId, address, (ushort)(size / 2), ref data);

                                    if (!success)
                                    {
                                        while (!success && count <= 4)
                                        {
                                            lock (readLocker)
                                                success = GateWay.ModbusRTUMaster.ReadInputRegisters(deviceId, address, (ushort)(size / 2), ref data);
                                            if (count <= 2)
                                                Thread.Sleep(100);
                                            else Thread.Sleep(300);
                                            count++;
                                        }
                                    }
                                    count = 0;
                                    if (success)
                                    {
                                        switch (location.DataType)
                                        {
                                            case "Word":
                                                readValueNew = Math.Round((GateWay.GetUshortAt(data, 0) * location.Gain) + location.Offset, 2);

                                                if (readValueNew >= location.ValueOld - location.OverValue && readValueNew <= location.ValueOld + location.OverValue)
                                                {
                                                    location.ValueOld = readValueNew;
                                                    location.Value = readValueNew.ToString();
                                                }
                                                else
                                                {
                                                    location.Value = location.ValueOld.ToString();
                                                }

                                                //location.Value = (GateWay.GetUshortAt(data, 0) * location.Gain).ToString();
                                                break;
                                            case "Short":
                                                readValueNew = Math.Round((GateWay.GetShortAt(data, 0) * location.Gain) + location.Offset, 2);

                                                if (readValueNew >= location.ValueOld - location.OverValue && readValueNew <= location.ValueOld + location.OverValue)
                                                {
                                                    location.ValueOld = readValueNew;
                                                    location.Value = readValueNew.ToString();
                                                }
                                                else
                                                {
                                                    location.Value = location.ValueOld.ToString();
                                                }

                                                //location.Value = (GateWay.GetShortAt(data, 0) * location.Gain).ToString();
                                                break;
                                            case "DWord":
                                                readValueNew = Math.Round((GateWay.GetUintAt(data, 0) * location.Gain) + location.Offset, 2);

                                                if (readValueNew >= location.ValueOld - location.OverValue && readValueNew <= location.ValueOld + location.OverValue)
                                                {
                                                    location.ValueOld = readValueNew;
                                                    location.Value = readValueNew.ToString();
                                                }
                                                else
                                                {
                                                    location.Value = location.ValueOld.ToString();
                                                }

                                                //location.Value = (GateWay.GetUintAt(data, 0) * location.Gain).ToString();
                                                break;
                                            case "Int":
                                                readValueNew = Math.Round((GateWay.GetIntAt(data, 0) * location.Gain) + location.Offset, 2);

                                                if (readValueNew >= location.ValueOld - location.OverValue && readValueNew <= location.ValueOld + location.OverValue)
                                                {
                                                    location.ValueOld = readValueNew;
                                                    location.Value = readValueNew.ToString();
                                                }
                                                else
                                                {
                                                    location.Value = location.ValueOld.ToString();
                                                }

                                                //location.Value = (GateWay.GetUshortAt(data, 0) * location.Gain).ToString();
                                                break;
                                            case "Float":
                                                readValueNew = Math.Round((GateWay.GetFloatAt(data, 0) * location.Gain) + location.Offset, 2);

                                                if (readValueNew >= location.ValueOld - location.OverValue && readValueNew <= location.ValueOld + location.OverValue)
                                                {
                                                    location.ValueOld = readValueNew;
                                                    location.Value = readValueNew.ToString();
                                                }
                                                else
                                                {
                                                    location.Value = location.ValueOld.ToString();
                                                }

                                                //location.Value = (GateWay.GetUshortAt(data, 0) * location.Gain).ToString();
                                                break;
                                            default:
                                                break;
                                        }
                                    }
                                    else
                                    {
                                        location.Value = "Null";
                                    }
                                }
                            }
                            #endregion

                            #region Holding Register 40001 - 49999
                            else if (address >= 40001 && address <= 49999)
                            {
                                int size = GetBufferSize(location.DataType);
                                if (size >= 2)
                                {
                                    byte[] data = new byte[size];
                                    address = (ushort)(address - 40001);
                                    lock (readLocker)
                                        success = GateWay.ModbusRTUMaster.ReadHoldingRegisters(deviceId, address, (ushort)(size / 2), ref data);

                                    if (!success)
                                    {
                                        while (!success && count <= 4)
                                        {
                                            lock (readLocker)
                                                success = GateWay.ModbusRTUMaster.ReadHoldingRegisters(deviceId, address, (ushort)(size / 2), ref data);
                                            if (count <= 2)
                                                Thread.Sleep(100);
                                            else Thread.Sleep(300);
                                            count++;
                                        }
                                    }
                                    count = 0;
                                    if (success)
                                    {
                                        switch (location.DataType)
                                        {
                                            case "Word":
                                                readValueNew = Math.Round((GateWay.GetUshortAt(data, 0) * location.Gain) + location.Offset, 2);

                                                if (readValueNew >= location.ValueOld - location.OverValue && readValueNew <= location.ValueOld + location.OverValue)//xử lý chống vọt lố. dữ liệu nằm trong 1 khoảng
                                                {
                                                    location.ValueOld = readValueNew;
                                                    location.Value = readValueNew.ToString();
                                                }
                                                else
                                                {
                                                    location.Value = location.ValueOld.ToString();
                                                }

                                                //location.Value = (GateWay.GetUshortAt(data, 0) * location.Gain).ToString();
                                                break;
                                            case "Short":
                                                readValueNew = Math.Round((GateWay.GetShortAt(data, 0) * location.Gain) + location.Offset, 2);

                                                if (readValueNew >= location.ValueOld - location.OverValue && readValueNew <= location.ValueOld + location.OverValue)
                                                {
                                                    location.ValueOld = readValueNew;
                                                    location.Value = readValueNew.ToString();
                                                }
                                                else
                                                {
                                                    location.Value = location.ValueOld.ToString();
                                                }

                                                //location.Value = (GateWay.GetShortAt(data, 0) * location.Gain).ToString();
                                                break;
                                            case "DWord":
                                                readValueNew = Math.Round((GateWay.GetUintAt(data, 0) * location.Gain) + location.Offset, 2);

                                                if (readValueNew >= location.ValueOld - location.OverValue && readValueNew <= location.ValueOld + location.OverValue)
                                                {
                                                    location.ValueOld = readValueNew;
                                                    location.Value = readValueNew.ToString();
                                                }
                                                else
                                                {
                                                    location.Value = location.ValueOld.ToString();
                                                }

                                                //location.Value = (GateWay.GetUintAt(data, 0) * location.Gain).ToString();
                                                break;
                                            case "Int":
                                                readValueNew = Math.Round((GateWay.GetIntAt(data, 0) * location.Gain) + location.Offset, 2);

                                                if (readValueNew >= location.ValueOld - location.OverValue && readValueNew <= location.ValueOld + location.OverValue)
                                                {
                                                    location.ValueOld = readValueNew;
                                                    location.Value = readValueNew.ToString();
                                                }
                                                else
                                                {
                                                    location.Value = location.ValueOld.ToString();
                                                }

                                                //location.Value = (GateWay.GetUshortAt(data, 0) * location.Gain).ToString();
                                                break;
                                            case "Float":
                                                readValueNew = Math.Round((GateWay.GetFloatAt(data, 0) * location.Gain) + location.Offset, 2);

                                                if (readValueNew >= location.ValueOld - location.OverValue && readValueNew <= location.ValueOld + location.OverValue)
                                                {
                                                    location.ValueOld = readValueNew;
                                                    location.Value = readValueNew.ToString();
                                                }
                                                else
                                                {
                                                    location.Value = location.ValueOld.ToString();
                                                }

                                                //location.Value = (GateWay.GetUshortAt(data, 0) * location.Gain).ToString();
                                                break;
                                            default:
                                                break;
                                        }

                                    }
                                    else
                                    {
                                        location.Value = "Null";
                                    }
                                }
                            }
                            #endregion

                            if (success)
                                location.Status = "Good";
                            else
                                location.Status = "Bad";
                        }
                        else
                        {
                            location.Value = "Disable";
                            location.Status = "Disable";
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Lỗi đọc modbus: {ex.Message}");
                    }
                    Thread.Sleep(500);
                }
            }
        }

        public int UpdateSMSTable(string updateString, int id)
        {
            int res = 0;
            string query;
            MySqlConnection connection = new MySqlConnection(ConnectionString);
            connection.Open();

            query = $"update test_sms set {updateString} where Id={id}";
            try
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                res = cmd.ExecuteNonQuery();
                cmd.Dispose();
                connection.Close();
                connection.Dispose();
            }
            catch (System.Exception)
            {
                connection.Close();
                connection.Dispose();
            }
            return res;
        }

        private void ReadAlarmLocations()
        {
            while (true)
            {
                foreach (var location in Locations)
                {
                    try
                    {
                        if (location.State == "Enable" && !string.IsNullOrEmpty(location.Value) && location.Value != "null" && location.Status == "Good")
                        {

                            DateTime CurrentTime = DateTime.Now;
                            double deadband = location.Deadband ?? 0;
                            if (double.TryParse(location.Value, out double value))
                            {
                                #region High Alarm
                                if (location.HighLevel.HasValue && value >= (location.HighLevel.Value + deadband) && !location.IsHighAlarm)
                                {
                                    Debug.WriteLine($"Hight Coutn Alarm {location.CountHighAlarm}");
                                    if (location.CountHighAlarm >= countAlarm)
                                    {
                                        location.CountHighAlarm = 100;
                                        location.CountNormalAlarm = 0;
                                        location.CountLowAlarm = 0;

                                        location.IsHighAlarm = true;
                                        location.IsNormalAlarm = false;
                                        location.IsLowAlarm = false;

                                        Alarm alarm = new Alarm();
                                        alarm.DateTime = CurrentTime;
                                        alarm.LocationName = location.Name;
                                        alarm.Type = "High Alarm";
                                        alarm.Value = value;
                                        alarm.LowLevel = location.LowLevel;
                                        alarm.HighLevel = location.HighLevel;
                                        alarm.Ack = "No";
                                        alarm.Save();

                                        if (EnableEmailAlarm)
                                        {
                                            SendAlarmEmail("High Alarm", location);
                                        }

                                        if (EnableSMSAlarm)
                                        {
                                            //UpdateSMSTable($"LocationName='KC Tu 1',Type='High Alarm',Value={value},LowLevel={location.LowLevel},HighLevel={location.HighLevel},Flag=100", 1);
                                            SendAlarmSMS("High Alarm", location);
                                        }

                                        Debug.WriteLine("Hight Alarm");
                                    }
                                    else
                                    {
                                        location.CountHighAlarm++;
                                    }
                                }
                                #endregion
                                #region Low Alarm
                                else if (location.LowLevel.HasValue && value <= (location.LowLevel.Value - deadband) && !location.IsLowAlarm)
                                {
                                    //Debug.WriteLine($"Low Coutn Alarm {location.CountLowAlarm}");
                                    if (location.CountLowAlarm >= countAlarm)
                                    {
                                        location.CountLowAlarm = 100;
                                        location.CountNormalAlarm = 0;
                                        location.CountHighAlarm = 0;

                                        location.IsHighAlarm = false;
                                        location.IsNormalAlarm = false;
                                        location.IsLowAlarm = true;

                                        Alarm alarm = new Alarm();
                                        alarm.DateTime = CurrentTime;
                                        alarm.LocationName = location.Name;
                                        alarm.Type = "Low Alarm";
                                        alarm.Value = value;
                                        alarm.LowLevel = location.LowLevel;
                                        alarm.HighLevel = location.HighLevel;
                                        alarm.Ack = "No";
                                        alarm.Save();

                                        if (EnableEmailAlarm)
                                        {
                                            SendAlarmEmail("Low Alarm", location);
                                        }

                                        if (EnableSMSAlarm)
                                        {
                                            SendAlarmSMS("Low Alarm", location);
                                        }
                                        Debug.WriteLine("Low Alarm");
                                    }
                                    else
                                    {
                                        location.CountLowAlarm++;
                                    }
                                }
                                #endregion
                                #region Normal Alarm
                                else if (value > (location.LowLevel.Value + deadband) && value < (location.HighLevel.Value - deadband) && !location.IsNormalAlarm && (location.IsHighAlarm || location.IsLowAlarm))
                                {
                                    //Debug.WriteLine($"Normal Coutn Alarm {location.CountNormalAlarm}");
                                    if (location.CountNormalAlarm >= 20)
                                    {
                                        location.CountNormalAlarm = 100;
                                        location.CountLowAlarm = 0;
                                        location.CountHighAlarm = 0;

                                        location.IsHighAlarm = false;
                                        location.IsNormalAlarm = true;
                                        location.IsLowAlarm = false;

                                        Alarm alarm = new Alarm();
                                        alarm.DateTime = CurrentTime;
                                        alarm.LocationName = location.Name;
                                        alarm.Type = "Normal Alarm";
                                        alarm.Value = value;
                                        alarm.LowLevel = location.LowLevel;
                                        alarm.HighLevel = location.HighLevel;
                                        alarm.Ack = "";
                                        alarm.Save();

                                        if (EnableEmailAlarm)
                                        {
                                            SendAlarmEmail("Normal Alarm", location);
                                        }

                                        if (EnableSMSAlarm)
                                        {
                                            SendAlarmSMS("Normal Alarm", location);
                                        }

                                        Debug.WriteLine("Normal");
                                    }
                                    else
                                    {
                                        location.CountNormalAlarm++;
                                    }
                                }
                                #endregion
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Lỗi đọc alarm: {ex.Message}");
                    }
                }
                Thread.Sleep(100);
            }
        }

        #region alarm nguyen mau error
        //private void ReadAlarmLocations()
        //{
        //    while (true)
        //    {
        //        foreach (var location in Locations)
        //        {
        //            try
        //            {
        //                if (location.State == "Enable" && !string.IsNullOrEmpty(location.Value) && location.Value != "null" && location.Status == "Good")
        //                {
        //                    DateTime CurrentTime = DateTime.Now;
        //                    double deadband = location.Deadband ?? 0;

        //                    #region High Alarm
        //                    if (location.HighLevel.HasValue && !location.IsHighAlarm)
        //                    {
        //                        if (double.TryParse(location.Value, out double value))
        //                        {
        //                            if (value >= (location.HighLevel.Value + deadband))
        //                            {
        //                                if (location.CountHighAlarm >= 1)
        //                                {
        //                                    location.CountHighAlarm = 100;
        //                                    location.CountNormalAlarm = 0;
        //                                    location.CountLowAlarm = 0;

        //                                    location.IsHighAlarm = true;
        //                                    location.IsNormalAlarm = false;
        //                                    location.IsLowAlarm = false;

        //                                    Alarm alarm = new Alarm();
        //                                    alarm.DateTime = CurrentTime;
        //                                    alarm.LocationName = location.Name;
        //                                    alarm.Type = "High Alarm";
        //                                    alarm.Value = value;
        //                                    alarm.LowLevel = location.LowLevel;
        //                                    alarm.HighLevel = location.HighLevel;
        //                                    alarm.Ack = "No";
        //                                    alarm.Save();

        //                                    //if (EnableEmailAlarm)
        //                                    //{
        //                                    //    SendAlarmEmail("High Alarm", location);
        //                                    //}

        //                                    //if (EnableSMSAlarm)
        //                                    //{
        //                                    //    SendAlarmSMS("High Alarm", location);
        //                                    //}
        //                                    string strAlarm = "Location : " + location.Name
        //             + "\nAlarm: Hight Alarm"
        //             + "\nValue: " + location.Value
        //             + "\nLow Level: " + (location.LowLevel.HasValue ? location.LowLevel.Value.ToString() : "")
        //             + "\nHigh Level: " + (location.HighLevel.HasValue ? location.HighLevel.Value.ToString() : "");
        //                                    Debug.WriteLine(strAlarm);
        //                                }
        //                                else
        //                                {
        //                                    location.CountHighAlarm++;
        //                                }
        //                            }
        //                        }
        //                    }
        //                    #endregion
        //                    #region Low Alarm
        //                    else if (location.LowLevel.HasValue && !location.IsLowAlarm)
        //                    {
        //                        if (double.TryParse(location.Value, out double value))
        //                        {
        //                            if (value <= (location.LowLevel.Value - deadband))
        //                            {
        //                                if (location.CountLowAlarm >= 1)
        //                                {
        //                                    location.CountLowAlarm = 100;
        //                                    location.CountNormalAlarm = 0;
        //                                    location.CountHighAlarm = 0;

        //                                    location.IsHighAlarm = false;
        //                                    location.IsNormalAlarm = false;
        //                                    location.IsLowAlarm = true;

        //                                    Alarm alarm = new Alarm();
        //                                    alarm.DateTime = CurrentTime;
        //                                    alarm.LocationName = location.Name;
        //                                    alarm.Type = "Low Alarm";
        //                                    alarm.Value = value;
        //                                    alarm.LowLevel = location.LowLevel;
        //                                    alarm.HighLevel = location.HighLevel;
        //                                    alarm.Ack = "No";
        //                                    alarm.Save();

        //                                    //if (EnableEmailAlarm)
        //                                    //{
        //                                    //    SendAlarmEmail("Low Alarm", location);
        //                                    //}

        //                                    //if (EnableSMSAlarm)
        //                                    //{
        //                                    //    SendAlarmSMS("Low Alarm", location);
        //                                    //}
        //                                    string strAlarm = "Location : " + location.Name
        //             + "\nAlarm: Low Alarm"
        //             + "\nValue: " + location.Value
        //             + "\nLow Level: " + (location.LowLevel.HasValue ? location.LowLevel.Value.ToString() : "")
        //             + "\nHigh Level: " + (location.HighLevel.HasValue ? location.HighLevel.Value.ToString() : "");
        //                                    Debug.WriteLine(strAlarm);
        //                                }
        //                                else
        //                                {
        //                                    location.CountLowAlarm++;
        //                                }
        //                            }
        //                        }

        //                    }
        //                    #endregion
        //                    #region Normal Alarm
        //                    else if ((location.LowLevel.HasValue || location.HighLevel.HasValue) && !location.IsNormalAlarm && (location.IsHighAlarm || location.IsLowAlarm))
        //                    {
        //                        if (double.TryParse(location.Value, out double value))
        //                        {
        //                            if ((!location.LowLevel.HasValue || value >= (location.LowLevel.Value + deadband)) && (!location.HighLevel.HasValue || (value <= (location.HighLevel.Value - deadband))))
        //                            {
        //                                if (location.CountNormalAlarm >= 1)
        //                                {
        //                                    location.CountNormalAlarm = 100;
        //                                    location.CountLowAlarm = 0;
        //                                    location.CountHighAlarm = 0;

        //                                    location.IsHighAlarm = false;
        //                                    location.IsNormalAlarm = true;
        //                                    location.IsLowAlarm = false;

        //                                    Alarm alarm = new Alarm();
        //                                    alarm.DateTime = CurrentTime;
        //                                    alarm.LocationName = location.Name;
        //                                    alarm.Type = "Normal Alarm";
        //                                    alarm.Value = value;
        //                                    alarm.LowLevel = location.LowLevel;
        //                                    alarm.HighLevel = location.HighLevel;
        //                                    alarm.Ack = "";
        //                                    alarm.Save();

        //                                    //if (EnableEmailAlarm)
        //                                    //{
        //                                    //    SendAlarmEmail("Normal Alarm", location);
        //                                    //}

        //                                    //if (EnableSMSAlarm)
        //                                    //{
        //                                    //    SendAlarmSMS("Normal Alarm", location);
        //                                    //}
        //                                    string strAlarm = "Location : " + location.Name
        //             + "\nAlarm: Normal"
        //             + "\nValue: " + location.Value
        //             + "\nLow Level: " + (location.LowLevel.HasValue ? location.LowLevel.Value.ToString() : "")
        //             + "\nHigh Level: " + (location.HighLevel.HasValue ? location.HighLevel.Value.ToString() : "");
        //                                    Debug.WriteLine(strAlarm);
        //                                }
        //                                else
        //                                {
        //                                    location.CountNormalAlarm++;
        //                                }
        //                            }
        //                        }
        //                    }
        //                    #endregion

        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                Console.WriteLine($"Lỗi đọc alarm: {ex.Message}");
        //            }
        //        }
        //    }
        //}
        #endregion

        private string GetMacAddress()
        {
            string macAddresses = string.Empty;

            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (nic.OperationalStatus == OperationalStatus.Up)
                {
                    macAddresses += nic.GetPhysicalAddress().ToString();
                    break;
                }
            }
            return macAddresses;
        }

        /// <summary>
        /// Hàm lấy kích thước của buffer dựa vào kiểu dữ liệu
        /// </summary>
        /// <param name="dataType"></param>
        /// <returns></returns>
        private int GetBufferSize(string dataType)
        {
            switch (dataType)
            {
                case "Word":
                    return 2;
                case "Short":
                    return 2;
                case "DWord":
                    return 4;
                case "Int":
                    return 4;
                case "Float":
                    return 4;
                case "Bool":
                    return 1;
                default:
                    break;
            }
            return 2;
        }

        #endregion

        #region Methods

        public void ReadValueFistTime()
        {
            foreach (var location in Locations)
            {
                try
                {
                    location.OverValue = VotLo;

                    if (location.State == "Enable")
                    {
                        bool success = false;
                        int count = 0;
                        ushort address = (ushort)location.MemoryAddress;
                        byte deviceId = (byte)location.DeviceId;

                        #region Coil 1 - 9999
                        if (address >= 1 && address <= 9999)
                        {
                            bool[] data = { true };
                            lock (readLocker)
                                success = GateWay.ModbusRTUMaster.ReadCoils(deviceId, address, 1, ref data);

                            if (!success)
                            {
                                while (!success && count <= 4)
                                {
                                    lock (readLocker)
                                        success = GateWay.ModbusRTUMaster.ReadCoils(deviceId, address, 1, ref data);
                                    if (count <= 2)
                                        Thread.Sleep(100);
                                    else Thread.Sleep(300);
                                    count++;
                                }
                            }
                            count = 0;
                            if (success)
                            {
                                if (data[0])
                                    location.Value = "1";
                                else
                                    location.Value = "0";
                            }
                            else
                            {
                                location.Value = "Null";
                            }
                        }
                        #endregion

                        #region Input 10001 - 19999
                        else if (address >= 10001 && address <= 19999)
                        {
                            bool[] data = { true };
                            address = (ushort)(address - 10001);
                            lock (readLocker)
                                success = GateWay.ModbusRTUMaster.ReadDiscreteInputContact(deviceId, address, 1, ref data);

                            if (!success)
                            {
                                while (!success && count <= 4)
                                {
                                    lock (readLocker)
                                        success = GateWay.ModbusRTUMaster.ReadDiscreteInputContact(deviceId, address, 1, ref data);
                                    if (count <= 2)
                                        Thread.Sleep(100);
                                    else Thread.Sleep(300);
                                    count++;
                                }
                            }
                            count = 0;
                            if (success)
                            {
                                if (data[0])
                                    location.Value = "1";
                                else
                                    location.Value = "0";
                            }
                            else
                            {
                                location.Value = "Null";
                            }
                        }
                        #endregion

                        #region Input Register 30001 - 39999
                        else if (address >= 30001 && address <= 39999)
                        {
                            int size = GetBufferSize(location.DataType);
                            if (size >= 2)
                            {
                                byte[] data = new byte[size];
                                address = (ushort)(address - 30001);
                                lock (readLocker)
                                    success = GateWay.ModbusRTUMaster.ReadInputRegisters(deviceId, address, (ushort)(size / 2), ref data);

                                if (!success)
                                {
                                    while (!success && count <= 4)
                                    {
                                        lock (readLocker)
                                            success = GateWay.ModbusRTUMaster.ReadInputRegisters(deviceId, address, (ushort)(size / 2), ref data);
                                        if (count <= 2)
                                            Thread.Sleep(100);
                                        else Thread.Sleep(300);
                                        count++;
                                    }
                                }
                                count = 0;
                                if (success)
                                {
                                    switch (location.DataType)
                                    {
                                        case "Word":
                                            readValueNew = location.ValueOld = Math.Round((GateWay.GetShortAt(data, 0) * location.Gain) + location.Offset, 2);
                                            location.Value = location.ValueOld.ToString();
                                            break;
                                        case "Short":
                                            readValueNew = location.ValueOld = Math.Round((GateWay.GetShortAt(data, 0) * location.Gain) + location.Offset, 2);
                                            location.Value = location.ValueOld.ToString();
                                            break;
                                        case "DWord":
                                            readValueNew = location.ValueOld = Math.Round((GateWay.GetShortAt(data, 0) * location.Gain) + location.Offset, 2);
                                            location.Value = location.ValueOld.ToString();
                                            break;
                                        case "Int":
                                            readValueNew = location.ValueOld = Math.Round((GateWay.GetShortAt(data, 0) * location.Gain) + location.Offset, 2);
                                            location.Value = location.ValueOld.ToString();
                                            break;
                                        case "Float":
                                            readValueNew = location.ValueOld = Math.Round((GateWay.GetShortAt(data, 0) * location.Gain) + location.Offset, 2);
                                            location.Value = location.ValueOld.ToString();
                                            break;
                                        default:
                                            break;
                                    }
                                }
                                else
                                {
                                    location.Value = "Null";
                                }
                            }
                        }
                        #endregion

                        #region Holding Register 40001 - 49999
                        if (address >= 40001 && address <= 49999)
                        {
                            int size = GetBufferSize(location.DataType);
                            if (size >= 2)
                            {
                                byte[] data = new byte[size];
                                address = (ushort)(address - 40001);
                                lock (readLocker)
                                    success = GateWay.ModbusRTUMaster.ReadHoldingRegisters(deviceId, address, (ushort)(size / 2), ref data);

                                if (!success)
                                {
                                    while (!success && count <= 4)
                                    {
                                        lock (readLocker)
                                            success = GateWay.ModbusRTUMaster.ReadHoldingRegisters(deviceId, address, (ushort)(size / 2), ref data);
                                        if (count <= 2)
                                            Thread.Sleep(100);
                                        else Thread.Sleep(300);
                                        count++;
                                    }
                                }
                                count = 0;
                                if (success)
                                {
                                    switch (location.DataType)
                                    {
                                        case "Word":
                                            readValueNew = location.ValueOld = Math.Round((GateWay.GetShortAt(data, 0) * location.Gain) + location.Offset, 2);
                                            location.Value = location.ValueOld.ToString();
                                            break;
                                        case "Short":
                                            readValueNew = location.ValueOld = Math.Round((GateWay.GetShortAt(data, 0) * location.Gain) + location.Offset, 2);
                                            location.Value = location.ValueOld.ToString();
                                            break;
                                        case "DWord":
                                            readValueNew = location.ValueOld = Math.Round((GateWay.GetShortAt(data, 0) * location.Gain) + location.Offset, 2);
                                            location.Value = location.ValueOld.ToString();
                                            break;
                                        case "Int":
                                            readValueNew = location.ValueOld = Math.Round((GateWay.GetShortAt(data, 0) * location.Gain) + location.Offset, 2);
                                            location.Value = location.ValueOld.ToString();
                                            break;
                                        case "Float":
                                            readValueNew = location.ValueOld = Math.Round((GateWay.GetShortAt(data, 0) * location.Gain) + location.Offset, 2);
                                            location.Value = location.ValueOld.ToString();
                                            break;
                                        default:
                                            break;
                                    }

                                }
                                else
                                {
                                    location.Value = "Null";
                                }
                            }
                        }
                        #endregion

                        if (success)
                            location.Status = "Good";
                        else
                            location.Status = "Bad";
                    }
                    else
                    {
                        location.Value = "Disable";
                        location.Status = "Disable";
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Lỗi đọc modbus: {ex.Message}");
                }
                Thread.Sleep(500);
            }
        }

        public void SendAlarmEmail(string alarm, Location location)
        {
            try
            {
                GateWay.Email.CredentialEmail = "giamsat.canhbao@gmail.com";
                GateWay.Email.CredentialPass = "1@3$5^7*";

                if (location.Name.Contains("KDNT") == false)
                {
                    GateWay.Email.emailTo = EmailString;
                }
                else
                {
                    GateWay.Email.emailTo = EmailStringKDNT;
                }

                GateWay.Email.subjectEmail = "Alarm";
                //string strAlarm = "Location : " + location.Name + Environment.NewLine
                //     + "Alarm: " + alarm + Environment.NewLine
                //     + "Value: " + location.Value + Environment.NewLine
                //     + "Low Level: " + (location.LowLevel.HasValue ? location.LowLevel.Value.ToString() : "") + Environment.NewLine
                //     + "High Level: " + (location.HighLevel.HasValue ? location.HighLevel.Value.ToString() : "");
                string strAlarm = $"{DateTime.Now}. {Environment.NewLine}Location: {location.Name}. {Environment.NewLine}Alarm: {alarm}. {Environment.NewLine}Value: {location.Value}" +
                    $". {Environment.NewLine}Low Level: {(location.LowLevel.HasValue ? location.LowLevel.Value.ToString() : "")}" +
                    $". {Environment.NewLine}High Level: {(location.HighLevel.HasValue ? location.HighLevel.Value.ToString() : "")}.";
                Debug.WriteLine($"EMAIL {strAlarm}");
                GateWay.Email.bodyEmail = strAlarm;
                GateWay.Email.TimeOut = 2000;
                GateWay.Email.SendEmail();
                Thread.Sleep(10);
                Console.WriteLine($"Gui Email");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi gửi Email : {ex.Message}");
            }
        }

        public void SendAlarmSMS(string alarm, Location location)
        {
            try
            {
                string noidung = $"{DateTime.Now}\nLocation: {location.Name}\nAlarm: {alarm}\nValue: {location.Value}" +
                    $"\nLow Level: {(location.LowLevel.HasValue ? location.LowLevel.Value.ToString() : "")}" +
                    $"\nHigh Level: {(location.HighLevel.HasValue ? location.HighLevel.Value.ToString() : "")}";
                Console.WriteLine($"SMS {noidung}");
                //for (int j = 0; j < SMSString.Split(',').Length; j++)
                //{
                //    //GateWay.SMS.GuiSMS(SMSString.Split(',')[j], noidung);
                //    Debug.WriteLine($"Gui SMS {GateWay.SMS.GuiSMS(SMSString.Split(',')[j], noidung)}");
                //    Thread.Sleep(2000);
                //}

                //nguyên mẫu
                //Console.WriteLine($"SDT {SMSString}");
                //Console.WriteLine($"Gui SMS {GateWay.SMS.GuiSMS(SMSString, noidung)}");

                //Cong update 09/01/2021
                //dùng SDT của kho chính và kho lẻ
                if (location.Name.Contains("KDNT") == false)
                {
                    Console.WriteLine($"SDT {SMSString}");
                    Console.WriteLine($"Gui SMS {GateWay.SMS.GuiSMS(SMSString, noidung)}");
                }
                else//dùng SDT của kho dược ngoại trú
                {
                    Console.WriteLine($"SDT {SMSStringKDNT}");
                    Console.WriteLine($"Gui SMS {GateWay.SMS.GuiSMS(SMSStringKDNT, noidung)}");
                }

                Console.WriteLine($"-----------------------------------------------------");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi gửi tin SMS : {ex.Message}");
            }
        }

        public string WriteValue(int deviceId, int address, string dataType, double value)
        {
            try
            {
                string type = "";
                if (address >= 10001 && address <= 19999)
                {
                    address = address - 10001;
                    type = "InputCoil";

                    return "Failed";
                }
                else if (address >= 30001 && address <= 39999)
                {
                    address = address - 30001;
                    type = "InputRegister";
                    return "Failed";
                }
                else if (address >= 40001 && address <= 49999)
                {
                    address = address - 40001;
                    type = "HoldingRegister";
                }
                else if (address >= 1 && address <= 9999)
                {
                    type = "Coil";
                    if (value > 1 || value < 0)
                        return "Failed";
                }
                else
                {
                    return "Failed";
                }


                int size = GetBufferSize(dataType);
                byte[] buffer = new byte[size];

                switch (dataType)
                {
                    case "Word":
                        {
                            GateWay.SetWord(buffer, 0, Convert.ToUInt16(value));
                            break;
                        }
                    case "Short":
                        {
                            SetShortAt(buffer, 0, Convert.ToInt16(value));
                            break;
                        }
                    case "DWord":
                        {
                            GateWay.SetDWord(buffer, 0, Convert.ToUInt32(value));
                            break;
                        }
                    case "Int":
                        {
                            GateWay.SetInt(buffer, 0, Convert.ToInt16(value));
                            break;
                        }
                    case "Float":
                        {
                            GateWay.SetFloat(buffer, 0, Convert.ToSingle(value));
                            break;
                        }
                    case "Bool":
                        {
                            GateWay.SetBit(buffer, 0, 0, Convert.ToInt32(1));
                            break;
                        }
                    default:
                        break;
                }

                bool result = false;
                lock (readLocker)
                {
                    switch (type)
                    {
                        case "Coil":
                            result = GateWay.ModbusRTUMaster.WriteSingleCoil((byte)deviceId, (ushort)address, value == 1 ? true : false);
                            break;
                        case "InputCoil":
                            break;
                        case "InputRegister":
                            break;
                        case "HoldingRegister":
                            result = GateWay.ModbusRTUMaster.WriteHoldingRegisters((byte)deviceId, (ushort)address, (ushort)(size / 2), buffer);
                            break;
                        default:
                            break;
                    }
                }
                int count = 0;
                if (!result)
                {
                    while (!result && count <= 4)
                    {
                        lock (readLocker)
                        {
                            switch (type)
                            {
                                case "Coil":
                                    result = GateWay.ModbusRTUMaster.WriteSingleCoil((byte)deviceId, (ushort)address, value == 1 ? true : false);
                                    break;
                                case "InputCoil":
                                    break;
                                case "InputRegister":
                                    break;
                                case "HoldingRegister":
                                    result = GateWay.ModbusRTUMaster.WriteHoldingRegisters((byte)deviceId, (ushort)address, (ushort)(size / 2), buffer);
                                    break;
                                default:
                                    break;
                            }
                        }
                        count++;
                        Thread.Sleep(100);
                    }
                }

                if (result)
                    return "Ok";
                else
                    return "Failed";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi ghi giá trị: {ex.Message}");
                return "Failed";
            }

        }

        public string ReadValue(int deviceId, int address, string dataType, out double value)
        {
            value = 0;
            try
            {
                string type = "";
                if (address >= 10001 && address <= 19999)
                {
                    address = address - 10001;
                    type = "InputCoil";

                }
                else if (address >= 30001 && address <= 39999)
                {
                    address = address - 30001;
                    type = "InputRegister";
                    return "Failed";
                }
                else if (address >= 40001 && address <= 49999)
                {
                    address = address - 40001;
                    type = "HoldingRegister";
                }
                else if (address >= 1 && address <= 99999)
                {
                    type = "Coil";
                }
                else
                {
                    return "Failed";
                }


                int size = GetBufferSize(dataType);
                byte[] buffer = new byte[size];
                bool[] boolValue = new bool[1];

                switch (dataType)
                {
                    case "Word":
                        {
                            GateWay.SetWord(buffer, 0, Convert.ToUInt16(value));
                            break;
                        }
                    case "Short":
                        {
                            SetShortAt(buffer, 0, Convert.ToInt16(value));
                            break;
                        }
                    case "DWord":
                        {
                            GateWay.SetDWord(buffer, 0, Convert.ToUInt32(value));
                            break;
                        }
                    case "Int":
                        {
                            GateWay.SetInt(buffer, 0, Convert.ToInt16(value));
                            break;
                        }
                    case "Float":
                        {
                            GateWay.SetFloat(buffer, 0, Convert.ToSingle(value));
                            break;
                        }
                    case "Bool":
                        {
                            GateWay.SetBit(buffer, 0, 0, Convert.ToInt32(1));
                            break;
                        }
                    default:
                        break;
                }

                bool result = false;
                lock (readLocker)
                {
                    switch (type)
                    {
                        case "Coil":
                            result = GateWay.ModbusRTUMaster.ReadCoils((byte)deviceId, (ushort)address, 1, ref boolValue);
                            break;
                        case "InputCoil":
                            result = GateWay.ModbusRTUMaster.ReadDiscreteInputContact((byte)deviceId, (ushort)address, 1, ref boolValue);
                            break;
                        case "InputRegister":
                            result = GateWay.ModbusRTUMaster.ReadInputRegisters((byte)deviceId, (ushort)address, 1, ref buffer);
                            break;
                        case "HoldingRegister":
                            result = GateWay.ModbusRTUMaster.ReadHoldingRegisters((byte)deviceId, (ushort)address, (ushort)(size / 2), ref buffer);
                            break;
                        default:
                            break;
                    }
                }
                int count = 0;
                if (!result)
                {
                    while (!result && count <= 4)
                    {
                        lock (readLocker)
                        {
                            switch (type)
                            {
                                case "Coil":
                                    result = GateWay.ModbusRTUMaster.ReadCoils((byte)deviceId, (ushort)address, 1, ref boolValue);
                                    break;
                                case "InputCoil":
                                    result = GateWay.ModbusRTUMaster.ReadDiscreteInputContact((byte)deviceId, (ushort)address, 1, ref boolValue);
                                    break;
                                case "InputRegister":
                                    result = GateWay.ModbusRTUMaster.ReadInputRegisters((byte)deviceId, (ushort)address, 1, ref buffer);
                                    break;
                                case "HoldingRegister":
                                    result = GateWay.ModbusRTUMaster.ReadHoldingRegisters((byte)deviceId, (ushort)address, (ushort)(size / 2), ref buffer);
                                    break;
                                default:
                                    break;
                            }
                        }
                        count++;
                        Thread.Sleep(100);
                    }
                }

                if (result)
                {
                    switch (type)
                    {
                        case "Coil":
                        case "InputCoil":
                            value = boolValue[0] ? 1 : 0;
                            break;
                        case "InputRegister":
                        case "HoldingRegister":
                            {

                                if (dataType == "Word")
                                    value = (double)GetUIntAt(buffer, 0);
                                else if (dataType == "DWord")
                                    value = (double)GateWay.GetUintAt(buffer, 0);
                                else if (dataType == "Short")
                                    value = (double)GetShortAt(buffer, 0);
                                else if (dataType == "Int")
                                    value = (double)GateWay.GetIntAt(buffer, 0);
                                else if (dataType == "Float")
                                    value = (double)GateWay.GetFloatAt(buffer, 0);
                                break;
                            }
                        default:
                            break;
                    }

                    return "Ok";
                }
                else
                    return "Failed";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi đọc giá trị: {ex.Message}");
                return "Failed";
            }
        }

        public string WriteValueATMS(int deviceId, double deadband, double tempHigh, double tempLow, double offset)
        {
            try
            {
                bool result = false;

                byte[] buffer = new byte[12];

                GateWay.SetInt(buffer, 0, Convert.ToInt16(tempHigh));
                GateWay.SetInt(buffer, 2, Convert.ToInt16(tempLow));
                GateWay.SetInt(buffer, 4, Convert.ToInt16(deadband * 10));
                GateWay.SetInt(buffer, 6, Convert.ToInt16(offset * 10));
                GateWay.SetInt(buffer, 8, 0);
                GateWay.SetInt(buffer, 10, 100);

                result = GateWay.ModbusRTUMaster.WriteHoldingRegisters((byte)deviceId, 1, 6, buffer);
                int count = 0;
                if (!result)
                {
                    while (!result && count <= 4)
                    {
                        result = GateWay.ModbusRTUMaster.WriteHoldingRegisters((byte)deviceId, 1, 6, buffer);
                        count++;
                        Thread.Sleep(100);
                    }
                }

                if (result)
                    return "Ok";
                else

                    return "Failed";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi ghi giá trị TMS: {ex.Message}");
                return "Failed";
            }
        }

        public string ReadValueATMS(int deviceId, out double deadband, out double tempHigh, out double tempLow, out double offset)
        {
            deadband = 0;
            tempHigh = 0;
            tempLow = 0;
            offset = 0;
            try
            {
                bool result = false;

                byte[] buffer = new byte[12];

                GateWay.SetInt(buffer, 0, Convert.ToInt16(tempHigh));
                GateWay.SetInt(buffer, 2, Convert.ToInt16(tempLow));
                GateWay.SetInt(buffer, 4, Convert.ToInt16(deadband * 10));
                GateWay.SetInt(buffer, 6, Convert.ToInt16(offset * 10));
                GateWay.SetInt(buffer, 8, 0);
                GateWay.SetInt(buffer, 10, 100);

                result = GateWay.ModbusRTUMaster.ReadHoldingRegisters((byte)deviceId, 1, 6, ref buffer);

                int count = 0;
                if (!result)
                {
                    while (!result && count <= 4)
                    {
                        result = GateWay.ModbusRTUMaster.ReadHoldingRegisters((byte)deviceId, 1, 6, ref buffer);
                        count++;
                        Thread.Sleep(100);
                    }
                }

                if (result)
                {
                    deadband = GateWay.GetIntAt(buffer, 0);
                    tempHigh = GateWay.GetIntAt(buffer, 2);
                    tempLow = GateWay.GetIntAt(buffer, 4);
                    offset = GateWay.GetIntAt(buffer, 6);
                    return "Ok";
                }
                else

                    return "Failed";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi đọc giá trị TMS: {ex.Message}");
                return "Failed";
            }
        }

        public void RefreshLocations()
        {
            //Locations
        }

        /// <summary>
        /// Methods to reboot a AT-Web/Logger
        /// </summary>
        public void Restart()
        {
            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            proc.StartInfo.FileName = "sudo";
            proc.StartInfo.Arguments = "reboot";
            proc.Start();
            proc.Close();
            proc.Dispose();
        }

        /// <summary>
        /// Get all location in database
        /// </summary>
        /// <returns></returns>
        public Locations GetLocations()
        {
            return Locations;
        }

        /// <summary>
        /// Get an limit alarm in database 
        /// </summary>
        /// <param name="limit"></param>
        /// <returns></returns>
        public Alarms GetAlarms(int limit = 100)
        {
            Alarms alarms = new Alarms();
            alarms.GetBySqlStatement($"select * from {AlarmTableName} order by DateTime desc limit {limit}");
            return alarms;
        }

        /// <summary>
        /// Get all alarm in database 
        /// </summary>
        /// <param name="limit"></param>
        /// <returns></returns>
        public Alarms GetAlarms()
        {
            Alarms alarms = new Alarms();
            alarms.GetAll();
            return alarms;
        }

        /// <summary>
        /// Hàm đọc file text
        /// </summary>
        /// <param name="PathFile"></param>
        /// <returns></returns>
        public string ReadText(string PathFile)
        {
            try
            {
                FileStream fs = new FileStream(PathFile, FileMode.Open, FileAccess.Read, FileShare.None);
                System.IO.StreamReader sr = new System.IO.StreamReader(fs);
                string text = sr.ReadToEnd().Trim();
                sr.Close();
                fs.Close();
                return text;
            }
            catch { return "NULL"; }
        }

        public void WriteText(string Text, string PathFile)
        {
            FileStream fs = new FileStream(PathFile, FileMode.Create, FileAccess.Write, FileShare.None);
            System.IO.StreamWriter sw = new System.IO.StreamWriter(fs);
            sw.WriteLine(Text);
            sw.Flush();
            sw.Close();
            fs.Close();
        }

        /// <summary>
        /// Methods to create all necessery table in database
        /// </summary>
        public void CreateTableIfNotExists()
        {
            MySqlConnection connection = new MySqlConnection(ConnectionString);
            connection.Open();
            string macID = MACID;
            string query = $"create table if not exists gateway.{macID}_location (Id int(11) not null auto_increment, Name varchar(30) not null, DeviceId int(3) not null, MemoryAddress int(5) not null, DataType varchar(10) not null, LowLevel real, HighLevel real, Gain real not null, Offset real not null, Deadband real, State varchar(10) not null, primary key(Id));";
            query += $"Create table if not exists gateway.{macID}_alarm (Id int(5) not null auto_increment, DateTime datetime not null, LocationName varchar(30) not null, Type varchar(20), Value real, LowLevel real, HighLevel real, Ack varchar(10), primary key(Id));";
            query += $"Create table if not exists gateway.{macID}_data (Id int (11) not null auto_increment primary key, DateTime datetime not null, LocationId int(5), Value real);";
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.ExecuteNonQuery();
            cmd.Dispose();
            connection.Close();
            connection.Dispose();
        }

        public void SaveSettings()
        {
            //Save email string
            WriteText($"{EmailString.Trim()}|{EmailStringKDNT.Trim()}", PathFile + "email.txt");
            //Save sms string
            WriteText($"{SMSString.Trim()}|{SMSStringKDNT.Trim()}", PathFile + "sms.txt");

            string alarmSettings = EnableSMSAlarm.ToString() + "," + EnableEmailAlarm.ToString() + "," + LogType + "," + LogRate + "," + ServerIpAddress.Trim();
            WriteText(alarmSettings.Trim(), PathFile + "enableSMSEmail.txt");

            string modbusRTUString = "MacAdd:" + MACID + "|Port:" + PortName + "|Baudrate:" + BaudRate.ToString()
                + "|DataBit:" + DataBits.ToString() + "|Parity:" + Parity.ToString() + "|Stopbit:" + Stopbits.ToString()
                + "|TimeOut:" + Timeout.ToString();
            WriteText(modbusRTUString.Trim(), PathFile + "Parametter.txt");

        }

        public bool SaveSettingsPass(string newPass, string oldPass)
        {
            bool res = false;
            if (oldPass == Password)
            {
                //Save Pass
                WriteText(newPass.Trim(), PathFile + "password.txt");
                res = true;
            }

            return res;
        }



        #endregion

        #region Get/Set 16 bit signed value (S7 int) -32768..32767 SHORT
        public static int GetShortAt(byte[] Buffer, int Pos)
        {
            return (int)((Buffer[Pos] << 8) | Buffer[Pos + 1]);
        }
        public static void SetShortAt(byte[] Buffer, int Pos, Int16 Value)
        {
            Buffer[Pos] = (byte)(Value >> 8);
            Buffer[Pos + 1] = (byte)(Value & 0x00FF);
        }
        #endregion

        #region Get/Set 16 bit unsigned value (S7 UInt) 0..65535
        public static UInt16 GetUIntAt(byte[] Buffer, int Pos)
        {
            return (UInt16)((Buffer[Pos] << 8) | Buffer[Pos + 1]);
        }
        public static void SetUIntAt(byte[] Buffer, int Pos, UInt16 Value)
        {
            Buffer[Pos] = (byte)(Value >> 8);
            Buffer[Pos + 1] = (byte)(Value & 0x00FF);
        }
        #endregion
    }
}
