using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using System.Net.NetworkInformation;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace SnapShot
{
    public class Configuration
    {
        #region Attributes

        // device configuration
        DeviceType type;
        string id = "";
        string triggerFilePath = "",
               regex = "",
               outputFolderPath = "";
        int outputValidity,
            cameraNumber;

        // video configuration
        Resolution resolution;
        int contrastLevel;
        Color imageColor = SystemColors.Control;
        bool motionDetection = false;

        // network configuration - not a part of this sprint
        string serverIP = "",
               mediaPath = "",
               JSONconfigurationPath = "";
        int serverPort;
        bool connectionStatus = false;
        int synchronizationPeriod = 0,
            latestSynchronizationTicks = 0;

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
        
        public string Regex { get => regex; set => regex = value; }
        
        public string OutputFolderPath { get => outputFolderPath; set => outputFolderPath = value; }
        
        public int OutputValidity { get => outputValidity; set => outputValidity = value; }
        
        public int CameraNumber { get => cameraNumber; set => cameraNumber = value; }
        
        public Resolution Resolution { get => resolution; set => resolution = value; }
        
        public int ContrastLevel { get => contrastLevel; set => contrastLevel = value; }
        
        public Color ImageColor { get => imageColor; set => imageColor = value; }
        
        public bool MotionDetection { get => motionDetection; set => motionDetection = value; }
        
        public string ServerIP { get => serverIP; set => serverIP = value; }

        public string MediaPath { get => mediaPath; set => mediaPath = value; }

        public string JSONConfigPath { get => JSONconfigurationPath; set => JSONconfigurationPath = value; }

        public int ServerPort { get => serverPort; set => serverPort = value; }

        public int SynchronizationPeriod { get => synchronizationPeriod; set => synchronizationPeriod = value; }
        
        public int LatestSynchronizationTicks { get => latestSynchronizationTicks; set => latestSynchronizationTicks = value; }

        public bool ConnectionStatus { get => connectionStatus; set => connectionStatus = value; }
        
        public bool ImageCapture { get => imageCapture; set => imageCapture = value; }
        
        public bool SingleMode { get => singleMode; set => singleMode = value; }
        
        public int Duration { get => duration; set => duration = value; }
        
        public int Period { get => period; set => period = value; }

        #endregion

        #region Methods

        public static string? GetMACAddress()
        {
            return (from nic in NetworkInterface.GetAllNetworkInterfaces()
                    where nic.OperationalStatus == OperationalStatus.Up
                    select nic.GetPhysicalAddress().ToString()
                    ).FirstOrDefault();
        }

        #region Export JSON

        public static string CreateJSON()
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
                EXPORT += "\t\t\t\t\"regex\": \"" + config.Regex + "\", \n";
                EXPORT += "\t\t\t\t\"output_folder_path\": \"" + config.OutputFolderPath + "\",\n";
                EXPORT += "\t\t\t\t\"output_validity_days\": \"" + config.OutputValidity + "\",\n";
                EXPORT += "\t\t\t\t\"camera_number\": \"" + config.CameraNumber + "\"\n";
                EXPORT += "\t\t\t},\n";

                EXPORT += "\t\t\t\"video_configuration\":\n";
                EXPORT += "\t\t\t{\n";
                EXPORT += "\t\t\t\t\"resolution\": \"" + config.Resolution + "\",\n";
                EXPORT += "\t\t\t\t\"contrast_level\": \"" + config.ContrastLevel + "\",\n";
                EXPORT += "\t\t\t\t\"image_color\": \"" + config.ImageColor.ToArgb() + "\",\n";
                EXPORT += "\t\t\t\t\"motion_detection\": \"" + config.MotionDetection + "\"\n";
                EXPORT += "\t\t\t},\n";

                EXPORT += "\t\t\t\"network_configuration\":\n";
                EXPORT += "\t\t\t{\n";
                EXPORT += "\t\t\t\t\"server_IP_address\": \"" + config.ServerIP + "\",\n";
                EXPORT += "\t\t\t\t\"server_media_path\": \"" + config.MediaPath + "\",\n";
                EXPORT += "\t\t\t\t\"server_JSON_configuration_path\": \"" + config.JSONConfigPath + "\",\n";
                EXPORT += "\t\t\t\t\"server_port\": \"" + config.ServerPort + "\",\n";
                EXPORT += "\t\t\t\t\"synchronization_period\": \"" + config.SynchronizationPeriod + "\",\n";
                EXPORT += "\t\t\t\t\"latest_synchronization_ticks\": \"" + config.LatestSynchronizationTicks + "\",\n";
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

            return EXPORT;
        }

        public static bool ExportToJSON(string path)
        {
            string EXPORT = CreateJSON();
            try
            {
                // local export
                if (!path.StartsWith("http"))
                {
                    File.WriteAllText(path, EXPORT);
                    return true;
                }

                // export to server
                else
                {
                    // create JSON file for upload
                    File.WriteAllText("configuration.json", EXPORT);

                    // upload file to specified server
                    bool result = UploadFile(path, "configuration.json", "");
                    return result;
                }
            }
            catch
            {
                return false;
            }
        }

        public static bool UploadFile(string endpointUrl, string filePath, string nativeFolder)
        {
            FileStream fs;
            Stream rs;
            try
            {
                string uploadFileName = Path.GetFileName(filePath);
                string path = filePath.Replace(uploadFileName, "").TrimEnd('\\');

                if (nativeFolder.Length > 0)
                    fs = new FileStream(nativeFolder + "\\" + filePath, FileMode.Open, FileAccess.Read);
                else
                    fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);

                var request = (HttpWebRequest)WebRequest.Create(endpointUrl);
                request.Method = WebRequestMethods.Http.Post;
                request.AllowWriteStreamBuffering = false;
                request.SendChunked = true;
                String CRLF = "\r\n";        
                long timestamp = DateTimeOffset.Now.ToUnixTimeSeconds();

                string boundary = timestamp.ToString("x");
                request.ContentType = "multipart/form-data; boundary=" + boundary;

                long bytesAvailable = fs.Length;
                long maxBufferSize = 1 * 1024 * 1024;


                rs = request.GetRequestStream();
                byte[] buffer = new byte[50];
                int read = 0;

                byte[] buf = Encoding.UTF8.GetBytes("--" + boundary + CRLF);
                rs.Write(buf, 0, buf.Length);

                if (nativeFolder.Length > 0)
                    buf = Encoding.UTF8.GetBytes("Content-Disposition: form-data; name=\"" + Configuration.GetMACAddress() + "\\" + path + "\"; filename=\"" + uploadFileName + "\"" + CRLF);
                else
                    buf = Encoding.UTF8.GetBytes("Content-Disposition: form-data; name=\"" + Configuration.GetMACAddress() + "\"; filename=\"" + uploadFileName + "\"" + CRLF);

                rs.Write(buf, 0, buf.Length);

                buf = Encoding.UTF8.GetBytes("Content-Type: application/octet-stream;" + CRLF);
                rs.Write(buf, 0, buf.Length);

                buf = Encoding.UTF8.GetBytes(CRLF);
                rs.Write(buf, 0, buf.Length);
                rs.Flush();


                long bufferSize = Math.Min(bytesAvailable, maxBufferSize);
                buffer = new byte[bufferSize];
                while ((read = fs.Read(buffer, 0, buffer.Length)) != 0)
                {
                    rs.Write(buffer, 0, read);
                }
                buf = Encoding.UTF8.GetBytes(CRLF);
                rs.Write(buf, 0, buf.Length);
                rs.Flush();

                buffer = Encoding.UTF8.GetBytes("--" + boundary + "--" + CRLF);
                rs.Write(buffer, 0, buffer.Length);

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                return response.StatusCode == HttpStatusCode.OK;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region Import JSON

        public static Snapshot CreateConfiguration(string IMPORT)
        {
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

                    string[] device_type = rows[i].Split(new[] { ' ' }, 2);
                    device_type[1] = device_type[1].Replace("\"", "").Replace(",", "");
                    i++;

                    string[] id = rows[i].Split(new[] { ' ' }, 2);
                    id[1] = id[1].Replace("\"", "").Replace(",", "");
                    i++;

                    string[] trigger_file_path = rows[i].Split(new[] { ' ' }, 2);
                    trigger_file_path[1] = trigger_file_path[1].Replace("\"", "").Replace(",", "");
                    i++;

                    string[] regex = rows[i].Split(new[] { ' ' }, 2);
                    regex[1] = regex[1].Replace("\"", "").Replace(",", "");
                    i++;

                    string[] output_folder_path = rows[i].Split(new[] { ' ' }, 2);
                    output_folder_path[1] = output_folder_path[1].Replace("\"", "").Replace(",", "");
                    i++;

                    string[] output_validity_days = rows[i].Split(new[] { ' ' }, 2);
                    output_validity_days[1] = output_validity_days[1].Replace("\"", "").Replace(",", "");
                    i++;

                    string[] camera_number = rows[i].Split(new[] { ' ' }, 2);
                    camera_number[1] = camera_number[1].Replace("\"", "").Replace(",", "");

                    config.Type = (DeviceType)Enum.Parse(typeof(DeviceType), device_type[1]);
                    config.Id = id[1];
                    config.TriggerFilePath = trigger_file_path[1];
                    config.Regex = regex[1];
                    config.OutputFolderPath = output_folder_path[1];
                    config.OutputValidity = Int32.Parse(output_validity_days[1]);
                    config.CameraNumber = Int32.Parse(camera_number[1]);
                }

                i += 2;
                if (rows[i].Contains("video_configuration"))
                {
                    i += 2;

                    string[] resolution = rows[i].Split(new[] { ' ' }, 2);
                    resolution[1] = resolution[1].Replace("\"", "").Replace(",", "");
                    i++;

                    string[] contrast_level = rows[i].Split(new[] { ' ' }, 2);
                    contrast_level[1] = contrast_level[1].Replace("\"", "").Replace(",", "");
                    i++;

                    string[] image_color = rows[i].Split(new[] { ' ' }, 2);
                    image_color[1] = image_color[1].Replace("\"", "").Replace(",", "");
                    i++;

                    string[] motion_detection = rows[i].Split(new[] { ' ' }, 2);
                    motion_detection[1] = motion_detection[1].Replace("\"", "").Replace(",", "");

                    config.Resolution = (Resolution)Enum.Parse(typeof(Resolution), resolution[1]);
                    config.ContrastLevel = Int32.Parse(contrast_level[1]);
                    config.ImageColor = Color.FromArgb(Int32.Parse(image_color[1]));
                    config.MotionDetection = Convert.ToBoolean(motion_detection[1]);
                }

                i += 2;
                if (rows[i].Contains("network_configuration"))
                {
                    i += 2;

                    string[] server_IP_address = rows[i].Split(new[] { ' ' }, 2);
                    server_IP_address[1] = server_IP_address[1].Replace("\"", "").Replace(",", "");
                    i++;

                    string[] server_media_path = rows[i].Split(new[] { ' ' }, 2);
                    server_media_path[1] = server_media_path[1].Replace("\"", "").Replace(",", "");
                    i++;

                    string[] server_JSON_configuration_path = rows[i].Split(new[] { ' ' }, 2);
                    server_JSON_configuration_path[1] = server_JSON_configuration_path[1].Replace("\"", "").Replace(",", "");
                    i++;

                    string[] server_port = rows[i].Split(new[] { ' ' }, 2);
                    server_port[1] = server_port[1].Replace("\"", "").Replace(",", "");
                    i++;

                    string[] synchronization_period = rows[i].Split(new[] { ' ' }, 2);
                    synchronization_period[1] = synchronization_period[1].Replace("\"", "").Replace(",", "");
                    i++;

                    string[] latest_synchronization_ticks = rows[i].Split(new[] { ' ' }, 2);
                    latest_synchronization_ticks[1] = latest_synchronization_ticks[1].Replace("\"", "").Replace(",", "");
                    i++;

                    string[] connection_status = rows[i].Split(new[] { ' ' }, 2);
                    connection_status[1] = connection_status[1].Replace("\"", "").Replace(",", "");

                    config.ServerIP = server_IP_address[1];
                    config.MediaPath = server_media_path[1];
                    config.JSONConfigPath = server_JSON_configuration_path[1];
                    config.ServerPort = Int32.Parse(server_port[1]);
                    config.SynchronizationPeriod = Int32.Parse(synchronization_period[1]);
                    config.LatestSynchronizationTicks = Int32.Parse(latest_synchronization_ticks[1]);
                    config.ConnectionStatus = Convert.ToBoolean(connection_status[1]);
                }

                i += 2;
                if (rows[i].Contains("capture_configuration"))
                {
                    i += 2;

                    string[] image_capture = rows[i].Split(new[] { ' ' }, 2);
                    image_capture[1] = image_capture[1].Replace("\"", "").Replace(",", "");
                    i++;

                    string[] single_mode = rows[i].Split(new[] { ' ' }, 2);
                    single_mode[1] = single_mode[1].Replace("\"", "").Replace(",", "");
                    i++;

                    string[] duration = rows[i].Split(new[] { ' ' }, 2);
                    duration[1] = duration[1].Replace("\"", "").Replace(",", "");
                    i++;

                    string[] burst_period = rows[i].Split(new[] { ' ' }, 2);
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
            return newSnapshot;
        }

        public static bool ImportFromJSON(string path)
        {
            try
            {
                string IMPORT = "";

                // local import
                if (!path.StartsWith("http"))
                    IMPORT = File.ReadAllText(path);

                // import from server
                else
                {
                    // upload file to specified server
                    IMPORT = DownloadFile(path);
                }

                Snapshot newSnapshot = CreateConfiguration(IMPORT);

                newSnapshot.Connected = Program.Snapshot.Connected;
                Program.Snapshot = newSnapshot;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static string DownloadFile(string endpointUrl)
        {
            HttpWebRequest webRequest;

            webRequest = (HttpWebRequest)WebRequest.Create(endpointUrl + "/" + GetMACAddress());
            webRequest.Method = "GET";
            
            using (WebResponse response = webRequest.GetResponse())
            {
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader rdr = new StreamReader(responseStream, Encoding.UTF8);
                    string Json = rdr.ReadToEnd();
                    return Json;
                }
            }
        }

        #endregion

        #endregion
    }
}
