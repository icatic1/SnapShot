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
using System.Drawing.Imaging;
using Newtonsoft.Json;
using SnapShot.Model;

namespace SnapShot
{
    public class Configuration
    {
        #region Attributes

        // device configuration
        string triggerFilePath = "",
               regex = "",
               outputFolderPath = "";
        int outputValidity;

        // cameras
        List<Camera> cameras = new List<Camera>()
        {
            new Camera(),
            new Camera(),
            new Camera()
        };

        // network configuration
        string serverIP = "",
               mediaFolderPath = "";
        int serverPort;
        bool connectionStatus = false;
        int JSONSynchronizationPeriod = 0,
            JSONLatestSynchronizationTicks = 0,
            mediaSynchronizationPeriod = 0,
            mediaLatestSynchronizationTicks = 0;

        TimeSpan JSONSyncTime = new TimeSpan(0, 0, 0),
                 mediaSyncTime = new TimeSpan(0, 0, 0);

        // capture configuration
        bool imageCapture = true,
             singleMode = true;
        int duration,
            period;

        #endregion

        #region Properties

        public string TriggerFilePath { get => triggerFilePath; set => triggerFilePath = value; }   
        
        public string Regex { get => regex; set => regex = value; }
        
        public string OutputFolderPath { get => outputFolderPath; set => outputFolderPath = value; }
        
        public int OutputValidity { get => outputValidity; set => outputValidity = value; }
        
        public List<Camera> Cameras { get => cameras; set => cameras = value; }
                
        public string ServerIP { get => serverIP; set => serverIP = value; }

        public string MediaFolderPath { get => mediaFolderPath; set => mediaFolderPath = value; }

        public int ServerPort { get => serverPort; set => serverPort = value; }

        public int JSONSyncPeriod { get => JSONSynchronizationPeriod; set => JSONSynchronizationPeriod = value; }
        
        public int JSONTicks { get => JSONLatestSynchronizationTicks; set => JSONLatestSynchronizationTicks = value; }

        public int MediaSyncPeriod { get => mediaSynchronizationPeriod; set => mediaSynchronizationPeriod = value; }

        public int MediaTicks { get => mediaLatestSynchronizationTicks; set => mediaLatestSynchronizationTicks = value; }

        public TimeSpan JSONTime { get => JSONSyncTime; set => JSONSyncTime = value; }

        public TimeSpan MediaTime { get => mediaSyncTime; set => mediaSyncTime = value; }

        public bool ConnectionStatus { get => connectionStatus; set => connectionStatus = value; }
        
        public bool ImageCapture { get => imageCapture; set => imageCapture = value; }
        
        public bool SingleMode { get => singleMode; set => singleMode = value; }
        
        public int Duration { get => duration; set => duration = value; }
        
        public int Period { get => period; set => period = value; }

        #endregion

        #region Methods

        #region Device information

        /// <summary>
        /// Return the MAC address of the device
        /// </summary>
        /// <returns></returns>
        public static string? GetMACAddress()
        {
            return (from nic in NetworkInterface.GetAllNetworkInterfaces()
                    where nic.OperationalStatus == OperationalStatus.Up
                    select nic.GetPhysicalAddress().ToString()
                    ).FirstOrDefault();
        }

        /// <summary>
        /// Check information about the device on remote server
        /// </summary>
        /// <param name="connected"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static Tuple<string, bool> GetTerminalInformation(bool connected)
        {
            // the ID of the device is the machine name
            string terminalID = Environment.MachineName;
            bool debugLog = false;

            // only check about device information if we are connected to the internet
            if (connected)
            {
                try
                {
                    // create web-request for getting device information
                    HttpWebRequest webRequest;
                    string requestParams = "MacAddress=" + Configuration.GetMACAddress();
                    webRequest = (HttpWebRequest)WebRequest.Create(Program.LicencingURL + "/api/Licence/GetTerminalAndDebugLog" + "?" + requestParams);
                    webRequest.Method = "GET";

                    // send the web-request and check whether it returns a valid response
                    HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse();
                    if (response.StatusCode != HttpStatusCode.OK)
                        throw new Exception("Bad request!");

                    // read the resulting string
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        StreamReader rdr = new StreamReader(responseStream, Encoding.UTF8);
                        string Json = rdr.ReadToEnd();
                        var obj = JObject.Parse(Json);
                        terminalID = obj.Value<string>("terminalID") ?? "";
                        debugLog = obj.Value<bool>("debugLog");
                    }
                }

                // information about the device is not present at the server - 
                // create new configuration at the server
                catch
                {
                    // create web-request for adding a new device
                    HttpWebRequest webRequest;
                    string requestParams = "MacAddress=" + Configuration.GetMACAddress() + "&"
                                           + "TerminalID=" + terminalID + "&"
                                           + "DebugLog=" + debugLog;
                    webRequest = (HttpWebRequest)WebRequest.Create(Program.LicencingURL + "/api/Licence/InitialAddDevice" + "?" + requestParams);
                    webRequest.Method = "POST";

                    // send the web-request and check whether it returns a valid response
                    HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse();
                    if (response.StatusCode != HttpStatusCode.OK)
                        throw new Exception("Bad request!");
                }
            }
            return new Tuple<string, bool>(terminalID, debugLog);
        }

