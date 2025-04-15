using MySql.Data.MySqlClient;

namespace mindquest
{
    public class DBHelper
    {
        private static string connectionString = "server=localhost;user=root;password=12345678;database=mindquest;";
        public static MySqlConnection GetConnection()
        {
            return new MySqlConnection(connectionString);
        }
    }
} 