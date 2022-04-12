using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PLCPiProject;
using MySql.Data.MySqlClient;
using System.IO;
using System.Data;
using System.Diagnostics;

namespace SendSMS
{
    class Program
    {
        static PLCPi myPLCpi = new PLCPi();
        static string _path = $"D:\\ATPro\\CodeProject\\GatewayPi\\WeblogMVC\\SourceCode\\";
        //static string _path = $"/home/pi/";
        static string SMSString = "", ServerIpAddress="",noiDung="";
        static string ConnectionString = "";
        static DataTable bangSMS = new DataTable();

        static void Main(string[] args)
        {
            string temp = ReadText(_path + "enableSMSEmail.txt");
            string[] prams = temp.Split(',');
            ServerIpAddress = prams[4].Trim();

            Console.WriteLine($"EnableSMS/EnableEmail/LogType/LogRate/ServerIp: {temp}");
            ConnectionString = $"user id=root;password=100100;database=gateway;server={ServerIpAddress};convertzerodatetime=True;port=3306";

            SMSString = ReadText(_path + "sms.txt").Trim();
            Console.WriteLine($"{ConnectionString}|SDT {SMSString}");

            Console.WriteLine("Khởi tạo USB3G");
            //myPLCpi.SMS.Port_USB3G = "/dev/ttyUSB1";
            myPLCpi.SMS.Port_USB3G = "COM12";
            //GateWay.SMS.Khoitao();
            Console.WriteLine($"Com SMS {myPLCpi.SMS.Port_USB3G}| Khoi Tao {myPLCpi.SMS.Khoitao()}");
            Console.WriteLine("Khởi tạo USB3G thành công");

            //Console.ReadKey();
            while (true)
            {
                bangSMS.Clear();
                bangSMS = GetSMS();
                if (bangSMS!=null&&bangSMS.Rows.Count>0)
                {
                    for (int i = 0; i < bangSMS.Rows.Count; i++)
                    {
                       noiDung= $"{DateTime.Now}\nLocation: {bangSMS.Rows[i][1].ToString()}\nAlarm: {bangSMS.Rows[i][2].ToString()}\nValue: {bangSMS.Rows[i][3].ToString()}" +
                    $"\nLow Level: {bangSMS.Rows[i][4].ToString()}" +
                    $"\nHigh Level: {bangSMS.Rows[i][5].ToString()}";
                        Console.WriteLine($"noi dung sms\n{noiDung}");
                        if (myPLCpi.SMS.GuiSMS(SMSString, noiDung)=="Good")
                        {
                            SetSMS(Convert.ToInt16(bangSMS.Rows[i][0].ToString()));
                            Console.WriteLine($"gui SMS thanh cong");
                        }
                
                    }
                }

            }
        }

        static string ReadText(string PathFile)
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

        static DataTable GetSMS()
        {
            DataTable userData = new DataTable();
            string query;
            MySqlConnection conn;
            conn = new MySqlConnection(ConnectionString);
            conn.Open();

            query = $"select * from test_sms where Flag=100";
            try
            {
                MySqlDataAdapter Adapter = new MySqlDataAdapter(query, conn);
                Adapter.Fill(userData);
                conn.Close();
                conn.Dispose();
            }
            catch (System.Exception)
            {
                userData = null;
                conn.Close();
                conn.Dispose();
            }
            return userData;
        }

        static int SetSMS(int id)
        {
            int res = 0;
            string query;
            MySqlConnection conn;
            conn = new MySqlConnection(ConnectionString);
            conn.Open();

            query = $"update test_sms set Flag=0 where Id='{id}'";
            try
            {
                MySqlCommand cmd = new MySqlCommand(query, conn);
                res = cmd.ExecuteNonQuery();
                cmd.Dispose();
                conn.Close();
                conn.Dispose();
            }
            catch (System.Exception)
            {
                conn.Close();
                conn.Dispose();
            }
            return res;
        }
    }
}
