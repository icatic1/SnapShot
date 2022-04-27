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
    public class VideoConfigurationTest : TestSession
    {
        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            Setup(context);
            Thread.Sleep(10000);
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
        public void Test1_PickResolution1()
        {


            WindowsElement comboBoxElement = (WindowsElement)desktopSession.FindElementByAccessibilityId("comboBox3");
            Assert.IsNotNull(comboBoxElement.Selected);
            Assert.AreEqual(comboBoxElement.Text, "640x480");

        }

        [TestMethod]
        public void Test2_PickResolution2()
        {
            WindowsElement comboBoxElement = (WindowsElement)desktopSession.FindElementByAccessibilityId("comboBox3");
            comboBoxElement.Click();
            comboBoxElement.SendKeys(Keys.Down);
            comboBoxElement.SendKeys(Keys.Down);
            comboBoxElement.SendKeys(Keys.Down);
            comboBoxElement.SendKeys(Keys.Enter);
            Assert.IsNotNull(comboBoxElement.Selected);
            Assert.AreEqual(comboBoxElement.Text, "1280x1024");

        }

        [TestMethod]
        public void Test3_Slider1()
        {
            WindowsElement slider = (WindowsElement)desktopSession.FindElementByAccessibilityId("trackBar1");



            Assert.AreEqual("0", slider.Text);


        }

        [TestMethod]
        public void Test4_Slider2()
        {

            WindowsElement slider = (WindowsElement)desktopSession.FindElementByAccessibilityId("trackBar1");
            slider.SendKeys(Keys.Right);
            slider.SendKeys(Keys.Right);
            slider.SendKeys(Keys.Right);

            Assert.AreEqual("30", slider.Text);

        }

        [TestMethod]
        public void Test5_Slider3()
        {
            WindowsElement slider = (WindowsElement)desktopSession.FindElementByAccessibilityId("trackBar1");
            slider.SendKeys(Keys.Right);
            slider.SendKeys(Keys.Right);
            slider.SendKeys(Keys.Right);
            slider.SendKeys(Keys.Right);
            slider.SendKeys(Keys.Right);
            slider.SendKeys(Keys.Right);
            slider.SendKeys(Keys.Right);
            slider.SendKeys(Keys.Right);
            slider.SendKeys(Keys.Right);
            slider.SendKeys(Keys.Right);
            Assert.AreEqual("100", slider.Text);


        }

        [TestMethod]
        public void Test6_MotionDetection()
        {
            WindowsElement checkBox = (WindowsElement)desktopSession.FindElementByAccessibilityId("checkBox1");
            checkBox.Clear();
            checkBox.Click();
            Assert.IsTrue(checkBox.Selected);
            checkBox.Click();
            Assert.IsFalse(checkBox.Selected);
        }

        [TestMethod]
        public void Test7_CameraPreview()
        {
            WindowsElement button = (WindowsElement)desktopSession.FindElementByAccessibilityId("button5");
            button.Click();
            Thread.Sleep(1000);

            WindowsElement label = (WindowsElement)desktopSession.FindElementByAccessibilityId("label3");
            Assert.AreEqual("Not recording", label.Text);

        }






    }
}
