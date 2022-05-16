using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnapShot.Model
{
    public class Camera
    {
        #region Attributes

        DeviceType type;
        string id = "";
        Resolution resolution = Resolution.Resolution640x480;
        int contrastLevel,
            cameraNumber;
        Color imageColor = SystemColors.Control;
        bool motionDetection = false;

        #endregion

        #region Properties

        public DeviceType Type { get => type; set => type = value; }

        public string Id { get => id; set => id = value; }

        public Resolution Resolution { get => resolution; set => resolution = value; }

        public int ContrastLevel { get => contrastLevel; set => contrastLevel = value; }

        public int CameraNumber { get => cameraNumber; set => cameraNumber = value; }

        public Color ImageColor { get => imageColor; set => imageColor = value; }

        public bool MotionDetection { get => motionDetection; set => motionDetection = value; }

        #endregion
    }
}
