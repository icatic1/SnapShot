using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace SnapShot.Model
{
    public class Database
    {
        #region Attributes

        static SqlConnection? connection;

        #endregion

        public static bool Connect()
        {
            try
            {
                string connectionString = @Properties.Resources.connectionString;
                connection = new SqlConnection(connectionString);
                connection.Open();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static void Disconnect()
        {
            if (connection != null)
                connection.Close();
        }

        public static string ReadConfiguration()
        {
            string SQL = "SELECT * FROM JSON_configurations WHERE MAC_address = '" + "AAAAA" + "';"; //TODO Implement GetMacAddress
            SqlCommand command = new SqlCommand(SQL, connection);
            SqlDataReader result = command.ExecuteReader();
            while (result.Read())
            {
                return result["configuration"].ToString() ?? throw new Exception("Configuration not found!");
            }
            return "";
        }
    }
}
