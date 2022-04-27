using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows;
using System;
using Xamarin.Essentials;
using System.IO;
using OpenQA.Selenium;
using System.Threading;

namespace GUI_Tests
{
    [TestClass]
    public class CaptureConfigurationTest : TestSession
    {
        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            Setup(context);

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
        public void Test1_radioButtonCheck1()
        {
            var configuration = session.FindElementByName("Configuration");
            configuration.Click();
            Thread.Sleep(1000);

            var radioButton4 = desktopSession.FindElementByAccessibilityId("radioButton4");

            radioButton4.Click();
            Assert.IsTrue(radioButton4.Selected);

            
        }
        [TestMethod]
        public void Test2_radioButtonCheck2()
        {
           // var configuration = session.FindElementByName("Configuration");
          //  configuration.Click();
            Thread.Sleep(1000);

            var radioButton3 = desktopSession.FindElementByAccessibilityId("radioButton3");
            
            radioButton3.Click();
            Assert.IsTrue(radioButton3.Selected);

           
        }
        [TestMethod]
        public void Test3_radioButtonCheck3()
        {
           // var configuration = session.FindElementByName("Configuration");
         //   configuration.Click();
            Thread.Sleep(1000);

            var radioButton1 = desktopSession.FindElementByAccessibilityId("radioButton1");

            radioButton1.Click();
            Assert.IsTrue(radioButton1.Selected);


        }
        [TestMethod]
        public void Test4_radioButtonCheck4()
        {
         //   var configuration = session.FindElementByName("Configuration");
        //    configuration.Click();
            Thread.Sleep(1000);

            var radioButton2 = desktopSession.FindElementByAccessibilityId("radioButton2");

            radioButton2.Click();
            Assert.IsFalse(radioButton2.Selected);


        }

        [TestMethod]
        public void Test5_Duration1()
        {
            // LeftClick on MenuItem "Configuration" at (40,9)
            Console.WriteLine("LeftClick on MenuItem \"Configuration\" at (40,9)");
            string xpath_LeftClickMenuItemConfigurat_40_9 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 1\"]/Window[@Name=\"SnapShot\"][@AutomationId=\"LicencingForm\"]/MenuBar[@Name=\"menuStrip1\"][starts-with(@AutomationId,\"menuStrip\")]/MenuItem[@Name=\"Configuration\"]";
            var winElem_LeftClickMenuItemConfigurat_40_9 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickMenuItemConfigurat_40_9);
            if (winElem_LeftClickMenuItemConfigurat_40_9 != null)
            {
                winElem_LeftClickMenuItemConfigurat_40_9.Click();
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickMenuItemConfigurat_40_9}");
                return;
            }


