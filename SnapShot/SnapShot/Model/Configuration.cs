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
using Newtonsoft.Json.Linq;

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
            return Newtonsoft.Json.JsonConvert.SerializeObject(Program.Snapshot.Camera);
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
            Snapshot newSnapshot = new Snapshot();
            List<Configuration> cameras = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Configuration>>(IMPORT) ?? 
                new List<Configuration>()
                {
                    new Configuration(),
                    new Configuration(),
                    new Configuration()
                };
            newSnapshot.Camera = cameras;
            return newSnapshot;
        }

        public static Tuple<string, bool> GetTerminalInformation(bool connected)
        {
            string terminalID = Environment.MachineName;
            bool debugLog = false;
            if (connected)
            {
                try
                {
                    HttpWebRequest webRequest;
                    string requestParams = "MacAddress=" + Configuration.GetMACAddress();

                    webRequest = (HttpWebRequest)WebRequest.Create("http://sigrupa4-001-site1.ctempurl.com/api/Licence/GetTerminalAndDebugLog" + "?" + requestParams);

                    webRequest.Method = "GET";

                    HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse();
                    if (response.StatusCode != HttpStatusCode.OK)
                        throw new Exception("Bad request!");

                    using (Stream responseStream = response.GetResponseStream())
                    {
                        StreamReader rdr = new StreamReader(responseStream, Encoding.UTF8);
                        string Json = rdr.ReadToEnd();
                        var obj = JObject.Parse(Json);
                        terminalID = obj.Value<string>("terminalID") ?? "";
                        debugLog = obj.Value<bool>("debugLog");
                    }
                }
                // information about the terminal is not present at the server - 
                // create new configuration at the server
                catch
                {
                    HttpWebRequest webRequest;
                    string requestParams = "MacAddress=" + Configuration.GetMACAddress() + "&"
                                           + "TerminalID=" + terminalID + "&"
                                           + "DebugLog=" + debugLog;

                    webRequest = (HttpWebRequest)WebRequest.Create("http://sigrupa4-001-site1.ctempurl.com/api/Licence/InitialAddDevice" + "?" + requestParams);

                    webRequest.Method = "POST";

                    HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse();
                    if (response.StatusCode != HttpStatusCode.OK)
                        throw new Exception("Bad request!");
                }
            }
            return new Tuple<string, bool>(terminalID, debugLog);
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

                Tuple<string, bool> terminalInformation = GetTerminalInformation(path.StartsWith("http"));
                
                Snapshot newSnapshot = CreateConfiguration(IMPORT);

                newSnapshot.Connected = Program.Snapshot.Connected;
                newSnapshot.TerminalName = terminalInformation.Item1;
                newSnapshot.DebugLog = terminalInformation.Item2;
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
