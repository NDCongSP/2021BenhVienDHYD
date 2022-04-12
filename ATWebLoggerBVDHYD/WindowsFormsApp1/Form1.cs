using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using PLCPiProject;
using System.Net.Mail;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {

        static PLCPi GateWay = new PLCPi();
        public Form1()
        {
            InitializeComponent();

            //PLCPiProject.PLCPi pi = new PLCPiProject.PLCPi();
            //pi.ModbusRTUMaster.KetNoi("COM4", 9600, 8, System.IO.Ports.Parity.None, System.IO.Ports.StopBits.One);
            //bool[] result = new bool[1];
            //pi.ModbusRTUMaster.ReadDiscreteInputContact(1, 0, 1, ref result);
            //label1.Text = result[0].ToString();
            //Debug.WriteLine(pi.SMS.Port_USB3G);
        }

        public void SendAlarmEmail(string alarm)
        {
            try
            {
                GateWay.Email.CredentialEmail = "giamsat.canhbao@gmail.com";
                GateWay.Email.CredentialPass = "1@3$5^7*";
                GateWay.Email.Message.From = new System.Net.Mail.MailAddress("giamsat.canhbao@gmail.com");
                GateWay.Email.Message.To.Clear();
                GateWay.Email.Message.To.Add("ndcong08cddv02@gmail.com");
                GateWay.Email.Message.Subject = "Alarm";
                string strAlarm = "Location : 1" + Environment.NewLine
                    + "Alarm: " + alarm + Environment.NewLine
                    + "Value: 100" + Environment.NewLine
                    + "Low Level: 0" + Environment.NewLine
                    + "High Level: 100";
                GateWay.Email.Message.Body = strAlarm;
                GateWay.Email.TimeOut = 2000;
                GateWay.Email.SendEmail();
                Thread.Sleep(10);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi gửi Email : {ex.Message}");
            }
        }
        public void SendAlarmEmail1(string alarm)
        {
            try
            {
                GateWay.Email.CredentialEmail = "giamsat.canhbao@gmail.com";
                GateWay.Email.CredentialPass = "1@3$5^7*";

                GateWay.Email.emailTo = "ndcong08cddv02@gmail.com,soft@atpro.com.vn";
                GateWay.Email.subjectEmail = "Alarm";
                string strAlarm = "Location : 1" + Environment.NewLine
                    + "Alarm: " + alarm + Environment.NewLine
                    + "Value: 100" + Environment.NewLine
                    + "Low Level: 0" + Environment.NewLine
                    + "High Level: 100";
                GateWay.Email.bodyEmail = strAlarm;
                GateWay.Email.TimeOut = 2000;
                GateWay.Email.SendEmail();
                Thread.Sleep(10);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi gửi Email : {ex.Message}");
            }
        }

        public void sendEmail(string body)
        {
            if (String.IsNullOrEmpty("ndcong08cddv02@gmail.com"))
                return;
            try
            {
                MailMessage mail = new MailMessage();
                mail.To.Add("ndcong08cddv02@gmail.com,soft@atpro.com.vn");
                mail.From = new MailAddress("giamsat.canhbao@gmail.com");
                mail.Subject = "sub";

                mail.Body = body;

                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com"; //Or Your SMTP Server Address
                smtp.Credentials = new System.Net.NetworkCredential
                     ("giamsat.canhbao@gmail.com", "1@3$5^7*"); // ***use valid credentials***
                smtp.Port = 587;

                //Or your Smtp Email ID and Password
                smtp.EnableSsl = true;
                smtp.Send(mail);
                mail.Dispose();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception in sendEmail:" + ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SendAlarmEmail1("Alarm");
            
        }
    }
}
