using Microsoft.VisualStudio.TestTools.UnitTesting;
using SnapShot;
using SnapShot.Model;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Model_Tests
{
    [TestClass]
    public class DatabaseTest
    {
     
        [TestMethod]
        public void TestConnect()
        {
            bool v = Database.Connect();
            Assert.IsTrue(v);
            Database.Disconnect();
        }

        [TestMethod]
        public void TestDisconnect()
        {
            bool diskonekt = Database.Connect();
            Assert.IsTrue(diskonekt);
            Database.Disconnect();
        }

        [TestMethod]
        public void TestMacAdress()
        {
            String adresa = Database.GetMACAddress();
            Assert.AreNotEqual("Adresa",adresa);
        }

        [TestMethod]
        public void TestCheckLicenceFalse()
        {
            Database.Connect();
            try
            {
                var istina = Database.CheckLicence();
                Assert.IsFalse(istina);
                Database.Disconnect();
            }
            catch(Exception ex) {
                Database.Disconnect();
            }

        }

        [TestMethod]
        public void TestReadAndWriteConfiguration()
        {
            Database.Connect();
            try
            {
                Database.WriteConfiguration("Parametar");
                String rez = Database.ReadConfiguration();
                Assert.AreEqual("Parametar",rez);
                Database.Disconnect();
            }
            catch (Exception ex)
            {
                Database.Disconnect();
            }

        }

        [TestMethod]
        public void TestReadAndWriteConfigurationNotFound()
        {
            Database.Connect();
            try
            {
                String rez = Database.ReadConfiguration();
                Assert.AreEqual("Configuration not found!", rez);
                Database.Disconnect();
            }
            catch (Exception ex)
            {
                Database.Disconnect();
            }

        }

        [TestMethod]
        public void TestReadAndWriteConfigurationEmpty()
        {
            Database.Connect();
            try
            {
                Database.WriteConfiguration("");
                String rez = Database.ReadConfiguration();
                Assert.AreEqual("", rez);
                Database.Disconnect();
            }
            catch (Exception ex)
            {
                Database.Disconnect();
            }

        }


    }
}