using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data;

namespace GSNDDongDeuPhong
{
    public partial class BaoCao : Form
    {
        public BaoCao()
        {
            InitializeComponent();
        }
        ConnectMySQL _MysqlCmd = new ConnectMySQL();
        DataTable BangData = new DataTable();

        private void button1_Click(object sender, EventArgs e)
        {
            ////MessageBox.Show(comboBox1.Text);
            _MysqlCmd.ConectionString = "Server=localhost;Database=bvdhyd;Port=3306;Uid=root;Pwd=100100;charset=utf8";
            //if(comboBox1.Text=="Kho Chính")
            //    BangData = _MysqlCmd.Table("dataphong1", "ThoiGian>='2019-05-20 08:00:00' and ThoiGian<='2019-05-26 23:59:50'");
            //else
            //    BangData = _MysqlCmd.Table("dataphong2", "ThoiGian>='2019-05-13 08:00:00' and ThoiGian<='2019-05-19 23:59:50'");
            BangData = _MysqlCmd.TableWhere("dataphong2", "ThoiGian," + textBox2.Text, "ThoiGian>='2019-05-13 08:00:00' and ThoiGian<='2019-05-19 23:59:50'");
            dataGridView1.DataSource = BangData;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // creating Excel Application  
            Microsoft.Office.Interop.Excel._Application app = new Microsoft.Office.Interop.Excel.Application();
            // creating new WorkBook within Excel application  
            Microsoft.Office.Interop.Excel._Workbook workbook = app.Workbooks.Add(Type.Missing);
            // creating new Excelsheet in workbook  
            Microsoft.Office.Interop.Excel._Worksheet worksheet = null;
            // see the excel sheet behind the program  
            app.Visible = true;
            // get the reference of first sheet. By default its name is Sheet1.  
            // store its reference to worksheet  
            worksheet = workbook.Sheets["Sheet1"];
            worksheet = workbook.ActiveSheet;
            // changing the name of active sheet  
            worksheet.Name = "NhietDo";
            // storing header part in Excel  
            for (int i = 1; i < dataGridView1.Columns.Count + 1; i++)
            {
                worksheet.Cells[1, i] = dataGridView1.Columns[i - 1].HeaderText;
            }
            // storing Each row and column value to excel sheet  
            for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
            {
                for (int j = 0; j < dataGridView1.Columns.Count; j++)
                {
                    worksheet.Cells[i + 2, j + 1] = dataGridView1.Rows[i].Cells[j].Value.ToString();
                }
            }
            // save the application  
            workbook.SaveAs("c:\\output.xlsx", Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlExclusive, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            // Exit from the application  
            app.Quit();
        }
    }
}
