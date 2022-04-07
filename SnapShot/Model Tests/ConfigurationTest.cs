using Microsoft.VisualStudio.TestTools.UnitTesting;
using SnapShot;
using System;
using System.Collections.Generic;

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



    }
}
