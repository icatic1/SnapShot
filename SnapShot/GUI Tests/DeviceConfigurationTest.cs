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
    public class DeviceConfigurationTest : TestSession
    {
        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            Setup(context);
            var configuration = session.FindElementByName("Configuration");
            configuration.Click();
            Thread.Sleep(10000);
            /*var textBox2 = desktopSession.FindElementByAccessibilityId("textBox2");
            textBox2.SendKeys(Environment.MachineName.ToString());*/

        }

        [ClassCleanup]
        public static void ClassCleanup()
        {

            /* var textBox2 = desktopSession.FindElementByAccessibilityId("textBox2");
             textBox2.SendKeys(Environment.MachineName.ToString());

             var closeButton = desktopSession.FindElementByAccessibilityId("Close");
             closeButton.Click();*/

            TearDown();
        }


        [TestMethod]
        public void Test1_SelectCamera()
        {
            var comboBox2 = desktopSession.FindElementByAccessibilityId("comboBox2");
            Console.WriteLine(comboBox2.Selected.ToString());
            Assert.AreEqual("USB camera", comboBox2.Text);

            WindowsElement dropDownSupplier = (WindowsElement)desktopSession.FindElementByAccessibilityId("comboBox2");
            dropDownSupplier.Click(); dropDownSupplier.SendKeys("IP camera"); dropDownSupplier.Click();
            Assert.AreEqual("IP camera", comboBox2.Text);

        }

        [TestMethod]
        public void Test2_USBCameraConnected()
        {
            WindowsElement dropDownSupplier = (WindowsElement)desktopSession.FindElementByAccessibilityId("comboBox2");
            dropDownSupplier.Click(); dropDownSupplier.SendKeys("USB camera"); dropDownSupplier.Click();


            WindowsElement comboBoxElement = (WindowsElement)desktopSession.FindElementByAccessibilityId("comboBox1");
            comboBoxElement.Click();
            comboBoxElement.SendKeys(Keys.Down);
            comboBoxElement.SendKeys(Keys.Enter);
            Assert.IsNotNull(comboBoxElement.Selected);
            Assert.AreNotEqual(comboBoxElement.Text, "");

        }

        [TestMethod]
        public void Test3_TriggerFilePath()
        {
            WindowsElement textBox1 = (WindowsElement)desktopSession.FindElementByAccessibilityId("textBox1");
            textBox1.Clear();
            textBox1.SendKeys("C:\\SnapShotTestingGround\\trigger.txt");

            Assert.AreEqual("C:\\SnapShotTestingGround\\trigger.txt", textBox1.Text);


        }

        [TestMethod]
        public void Test4_TriggerRegex()
        {

            WindowsElement textBox6 = (WindowsElement)desktopSession.FindElementByAccessibilityId("textBox6");
            textBox6.Clear();
            textBox6.SendKeys("\\b(record|capture)\\b");

            Assert.AreEqual("\\b(record|capture)\\b", textBox6.Text);

        }

        [TestMethod]
        public void Test5_SaveOutput()
        {
            WindowsElement textBox2 = (WindowsElement)desktopSession.FindElementByAccessibilityId("textBox2");
            textBox2.Clear();
            textBox2.SendKeys("C:\\SnapShotTestingGround\\captures");

            Assert.AreEqual("C:\\SnapShotTestingGround\\captures", textBox2.Text);


        }

        //[TestMethod]
        //public void Test6_KeepCaptureDuration()
        // {
        //     WindowsElement textBox12 = (WindowsElement)desktopSession.FindElementByName("Keep capture for:");
        //    textBox12.Clear();
        //     textBox12.SendKeys("7");
        //
        //     Assert.AreEqual("7", textBox12.Text);
        // }




    }
}
