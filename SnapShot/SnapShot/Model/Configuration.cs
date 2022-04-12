using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using SnapShot.Model;

namespace SnapShot
{
    public class Configuration
    {
        #region Attributes

        // device configuration
        DeviceType type;
        string id = "";
        string triggerFilePath = "",
               outputFolderPath = "";
        int outputValidity,
            cameraNumber;

        // video configuration
        Resolution resolution;
        int contrastLevel;
        Color imageColor = SystemColors.Control;
        bool motionDetection = false;

        // network configuration - not a part of this sprint
        string serverVersion = "",
               serverIP = "";
        int serverPort;
        bool connectionStatus = false;

        // capture configuration
        bool imageCapture = true,
             singleMode = true;
        int duration,
            period;

        #endregion

        #region Properties

        public DeviceType Type { get => type; set => type = value; }
        public string Id { get => id; set => id = value; }
        public string TriggerFilePath { get => triggerFilePath; set => triggerFilePath = value; }
        public string OutputFolderPath { get => outputFolderPath; set => outputFolderPath = value; }
        public int OutputValidity { get => outputValidity; set => outputValidity = value; }
        public int CameraNumber { get => cameraNumber; set => cameraNumber = value; }
        public Resolution Resolution { get => resolution; set => resolution = value; }
        public int ContrastLevel { get => contrastLevel; set => contrastLevel = value; }
        public Color ImageColor { get => imageColor; set => imageColor = value; }
        public bool MotionDetection { get => motionDetection; set => motionDetection = value; }
        public string ServerVersion { get => serverVersion; set => serverVersion = value; }
        public string ServerIP { get => serverIP; set => serverIP = value; }
        public int ServerPort { get => serverPort; set => serverPort = value; }
        public bool ConnectionStatus { get => connectionStatus; set => connectionStatus = value; }
        public bool ImageCapture { get => imageCapture; set => imageCapture = value; }
        public bool SingleMode { get => singleMode; set => singleMode = value; }
        public int Duration { get => duration; set => duration = value; }
        public int Period { get => period; set => period = value; }

        #endregion

        #region Methods

