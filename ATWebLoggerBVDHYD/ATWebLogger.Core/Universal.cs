using System;
namespace CoreData
{
    public static class Universal
    {
        /// <summary>
        /// The encryption key that LayerGen uses internally to encrypt data.
        /// </summary>
        public static string LayerGenEncryptionKey = "L@y3rG3n";

        /// <summary>
        /// Gets the connection string to connect to the database
        /// </summary>
        /// <returns>A string containing the connection string</returns>
        public static string GetConnectionString()
        {
            // If this is an ASP.NET application, you can use a line like the following to pull
            // the connection string from the Web.Config:
            // return System.Configuration.ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;

            return ConnectionString;
        }

        internal static string ConnectionString { get; set; }

        public static string AlarmTableName { get; set; } = "alarm";
        public static string DataTableName { get; set; } = "data";
        public static string LocationTableName { get; set; } = "location";
    }
}

namespace ATWebLogger.Core
{

    /// <summary>
    /// Enumeration of various serialization formats
    /// </summary>
    public enum SerializationFormats
    {
        /// <summary>
        /// JSON format
        /// </summary>
        Json = 1,
        /// <summary>
        /// XML format
        /// </summary>
        Xml = 2,
        /// <summary>
        /// Base 64 encoded BSON format
        /// </summary>
        BsonBase64 = 3
    }

    /// <summary>
    /// The exception that is thrown when data in memory is out of sync with data in the database.
    /// </summary>
    public class OutOfSyncException : System.Exception
    {
        public OutOfSyncException()
        {
        }

        public OutOfSyncException(string message)
            : base(message)
        {
        }

        public OutOfSyncException(string message, System.Exception inner)
            : base(message, inner)
        {
        }
    }

    /// <summary>
    /// The exception that is thrown when data in memory is in a read only state.
    /// </summary>
    public class ReadOnlyException : System.Exception
    {
        public ReadOnlyException()
        {
        }

        public ReadOnlyException(string message)
            : base(message)
        {
        }

        public ReadOnlyException(string message, System.Exception inner)
            : base(message, inner)
        {
        }
    }

    /// <summary>
    /// The exception that is thrown when trying to read from a row that doesn't exist.
    /// </summary>
    public class RowNotFoundException : System.Exception
    {
        public RowNotFoundException()
        {
        }

        public RowNotFoundException(string message)
            : base(message)
        {
        }

        public RowNotFoundException(string message, System.Exception inner)
            : base(message, inner)
        {
        }
    }

    public class LayerGenConnectionString
    {
        private string _connectionString;

        public string ConnectionString
        {
            get
            {
                return _connectionString;
            }
            set
            {
                _connectionString = value;
            }
        }
    }

    public class Encryption64
    {
        private byte[] _key = {};
        private readonly byte[] _iv = {65, 108, 97, 110, 32, 66, 46, 9};

        public string Encrypt(string stringToEncrypt, string encryptionKey)
        {
            try
            {
                _key = System.Text.Encoding.UTF8.GetBytes(encryptionKey.Substring(0, 8));
                using (System.Security.Cryptography.DESCryptoServiceProvider des = new System.Security.Cryptography.DESCryptoServiceProvider())
                {
                    byte[] inputByteArray = System.Text.Encoding.UTF8.GetBytes(stringToEncrypt);
                    using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                    {
                        using (System.Security.Cryptography.CryptoStream cs = new System.Security.Cryptography.CryptoStream(ms, des.CreateEncryptor(_key, _iv), System.Security.Cryptography.CryptoStreamMode.Write))
                        {
                            cs.Write(inputByteArray, 0, inputByteArray.Length);
                            cs.FlushFinalBlock();
                            return Convert.ToBase64String(ms.ToArray());
                        }
                    }
                }
            } catch
            {
                return "";
            }
        }

        public string Decrypt(string stringToDecrypt, string encryptionKey)
        {
            try
            {
                _key = System.Text.Encoding.UTF8.GetBytes(encryptionKey.Substring(0, 8));
                using (System.Security.Cryptography.DESCryptoServiceProvider des = new System.Security.Cryptography.DESCryptoServiceProvider())
                {
                    byte[] inputByteArray = Convert.FromBase64String(stringToDecrypt);
                    using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                    {
                        using (System.Security.Cryptography.CryptoStream cs = new System.Security.Cryptography.CryptoStream(ms, des.CreateDecryptor(_key, _iv), System.Security.Cryptography.CryptoStreamMode.Write))
                        {
                            cs.Write(inputByteArray, 0, inputByteArray.Length);
                            cs.FlushFinalBlock();
                            System.Text.Encoding encoding = System.Text.Encoding.UTF8;
                            return encoding.GetString(ms.ToArray());
                        }
                    }
                }
            } catch
            {
                return "";
            }
        }
    }
}
