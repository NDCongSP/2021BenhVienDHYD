// Created using LayerGen 3.5

using System;
using System.Collections.Generic;
using System.Data;

namespace ATWebLogger.Core
{
    [Serializable]
    public partial class Alarm : CoreData.AlarmBase
    {
        public enum ConcurrencyOptions
        {
            /// <summary>
            /// Concurrency checking is disabled
            /// </summary>
            Ignore = 0,
            /// <summary>
            /// Concurrency checking is checked and an exception is thrown if the data changed
            /// </summary>
            Strict = 1
		}

        public enum Fields
        {
            Ack,
            DateTime,
            HighLevel,
            Id,
            LocationName,
            LowLevel,
            Type,
            Value
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Alarm"/> class.
        /// </summary>
        public Alarm() : base()
        {
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Alarm"/> class.
        /// </summary>
        /// <param name="connectionString">Sets the connection string to use to connect to the database.</param>
        public Alarm(LayerGenConnectionString connectionString) : base(connectionString)
        {
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Alarm"/> class,
        /// optionally using stored procedures or Sql text.
        /// </summary>
        /// <param name="useStoredProcedures">If true, then all data access will be done using stored procedures. Otherwise, data access will be done using Sql text.</param>
        public Alarm(bool useStoredProcedures) : base(useStoredProcedures)
        {
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Alarm"/> class,
        /// optionally using stored procedures or Sql text.
        /// </summary>
        /// <param name="connectionString">Sets the connection string to use to connect to the database.</param>
        /// <param name="useStoredProcedures">If true, then all data access will be done using stored procedures. Otherwise, data access will be done using Sql text.</param>
        public Alarm(LayerGenConnectionString connectionString, bool useStoredProcedures) : base(connectionString, useStoredProcedures)
        {
            
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="Alarm"/> class, loading a
        /// row from the database that matches the given primary key.
        /// </summary>
        /// <param name="id">The primary key of the row that gets loaded from the database.</param>
        public Alarm(int id) : base(id)
        {
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Alarm"/> class, loading a
        /// row from the database that matches the given primary key.
        /// </summary>
        /// <param name="connectionString">Sets the connection string to use to connect to the database.</param>
        /// <param name="id">The primary key of the row that gets loaded from the database.</param>
        public Alarm(LayerGenConnectionString connectionString, int id) : base(connectionString, id)
        {
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Alarm"/> class, loading a
        /// row from the database that matches the given primary key and optionally
        /// using stored procedures or Sql text.
        /// </summary>
        /// <param name="id">The primary key of the row that gets loaded from the database.</param>
        /// <param name="useStoredProcedures">If true, then all data access will be done using stored procedures. Otherwise, data access will be done using Sql text.</param>
        public Alarm(int id, bool useStoredProcedures) : base(id, useStoredProcedures)
        {
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Alarm"/> class, loading a
        /// row from the database that matches the given primary key and optionally
        /// using stored procedures or Sql text.
        /// </summary>
        /// <param name="connectionString">Sets the connection string to use to connect to the database.</param>
        /// <param name="id">The primary key of the row that gets loaded from the database.</param>
        /// <param name="useStoredProcedures">If true, then all data access will be done using stored procedures. Otherwise, data access will be done using Sql text.</param>
        public Alarm(LayerGenConnectionString connectionString, int id, bool useStoredProcedures) : base(connectionString, id, useStoredProcedures)
        {
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Alarm"/> class, loading a
        /// row from the database that matches the given primary key and loading
        /// only the fields specified.
        /// </summary>
        /// <param name="id">The primary key of the row that gets loaded from the database.</param>
        /// <param name="fields">The list of <see cref="ATWebLogger.Core.Alarm.Fields"/> to pull from the database.</param>
		public Alarm(int id, List<Fields> fields) : base(id, fields)
        {
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Alarm"/> class, loading a
        /// row from the database that matches the given primary key and loading
        /// only the fields specified.
        /// </summary>
        /// <param name="connectionString">Sets the connection string to use to connect to the database.</param>
        /// <param name="id">The primary key of the row that gets loaded from the database.</param>
        /// <param name="fields">The list of <see cref="ATWebLogger.Core.Alarm.Fields"/> to pull from the database.</param>
		public Alarm(LayerGenConnectionString connectionString, int id, List<Fields> fields) : base(connectionString, id, fields)
        {
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Alarm"/> class, loading a
        /// row from the database that matches the given primary key and loading
        /// only the fields specified. You can also specify if you want data access
        /// to be done with stored procedures or Sql text.
        /// </summary>
        /// <param name="id">The primary key of the row that gets loaded from the database.</param>
        /// <param name="fields">The list of <see cref="ATWebLogger.Core.Alarm.Fields"/> to pull from the database.</param>
        /// <param name="useStoredProcedures">If true, then all data access will be done using stored procedures. Otherwise, data access will be done using Sql text.</param>
		public Alarm(int id, List<Fields> fields, bool useStoredProcedures) : base(id, fields, useStoredProcedures)
        {
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Alarm"/> class, loading a
        /// row from the database that matches the given primary key and loading
        /// only the fields specified. You can also specify if you want data access
        /// to be done with stored procedures or Sql text.
        /// </summary>
        /// <param name="connectionString">Sets the connection string to use to connect to the database.</param>
        /// <param name="id">The primary key of the row that gets loaded from the database.</param>
        /// <param name="fields">The list of <see cref="ATWebLogger.Core.Alarm.Fields"/> to pull from the database.</param>
        /// <param name="useStoredProcedures">If true, then all data access will be done using stored procedures. Otherwise, data access will be done using Sql text.</param>
		public Alarm(LayerGenConnectionString connectionString, int id, List<Fields> fields, bool useStoredProcedures) : base(connectionString, id, fields, useStoredProcedures)
        {
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Alarm"/> class, loading a
        /// row from the given DataRow.
        /// </summary>
        /// <param name="dr">The DataRow that contains the data to be loaded into the instance.</param>
        protected internal Alarm(DataRow dr) : base(dr)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Alarm"/> class, loading a
        /// row from the given DataRow.
        /// </summary>
        /// <param name="connectionString">Sets the connection string to use to connect to the database.</param>
        /// <param name="dr">The DataRow that contains the data to be loaded into the instance.</param>
        protected internal Alarm(LayerGenConnectionString connectionString, DataRow dr) : base(connectionString, dr)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Alarm"/> class, loading a
        /// row from the given DataRow. You can also specify if you want data access
        /// to be done with stored procedures or Sql text.
        /// </summary>
        /// <param name="dr">The DataRow that contains the data to be loaded into the instance.</param>
        /// <param name="useStoredProcedures">If true, then all data access will be done using stored procedures. Otherwise, data access will be done using Sql text.</param>
        protected internal Alarm(DataRow dr, bool useStoredProcedures) : base(dr, useStoredProcedures)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Alarm"/> class, loading a
        /// row from the given DataRow. You can also specify if you want data access
        /// to be done with stored procedures or Sql text.
        /// </summary>
        /// <param name="connectionString">Sets the connection string to use to connect to the database.</param>
        /// <param name="dr">The DataRow that contains the data to be loaded into the instance.</param>
        /// <param name="useStoredProcedures">If true, then all data access will be done using stored procedures. Otherwise, data access will be done using Sql text.</param>
        protected internal Alarm(LayerGenConnectionString connectionString, DataRow dr, bool useStoredProcedures) : base(connectionString, dr, useStoredProcedures)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Alarm"/> class, explicitly enabling
        /// or disabling the concurrency option.
        /// </summary>
        /// <param name="concurrency">A <see cref="ATWebLogger.Core.Alarm.ConcurrencyOptions"/> value indicating the level of concurrency.</param>
        public Alarm(ConcurrencyOptions concurrency) : base(concurrency)
        {
            
        }

		/// <summary>
        /// Initializes a new instance of the <see cref="Alarm"/> class, explicitly enabling
        /// or disabling the concurrency option.
        /// </summary>
        /// <param name="connectionString">Sets the connection string to use to connect to the database.</param>
        /// <param name="concurrency">A <see cref="ATWebLogger.Core.Alarm.ConcurrencyOptions"/> value indicating the level of concurrency.</param>
        public Alarm(LayerGenConnectionString connectionString, ConcurrencyOptions concurrency) : base(connectionString, concurrency)
        {
            
        }

		/// <summary>
        /// Initializes a new instance of the <see cref="Alarm"/> class, explicitly enabling
        /// or disabling the concurrency option. You can also specify if you want data access
        /// to be done with stored procedures or Sql text.
        /// </summary>
        /// <param name="concurrency">A <see cref="ATWebLogger.Core.Alarm.ConcurrencyOptions"/> value indicating the level of concurrency.</param>
        /// <param name="useStoredProcedures">If true, then all data access will be done using stored procedures. Otherwise, data access will be done using Sql text.</param>
        public Alarm(ConcurrencyOptions concurrency, bool useStoredProcedures) : base(concurrency, useStoredProcedures)
        {
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Alarm"/> class, explicitly enabling
        /// or disabling the concurrency option. You can also specify if you want data access
        /// to be done with stored procedures or Sql text.
        /// </summary>
        /// <param name="connectionString">Sets the connection string to use to connect to the database.</param>
        /// <param name="concurrency">A <see cref="ATWebLogger.Core.Alarm.ConcurrencyOptions"/> value indicating the level of concurrency.</param>
        /// <param name="useStoredProcedures">If true, then all data access will be done using stored procedures. Otherwise, data access will be done using Sql text.</param>
        public Alarm(LayerGenConnectionString connectionString, ConcurrencyOptions concurrency, bool useStoredProcedures) : base(connectionString, concurrency, useStoredProcedures)
        {
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Alarm"/> class, loading a
        /// row from the database that matches the given primary key and explicitly
        /// enabling or disabling the concurrency option.
        /// </summary>
        /// <param name="id">The primary key of the row that gets loaded from the database.</param>
        /// <param name="concurrency">A <see cref="ATWebLogger.Core.Alarm.ConcurrencyOptions"/> value indicating the level of concurrency.</param>
        public Alarm(int id, ConcurrencyOptions concurrency) : base(id, concurrency)
        {
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Alarm"/> class, loading a
        /// row from the database that matches the given primary key and explicitly
        /// enabling or disabling the concurrency option.
        /// </summary>
        /// <param name="connectionString">Sets the connection string to use to connect to the database.</param>
        /// <param name="id">The primary key of the row that gets loaded from the database.</param>
        /// <param name="concurrency">A <see cref="ATWebLogger.Core.Alarm.ConcurrencyOptions"/> value indicating the level of concurrency.</param>
        public Alarm(LayerGenConnectionString connectionString, int id, ConcurrencyOptions concurrency) : base(connectionString, id, concurrency)
        {
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Alarm"/> class, loading a
        /// row from the database that matches the given primary key and explicitly
        /// enabling or disabling the concurrency option. You can also specify if you want data
        /// access to be done with stored procedures or Sql text.
        /// </summary>
        /// <param name="id">The primary key of the row that gets loaded from the database.</param>
        /// <param name="concurrency">A <see cref="ATWebLogger.Core.Alarm.ConcurrencyOptions"/> value indicating the level of concurrency.</param>
        /// <param name="useStoredProcedures">If true, then all data access will be done using stored procedures. Otherwise, data access will be done using Sql text.</param>
        public Alarm(int id, ConcurrencyOptions concurrency, bool useStoredProcedures) : base(id, concurrency, useStoredProcedures)
        {
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Alarm"/> class, loading a
        /// row from the database that matches the given primary key and explicitly
        /// enabling or disabling the concurrency option. You can also specify if you want data
        /// access to be done with stored procedures or Sql text.
        /// </summary>
        /// <param name="connectionString">Sets the connection string to use to connect to the database.</param>
        /// <param name="id">The primary key of the row that gets loaded from the database.</param>
        /// <param name="concurrency">A <see cref="ATWebLogger.Core.Alarm.ConcurrencyOptions"/> value indicating the level of concurrency.</param>
        /// <param name="useStoredProcedures">If true, then all data access will be done using stored procedures. Otherwise, data access will be done using Sql text.</param>
        public Alarm(LayerGenConnectionString connectionString, int id, ConcurrencyOptions concurrency, bool useStoredProcedures) : base(connectionString, id, concurrency, useStoredProcedures)
        {
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Alarm"/> class, loading a
        /// row from the database that matches the given primary key and loading
        /// only the fields specified and explicitly enabling or disabling the
        /// concurrency option.
        /// </summary>
        /// <param name="id">The primary key of the row that gets loaded from the database.</param>
        /// <param name="fields">The list of <see cref="ATWebLogger.Core.Alarm.Fields"/> to pull from the database.</param>
        /// <param name="concurrency">A <see cref="ATWebLogger.Core.Alarm.ConcurrencyOptions"/> value indicating the level of concurrency.</param>
		public Alarm(int id, List<Fields> fields, ConcurrencyOptions concurrency) : base(id, fields, concurrency)
        {
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Alarm"/> class, loading a
        /// row from the database that matches the given primary key and loading
        /// only the fields specified and explicitly enabling or disabling the
        /// concurrency option.
        /// </summary>
        /// <param name="connectionString">Sets the connection string to use to connect to the database.</param>
        /// <param name="id">The primary key of the row that gets loaded from the database.</param>
        /// <param name="fields">The list of <see cref="ATWebLogger.Core.Alarm.Fields"/> to pull from the database.</param>
        /// <param name="concurrency">A <see cref="ATWebLogger.Core.Alarm.ConcurrencyOptions"/> value indicating the level of concurrency.</param>
		public Alarm(LayerGenConnectionString connectionString, int id, List<Fields> fields, ConcurrencyOptions concurrency) : base(connectionString, id, fields, concurrency)
        {
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Alarm"/> class, loading a
        /// row from the database that matches the given primary key and loading
        /// only the fields specified and explicitly enabling or disabling the
        /// concurrency option. You can also specify if you want data access to be
        /// done with stored procedures or Sql text.
        /// </summary>
        /// <param name="id">The primary key of the row that gets loaded from the database.</param>
        /// <param name="fields">The list of <see cref="ATWebLogger.Core.Alarm.Fields"/> to pull from the database.</param>
        /// <param name="concurrency">A <see cref="ATWebLogger.Core.Alarm.ConcurrencyOptions"/> value indicating the level of concurrency.</param>
        /// <param name="useStoredProcedures">If true, then all data access will be done using stored procedures. Otherwise, data access will be done using Sql text.</param>
		public Alarm(int id, List<Fields> fields, ConcurrencyOptions concurrency, bool useStoredProcedures) : base(id, fields, concurrency, useStoredProcedures)
        {
            
        }

		/// <summary>
        /// Initializes a new instance of the <see cref="Alarm"/> class, loading a
        /// row from the database that matches the given primary key and loading
        /// only the fields specified and explicitly enabling or disabling the
        /// concurrency option. You can also specify if you want data access to be
        /// done with stored procedures or Sql text.
        /// </summary>
        /// <param name="connectionString">Sets the connection string to use to connect to the database.</param>
        /// <param name="id">The primary key of the row that gets loaded from the database.</param>
        /// <param name="fields">The list of <see cref="ATWebLogger.Core.Alarm.Fields"/> to pull from the database.</param>
        /// <param name="concurrency">A <see cref="ATWebLogger.Core.Alarm.ConcurrencyOptions"/> value indicating the level of concurrency.</param>
        /// <param name="useStoredProcedures">If true, then all data access will be done using stored procedures. Otherwise, data access will be done using Sql text.</param>
		public Alarm(LayerGenConnectionString connectionString, int id, List<Fields> fields, ConcurrencyOptions concurrency, bool useStoredProcedures) : base(connectionString, id, fields, concurrency, useStoredProcedures)
        {
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Alarm"/> class, loading a
        /// row from the given DataRow and explicitly enabling or disabling the
        /// concurrency option.
        /// </summary>
        /// <param name="dr">The DataRow that contains the data to be loaded into the instance.</param>
        /// <param name="concurrency">A <see cref="ATWebLogger.Core.Alarm.ConcurrencyOptions"/> value indicating the level of concurrency.</param>
        protected internal Alarm(DataRow dr, ConcurrencyOptions concurrency) : base(dr, concurrency)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Alarm"/> class, loading a
        /// row from the given DataRow and explicitly enabling or disabling the
        /// concurrency option.
        /// </summary>
        /// <param name="connectionString">Sets the connection string to use to connect to the database.</param>
        /// <param name="dr">The DataRow that contains the data to be loaded into the instance.</param>
        /// <param name="concurrency">A <see cref="ATWebLogger.Core.Alarm.ConcurrencyOptions"/> value indicating the level of concurrency.</param>
        protected internal Alarm(LayerGenConnectionString connectionString, DataRow dr, ConcurrencyOptions concurrency) : base(connectionString, dr, concurrency)
        {

        }

		/// <summary>
        /// Initializes a new instance of the <see cref="Alarm"/> class, loading a
        /// row from the given DataRow and explicitly enabling or disabling the
        /// concurrency option. You can also specify if you want data access to be
        /// done with stored procedures or Sql text.
        /// </summary>
        /// <param name="dr">The DataRow that contains the data to be loaded into the instance.</param>
        /// <param name="concurrency">A <see cref="ATWebLogger.Core.Alarm.ConcurrencyOptions"/> value indicating the level of concurrency.</param>
        /// <param name="useStoredProcedures">If true, then all data access will be done using stored procedures. Otherwise, data access will be done using Sql text.</param>
        protected internal Alarm(DataRow dr, ConcurrencyOptions concurrency, bool useStoredProcedures) : base(dr, concurrency, useStoredProcedures)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Alarm"/> class, loading a
        /// row from the given DataRow and explicitly enabling or disabling the
        /// concurrency option. You can also specify if you want data access to be
        /// done with stored procedures or Sql text.
        /// </summary>
        /// <param name="connectionString">Sets the connection string to use to connect to the database.</param>
        /// <param name="dr">The DataRow that contains the data to be loaded into the instance.</param>
        /// <param name="concurrency">A <see cref="ATWebLogger.Core.Alarm.ConcurrencyOptions"/> value indicating the level of concurrency.</param>
        /// <param name="useStoredProcedures">If true, then all data access will be done using stored procedures. Otherwise, data access will be done using Sql text.</param>
        protected internal Alarm(LayerGenConnectionString connectionString, DataRow dr, ConcurrencyOptions concurrency, bool useStoredProcedures) : base(connectionString, dr, concurrency, useStoredProcedures)
        {

        }

        /// <summary>
        /// The name of the table
        /// </summary>
        public static string LgTableName
        {
            get { return LayerGenTableName; }
        }

        /// <summary>
        /// The name of table, delimited with backticks
        /// e.g. "`alarm`" instead of "alarm"
        /// </summary>
        public static string LgTableNameDelimited
        {
            get { return LayerGenTableNameDelimited; }
        }

        protected internal bool LayerGenIsUpdate()
        {
            return _layerGenIsUpdate;
        }

        protected internal string LayerGenConnectionString()
        {
            return _connectionString;
        }

        /// <summary>
        /// The name of the primary key in the table
        /// </summary>
        public static string LgPrimaryKeyName
        {
            get { return LayerGenPrimaryKey; }
        }

        /// <summary>
        /// Creates an instance of Alarm from a JSON string
        /// </summary>
        /// <param name="json">The JSON string</param>
        /// <returns>A Alarm object instance</returns>
        public static Alarm FromJson(string json)
        {
            return JsonToAlarm(json);
        }

        /// <summary>
        /// Creates an instance of Alarm from an XML string
        /// </summary>
        /// <param name="xml">The XML string</param>
        /// <returns>A Alarm object instance</returns>
        public static Alarm FromXml(string xml)
        {
            return XmlToAlarm(xml);
        }

        /// <summary>
        /// Creates an instance of Alarm from a base64 encoded BSON string
        /// </summary>
        /// <param name="bson">The base64 encoded BSON string</param>
        /// <returns>A Alarm object instance</returns>
        public static Alarm FromBson(string bson)
        {
            return BsonToAlarm(bson);
        }

    }

    /// <summary>
    /// Represents a collection of <see cref="Alarm"/> objects.
    /// </summary>
    [Serializable]
    public class Alarms : List<Alarm>
    {
        private readonly bool _useStoredProcedures;
        private string _connectionString;
        private readonly Alarm.ConcurrencyOptions _concurrency;

        public Alarms(Alarm.ConcurrencyOptions concurrency)
        {
            _concurrency = concurrency;
            _useStoredProcedures = false;
            _connectionString = CoreData.Universal.GetConnectionString();
		}

        public Alarms(LayerGenConnectionString connectionString, Alarm.ConcurrencyOptions concurrency)
        {
            _concurrency = concurrency;
            _useStoredProcedures = false;
            _connectionString = connectionString.ConnectionString;
		}

        public Alarms(Alarm.ConcurrencyOptions concurrency, bool useStoredProcedures)
        {
            _useStoredProcedures = useStoredProcedures;
            _concurrency = concurrency;
            _connectionString = CoreData.Universal.GetConnectionString();
		}

        public Alarms(LayerGenConnectionString connectionString, Alarm.ConcurrencyOptions concurrency, bool useStoredProcedures)
        {
            _useStoredProcedures = useStoredProcedures;
            _concurrency = concurrency;
            _connectionString = connectionString.ConnectionString;
		}
		
        public Alarms()
        {
            _concurrency = Alarm.ConcurrencyOptions.Ignore;
            _useStoredProcedures = false;
            _connectionString = CoreData.Universal.GetConnectionString();
		}

        public Alarms(LayerGenConnectionString connectionString)
        {
            _concurrency = Alarm.ConcurrencyOptions.Ignore;
            _useStoredProcedures = false;
            _connectionString = connectionString.ConnectionString;
		}

        public Alarms(bool useStoredProcedures)
        {
            _concurrency = Alarm.ConcurrencyOptions.Ignore;
            _useStoredProcedures = useStoredProcedures;
            _connectionString = CoreData.Universal.GetConnectionString();
		}

        public Alarms(LayerGenConnectionString connectionString, bool useStoredProcedures)
        {
            _concurrency = Alarm.ConcurrencyOptions.Ignore;
            _useStoredProcedures = useStoredProcedures;
            _connectionString = connectionString.ConnectionString;
		}

        protected void Load(DataRowCollection dataRows)
        {
            LayerGenConnectionString connectString = new LayerGenConnectionString();
            connectString.ConnectionString = _connectionString;

            Clear();
            foreach (DataRow dr in dataRows)
            {
                Add(new Alarm(connectString, dr, _concurrency, _useStoredProcedures));
            }
        }


        /// <summary>
        /// Retrieves rows from the Alarm table by executing the given stored procedure.
        /// </summary>
        /// <param name="procedureName">The name of the stored procedure to execute.</param>
        /// <param name="procedureParams">A dictionary of parameter/value pairs. This can be null if there are no parameters.</param>
        public void GetByStoredProcedure(string procedureName, Dictionary<string, object> procedureParams)
        {
            LayerGenConnectionString connectString = new LayerGenConnectionString();
            connectString.ConnectionString = _connectionString;

            DataTable dt = CoreData.AlarmBase.GetByStoredProcedure(connectString, procedureName, procedureParams);
            if (dt != null)
            {
                Load(dt.Rows);
            }
        }

        /// <summary>
        /// Retrieves rows from the Alarm table by executing the given stored procedure.
        /// </summary>
        /// <param name="procedureName">The name of the stored procedure to execute.</param>
        public void GetByStoredProcedure(string procedureName)
        {
            LayerGenConnectionString connectString = new LayerGenConnectionString();
            connectString.ConnectionString = _connectionString;

            DataTable dt = CoreData.AlarmBase.GetByStoredProcedure(connectString, procedureName, null);
            if (dt != null)
            {
                Load(dt.Rows);
            }
        }

        /// <summary>
        /// Retrieves rows from the Alarm table, based on the given SQL statement.
        /// </summary>
        /// <param name="sql">The SQL statement to execute.</param>
        /// <param name="sqlParams">Optional <see cref="System.String.Format"/> like parameters</param>
        public void GetBySqlStatement(string sql, params object[] sqlParams)
        {
            LayerGenConnectionString connectString = new LayerGenConnectionString();
            connectString.ConnectionString = _connectionString;

            DataTable dt = CoreData.AlarmBase.GetBySqlStatement(connectString, sql, sqlParams);
            if (dt != null)
            {
                Load(dt.Rows);
            }
        }

        /// <summary>
        /// Retrieves all the rows from the Alarm table.
        /// </summary>
        public void GetAll()
        {
		    GetAll(_useStoredProcedures);
        }

        /// <summary>
        /// Retrieves all the rows from the Alarm table.
        /// </summary>
        /// <param name="useStoredProcedures">If true, then all data access will be done using stored procedures. Otherwise, data access will be done using Sql text</param>
        private void GetAll(bool useStoredProcedures)
        {
            LayerGenConnectionString connectString = new LayerGenConnectionString();
            connectString.ConnectionString = _connectionString;

            DataTable dt = CoreData.AlarmBase.GetAll(connectString, useStoredProcedures);
            if (dt != null)
            {
                Load(dt.Rows);
            }
        }
        /// <summary>
        /// Creates an instance of Alarms from a base64 encoded BSON string
        /// </summary>
        /// <param name="bson">The base64 encoded BSON string</param>
        /// <returns>A Alarms object instance</returns>
        public static Alarms FromBson(string bson)
        {
            List<CoreData.AlarmBase.SerializableAlarm> zc;
            byte[] data = Convert.FromBase64String(bson);
            Alarms tmp = new Alarms();

            using (System.IO.MemoryStream ms = new System.IO.MemoryStream(data))
            {
                using (Newtonsoft.Json.Bson.BsonReader reader = new Newtonsoft.Json.Bson.BsonReader(ms))
                {
                    reader.ReadRootValueAsArray = true;
                    Newtonsoft.Json.JsonSerializer serializer = new Newtonsoft.Json.JsonSerializer();
                    zc = serializer.Deserialize<List<CoreData.AlarmBase.SerializableAlarm>>(reader);
                }
            }

            foreach (CoreData.AlarmBase.SerializableAlarm z in zc)
            {
                tmp.Add(Alarm.FromJson(Newtonsoft.Json.JsonConvert.SerializeObject(z)));
            }

            if (zc.Count > 0)
            {
                Encryption64 decryptor = new Encryption64();
                tmp._connectionString = decryptor.Decrypt(zc[0].SerializationConnectionString, CoreData.Universal.LayerGenEncryptionKey);
            }

            return tmp;
        }

        /// <summary>
        /// Creates an instance of Alarms from an XML string
        /// </summary>
        /// <param name="xml">The XML string</param>
        /// <returns>A Alarms object instance</returns>
        public static Alarms FromXml(string xml)
        {
            System.Xml.Serialization.XmlSerializer xType = new System.Xml.Serialization.XmlSerializer(typeof(List<CoreData.AlarmBase.SerializableAlarm>));
            List<CoreData.AlarmBase.SerializableAlarm> zc;
            Alarms tmp = new Alarms();

            using (System.IO.StringReader sr = new System.IO.StringReader(xml))
            {
                zc = (List<CoreData.AlarmBase.SerializableAlarm>)xType.Deserialize(sr);
            }

            foreach (CoreData.AlarmBase.SerializableAlarm z in zc)
            {
                tmp.Add(Alarm.FromJson(Newtonsoft.Json.JsonConvert.SerializeObject(z)));
            }

            if (zc.Count > 0)
            {
                Encryption64 decryptor = new Encryption64();
                tmp._connectionString = decryptor.Decrypt(zc[0].SerializationConnectionString, CoreData.Universal.LayerGenEncryptionKey);
            }

            return tmp;
        }

        /// <summary>
        /// Creates an instance of Alarms from a JSON string
        /// </summary>
        /// <param name="json">The JSON string</param>
        /// <returns>A Alarms object instance</returns>
        public static Alarms FromJson(string json)
        {
            List<CoreData.AlarmBase.SerializableAlarm> zs = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CoreData.AlarmBase.SerializableAlarm>>(json);
            Alarms tmp = new Alarms();

            foreach (CoreData.AlarmBase.SerializableAlarm z in zs)
            {
                tmp.Add(Alarm.FromJson(Newtonsoft.Json.JsonConvert.SerializeObject(z)));
            }

            if (zs.Count > 0)
            {
                Encryption64 decryptor = new Encryption64();
                tmp._connectionString = decryptor.Decrypt(zs[0].SerializationConnectionString, CoreData.Universal.LayerGenEncryptionKey);
            }

            return tmp;
        }

        /// <summary>
        /// Converts an instance of an object to a string format
        /// </summary>
        /// <param name="format">Specifies if it should convert to XML, BSON or JSON</param>
        /// <returns>The object, converted to a string representation</returns>
        public string ToString(SerializationFormats format)
        {
            List<CoreData.AlarmBase.SerializableAlarm> zs = new List<CoreData.AlarmBase.SerializableAlarm>();
            foreach (Alarm z in this)
            {
                CoreData.AlarmBase.SerializableAlarm serializableAlarm = new CoreData.AlarmBase.SerializableAlarm();
                serializableAlarm.Id = z.IsNull(Alarm.Fields.Id)
                    ? (int?) null : z.Id;
                serializableAlarm.Ack = z.IsNull(Alarm.Fields.Ack)
                    ? null : z.Ack;
                serializableAlarm.DateTime = z.IsNull(Alarm.Fields.DateTime)
                    ? (DateTime?) null : z.DateTime;
                serializableAlarm.HighLevel = z.IsNull(Alarm.Fields.HighLevel)
                    ? (double?) null : z.HighLevel;
                serializableAlarm.LocationName = z.IsNull(Alarm.Fields.LocationName)
                    ? null : z.LocationName;
                serializableAlarm.LowLevel = z.IsNull(Alarm.Fields.LowLevel)
                    ? (double?) null : z.LowLevel;
                serializableAlarm.Type = z.IsNull(Alarm.Fields.Type)
                    ? null : z.Type;
                serializableAlarm.Value = z.IsNull(Alarm.Fields.Value)
                    ? (double?) null : z.Value;
                serializableAlarm.SerializationIsUpdate = z.LayerGenIsUpdate();
                serializableAlarm.SerializationConnectionString = z.LayerGenConnectionString();
                zs.Add(serializableAlarm);
            }
            
            if (format == SerializationFormats.Json)
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(zs);
            }

            if (format == SerializationFormats.Xml)
            {
                System.Xml.Serialization.XmlSerializer xType = new System.Xml.Serialization.XmlSerializer(zs.GetType());

                using (System.IO.StringWriter sw = new System.IO.StringWriter())
                {
                    xType.Serialize(sw, zs);
                    return sw.ToString();
                }
            }

            if (format == SerializationFormats.BsonBase64)
            {
                using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                {
                    using (Newtonsoft.Json.Bson.BsonWriter writer = new Newtonsoft.Json.Bson.BsonWriter(ms))
                    {
                        Newtonsoft.Json.JsonSerializer serializer = new Newtonsoft.Json.JsonSerializer();
                        serializer.Serialize(writer, zs);
                    }
                    return Convert.ToBase64String(ms.ToArray());
                }
            }

            return "";
        }

    }
}