        /// <summary>
        /// Check information about the device on remote server
        /// </summary>
        /// <param name="url"></param>
        /// <exception cref="Exception"></exception>
        public static void DeviceCheck(string url)
        {
            try
            {
                // create web-request for getting device information
                HttpWebRequest webRequest;
                string requestParams = "MacAddress=" + Configuration.GetMACAddress();
                webRequest = (HttpWebRequest)WebRequest.Create(url + "/api/Licence/GetDeviceByMAC" + "?" + requestParams);
                webRequest.Method = "GET";

                // send the web-request and check whether it returns a valid response
                HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse();
                if (response.StatusCode != HttpStatusCode.OK)
                    throw new Exception("Bad request!");
            }

            // information about the device is not present at the server - 
            // create new configuration at the server
            catch
            {
                // create web-request for adding a new device
                HttpWebRequest webRequest;
                string requestParams = "MacAddress=" + Configuration.GetMACAddress() + "&"
                                       + "TerminalID=" + Program.Snapshot.TerminalName;
                webRequest = (HttpWebRequest)WebRequest.Create(url + "/api/Licence/AddDevice" + "?" + requestParams);
                webRequest.Method = "POST";

                // send the web-request and check whether it returns a valid response
                HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse();
                if (response.StatusCode != HttpStatusCode.OK)
                    throw new Exception("Bad request!");
            }
        }

        #endregion

        #region Export JSON

        /// <summary>
        /// Serialize Configuration object and turn it into a JSON object
        /// </summary>
        /// <returns></returns>
        public static string CreateJSON()
        {
            return JsonConvert.SerializeObject(Program.Snapshot.Configuration);
        }