            // LeftClick on RadioButton "Video" at (8,9)
            Console.WriteLine("LeftClick on RadioButton \"Video\" at (8,9)");
            string xpath_LeftClickRadioButtonVideo_8_9 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 1\"]/Window[@Name=\"SnapShot\"][@AutomationId=\"ConfigurationForm\"]/Pane[starts-with(@AutomationId,\"tableLayoutPanel\")]/Group[@Name=\"Capture configuration\"][starts-with(@AutomationId,\"groupBox\")]/Pane[starts-with(@AutomationId,\"panel\")]/RadioButton[@Name=\"Video\"][starts-with(@AutomationId,\"radioButton\")]";
            var winElem_LeftClickRadioButtonVideo_8_9 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickRadioButtonVideo_8_9);
            if (winElem_LeftClickRadioButtonVideo_8_9 != null)
            {
                winElem_LeftClickRadioButtonVideo_8_9.Click();
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickRadioButtonVideo_8_9}");
                return;
            }


            // LeftClick on Button "Up" at (10,6)
            Console.WriteLine("LeftClick on Button \"Up\" at (10,6)");
            string xpath_LeftClickButtonUp_10_6 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 1\"]/Window[@Name=\"SnapShot\"][@AutomationId=\"ConfigurationForm\"]/Pane[starts-with(@AutomationId,\"tableLayoutPanel\")]/Group[@Name=\"Capture configuration\"][starts-with(@AutomationId,\"groupBox\")]/Spinner[@Name=\"Duration:\"][starts-with(@AutomationId,\"numericUpDown\")]/Spinner[starts-with(@ClassName,\"WindowsForms10\")][@Name=\"UpDown\"]/Button[@Name=\"Up\"]";
            var winElem_LeftClickButtonUp_10_6 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickButtonUp_10_6);
            if (winElem_LeftClickButtonUp_10_6 != null)
            {
                winElem_LeftClickButtonUp_10_6.Click();
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickButtonUp_10_6}");
                return;
            }


            // LeftClick on Button "Up" at (11,7)
            Console.WriteLine("LeftClick on Button \"Up\" at (11,7)");
            string xpath_LeftClickButtonUp_11_7 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 1\"]/Window[@Name=\"SnapShot\"][@AutomationId=\"ConfigurationForm\"]/Pane[starts-with(@AutomationId,\"tableLayoutPanel\")]/Group[@Name=\"Capture configuration\"][starts-with(@AutomationId,\"groupBox\")]/Spinner[@Name=\"Mode of capture:\"][starts-with(@AutomationId,\"domainUpDown\")]/Spinner[starts-with(@ClassName,\"WindowsForms10\")][@Name=\"UpDown\"]/Button[@Name=\"Up\"]";
            var winElem_LeftClickButtonUp_11_7 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickButtonUp_11_7);
            if (winElem_LeftClickButtonUp_11_7 != null)
            {
                winElem_LeftClickButtonUp_11_7.Click();
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickButtonUp_11_7}");
                return;
            }






        }

        [TestMethod]
        public void Test6_BurstPeriod1() 
        {
            // LeftClick on MenuItem "Configuration" at (56,11)
            Console.WriteLine("LeftClick on MenuItem \"Configuration\" at (56,11)");
            string xpath_LeftClickMenuItemConfigurat_56_11 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 1\"]/Window[@Name=\"SnapShot\"][@AutomationId=\"LicencingForm\"]/MenuBar[@Name=\"menuStrip1\"][starts-with(@AutomationId,\"menuStrip\")]/MenuItem[@Name=\"Configuration\"]";
            var winElem_LeftClickMenuItemConfigurat_56_11 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickMenuItemConfigurat_56_11);
            if (winElem_LeftClickMenuItemConfigurat_56_11 != null)
            {
                winElem_LeftClickMenuItemConfigurat_56_11.Click();
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickMenuItemConfigurat_56_11}");
                return;
            }


            // LeftClick on RadioButton "Burst" at (7,15)
            Console.WriteLine("LeftClick on RadioButton \"Burst\" at (7,15)");
            string xpath_LeftClickRadioButtonBurst_7_15 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 1\"]/Window[@Name=\"SnapShot\"][@AutomationId=\"ConfigurationForm\"]/Pane[starts-with(@AutomationId,\"tableLayoutPanel\")]/Group[@Name=\"Capture configuration\"][starts-with(@AutomationId,\"groupBox\")]/Pane[starts-with(@AutomationId,\"panel\")]/RadioButton[@Name=\"Burst\"][starts-with(@AutomationId,\"radioButton\")]";
            var winElem_LeftClickRadioButtonBurst_7_15 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickRadioButtonBurst_7_15);
            if (winElem_LeftClickRadioButtonBurst_7_15 != null)
            {
                winElem_LeftClickRadioButtonBurst_7_15.Click();
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickRadioButtonBurst_7_15}");
                return;
            }


            // LeftClick on Button "Up" at (8,7)
            Console.WriteLine("LeftClick on Button \"Up\" at (8,7)");
            string xpath_LeftClickButtonUp_8_7 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 1\"]/Window[@Name=\"SnapShot\"][@AutomationId=\"ConfigurationForm\"]/Pane[starts-with(@AutomationId,\"tableLayoutPanel\")]/Group[@Name=\"Capture configuration\"][starts-with(@AutomationId,\"groupBox\")]/Spinner[@Name=\"Duration:\"][starts-with(@AutomationId,\"numericUpDown\")]/Spinner[starts-with(@ClassName,\"WindowsForms10\")][@Name=\"UpDown\"]/Button[@Name=\"Up\"]";
            var winElem_LeftClickButtonUp_8_7 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickButtonUp_8_7);
            if (winElem_LeftClickButtonUp_8_7 != null)
            {
                winElem_LeftClickButtonUp_8_7.Click();
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickButtonUp_8_7}");
                return;
            }


            // LeftClick on Button "Up" at (10,7)
            Console.WriteLine("LeftClick on Button \"Up\" at (10,7)");
            string xpath_LeftClickButtonUp_10_7 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 1\"]/Window[@Name=\"SnapShot\"][@AutomationId=\"ConfigurationForm\"]/Pane[starts-with(@AutomationId,\"tableLayoutPanel\")]/Group[@Name=\"Capture configuration\"][starts-with(@AutomationId,\"groupBox\")]/Spinner[@Name=\"Burst period:\"][starts-with(@AutomationId,\"numericUpDown\")]/Spinner[starts-with(@ClassName,\"WindowsForms10\")][@Name=\"UpDown\"]/Button[@Name=\"Up\"]";
            var winElem_LeftClickButtonUp_10_7 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickButtonUp_10_7);
            if (winElem_LeftClickButtonUp_10_7 != null)
            {
                winElem_LeftClickButtonUp_10_7.Click();
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickButtonUp_10_7}");
                return;
            }



        }

        [TestMethod]
        public void Test6_BurstPeriod2()
        {
            // LeftClick on MenuItem "Configuration" at (49,14)
            Console.WriteLine("LeftClick on MenuItem \"Configuration\" at (49,14)");
            string xpath_LeftClickMenuItemConfigurat_49_14 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 1\"]/Window[@Name=\"SnapShot\"][@AutomationId=\"LicencingForm\"]/MenuBar[@Name=\"menuStrip1\"][starts-with(@AutomationId,\"menuStrip\")]/MenuItem[@Name=\"Configuration\"]";
            var winElem_LeftClickMenuItemConfigurat_49_14 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickMenuItemConfigurat_49_14);
            if (winElem_LeftClickMenuItemConfigurat_49_14 != null)
            {
                winElem_LeftClickMenuItemConfigurat_49_14.Click();
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickMenuItemConfigurat_49_14}");
                return;
            }


            // LeftClick on RadioButton "Burst" at (15,9)
            Console.WriteLine("LeftClick on RadioButton \"Burst\" at (15,9)");
            string xpath_LeftClickRadioButtonBurst_15_9 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 1\"]/Window[@Name=\"SnapShot\"][@AutomationId=\"ConfigurationForm\"]/Pane[starts-with(@AutomationId,\"tableLayoutPanel\")]/Group[@Name=\"Capture configuration\"][starts-with(@AutomationId,\"groupBox\")]/Pane[starts-with(@AutomationId,\"panel\")]/RadioButton[@Name=\"Burst\"][starts-with(@AutomationId,\"radioButton\")]";
            var winElem_LeftClickRadioButtonBurst_15_9 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickRadioButtonBurst_15_9);
            if (winElem_LeftClickRadioButtonBurst_15_9 != null)
            {
                winElem_LeftClickRadioButtonBurst_15_9.Click();
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickRadioButtonBurst_15_9}");
                return;
            }


            // LeftClick on Button "Up" at (9,3)
            Console.WriteLine("LeftClick on Button \"Up\" at (9,3)");
            string xpath_LeftClickButtonUp_9_3 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 1\"]/Window[@Name=\"SnapShot\"][@AutomationId=\"ConfigurationForm\"]/Pane[starts-with(@AutomationId,\"tableLayoutPanel\")]/Group[@Name=\"Capture configuration\"][starts-with(@AutomationId,\"groupBox\")]/Spinner[@Name=\"Duration:\"][starts-with(@AutomationId,\"numericUpDown\")]/Spinner[starts-with(@ClassName,\"WindowsForms10\")][@Name=\"UpDown\"]/Button[@Name=\"Up\"]";
            var winElem_LeftClickButtonUp_9_3 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickButtonUp_9_3);
            if (winElem_LeftClickButtonUp_9_3 != null)
            {
                winElem_LeftClickButtonUp_9_3.Click();
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickButtonUp_9_3}");
                return;
            }


            // LeftClick on Button "Up" at (7,4)
            Console.WriteLine("LeftClick on Button \"Up\" at (7,4)");
            string xpath_LeftClickButtonUp_7_4 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 1\"]/Window[@Name=\"SnapShot\"][@AutomationId=\"ConfigurationForm\"]/Pane[starts-with(@AutomationId,\"tableLayoutPanel\")]/Group[@Name=\"Capture configuration\"][starts-with(@AutomationId,\"groupBox\")]/Spinner[@Name=\"Burst period:\"][starts-with(@AutomationId,\"numericUpDown\")]/Spinner[starts-with(@ClassName,\"WindowsForms10\")][@Name=\"UpDown\"]/Button[@Name=\"Up\"]";
            var winElem_LeftClickButtonUp_7_4 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickButtonUp_7_4);
            if (winElem_LeftClickButtonUp_7_4 != null)
            {
                winElem_LeftClickButtonUp_7_4.Click();
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickButtonUp_7_4}");
                return;
            }







        }


        [TestMethod]
        public void Test7_DurationPeriod2()
        {

            // LeftClick on MenuItem "Configuration" at (57,12)
            Console.WriteLine("LeftClick on MenuItem \"Configuration\" at (57,12)");
            string xpath_LeftClickMenuItemConfigurat_57_12 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 1\"]/Window[@Name=\"SnapShot\"][@AutomationId=\"LicencingForm\"]/MenuBar[@Name=\"menuStrip1\"][starts-with(@AutomationId,\"menuStrip\")]/MenuItem[@Name=\"Configuration\"]";
            var winElem_LeftClickMenuItemConfigurat_57_12 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickMenuItemConfigurat_57_12);
            if (winElem_LeftClickMenuItemConfigurat_57_12 != null)
            {
                winElem_LeftClickMenuItemConfigurat_57_12.Click();
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickMenuItemConfigurat_57_12}");
                return;
            }


            // LeftClick on RadioButton "Video" at (5,9)
            Console.WriteLine("LeftClick on RadioButton \"Video\" at (5,9)");
            string xpath_LeftClickRadioButtonVideo_5_9 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 1\"]/Window[@Name=\"SnapShot\"][@AutomationId=\"ConfigurationForm\"]/Pane[starts-with(@AutomationId,\"tableLayoutPanel\")]/Group[@Name=\"Capture configuration\"][starts-with(@AutomationId,\"groupBox\")]/Pane[starts-with(@AutomationId,\"panel\")]/RadioButton[@Name=\"Video\"][starts-with(@AutomationId,\"radioButton\")]";
            var winElem_LeftClickRadioButtonVideo_5_9 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickRadioButtonVideo_5_9);
            if (winElem_LeftClickRadioButtonVideo_5_9 != null)
            {
                winElem_LeftClickRadioButtonVideo_5_9.Click();
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickRadioButtonVideo_5_9}");
                return;
            }


            // LeftClick on Button "Up" at (7,4)
            Console.WriteLine("LeftClick on Button \"Up\" at (7,4)");
            string xpath_LeftClickButtonUp_7_4 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 1\"]/Window[@Name=\"SnapShot\"][@AutomationId=\"ConfigurationForm\"]/Pane[starts-with(@AutomationId,\"tableLayoutPanel\")]/Group[@Name=\"Capture configuration\"][starts-with(@AutomationId,\"groupBox\")]/Spinner[@Name=\"Duration:\"][starts-with(@AutomationId,\"numericUpDown\")]/Spinner[starts-with(@ClassName,\"WindowsForms10\")][@Name=\"UpDown\"]/Button[@Name=\"Up\"]";
            var winElem_LeftClickButtonUp_7_4 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickButtonUp_7_4);
            if (winElem_LeftClickButtonUp_7_4 != null)
            {
                winElem_LeftClickButtonUp_7_4.Click();
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickButtonUp_7_4}");
                return;
            }


        }
        [TestMethod]
        public void Test8_DurationPeriod()
        {
            // LeftClick on MenuItem "Configuration" at (45,13)
            Console.WriteLine("LeftClick on MenuItem \"Configuration\" at (45,13)");
            string xpath_LeftClickMenuItemConfigurat_45_13 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 1\"]/Window[@Name=\"SnapShot\"][@AutomationId=\"LicencingForm\"]/MenuBar[@Name=\"menuStrip1\"][starts-with(@AutomationId,\"menuStrip\")]/MenuItem[@Name=\"Configuration\"]";
            var winElem_LeftClickMenuItemConfigurat_45_13 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickMenuItemConfigurat_45_13);
            if (winElem_LeftClickMenuItemConfigurat_45_13 != null)
            {
                winElem_LeftClickMenuItemConfigurat_45_13.Click();
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickMenuItemConfigurat_45_13}");
                return;
            }


            // LeftClick on RadioButton "Video" at (3,11)
            Console.WriteLine("LeftClick on RadioButton \"Video\" at (3,11)");
            string xpath_LeftClickRadioButtonVideo_3_11 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 1\"]/Window[@Name=\"SnapShot\"][@AutomationId=\"ConfigurationForm\"]/Pane[starts-with(@AutomationId,\"tableLayoutPanel\")]/Group[@Name=\"Capture configuration\"][starts-with(@AutomationId,\"groupBox\")]/Pane[starts-with(@AutomationId,\"panel\")]/RadioButton[@Name=\"Video\"][starts-with(@AutomationId,\"radioButton\")]";
            var winElem_LeftClickRadioButtonVideo_3_11 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickRadioButtonVideo_3_11);
            if (winElem_LeftClickRadioButtonVideo_3_11 != null)
            {
                winElem_LeftClickRadioButtonVideo_3_11.Click();
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickRadioButtonVideo_3_11}");
                return;
            }


            // LeftClick on RadioButton "Single" at (5,10)
            Console.WriteLine("LeftClick on RadioButton \"Single\" at (5,10)");
            string xpath_LeftClickRadioButtonSingle_5_10 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 1\"]/Window[@Name=\"SnapShot\"][@AutomationId=\"ConfigurationForm\"]/Pane[starts-with(@AutomationId,\"tableLayoutPanel\")]/Group[@Name=\"Capture configuration\"][starts-with(@AutomationId,\"groupBox\")]/Pane[starts-with(@AutomationId,\"panel\")]/RadioButton[@Name=\"Single\"][starts-with(@AutomationId,\"radioButton\")]";
            var winElem_LeftClickRadioButtonSingle_5_10 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickRadioButtonSingle_5_10);
            if (winElem_LeftClickRadioButtonSingle_5_10 != null)
            {
                winElem_LeftClickRadioButtonSingle_5_10.Click();
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickRadioButtonSingle_5_10}");
                return;
            }


            // LeftClick on Button "Up" at (9,5)
            Console.WriteLine("LeftClick on Button \"Up\" at (9,5)");
            string xpath_LeftClickButtonUp_9_5 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 1\"]/Window[@Name=\"SnapShot\"][@AutomationId=\"ConfigurationForm\"]/Pane[starts-with(@AutomationId,\"tableLayoutPanel\")]/Group[@Name=\"Capture configuration\"][starts-with(@AutomationId,\"groupBox\")]/Spinner[@Name=\"Duration:\"][starts-with(@AutomationId,\"numericUpDown\")]/Spinner[starts-with(@ClassName,\"WindowsForms10\")][@Name=\"UpDown\"]/Button[@Name=\"Up\"]";
            var winElem_LeftClickButtonUp_9_5 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickButtonUp_9_5);
            if (winElem_LeftClickButtonUp_9_5 != null)
            {
                winElem_LeftClickButtonUp_9_5.Click();
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickButtonUp_9_5}");
                return;
            }


            // LeftClick on Button "Down" at (11,0)
            Console.WriteLine("LeftClick on Button \"Down\" at (11,0)");
            string xpath_LeftClickButtonDown_11_0 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 1\"]/Window[@Name=\"SnapShot\"][@AutomationId=\"ConfigurationForm\"]/Pane[starts-with(@AutomationId,\"tableLayoutPanel\")]/Group[@Name=\"Capture configuration\"][starts-with(@AutomationId,\"groupBox\")]/Spinner[@Name=\"Mode of capture:\"][starts-with(@AutomationId,\"domainUpDown\")]/Spinner[starts-with(@ClassName,\"WindowsForms10\")][@Name=\"UpDown\"]/Button[@Name=\"Down\"]";
            var winElem_LeftClickButtonDown_11_0 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickButtonDown_11_0);
            if (winElem_LeftClickButtonDown_11_0 != null)
            {
                winElem_LeftClickButtonDown_11_0.Click();
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickButtonDown_11_0}");
                return;
            }


            // LeftClick on Button "Up" at (11,6)
            Console.WriteLine("LeftClick on Button \"Up\" at (11,6)");
            string xpath_LeftClickButtonUp_11_6 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 1\"]/Window[@Name=\"SnapShot\"][@AutomationId=\"ConfigurationForm\"]/Pane[starts-with(@AutomationId,\"tableLayoutPanel\")]/Group[@Name=\"Capture configuration\"][starts-with(@AutomationId,\"groupBox\")]/Spinner[@Name=\"Duration:\"][starts-with(@AutomationId,\"numericUpDown\")]/Spinner[starts-with(@ClassName,\"WindowsForms10\")][@Name=\"UpDown\"]/Button[@Name=\"Up\"]";
            var winElem_LeftClickButtonUp_11_6 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickButtonUp_11_6);
            if (winElem_LeftClickButtonUp_11_6 != null)
            {
                winElem_LeftClickButtonUp_11_6.Click();
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickButtonUp_11_6}");
                return;
            }


            // LeftClick on Button "Up" at (5,5)
            Console.WriteLine("LeftClick on Button \"Up\" at (5,5)");
            string xpath_LeftClickButtonUp_5_5 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 1\"]/Window[@Name=\"SnapShot\"][@AutomationId=\"ConfigurationForm\"]/Pane[starts-with(@AutomationId,\"tableLayoutPanel\")]/Group[@Name=\"Capture configuration\"][starts-with(@AutomationId,\"groupBox\")]/Spinner[@Name=\"Mode of capture:\"][starts-with(@AutomationId,\"domainUpDown\")]/Spinner[starts-with(@ClassName,\"WindowsForms10\")][@Name=\"UpDown\"]/Button[@Name=\"Up\"]";
            var winElem_LeftClickButtonUp_5_5 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickButtonUp_5_5);
            if (winElem_LeftClickButtonUp_5_5 != null)
            {
                winElem_LeftClickButtonUp_5_5.Click();
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickButtonUp_5_5}");
                return;
            }


            // LeftClick on Button "Up" at (10,5)
            Console.WriteLine("LeftClick on Button \"Up\" at (10,5)");
            string xpath_LeftClickButtonUp_10_5 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 1\"]/Window[@Name=\"SnapShot\"][@AutomationId=\"ConfigurationForm\"]/Pane[starts-with(@AutomationId,\"tableLayoutPanel\")]/Group[@Name=\"Capture configuration\"][starts-with(@AutomationId,\"groupBox\")]/Spinner[@Name=\"Duration:\"][starts-with(@AutomationId,\"numericUpDown\")]/Spinner[starts-with(@ClassName,\"WindowsForms10\")][@Name=\"UpDown\"]/Button[@Name=\"Up\"]";
            var winElem_LeftClickButtonUp_10_5 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickButtonUp_10_5);
            if (winElem_LeftClickButtonUp_10_5 != null)
            {
                winElem_LeftClickButtonUp_10_5.Click();
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickButtonUp_10_5}");
                return;
            }


            // LeftClick on Button "Down" at (8,5)
            Console.WriteLine("LeftClick on Button \"Down\" at (8,5)");
            string xpath_LeftClickButtonDown_8_5 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 1\"]/Window[@Name=\"SnapShot\"][@AutomationId=\"ConfigurationForm\"]/Pane[starts-with(@AutomationId,\"tableLayoutPanel\")]/Group[@Name=\"Capture configuration\"][starts-with(@AutomationId,\"groupBox\")]/Spinner[@Name=\"Mode of capture:\"][starts-with(@AutomationId,\"domainUpDown\")]/Spinner[starts-with(@ClassName,\"WindowsForms10\")][@Name=\"UpDown\"]/Button[@Name=\"Down\"]";
            var winElem_LeftClickButtonDown_8_5 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickButtonDown_8_5);
            if (winElem_LeftClickButtonDown_8_5 != null)
            {
                winElem_LeftClickButtonDown_8_5.Click();
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickButtonDown_8_5}");
                return;
            }




        }

        [TestMethod]
        public void Test9_DurationPeriod()
        {
            // LeftClick on MenuItem "Configuration" at (55,7)
            Console.WriteLine("LeftClick on MenuItem \"Configuration\" at (55,7)");
            string xpath_LeftClickMenuItemConfigurat_55_7 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 1\"]/Window[@Name=\"SnapShot\"][@AutomationId=\"LicencingForm\"]/MenuBar[@Name=\"menuStrip1\"][starts-with(@AutomationId,\"menuStrip\")]/MenuItem[@Name=\"Configuration\"]";
            var winElem_LeftClickMenuItemConfigurat_55_7 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickMenuItemConfigurat_55_7);
            if (winElem_LeftClickMenuItemConfigurat_55_7 != null)
            {
                winElem_LeftClickMenuItemConfigurat_55_7.Click();
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickMenuItemConfigurat_55_7}");
                return;
            }


            // LeftClick on RadioButton "Video" at (8,15)
            Console.WriteLine("LeftClick on RadioButton \"Video\" at (8,15)");
            string xpath_LeftClickRadioButtonVideo_8_15 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 1\"]/Window[@Name=\"SnapShot\"][@AutomationId=\"ConfigurationForm\"]/Pane[starts-with(@AutomationId,\"tableLayoutPanel\")]/Group[@Name=\"Capture configuration\"][starts-with(@AutomationId,\"groupBox\")]/Pane[starts-with(@AutomationId,\"panel\")]/RadioButton[@Name=\"Video\"][starts-with(@AutomationId,\"radioButton\")]";
            var winElem_LeftClickRadioButtonVideo_8_15 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickRadioButtonVideo_8_15);
            if (winElem_LeftClickRadioButtonVideo_8_15 != null)
            {
                winElem_LeftClickRadioButtonVideo_8_15.Click();
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickRadioButtonVideo_8_15}");
                return;
            }


            // LeftClick on Button "Up" at (8,6)
            Console.WriteLine("LeftClick on Button \"Up\" at (8,6)");
            string xpath_LeftClickButtonUp_8_6 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 1\"]/Window[@Name=\"SnapShot\"][@AutomationId=\"ConfigurationForm\"]/Pane[starts-with(@AutomationId,\"tableLayoutPanel\")]/Group[@Name=\"Capture configuration\"][starts-with(@AutomationId,\"groupBox\")]/Spinner[@Name=\"Duration:\"][starts-with(@AutomationId,\"numericUpDown\")]/Spinner[starts-with(@ClassName,\"WindowsForms10\")][@Name=\"UpDown\"]/Button[@Name=\"Up\"]";
            var winElem_LeftClickButtonUp_8_6 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickButtonUp_8_6);
            if (winElem_LeftClickButtonUp_8_6 != null)
            {
                winElem_LeftClickButtonUp_8_6.Click();
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickButtonUp_8_6}");
                return;
            }


            // LeftClick on Button "Up" at (5,6)
            Console.WriteLine("LeftClick on Button \"Up\" at (5,6)");
            string xpath_LeftClickButtonUp_5_6 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 1\"]/Window[@Name=\"SnapShot\"][@AutomationId=\"ConfigurationForm\"]/Pane[starts-with(@AutomationId,\"tableLayoutPanel\")]/Group[@Name=\"Capture configuration\"][starts-with(@AutomationId,\"groupBox\")]/Spinner[@Name=\"Duration:\"][starts-with(@AutomationId,\"numericUpDown\")]/Spinner[starts-with(@ClassName,\"WindowsForms10\")][@Name=\"UpDown\"]/Button[@Name=\"Up\"]";
            var winElem_LeftClickButtonUp_5_6 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickButtonUp_5_6);
            if (winElem_LeftClickButtonUp_5_6 != null)
            {
                winElem_LeftClickButtonUp_5_6.Click();
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickButtonUp_5_6}");
                return;
            }


            // LeftClick on Button "Up" at (10,1)
            Console.WriteLine("LeftClick on Button \"Up\" at (10,1)");
            string xpath_LeftClickButtonUp_10_1 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 1\"]/Window[@Name=\"SnapShot\"][@AutomationId=\"ConfigurationForm\"]/Pane[starts-with(@AutomationId,\"tableLayoutPanel\")]/Group[@Name=\"Capture configuration\"][starts-with(@AutomationId,\"groupBox\")]/Spinner[@Name=\"Duration:\"][starts-with(@AutomationId,\"numericUpDown\")]/Spinner[starts-with(@ClassName,\"WindowsForms10\")][@Name=\"UpDown\"]/Button[@Name=\"Up\"]";
            var winElem_LeftClickButtonUp_10_1 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickButtonUp_10_1);
            if (winElem_LeftClickButtonUp_10_1 != null)
            {
                winElem_LeftClickButtonUp_10_1.Click();
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickButtonUp_10_1}");
                return;
            }


            // LeftClick on Button "Down" at (8,4)
            Console.WriteLine("LeftClick on Button \"Down\" at (8,4)");
            string xpath_LeftClickButtonDown_8_4 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 1\"]/Window[@Name=\"SnapShot\"][@AutomationId=\"ConfigurationForm\"]/Pane[starts-with(@AutomationId,\"tableLayoutPanel\")]/Group[@Name=\"Capture configuration\"][starts-with(@AutomationId,\"groupBox\")]/Spinner[@Name=\"Duration:\"][starts-with(@AutomationId,\"numericUpDown\")]/Spinner[starts-with(@ClassName,\"WindowsForms10\")][@Name=\"UpDown\"]/Button[@Name=\"Down\"]";
            var winElem_LeftClickButtonDown_8_4 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickButtonDown_8_4);
            if (winElem_LeftClickButtonDown_8_4 != null)
            {
                winElem_LeftClickButtonDown_8_4.Click();
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickButtonDown_8_4}");
                return;
            }






        }

        [TestMethod]
        public void Test_BurstPeriod()
        {
            // LeftClick on MenuItem "Configuration" at (67,11)
            Console.WriteLine("LeftClick on MenuItem \"Configuration\" at (67,11)");
            string xpath_LeftClickMenuItemConfigurat_67_11 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 1\"]/Window[@Name=\"SnapShot\"][@AutomationId=\"LicencingForm\"]/MenuBar[@Name=\"menuStrip1\"][starts-with(@AutomationId,\"menuStrip\")]/MenuItem[@Name=\"Configuration\"]";
            var winElem_LeftClickMenuItemConfigurat_67_11 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickMenuItemConfigurat_67_11);
            if (winElem_LeftClickMenuItemConfigurat_67_11 != null)
            {
                winElem_LeftClickMenuItemConfigurat_67_11.Click();
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickMenuItemConfigurat_67_11}");
                return;
            }


            // LeftClick on RadioButton "Burst" at (8,13)
            Console.WriteLine("LeftClick on RadioButton \"Burst\" at (8,13)");
            string xpath_LeftClickRadioButtonBurst_8_13 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 1\"]/Window[@Name=\"SnapShot\"][@AutomationId=\"ConfigurationForm\"]/Pane[starts-with(@AutomationId,\"tableLayoutPanel\")]/Group[@Name=\"Capture configuration\"][starts-with(@AutomationId,\"groupBox\")]/Pane[starts-with(@AutomationId,\"panel\")]/RadioButton[@Name=\"Burst\"][starts-with(@AutomationId,\"radioButton\")]";
            var winElem_LeftClickRadioButtonBurst_8_13 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickRadioButtonBurst_8_13);
            if (winElem_LeftClickRadioButtonBurst_8_13 != null)
            {
                winElem_LeftClickRadioButtonBurst_8_13.Click();
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickRadioButtonBurst_8_13}");
                return;
            }


            // LeftClick on Button "Up" at (14,5)
            Console.WriteLine("LeftClick on Button \"Up\" at (14,5)");
            string xpath_LeftClickButtonUp_14_5 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 1\"]/Window[@Name=\"SnapShot\"][@AutomationId=\"ConfigurationForm\"]/Pane[starts-with(@AutomationId,\"tableLayoutPanel\")]/Group[@Name=\"Capture configuration\"][starts-with(@AutomationId,\"groupBox\")]/Spinner[@Name=\"Duration:\"][starts-with(@AutomationId,\"numericUpDown\")]/Spinner[starts-with(@ClassName,\"WindowsForms10\")][@Name=\"UpDown\"]/Button[@Name=\"Up\"]";
            var winElem_LeftClickButtonUp_14_5 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickButtonUp_14_5);
            if (winElem_LeftClickButtonUp_14_5 != null)
            {
                winElem_LeftClickButtonUp_14_5.Click();
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickButtonUp_14_5}");
                return;
            }


            // LeftClick on Button "Up" at (12,3)
            Console.WriteLine("LeftClick on Button \"Up\" at (12,3)");
            string xpath_LeftClickButtonUp_12_3 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 1\"]/Window[@Name=\"SnapShot\"][@AutomationId=\"ConfigurationForm\"]/Pane[starts-with(@AutomationId,\"tableLayoutPanel\")]/Group[@Name=\"Capture configuration\"][starts-with(@AutomationId,\"groupBox\")]/Spinner[@Name=\"Burst period:\"][starts-with(@AutomationId,\"numericUpDown\")]/Spinner[starts-with(@ClassName,\"WindowsForms10\")][@Name=\"UpDown\"]/Button[@Name=\"Up\"]";
            var winElem_LeftClickButtonUp_12_3 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickButtonUp_12_3);
            if (winElem_LeftClickButtonUp_12_3 != null)
            {
                winElem_LeftClickButtonUp_12_3.Click();
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickButtonUp_12_3}");
                return;
            }


            // LeftClick on Button "Up" at (8,2)
            Console.WriteLine("LeftClick on Button \"Up\" at (8,2)");
            string xpath_LeftClickButtonUp_8_2 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 1\"]/Window[@Name=\"SnapShot\"][@AutomationId=\"ConfigurationForm\"]/Pane[starts-with(@AutomationId,\"tableLayoutPanel\")]/Group[@Name=\"Capture configuration\"][starts-with(@AutomationId,\"groupBox\")]/Spinner[@Name=\"Duration:\"][starts-with(@AutomationId,\"numericUpDown\")]/Spinner[starts-with(@ClassName,\"WindowsForms10\")][@Name=\"UpDown\"]/Button[@Name=\"Up\"]";
            var winElem_LeftClickButtonUp_8_2 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickButtonUp_8_2);
            if (winElem_LeftClickButtonUp_8_2 != null)
            {
                winElem_LeftClickButtonUp_8_2.Click();
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickButtonUp_8_2}");
                return;
            }


            // LeftClick on Button "Down" at (12,5)
            Console.WriteLine("LeftClick on Button \"Down\" at (12,5)");
            string xpath_LeftClickButtonDown_12_5 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 1\"]/Window[@Name=\"SnapShot\"][@AutomationId=\"ConfigurationForm\"]/Pane[starts-with(@AutomationId,\"tableLayoutPanel\")]/Group[@Name=\"Capture configuration\"][starts-with(@AutomationId,\"groupBox\")]/Spinner[@Name=\"Burst period:\"][starts-with(@AutomationId,\"numericUpDown\")]/Spinner[starts-with(@ClassName,\"WindowsForms10\")][@Name=\"UpDown\"]/Button[@Name=\"Down\"]";
            var winElem_LeftClickButtonDown_12_5 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickButtonDown_12_5);
            if (winElem_LeftClickButtonDown_12_5 != null)
            {
                winElem_LeftClickButtonDown_12_5.Click();
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickButtonDown_12_5}");
                return;
            }



        }
    }

   }