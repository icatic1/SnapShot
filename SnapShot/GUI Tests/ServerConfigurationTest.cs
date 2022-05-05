using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows;
using System;
using Xamarin.Essentials;
using System.IO;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium.Windows;
using System.Collections.ObjectModel;
using OpenQA.Selenium.Remote;
using System.Threading;

namespace GUI_Tests
{
    [TestClass]
    public class ServerConfigurationTest : TestSession
    {
        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            Setup(context);
            var configuration = session.FindElementByName("Configuration");
            configuration.Click();
            Thread.Sleep(1000);
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {

            TearDown();
        }


        [TestMethod]
        public void Test1()
        {
            Thread.Sleep(1000);
            var ip_adress = session.FindElementByAccessibilityId("textBox3");
            var port = session.FindElementByAccessibilityId("textBox4");
            var media_path = session.FindElementByAccessibilityId("textBox7");
            var json_path = session.FindElementByAccessibilityId("textBox5");

            var check_button = session.FindElementByAccessibilityId("button2");

            var status = session.FindElementByAccessibilityId("label17");

            Assert.IsNotNull(ip_adress);
            Assert.IsNotNull(port);
            Assert.IsNotNull(media_path);
            Assert.IsNotNull(json_path);
            Assert.IsNotNull(check_button);
            Assert.IsNotNull(status);

        }

        [TestMethod]
        public void Test2()
        {
            Thread.Sleep(1000);
            var ip_adress = session.FindElementByAccessibilityId("textBox3");
            string test_string = "test";
            ip_adress.SendKeys(test_string);
            Assert.AreEqual(test_string, ip_adress.Text);

        }

        [TestMethod]
        public void Test3()
        {
            Thread.Sleep(1000);
            var port = session.FindElementByAccessibilityId("textBox4");
            string test_string = "test";
            port.SendKeys(test_string);
            Assert.AreEqual(test_string, port.Text);


        }

        [TestMethod]
        public void Test4_TriggerRegex()
        {

            Thread.Sleep(1000);
            var media_path = session.FindElementByAccessibilityId("textBox7");
            string test_string = "test";
            media_path.SendKeys(test_string);
            Assert.AreEqual(test_string, media_path.Text);

        }

        [TestMethod]
        public void Test5()
        {
            Thread.Sleep(1000);
            var json_path = session.FindElementByAccessibilityId("textBox5");
            string test_string = "test";
            json_path.SendKeys(test_string);
            Assert.AreEqual(test_string, json_path.Text);


        }

        




    }
}
