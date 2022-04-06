using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

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
        int outputValidity;

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

        #region Constructor

        public Configuration() { }

        #endregion

        #region Methods

        // not a part of this sprint
        public bool CheckServerConnectionStatus()
        {
            return false;
        }

        #endregion
    }
}
