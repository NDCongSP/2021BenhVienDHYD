using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ATWebLogger
{
    public interface IDataAccess
    {
        string DatabaseName { get; set; }
        string ServerName { get; set; }
        string Password { get; set; }
        string User { get; set; }

        string ConnectionString { get; set; }

        DataTable ExecuteQuery(string query, object[] parameters = null);
        object ExecuteScalarQuery(string query, object[] parameters = null);
        int ExecuteNonQuery(string query, object[] parameters = null);
        int CreateDatabaseIfNotExists();
        int CreateTableIfNotExists(string tableName, string columns);
    }

    public class MySqlDataAccess : IDataAccess
    {
        #region Properties

        private string connectionString;
        /// <summary>
        /// The connection string of database
        /// </summary>
        public string ConnectionString
        {
            get
            {
                if (string.IsNullOrEmpty(connectionString))
                    return $"server={ServerName};user id={User};password={Password};database={DatabaseName}";
                return connectionString;
            }
            set { connectionString = value; }
        }

        public string DatabaseName { get; set; }
        public string ServerName { get; set; }
        public string Password { get; set; }
        public string User { get; set; }

        #endregion

        #region Constructors

        public MySqlDataAccess(string connectionString = null)
        {
            ConnectionString = connectionString;
        }

        public MySqlDataAccess(string dbName, string serverName, string user, string password)
        {
            DatabaseName = dbName;
            ServerName = serverName;
            User = user;
            Password = password;
        }

        #endregion

        #region Methods

        public int ExecuteNonQuery(string query, object[] parameters = null)
        {
            int result = 0;

            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                try
                {
                    //Open connection
                    connection.Open();

                    //Create the command for query a database
                    MySqlCommand command = new MySqlCommand(query, connection);

                    //Add the paramters into a command
                    command.AddParameters(query, parameters);

                    //Execute the command
                    result = command.ExecuteNonQuery();

                    //Close connection and dispose resource when we are done
                    connection.Close();
                    command.Dispose();
                }
                catch (Exception ex)
                {
                    DebugLogger.Log(ex.Message);
                }
            }

            //Return result
            return result;
        }

        public DataTable ExecuteQuery(string query, object[] parameters = null)
        {
            DataTable dtTable = new DataTable();

            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                try
                {
                    //Open connection
                    connection.Open();
                    //Create the command for query a database
                    MySqlCommand command = new MySqlCommand(query, connection);

                    //Add the paramters into a command
                    command.AddParameters(query, parameters);

                    //Execute the command
                    MySqlDataAdapter adapter = new MySqlDataAdapter(command);

                    //Fill data to table
                    adapter.Fill(dtTable);

                    //Close connection and dispose resource when we are done
                    connection.Close();
                    command.Dispose();
                    adapter.Dispose();
                }
                catch (Exception ex)
                {
                    DebugLogger.Log(ex.Message);
                }
            }

            //Return result
            return dtTable;
        }

        public object ExecuteScalarQuery(string query, object[] parameters = null)
        {
            object result = 0;

            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                try
                {
                    //Open connection
                    connection.Open();

                    //Create the command for query a database
                    MySqlCommand command = new MySqlCommand(query, connection);

                    //Add the paramters into a command
                    command.AddParameters(query, parameters);

                    //Execute the command
                    result = command.ExecuteScalar();

                    //Close connection and dispose resource when we are done
                    connection.Close();
                    command.Dispose();
                }
                catch (Exception ex)
                {
                    DebugLogger.Log(ex.Message);
                }
            }

            //Return result
            return result;
        }

        public int CreateDatabaseIfNotExists()
        {
            if (string.IsNullOrEmpty(DatabaseName) ||
                string.IsNullOrEmpty(User) ||
                string.IsNullOrEmpty(Password) ||
                string.IsNullOrEmpty(ServerName))
                return 0;

            int result = 0;

            //Create connection string
            string constr = $"server={ServerName};user id={User};" +
                $"password={Password};";

            //Create MySql connection
            MySqlConnection connection = new MySqlConnection(constr);

            try
            {
                //Open connection
                connection.Open();

                //Query create database
                string query = $"create database if not exists {DatabaseName}";

                //Create the command for query a database
                MySqlCommand command = new MySqlCommand(query, connection);

                //Execute the command
                result = command.ExecuteNonQuery();

                //Close connection and dispose resource when we are done
                connection.Close();
                connection.Dispose();
                command.Dispose();

            }
            catch (Exception ex)
            {
                DebugLogger.Log(ex.Message);
            }
            finally
            {
                connection.Dispose();
            }

            return result;
        }

        public int CreateTableIfNotExists(string tableName, string columns)
        {
            if (string.IsNullOrEmpty(tableName))
                return 0;

            int result = 0;

            //Create connection string
            string constr = $"server={ServerName};user id={User};" +
                $"password={Password};database={DatabaseName}";

            //Create MySql connection
            MySqlConnection connection = new MySqlConnection(constr);

            try
            {
                //Open connection
                connection.Open();

                //Query create database
                string query = $"create table if not exists {tableName} {columns}";

                //Create the command for query a database
                MySqlCommand command = new MySqlCommand(query, connection);

                //Execute the command
                result = command.ExecuteNonQuery();

                //Close connection and dispose resource when we are done
                connection.Close();
                connection.Dispose();
                command.Dispose();

            }
            catch (Exception ex)
            {
                DebugLogger.Log(ex.Message);
            }
            finally
            {
                connection.Dispose();
            }

            return result;
        }

        #endregion
    }

    static class MySqlDataAccessExtensions
    {
        public static void AddParameters(this MySqlCommand command, string query, object[] parameters)
        {
            if (parameters == null)
                return;
            string[] splitQuery = query.Split(' ');
            int indexParam = 0;
            foreach (var item in splitQuery)
            {
                if (item.Contains('@'))
                {
                    command.Parameters.AddWithValue(item, parameters[indexParam]);
                    indexParam++;
                }
            }
        }
    }

    static class DebugLogger
    {
        public static void Log(
           string message,
           [CallerMemberName] string origin = "",
           [CallerFilePath] string filePath = "",
           [CallerLineNumber] int lineNumber = 0)
        {
            message = $"{message} [{Path.GetFileName(filePath)} > {origin}() > Line {lineNumber}]";
            Debug.WriteLine(message);
        }
    }
}