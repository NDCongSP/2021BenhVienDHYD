using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
        int ngay1 = 17, ngay2 = 17, gio = 8, phut = 0;
        string ThoiGianLog1 = "", ThoiGianLog2 = "";

        int rInt = 0;// rd.Next(22, 26); //for ints
        int range = 1;
        //double rDouble = rd.NextDouble() * range; //for doubles

        private void Form1_Load(object sender, EventArgs e)
        {
            _MysqlCmd.ConectionString = "Server=45.119.212.41;Database=bvdhyd;Port=3306;Uid=customer_ttp;Pwd=ThinhTamPhat!@#456&*(;charset=utf8";



            timer1.Enabled = true;
        }
        double mantissa = 0, exponent = 0;
        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;

            //mantissa = (rd.NextDouble());// * 2.0) - 1.0;
            //                             // choose -149 instead of -126 to also generate subnormal floats (*)
            //exponent = rd.Next(22, 27);
            //txtTest.Text = $"{mantissa}|{exponent}|{Math.Round(mantissa + exponent, 2)}";

            ////label1.Text = rd.Next(Convert.ToInt32(textBox1.Text), Convert.ToInt32(textBox2.Text)).ToString();//Chuyển giá trị ramdon về
            for (byte i = 0; i < 12; i++)
            {
                if (i <= 1)
                {
                    mantissa = (rd.NextDouble());// * 2.0) - 1.0;
                                                 // choose -149 instead of -126 to also generate subnormal floats (*)
                    exponent = rd.Next(24, 29);
                    nhiet1[i] = Math.Round(mantissa + exponent + 0.5, 2);
                }
                else if (i > 1 && i < 10)
                {
                    mantissa = (rd.NextDouble());// * 2.0) - 1.0;
                                                 // choose -149 instead of -126 to also generate subnormal floats (*)
                    exponent = rd.Next(23, 25);

                    nhiet1[i] = Math.Round(mantissa + exponent, 2);
                }
                else
                {
                    mantissa = (rd.NextDouble());// * 2.0) - 1.0;
                                                 // choose -149 instead of -126 to also generate subnormal floats (*)
                    exponent = rd.Next(16, 22);
                    nhiet1[i] = Math.Round(mantissa + exponent, 2);
                }

            }
            for (byte i = 0; i < 18; i++)
            {


                if (i <= 2)
                {
                    mantissa = (rd.NextDouble());// * 2.0) - 1.0;
                                                 // choose -149 instead of -126 to also generate subnormal floats (*)
                    exponent = rd.Next(22, 29);

                    nhiet2[i] = Math.Round(mantissa + exponent + 0.5, 2);
                }
                else if (i > 2 && i <= 14)
                {
                    mantissa = (rd.NextDouble());// * 2.0) - 1.0;
                                                 // choose -149 instead of -126 to also generate subnormal floats (*)
                    exponent = rd.Next(22, 25);
                    nhiet2[i] = Math.Round(mantissa + exponent, 2);
                }
                else
                {
                    mantissa = (rd.NextDouble());// * 2.0) - 1.0;
                                                 // choose -149 instead of -126 to also generate subnormal floats (*)
                    exponent = rd.Next(17, 22);
                    nhiet2[i] = Math.Round(mantissa + exponent, 2);
                }
            }
            label1.Text = nhiet1[0].ToString();
            label2.Text = nhiet1[1].ToString();
            label3.Text = nhiet1[2].ToString();
            label4.Text = nhiet1[3].ToString();
            label5.Text = nhiet1[4].ToString();
            label6.Text = nhiet1[5].ToString();
            label7.Text = nhiet1[6].ToString();
            label8.Text = nhiet1[7].ToString();
            label9.Text = nhiet1[8].ToString();
            label10.Text = nhiet1[9].ToString();
            label11.Text = nhiet1[10].ToString();
            label12.Text = nhiet1[11].ToString();
            ThoiGianLog1 = "2020-12-" + ngay1.ToString() + " " + gio.ToString("00") + ":" + phut.ToString("00") + ":33";
            ThoiGianLog2 = "2020-12-" + ngay2.ToString() + " " + gio.ToString("00") + ":" + phut.ToString("00") + ":48";
            _MysqlCmd.Insert("dataphong1", "ThoiGian,Diem1_30,Diem1_120,Diem1_210,Diem2_30,Diem2_120,Diem2_210,Diem3_30" +
                ",Diem3_120,Diem3_210,Diem4_30,Diem4_120,Diem4_210",
                "'" + ThoiGianLog1 + "','" + nhiet1[0].ToString() + "','" + nhiet1[1].ToString() + "','" + nhiet1[2].ToString() + "','" + nhiet1[3].ToString() + "','" + nhiet1[4].ToString()
                + "','" + nhiet1[5].ToString() + "','" + nhiet1[6].ToString() + "','" + nhiet1[7].ToString() + "','" + nhiet1[8].ToString() + "','" + nhiet1[9].ToString()
                + "','" + nhiet1[10].ToString() + "','" + nhiet1[11].ToString() + "'");

            _MysqlCmd.Insert("dataphong2", "ThoiGian,Diem1_30,Diem1_120,Diem1_210,Diem2_30,Diem2_120,Diem2_210,Diem3_30," +
                "Diem3_120,Diem3_210,Diem4_30,Diem4_120,Diem4_210,Diem5_30,Diem5_120,Diem5_210,Diem6_30,Diem6_120," +
                "Diem6_210",
                "'" + ThoiGianLog2 + "','" + nhiet2[0].ToString() + "','" + nhiet2[1].ToString() + "','" + nhiet2[2].ToString() + "','" + nhiet2[3].ToString() + "','" + nhiet2[4].ToString()
                + "','" + nhiet2[5].ToString() + "','" + nhiet2[6].ToString() + "','" + nhiet2[7].ToString() + "','" + nhiet2[8].ToString() + "','" + nhiet2[9].ToString()
                + "','" + nhiet2[10].ToString() + "','" + nhiet2[11].ToString() + "','" + nhiet2[12].ToString() + "','" + nhiet2[13].ToString() + "','" + nhiet2[14].ToString()
                + "','" + nhiet2[15].ToString() + "','" + nhiet2[16].ToString() + "','" + nhiet2[17].ToString() + "'");
            label13.Text = ThoiGianLog1 + "|" + ThoiGianLog2;
            phut = phut + 30;
            if (phut == 60)
            {
                phut = 0;
                gio++;
                if (gio == 24)
                {
                    gio = 0;
                    ngay1++;
                    ngay2++;
                }
            }
            if (ngay1 <= 23)
                timer1.Enabled = true;

            //timer1.Enabled = true;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
