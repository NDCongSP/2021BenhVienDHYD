using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GSNDDongDeuPhong
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        ConnectMySQL _MysqlCmd = new ConnectMySQL();
        Random rd = new Random();
        double[] nhiet1 = new double[12];
        double[] nhiet2 = new double[18];
        int ngay1 = 20, ngay2 = 13, gio = 0, phut = 0;
        string ThoiGianLog1 = "", ThoiGianLog2 = "";
        DateTime ThoiGianLog, ThoiGianLogOld;

        private void button2_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //aInterval = new System.TimeSpan(0, 0, 5, 0);

            //// Thêm một khoảng thời gian.
            //ThoiGianLog = ThoiGianLogOld.Add(aInterval);
            //ThoiGianLogOld = ThoiGianLog;
            //Debug.WriteLine(ThoiGianLog);

            ////_MysqlCmd.Insert("b827eb593e42_data", $"DateTimeData, Location_1,Low_Level_1,High_Level_1, Location_2,Low_Level_2,High_Level_2, Location_3,Low_Level_3,High_Level_3,Location_4,Low_Level_4,High_Level_4"
            ////    , $"'{ThoiGianLog.ToString("yyyy-MM-dd HH:mm:ss")}',round(RAND() * (-18 - -23 + 1) + -23,1), -25,10, round(RAND() * (88 - 70 + 1) + 70,1), 0,90,round(RAND() * (-18 - -23 + 1) + -23,1), -25,10,round(RAND() * (88 - 70 + 1) + 70,1), 0,90");
            //_MysqlCmd.Insert("b827eb593e42_data", $"DateTimeData, Location_1,Low_Level_1,High_Level_1, Location_2,Low_Level_2,High_Level_2, Location_3,Low_Level_3,High_Level_3,Location_4,Low_Level_4,High_Level_4,Location_5,Low_Level_5,High_Level_5" +
            //                $",Location_6,Low_Level_6,High_Level_6, Location_7,Low_Level_7,High_Level_7, Location_8,Low_Level_8,High_Level_8,Location_9,Low_Level_9,High_Level_9,Location_10,Low_Level_10,High_Level_10" +
            //                $",Location_11,Low_Level_11,High_Level_11, Location_12,Low_Level_12,High_Level_12, Location_13,Low_Level_13,High_Level_13,Location_14,Low_Level_14,High_Level_14,Location_15,Low_Level_15,High_Level_15,Location_16,Low_Level_16,High_Level_16"
            //                , $"'{ThoiGianLog.ToString("yyyy-MM-dd HH:mm:ss")}',round(RAND() * (-18 - -23 + 1) + -23,1), -25,10, round(RAND() * (88 - 70 + 1) + 70,1), 0,90,round(RAND() * (-18 - -23 + 1) + -23,1), -25,10,round(RAND() * (88 - 70 + 1) + 70,1), 0,90,null,0,30" +
            //                $",null,0,30,null,0,30,null,0,30,null,0,30,null,0,30" +
            //                $",null,0,30,null,0,30,null,0,30,null,0,30,null,0,30,null,0,30");
            ThoiGianLog = ThoiGianLogOld = Convert.ToDateTime(textBox1.Text);
            Debug.WriteLine(ThoiGianLog);
            timer1.Enabled = true;
        }

        TimeSpan aInterval;

        private void Form1_Load(object sender, EventArgs e)
        {
            _MysqlCmd.ConectionString = "Server = 103.48.195.37; Port = 3306; Uid = plcpi_giamsat; Pwd = 12345; Database = gateway";
            
            //timer1.Enabled = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;

            // Một khoảng thời gian. 
            // 0 giờ + 5 phút
            aInterval = new System.TimeSpan(0, 0, 5, 0);

            // Thêm một khoảng thời gian.
            ThoiGianLog = ThoiGianLogOld.Add(aInterval);
            ThoiGianLogOld = ThoiGianLog;
            Debug.WriteLine(ThoiGianLog);

            _MysqlCmd.Insert("b827eb593e42_data", $"DateTimeData, Location_1,Low_Level_1,High_Level_1, Location_2,Low_Level_2,High_Level_2, Location_3,Low_Level_3,High_Level_3,Location_4,Low_Level_4,High_Level_4,Location_5,Low_Level_5,High_Level_5" +
                            $",Location_6,Low_Level_6,High_Level_6, Location_7,Low_Level_7,High_Level_7, Location_8,Low_Level_8,High_Level_8,Location_9,Low_Level_9,High_Level_9,Location_10,Low_Level_10,High_Level_10" +
                            $",Location_11,Low_Level_11,High_Level_11, Location_12,Low_Level_12,High_Level_12, Location_13,Low_Level_13,High_Level_13,Location_14,Low_Level_14,High_Level_14,Location_15,Low_Level_15,High_Level_15,Location_16,Low_Level_16,High_Level_16"
                            , $"'{ThoiGianLog.ToString("yyyy-MM-dd HH:mm:ss")}',round(RAND() * (-18 - -23 + 1) + -23,1), -25,10, round(RAND() * (88 - 70 + 1) + 70,1), 0,90,round(RAND() * (-18 - -23 + 1) + -23,1), -25,10,round(RAND() * (88 - 70 + 1) + 70,1), 0,90,null,0,30" +
                            $",null,0,30,null,0,30,null,0,30,null,0,30,null,0,30" +
                            $",null,0,30,null,0,30,null,0,30,null,0,30,null,0,30,null,0,30");
            //Debug.WriteLine(Convert.ToDateTime("2019-01-30 14:25:05"));
            //kiem tra thoi gian ket thuc 
            //if (DateTime.Compare(ThoiGianLog, Convert.ToDateTime("2019-01-30 14:25:05"))>0)
            //    timer1.Enabled = true;
            timer1.Enabled = true;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
