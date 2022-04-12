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

        #region Methods

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

        public static string? GetMACAddress()
        {
            return (from nic in NetworkInterface.GetAllNetworkInterfaces()
                    where nic.OperationalStatus == OperationalStatus.Up
                    select nic.GetPhysicalAddress().ToString()
                    ).FirstOrDefault();
        }

        public static bool CheckLicence()
        {
            string SQL = "SELECT * FROM licences WHERE MAC_address = '" + GetMACAddress() + "' AND licenced = 1;";
            SqlCommand command = new SqlCommand(SQL, connection);
            SqlDataReader result = command.ExecuteReader();

            return result.HasRows;
        }

        public static void WriteConfiguration(string configuration)
        {
            string SQL = "DELETE FROM JSON_configurations WHERE MAC_address = '" + GetMACAddress() + "';";
            SqlCommand command = new SqlCommand(SQL, connection);
            command.ExecuteNonQuery();

            command.CommandText = "INSERT INTO JSON_configurations (MAC_address, configuration) VALUES ('" + GetMACAddress() + "', @configuration);";
            SqlParameter param = new SqlParameter("@configuration", System.Data.SqlDbType.Text, configuration.Length);
            param.Value = configuration;
            command.Parameters.Add(param);

            command.Prepare();
            command.ExecuteNonQuery();
        }

        public static string ReadConfiguration()
        {
            string SQL = "SELECT * FROM JSON_configurations WHERE MAC_address = '" + GetMACAddress() + "';";
            SqlCommand command = new SqlCommand(SQL, connection);
            SqlDataReader result = command.ExecuteReader();
            while (result.Read())
            {
                return result["configuration"].ToString() ?? throw new Exception("Configuration not found!");
            }
            return "";
        }

        #endregion
    }
}