        public static bool ExportToJSON(string path, int mode)
        {
            string EXPORT = "";
            EXPORT += "{\n";
            EXPORT += "\t\"cameras\":\n";
            EXPORT += "\t[\n";
            int i = 0;
            foreach (var config in Program.Snapshot.Camera)
            {
                EXPORT += "\t\t{\n";

                EXPORT += "\t\t\t\"device_configuration\":\n";
                EXPORT += "\t\t\t{\n";
                EXPORT += "\t\t\t\t\"device_type\": \"" + config.Type + "\",\n";
                EXPORT += "\t\t\t\t\"id\": \"" + config.Id + "\",\n";
                EXPORT += "\t\t\t\t\"trigger_file_path\": \"" + config.TriggerFilePath + "\",\n";
                EXPORT += "\t\t\t\t\"output_folder_path\": \"" + config.OutputFolderPath + "\",\n";
                EXPORT += "\t\t\t\t\"output_validity_days\": \"" + config.OutputValidity + "\",\n";
                EXPORT += "\t\t\t\t\"camera_number\": \"" + config.CameraNumber + "\"\n";
                EXPORT += "\t\t\t},\n";

                EXPORT += "\t\t\t\"video_configuration\":\n";
                EXPORT += "\t\t\t{\n";
                EXPORT += "\t\t\t\t\"resolution\": \"" + config.Resolution + "\",\n";
                EXPORT += "\t\t\t\t\"contrast_level\": \"" + config.ContrastLevel + "\",\n";
                EXPORT += "\t\t\t\t\"image_color\": \"" + config.ImageColor.Name.ToString() + "\",\n";
                EXPORT += "\t\t\t\t\"motion_detection\": \"" + config.MotionDetection + "\"\n";
                EXPORT += "\t\t\t},\n";

                EXPORT += "\t\t\t\"network_configuration\":\n";
                EXPORT += "\t\t\t{\n";
                EXPORT += "\t\t\t\t\"server_version\": \"" + config.ServerVersion + "\",\n";
                EXPORT += "\t\t\t\t\"server_IP_address\": \"" + config.ServerIP + "\",\n";
                EXPORT += "\t\t\t\t\"server_port\": \"" + config.ServerPort + "\",\n";
                EXPORT += "\t\t\t\t\"connection_status\": \"" + config.ConnectionStatus + "\"\n";
                EXPORT += "\t\t\t},\n";

                EXPORT += "\t\t\t\"capture_configuration\":\n";
                EXPORT += "\t\t\t{\n";
                EXPORT += "\t\t\t\t\"image_capture\": \"" + config.ImageCapture + "\",\n";
                EXPORT += "\t\t\t\t\"single_mode\": \"" + config.SingleMode + "\",\n";
                EXPORT += "\t\t\t\t\"duration\": \"" + config.Duration + "\",\n";
                EXPORT += "\t\t\t\t\"burst_period\": \"" + config.Period + "\"\n";
                EXPORT += "\t\t\t}\n";

                EXPORT += "\t\t}";
                if (i < 2)
                    EXPORT += ",";
                EXPORT += "\n";
                i++;
            }
            EXPORT += "\t]\n";
            EXPORT += "}";

            try
            {
                // local export
                if (mode == 1)
                    File.WriteAllText(path, EXPORT);

                // export to server
                else if (mode == 2)
                {
                    Database.Connect();
                    Database.WriteConfiguration(EXPORT);
                    Database.Disconnect();
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool ImportFromJSON(string path, int mode)
        {
            try
            {
                string IMPORT = "";

                // local import
                if (mode == 1)
                    IMPORT = File.ReadAllText(path);

                // import from server
                else if (mode == 2)
                {
                    Database.Connect();
                    IMPORT = Database.ReadConfiguration();
                    Database.Disconnect();
                }

                string[] rows = IMPORT.Split('\n');
                Snapshot newSnapshot = new Snapshot();
                int camera = 0;
                int i = 4;
                while (camera < 3)
                {
                    Configuration config = new Configuration();
                    if (rows[i].Contains("device_configuration"))
                    {
                        i += 2;

                        string[] device_type = rows[i].Split(" ");
                        device_type[1] = device_type[1].Replace("\"", "").Replace(",", "");
                        i++;

                        string[] id = rows[i].Split(" ");
                        id[1] = id[1].Replace("\"", "").Replace(",", "");
                        i++;

                        string[] trigger_file_path = rows[i].Split(" ");
                        trigger_file_path[1] = trigger_file_path[1].Replace("\"", "").Replace(",", "");
                        i++;

                        string[] output_folder_path = rows[i].Split(" ");
                        output_folder_path[1] = output_folder_path[1].Replace("\"", "").Replace(",", "");
                        i++;

                        string[] output_validity_days = rows[i].Split(" ");
                        output_validity_days[1] = output_validity_days[1].Replace("\"", "").Replace(",", "");
                        i++;

                        string[] camera_number = rows[i].Split(" ");
                        camera_number[1] = camera_number[1].Replace("\"", "").Replace(",", "");

                        config.Type = (DeviceType)Enum.Parse(typeof(DeviceType), device_type[1]);
                        config.Id = id[1];
                        config.TriggerFilePath = trigger_file_path[1];
                        config.OutputFolderPath = output_folder_path[1];
                        config.OutputValidity = Int32.Parse(output_validity_days[1]);
                        config.CameraNumber = Int32.Parse(camera_number[1]);
                    }

                    i += 2;
                    if (rows[i].Contains("video_configuration"))
                    {
                        i += 2;

                        string[] resolution = rows[i].Split(" ");
                        resolution[1] = resolution[1].Replace("\"", "").Replace(",", "");
                        i++;

                        string[] contrast_level = rows[i].Split(" ");
                        contrast_level[1] = contrast_level[1].Replace("\"", "").Replace(",", "");
                        i++;

                        string[] image_color = rows[i].Split(" ");
                        image_color[1] = image_color[1].Replace("\"", "").Replace(",", "");
                        i++;

                        string[] motion_detection = rows[i].Split(" ");
                        motion_detection[1] = motion_detection[1].Replace("\"", "").Replace(",", "");

                        config.Resolution = (Resolution)Enum.Parse(typeof(Resolution), resolution[1]);
                        config.ContrastLevel = Int32.Parse(contrast_level[1]);
                        config.ImageColor = Color.FromName(image_color[1]);
                        config.MotionDetection = Convert.ToBoolean(motion_detection[1]);
                    }

                    i += 2;
                    if (rows[i].Contains("network_configuration"))
                    {
                        i += 2;

                        string[] server_version = rows[i].Split(" ");
                        server_version[1] = server_version[1].Replace("\"", "").Replace(",", "");
                        i++;

                        string[] server_IP_address = rows[i].Split(" ");
                        server_IP_address[1] = server_IP_address[1].Replace("\"", "").Replace(",", "");
                        i++;

                        string[] server_port = rows[i].Split(" ");
                        server_port[1] = server_port[1].Replace("\"", "").Replace(",", "");
                        i++;

                        string[] connection_status = rows[i].Split(" ");
                        connection_status[1] = connection_status[1].Replace("\"", "").Replace(",", "");

                        config.ServerVersion = server_version[1];
                        config.ServerIP = server_IP_address[1];
                        config.ServerPort = Int32.Parse(server_port[1]);
                        config.ConnectionStatus = Convert.ToBoolean(connection_status[1]);
                    }

                    i += 2;
                    if (rows[i].Contains("capture_configuration"))
                    {
                        i += 2;

                        string[] image_capture = rows[i].Split(" ");
                        image_capture[1] = image_capture[1].Replace("\"", "").Replace(",", "");
                        i++;

                        string[] single_mode = rows[i].Split(" ");
                        single_mode[1] = single_mode[1].Replace("\"", "").Replace(",", "");
                        i++;

                        string[] duration = rows[i].Split(" ");
                        duration[1] = duration[1].Replace("\"", "").Replace(",", "");
                        i++;

                        string[] burst_period = rows[i].Split(" ");
                        burst_period[1] = burst_period[1].Replace("\"", "").Replace(",", "");

                        config.ImageCapture = Convert.ToBoolean(image_capture[1]);
                        config.SingleMode = Convert.ToBoolean(single_mode[1]);
                        config.Duration = Int32.Parse(duration[1]);
                        config.Period = Int32.Parse(burst_period[1]);
                    }

                    i += 4;
                    newSnapshot.Camera[camera] = config;
                    camera++;
                }
                newSnapshot.Connected = Program.Snapshot.Connected;
                Program.Snapshot = newSnapshot;
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion
    }
}
