using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HashSumOfFiles
{
    /*
        Подключение к БД
    */
    public class DBUtils
    {
        private static string connString = "Data Source=localhost:1521/xe;User Id={0};Password={1};";
        private static string user = "hash";
        private static string password = "1234";
        private static OracleConnection connection = null;

        static DBUtils()
        {
            connString = string.Format(connString, user, password);
        }

        public static OracleConnection GetConnection()
        {            
            OracleConnection conn = new OracleConnection();
            conn.ConnectionString = connString;
            return conn;
        }        
    }
}