        /// <summary>
        /// Export configuration to a local file or remote server
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool ExportToJSON(string path)
        {
            // serialize configuration object
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

        /// <summary>
        /// Send the JSON object to the remote server
        /// </summary>
        /// <param name="endpointUrl"></param>
        /// <param name="filePath"></param>
        /// <param name="nativeFolder"></param>
        /// <returns></returns>
        public static bool UploadFile(string endpointUrl, string filePath, string nativeFolder)
        {
            FileStream fs;
            Stream rs;
            try
            {
                // locate the necessary file
                string uploadFileName = Path.GetFileName(filePath);
                string path = filePath.Replace(uploadFileName, "").TrimEnd('\\');

                // open file containing configuration for writing
                if (nativeFolder.Length > 0)
                    fs = new FileStream(nativeFolder + "\\" + filePath, FileMode.Open, FileAccess.Read);
                else
                    fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);

                // create the web request to be sent to the server
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

                // write the file byte by byte
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

                fs.Close();

                buf = Encoding.UTF8.GetBytes(CRLF);
                rs.Write(buf, 0, buf.Length);
                rs.Flush();

                buffer = Encoding.UTF8.GetBytes("--" + boundary + "--" + CRLF);
                rs.Write(buffer, 0, buffer.Length);

                // send the web request and check whether it returns a valid response
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                return response.StatusCode == HttpStatusCode.OK;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Send an image frame (as byte array) of the livestream to the remote server
        /// </summary>
        /// <param name="endpointUrl"></param>
        /// <param name="filePath"></param>
        /// <param name="image"></param>
        /// <returns></returns>
        public static bool UploadImage(string endpointUrl, string filePath, Bitmap image)
        {
            Stream rs;
            try
            {
                // turn bitmap into byte array
                MemoryStream ms = new MemoryStream();
                image.Save(ms, ImageFormat.Png);
                byte[] fs = ms.ToArray();

                // determine where the image is located
                string uploadFileName = Path.GetFileName(filePath);
                string path = filePath.Replace(uploadFileName, "").TrimEnd('\\');

                // create web-request to the remote server
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

                // write the file byte by byte
                rs = request.GetRequestStream();
                byte[] buffer = new byte[50];

                byte[] buf = Encoding.UTF8.GetBytes("--" + boundary + CRLF);
                rs.Write(buf, 0, buf.Length);

                buf = Encoding.UTF8.GetBytes("Content-Disposition: form-data; name=\"" + Configuration.GetMACAddress() + "\"; filename=\"" + uploadFileName + "\"" + CRLF);

                rs.Write(buf, 0, buf.Length);

                buf = Encoding.UTF8.GetBytes("Content-Type: application/octet-stream;" + CRLF);
                rs.Write(buf, 0, buf.Length);

                buf = Encoding.UTF8.GetBytes(CRLF);
                rs.Write(buf, 0, buf.Length);
                rs.Flush();

                long bufferSize = Math.Min(bytesAvailable, maxBufferSize);
                buffer = new byte[bufferSize];
                int i = 0;
                while (i < fs.Length)
                {
                    buffer = fs.Skip(i).Take((int)bufferSize).ToArray();
                    rs.Write(buffer, 0, (int)bufferSize);
                    i += (int)bufferSize;
                }

                buf = Encoding.UTF8.GetBytes(CRLF);
                rs.Write(buf, 0, buf.Length);
                rs.Flush();

                buffer = Encoding.UTF8.GetBytes("--" + boundary + "--" + CRLF);
                rs.Write(buffer, 0, buffer.Length);
                
                // send the web-request and check whether it returns a valid response
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                return response.StatusCode == HttpStatusCode.OK;
            }
            catch
            {
                return false;
            }

        }

        /// <summary>
        /// Send an image frame (as Base64String) of the livestream to the remote server
        /// </summary>
        /// <param name="endpointUrl"></param>
        /// <param name="base64"></param>
        /// <returns></returns>
        public static bool UploadBase64(string endpointUrl, List<string> base64)
        {
            string json = JsonConvert.SerializeObject(base64);

            // create web-request to the remote server
            HttpWebRequest webRequest;
            webRequest = (HttpWebRequest)WebRequest.Create(endpointUrl + "?MACAddress=" + Configuration.GetMACAddress());
            webRequest.Method = "POST";
            webRequest.ContentType = "application/json";
            using (var streamWriter = new StreamWriter(webRequest.GetRequestStream()))
                streamWriter.Write(json);

            // send the web-request and check whether it returns a valid response
            HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse();
            return (response.StatusCode == HttpStatusCode.OK);
        }

        #endregion

        #region Import JSON

        /// <summary>
        /// Read the JSON object into the Configuration object
        /// </summary>
        /// <param name="IMPORT"></param>
        /// <returns></returns>
        public static Snapshot CreateConfiguration(string IMPORT)
        {
            Snapshot newSnapshot = new Snapshot();
            Configuration config = JsonConvert.DeserializeObject<Configuration>(IMPORT) ?? new Configuration();

            // deserialization of the list of cameras is not good - need to modify it a little bit
            config.Cameras.RemoveRange(0, 3);
            
            newSnapshot.Configuration = config;
            return newSnapshot;
        }

        /// <summary>
        /// Import configuration from a local file or remote server
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
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

                // import device information, not just JSON configuration
                Tuple<string, bool> terminalInformation = GetTerminalInformation(path.StartsWith("http"));
                
                Snapshot newSnapshot = CreateConfiguration(IMPORT);
                newSnapshot.Connected = Program.Snapshot.Connected;
                newSnapshot.TerminalName = terminalInformation.Item1;
                newSnapshot.DebugLog = terminalInformation.Item2;
                string oldTrigger = Program.Snapshot.Configuration.TriggerFilePath;
                Program.Snapshot = newSnapshot;

                // change the trigger file that is being monitored
                if (oldTrigger != newSnapshot.Configuration.TriggerFilePath)
                    Program.ChangeTrigger();
                
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Import JSON from remote server by downloading its contents
        /// </summary>
        /// <param name="endpointUrl"></param>
        /// <returns></returns>
        public static string DownloadFile(string endpointUrl)
        {
            // create web-request for file download
            HttpWebRequest webRequest;
            webRequest = (HttpWebRequest)WebRequest.Create(endpointUrl + "/" + GetMACAddress());
            webRequest.Method = "GET";
            
            // send the web-request and check whether it returns a valid response
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
