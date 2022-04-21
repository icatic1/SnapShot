using Microsoft.VisualStudio.TestTools.UnitTesting;
using SnapShot;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Model_Tests
{
    [TestClass]
    public class ConfigurationTest
    {
        private static Configuration configuration;

        [TestInitialize]
        public void InitializeSnapshotInstance()
        {
            configuration = new Configuration();
        }
        //ServerIP
        [TestMethod]
        public void GetInitialServerIPTest()
        {
            Assert.IsTrue(configuration.ServerIP.Length == 0);
            Assert.AreEqual("", configuration.ServerIP);
            
        }

        [TestMethod]
        public void SetServerIPTest()
        {
            configuration.ServerIP = "100.100.0.0";
            Assert.AreEqual("100.100.0.0", configuration.ServerIP);

        }
        //ServerPort
        [TestMethod]
        public void SetGetServerPortTest()
        {
            configuration.ServerPort = 8080;

            Assert.AreEqual(8080, configuration.ServerPort);

        }
        //ConnectionStatus
        [TestMethod]
        public void GetInitialConnectionStatusTest()
        {
            Assert.IsFalse(configuration.ConnectionStatus);
        }

        [TestMethod]
        public void SetConnectionStatusTest()
        {
            configuration.ConnectionStatus = true;
            Assert.IsTrue(configuration.ConnectionStatus);

        }
        //ImageCapture
        [TestMethod]
        public void GetInitialImageCaptureTest()
        {
            Assert.IsTrue(configuration.ImageCapture);
        }

        [TestMethod]
        public void SetImageCaptureTest()
        {
            configuration.ImageCapture = false;
            Assert.IsFalse(configuration.ImageCapture);

        }

        //SingleMode
        [TestMethod]
        public void GetInitialSingleModeTest()
        {
            Assert.IsTrue(configuration.SingleMode);
        }

        [TestMethod]
        public void SetSingleModeTest()
        {
            configuration.SingleMode = false;
            Assert.IsFalse(configuration.SingleMode);

        }
        //Duration
        [TestMethod]
        public void GetInitialDurationTest()
        {
            Assert.AreEqual(0, configuration.Duration);

        }

        [TestMethod]
        public void SetDurationTest()
        {
            configuration.Duration = 3;
            Assert.AreEqual(3, configuration.Duration);

        }
        //Period
        [TestMethod]
        public void GetInitialPeriodTest()
        {
            Assert.AreEqual(0, configuration.Period);

        }

        [TestMethod]
        public void SetPeriodTest()
        {
            configuration.Period = 5;
            Assert.AreEqual(5, configuration.Period);

        }

        [TestMethod]
        public void TestSetType()
        {
            Configuration configuration = new Configuration();
            configuration.Type = new DeviceType();
            configuration.Type = DeviceType.USBCamera;
            Assert.AreNotEqual(configuration.Type.ToString(), "IPCamera");
            Assert.AreEqual(configuration.Type.ToString(), "USBCamera");
        }

        [TestMethod]
        public void TestGetType()
        {
            Configuration configuration = new Configuration();
            Assert.IsNotNull(configuration.Type.ToString());
        }

        [TestMethod]
        public void TestSetId()
        {
            Configuration configuration = new Configuration();
            configuration.Id = "noviId";
            Assert.AreEqual("noviId",configuration.Id);
        }

        [TestMethod]
        public void TestGetId()
        {
            Configuration configuration = new Configuration();
            Assert.AreEqual("",configuration.Id);
        }



        [TestMethod]
        public void TestSetTriggerFilePath()
        {
            Configuration configuration = new Configuration();
            configuration.TriggerFilePath = "noviTriggerFilePath";
            Assert.AreEqual(configuration.TriggerFilePath, "noviTriggerFilePath");
        }

        [TestMethod]
        public void TestGetTriggerFilePath()
        {
            Configuration configuration = new Configuration();
            Assert.AreEqual("",configuration.TriggerFilePath);
        }


        [TestMethod]
        public void TestSetOutputFolderPath()
        {
            Configuration configuration = new Configuration();
            configuration.OutputFolderPath = "noviOutputFolderPath";
            Assert.AreEqual(configuration.OutputFolderPath, "noviOutputFolderPath");
        }

        [TestMethod]
        public void TestGetOutputFolderPath()
        {
            Configuration configuration = new Configuration();
            Assert.AreEqual( "", configuration.OutputFolderPath);
        }


        [TestMethod]
        public void TestSetOutputValidity()
        {
            Configuration configuration = new Configuration();
            configuration.OutputValidity = 1;
            Assert.AreEqual(configuration.OutputValidity, 1);
        }

        [TestMethod]
        public void TestGetOutputValidity()
        {
            Configuration configuration = new Configuration();
            Assert.AreEqual(0, configuration.OutputValidity);
        }
        
        // Resolution
        [TestMethod]
        public void GetInitialResolutionTest()
        {
            Assert.IsNotNull(configuration.Resolution);
        }

        [TestMethod]
        public void SetResolutionTest()
        {
            configuration.Resolution = Resolution.Resolution720x480;
            Assert.AreEqual(Resolution.Resolution720x480, configuration.Resolution);
        }

        // ContrastLevel
        [TestMethod]
        public void GetInitialContrastLevelTest()
        {
            Assert.AreEqual(0, configuration.ContrastLevel);
        }

        [TestMethod]
        public void SetContrastLevelTest()
        {
            configuration.ContrastLevel = 6;
            Assert.AreEqual(6, configuration.ContrastLevel);
        }

        // ImageColor
        [TestMethod]
        public void GetInitialImageColorTest()
        {
            Assert.AreEqual(SystemColors.Control, configuration.ImageColor);
        }

        [TestMethod]
        public void SetImageColorTest()
        {
            configuration.ImageColor = SystemColors.ControlDark;
            Assert.AreEqual(SystemColors.ControlDark, configuration.ImageColor);
        }

        // MotionDetection
        [TestMethod]
        public void GetInitialMotionDetectionTest()
        {
            Assert.IsFalse(configuration.MotionDetection);
        }

        [TestMethod]
        public void SetMotionDetectionTest()
        {
            configuration.MotionDetection = true;
            Assert.IsTrue(configuration.MotionDetection);
        }

        // ServerVersion
        [TestMethod]
        public void GetInitialServerVersionTest()
        {
            Assert.AreEqual("", configuration.ServerVersion);
        }

        [TestMethod] 
        public void SetServerVersionTest()
        {
            configuration.ServerVersion = "Server Version";
            Assert.AreEqual("Server Version", configuration.ServerVersion);
        }
        
    }
}
