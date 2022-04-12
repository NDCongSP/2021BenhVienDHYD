using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSNDDongDeuPhong
{
    public class ConnectMySQL
    {


        MySqlConnection conn = null;
        public string ConectionString = "";// "localhost";

        #region Constructors

        public ConnectMySQL()
        {
            //conn = new MySqlConnection(ConectionString);
        }

        #endregion

        #region Open/Close Connection
        private bool Open()
        {
            try
            {
                conn = new MySqlConnection(ConectionString);
                conn.Open();
                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool Close()
        {
            try
            {
                conn.Close();
                conn.Dispose();
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        public string set_SAFE_UPDATES()
        {
            try
            {
                string MysqlCmd = "SET SQL_SAFE_UPDATES = 0";
                if (this.Open())
                {
                    MySqlCommand cmd = new MySqlCommand(MysqlCmd, conn);
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                    cmd = null;
                    this.Close();
                }
                return "Good";
            }
            catch { return "Bad"; }
        }

        public int Insert(string table, string column, string value)
        {

            int kq = 0;
            string query = "INSERT INTO " + table + " (" + column + ") VALUES (" + value + ")";

            //try
            {
                if (this.Open())
                {

                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    kq = cmd.ExecuteNonQuery();
                    this.Close();
                }
            }
            //catch { this.Close(); }
            return kq;
        }

        public int Update(string table, string SET, string WHERE)
        {

            int kq = 0;

            string query = "UPDATE " + table + " SET " + SET + " WHERE " + WHERE + "";
            if (this.Open())
            {
                try
                {

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    kq = cmd.ExecuteNonQuery();
                    this.Close();
                }
                catch { this.Close(); }
            }
            return kq;
        }

        public int Delete(string table, string WHERE, string VALUE_WHERE)
        {
            int kq = 0;
            string query = "DELETE FROM " + table + " WHERE " + WHERE + "='" + VALUE_WHERE + "'";

            if (this.Open())
            {
                try
                {
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    kq = cmd.ExecuteNonQuery();
                    this.Close();
                }
                catch { this.Close(); }
            }
            return kq;
        }
        public int Delete1(string table, string WHERE)
        {
            int kq = 0;
            string query = "DELETE FROM " + table + " WHERE " + WHERE;

            if (this.Open())
            {
                try
                {
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    kq = cmd.ExecuteNonQuery();
                    this.Close();
                }
                catch { this.Close(); }
            }
            return kq;
        }


        public DataTable Table(string table)
        {

            DataTable datatable = new DataTable();
            string query = "select * from " + table + "";
            if (this.Open() == true)
            {
                try
                {
                    MySqlDataAdapter Adapter = new MySqlDataAdapter(query, conn);
                    Adapter.Fill(datatable);
                    this.Close();
                }
                catch (System.Exception)
                {
                    datatable = null;
                    this.Close();
                }
            }
            return datatable;
        }

        public DataTable TableWhere(string table, string select, string where)
        {

            DataTable datatable = new DataTable();
            string query = "select " + select + " from " + table + " where " + where + "";
            if (this.Open() == true)
            {
                try
                {
                    MySqlDataAdapter Adapter = new MySqlDataAdapter(query, conn);
                    Adapter.Fill(datatable);
                    this.Close();
                }
                catch (System.Exception)
                {
                    datatable = null;
                    this.Close();
                }
            }
            return datatable;
        }

        public DataSet Columns(string table, string col)
        {

            DataSet ds = new DataSet();
            string query = "select " + col + " from " + table + "";

            if (this.Open() == true)
            {
                try
                {
                    MySqlDataAdapter Adapter = new MySqlDataAdapter(query, conn);
                    Adapter.Fill(ds);
                    this.Close();
                }
                catch (System.Exception)
                {
                    ds = null;
                    this.Close();
                }
            }
            return ds;
        }

        public DataSet ColumnsWhere(string table, string col, string where)
        {

            DataSet ds = new DataSet();
            string query = "select " + col + " from " + table + " where " + where + "";

            if (this.Open() == true)
            {
                try
                {
                    MySqlDataAdapter Adapter = new MySqlDataAdapter(query, conn);
                    Adapter.Fill(ds);
                    this.Close();
                }
                catch (System.Exception)
                {
                    ds = null;
                    this.Close();
                }
            }
            return ds;
        }

        public int CountWhere(string table, string value)
        {
            string query = "SELECT Count(*) FROM " + table + " WHERE " + value + "";
            int Count = -1;
            if (this.Open() == true)
            {
                try
                {
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    Count = int.Parse(cmd.ExecuteScalar() + "");
                    this.Close();
                }
                catch { this.Close(); }
                return Count;
            }
            else
            {
                return Count;
            }
        }
        public Dictionary<string, string> Select(string table, string WHERE)
        {

            string query = "SELECT * FROM " + table + " WHERE " + WHERE + "";

            Dictionary<string, string> selectResult = new Dictionary<string, string>();

            if (this.Open())
            {
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataReader dataReader = cmd.ExecuteReader();

                try
                {
                    while (dataReader.Read())
                    {

                        for (int i = 0; i < dataReader.FieldCount; i++)
                        {
                            selectResult.Add(dataReader.GetName(i).ToString(), dataReader.GetValue(i).ToString());
                        }

                    }
                    dataReader.Close();
                }
                catch { }
                this.Close();

                return selectResult;
            }
            else
            {
                return selectResult;
            }
        }

        public int Count(string table)
        {
            string query = "SELECT Count(*) FROM " + table + "";
            int Count = -1;
            if (this.Open() == true)
            {
                try
                {
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    Count = int.Parse(cmd.ExecuteScalar() + "");
                    this.Close();
                }
                catch { this.Close(); }
                return Count;
            }
            else
            {
                return Count;
            }
        }

        public bool Taobang(string tenbang)
        {
            bool kq = false;
            string query = "create table " + tenbang +
                    "("
                    + "mahang VARCHAR(50) NOT NULL,"
                    + "tenhang VARCHAR(500) NULL,"
                    + "soluong INT NULL,"
                    + "dongia INT NULL,"
                    + "ngaynhapkho VARCHAR(50) NULL,"
                    + "nguoinhapkho VARCHAR(45) NULL,"
                    + "chuthich VARCHAR(500) NULL,"
                    + "slcanhbao VARCHAR(50) NULL,CONSTRAINT pk_PersonID PRIMARY KEY (mahang)"
                    + ")";
            try
            {
                if (this.Open())
                {

                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    cmd.ExecuteNonQuery();
                    kq = true;
                    this.Close();
                }
            }
            catch
            {
                kq = false;
                this.Close();
            }
            return kq;
        }

        public DataTable LayDSTenbang()
        {
            DataTable datatable = new DataTable();
            string query = "select table_name from information_schema.tables where TABLE_SCHEMA='quanlykho';";
            try
            {
                if (this.Open())
                {
                    MySqlDataAdapter Adapter = new MySqlDataAdapter(query, conn);
                    Adapter.Fill(datatable);
                    this.Close();
                }
            }
            catch
            {
                this.Close();
                datatable = null;
            }
            return datatable;
        }
    }
}
