using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows;
using System;
using Xamarin.Essentials;
using System.IO;
using OpenQA.Selenium;

namespace GUI_Tests
{
    [TestClass]
    public class TestCameraConfiguration : TestSession
    {
        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            Setup(context);
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            TearDown();
        }


        [TestMethod]
        public void Test1_Camera1And2Set()
        {
            // LeftClick on Button "Save configuration" at (69,3)
            Console.WriteLine("LeftClick on Button \"Save configuration\" at (69,3)");
            string xpath_LeftClickButtonSaveconfig_69_3 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 1\"]/Window[@Name=\"SnapShot\"][@AutomationId=\"ConfigurationForm\"]/Pane[starts-with(@AutomationId,\"tableLayoutPanel\")]/Button[@Name=\"Save configuration\"][starts-with(@AutomationId,\"button\")]";
            var winElem_LeftClickButtonSaveconfig_69_3 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickButtonSaveconfig_69_3);
            if (winElem_LeftClickButtonSaveconfig_69_3 != null)
            {
                winElem_LeftClickButtonSaveconfig_69_3.Click();
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickButtonSaveconfig_69_3}");
                return;
            }


            // LeftClick on Edit "Trigger file path:" at (187,2)
            Console.WriteLine("LeftClick on Edit \"Trigger file path:\" at (187,2)");
            string xpath_LeftClickEditTriggerfil_187_2 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 1\"]/Window[@Name=\"SnapShot\"][@AutomationId=\"ConfigurationForm\"]/Pane[starts-with(@AutomationId,\"tableLayoutPanel\")]/Group[@Name=\"Device configuration\"][starts-with(@AutomationId,\"groupBox\")]/Edit[@Name=\"Trigger file path:\"][starts-with(@AutomationId,\"textBox\")]";
            var winElem_LeftClickEditTriggerfil_187_2 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickEditTriggerfil_187_2);
            if (winElem_LeftClickEditTriggerfil_187_2 != null)
            {
                winElem_LeftClickEditTriggerfil_187_2.Click();
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickEditTriggerfil_187_2}");
                return;
            }


            // LeftClick on RadioButton "Camera 2" at (4,11)
            Console.WriteLine("LeftClick on RadioButton \"Camera 2\" at (4,11)");
            string xpath_LeftClickRadioButtonCamera2_4_11 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 1\"]/Window[@Name=\"SnapShot\"][@AutomationId=\"ConfigurationForm\"]/Pane[starts-with(@AutomationId,\"tableLayoutPanel\")]/Pane[starts-with(@AutomationId,\"panel\")]/RadioButton[@Name=\"Camera 2\"][starts-with(@AutomationId,\"radioButton\")]";
            var winElem_LeftClickRadioButtonCamera2_4_11 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickRadioButtonCamera2_4_11);
            if (winElem_LeftClickRadioButtonCamera2_4_11 != null)
            {
                winElem_LeftClickRadioButtonCamera2_4_11.Click();
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickRadioButtonCamera2_4_11}");
                return;
            }


            // LeftClick on RadioButton "Camera 1" at (10,14)
            Console.WriteLine("LeftClick on RadioButton \"Camera 1\" at (10,14)");
            string xpath_LeftClickRadioButtonCamera1_10_14 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 1\"]/Window[@Name=\"SnapShot\"][@AutomationId=\"ConfigurationForm\"]/Pane[starts-with(@AutomationId,\"tableLayoutPanel\")]/Pane[starts-with(@AutomationId,\"panel\")]/RadioButton[@Name=\"Camera 1\"][starts-with(@AutomationId,\"radioButton\")]";
            var winElem_LeftClickRadioButtonCamera1_10_14 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickRadioButtonCamera1_10_14);
            if (winElem_LeftClickRadioButtonCamera1_10_14 != null)
            {
                winElem_LeftClickRadioButtonCamera1_10_14.Click();
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickRadioButtonCamera1_10_14}");
                return;
            }
        }

        [TestMethod]
        public void SaveCamera2Then1()
        {
            //testing situation when camera 1 is not saved
            // LeftClick on Edit "Trigger file path:" at (34,11)
            Console.WriteLine("LeftClick on Edit \"Trigger file path:\" at (34,11)");
            string xpath_LeftClickEditTriggerfil_34_11 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 1\"]/Window[@Name=\"SnapShot\"][@AutomationId=\"ConfigurationForm\"]/Pane[starts-with(@AutomationId,\"tableLayoutPanel\")]/Group[@Name=\"Device configuration\"][starts-with(@AutomationId,\"groupBox\")]/Edit[@Name=\"Trigger file path:\"][starts-with(@AutomationId,\"textBox\")]";
            var winElem_LeftClickEditTriggerfil_34_11 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickEditTriggerfil_34_11);
            if (winElem_LeftClickEditTriggerfil_34_11 != null)
            {
                winElem_LeftClickEditTriggerfil_34_11.Click();
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickEditTriggerfil_34_11}");
                return;
            }


            // KeyboardInput VirtualKeys=""regex1"" CapsLock=False NumLock=True ScrollLock=False
            Console.WriteLine("KeyboardInput VirtualKeys=\"\"regex1\"\" CapsLock=False NumLock=True ScrollLock=False");
            System.Threading.Thread.Sleep(100);
            winElem_LeftClickEditTriggerfil_34_11.SendKeys("regex1");


            // LeftClick on Edit "Device type:" at (60,13)
            Console.WriteLine("LeftClick on Edit \"Device type:\" at (60,13)");
            string xpath_LeftClickEditDevicetype_60_13 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 1\"]/Window[@Name=\"SnapShot\"][@AutomationId=\"ConfigurationForm\"]/Pane[starts-with(@AutomationId,\"tableLayoutPanel\")]/Group[@Name=\"Device configuration\"][starts-with(@AutomationId,\"groupBox\")]/Edit[@Name=\"Device type:\"][starts-with(@AutomationId,\"textBox\")]";
            var winElem_LeftClickEditDevicetype_60_13 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickEditDevicetype_60_13);
            if (winElem_LeftClickEditDevicetype_60_13 != null)
            {
                winElem_LeftClickEditDevicetype_60_13.Click();
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickEditDevicetype_60_13}");
                return;
            }


            // KeyboardInput VirtualKeys=""somewhere"" CapsLock=False NumLock=True ScrollLock=False
            Console.WriteLine("KeyboardInput VirtualKeys=\"\"somewhere\"\" CapsLock=False NumLock=True ScrollLock=False");
            System.Threading.Thread.Sleep(100);
            winElem_LeftClickEditDevicetype_60_13.SendKeys("somewhere");


            // LeftClick on Group "Video configuration" at (203,71)
            Console.WriteLine("LeftClick on Group \"Video configuration\" at (203,71)");
            string xpath_LeftClickGroupVideoconfi_203_71 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 1\"]/Window[@Name=\"SnapShot\"][@AutomationId=\"ConfigurationForm\"]/Pane[starts-with(@AutomationId,\"tableLayoutPanel\")]/Group[@Name=\"Video configuration\"][starts-with(@AutomationId,\"groupBox\")]";
            var winElem_LeftClickGroupVideoconfi_203_71 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickGroupVideoconfi_203_71);
            if (winElem_LeftClickGroupVideoconfi_203_71 != null)
            {
                winElem_LeftClickGroupVideoconfi_203_71.Click();
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickGroupVideoconfi_203_71}");
                return;
            }


            // LeftClick on Slider "Contrast:" at (56,16)
            Console.WriteLine("LeftClick on Slider \"Contrast:\" at (56,16)");
            string xpath_LeftClickSliderContrast_56_16 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 1\"]/Window[@Name=\"SnapShot\"][@AutomationId=\"ConfigurationForm\"]/Pane[starts-with(@AutomationId,\"tableLayoutPanel\")]/Group[@Name=\"Video configuration\"][starts-with(@AutomationId,\"groupBox\")]/Slider[@Name=\"Contrast:\"][starts-with(@AutomationId,\"trackBar\")]";
            var winElem_LeftClickSliderContrast_56_16 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickSliderContrast_56_16);
            if (winElem_LeftClickSliderContrast_56_16 != null)
            {
                winElem_LeftClickSliderContrast_56_16.Click();
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickSliderContrast_56_16}");
                return;
            }


            // LeftClick on RadioButton "Camera 2" at (6,10)
            Console.WriteLine("LeftClick on RadioButton \"Camera 2\" at (6,10)");
            string xpath_LeftClickRadioButtonCamera2_6_10 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 1\"]/Window[@Name=\"SnapShot\"][@AutomationId=\"ConfigurationForm\"]/Pane[starts-with(@AutomationId,\"tableLayoutPanel\")]/Pane[starts-with(@AutomationId,\"panel\")]/RadioButton[@Name=\"Camera 2\"][starts-with(@AutomationId,\"radioButton\")]";
            var winElem_LeftClickRadioButtonCamera2_6_10 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickRadioButtonCamera2_6_10);
            if (winElem_LeftClickRadioButtonCamera2_6_10 != null)
            {
                winElem_LeftClickRadioButtonCamera2_6_10.Click();
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickRadioButtonCamera2_6_10}");
                return;
            }


            // LeftClick on Edit "Trigger file path:" at (92,10)
            Console.WriteLine("LeftClick on Edit \"Trigger file path:\" at (92,10)");
            string xpath_LeftClickEditTriggerfil_92_10 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 1\"]/Window[@Name=\"SnapShot\"][@AutomationId=\"ConfigurationForm\"]/Pane[starts-with(@AutomationId,\"tableLayoutPanel\")]/Group[@Name=\"Device configuration\"][starts-with(@AutomationId,\"groupBox\")]/Edit[@Name=\"Trigger file path:\"][starts-with(@AutomationId,\"textBox\")]";
            var winElem_LeftClickEditTriggerfil_92_10 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickEditTriggerfil_92_10);
            if (winElem_LeftClickEditTriggerfil_92_10 != null)
            {
                winElem_LeftClickEditTriggerfil_92_10.Click();
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickEditTriggerfil_92_10}");
                return;
            }


            // KeyboardInput VirtualKeys=""trigger"" CapsLock=False NumLock=True ScrollLock=False
            Console.WriteLine("KeyboardInput VirtualKeys=\"\"trigger\"\" CapsLock=False NumLock=True ScrollLock=False");
            System.Threading.Thread.Sleep(100);
            winElem_LeftClickEditTriggerfil_92_10.SendKeys("trigger");


            // KeyboardInput VirtualKeys=""regex"" CapsLock=False NumLock=True ScrollLock=False
            Console.WriteLine("KeyboardInput VirtualKeys=\"\"regex\"\" CapsLock=False NumLock=True ScrollLock=False");
            System.Threading.Thread.Sleep(100);
            winElem_LeftClickEditTriggerfil_92_10.SendKeys("regex");


            // LeftClick on Edit "Trigger regex:" at (57,9)
            Console.WriteLine("LeftClick on Edit \"Trigger regex:\" at (57,9)");
            string xpath_LeftClickEditTriggerreg_57_9 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 1\"]/Window[@Name=\"SnapShot\"][@AutomationId=\"ConfigurationForm\"]/Pane[starts-with(@AutomationId,\"tableLayoutPanel\")]/Group[@Name=\"Device configuration\"][starts-with(@AutomationId,\"groupBox\")]/Edit[@Name=\"Trigger regex:\"][starts-with(@AutomationId,\"textBox\")]";
            var winElem_LeftClickEditTriggerreg_57_9 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickEditTriggerreg_57_9);
            if (winElem_LeftClickEditTriggerreg_57_9 != null)
            {
                winElem_LeftClickEditTriggerreg_57_9.Click();
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickEditTriggerreg_57_9}");
                return;
            }


            // KeyboardInput VirtualKeys=""somewhere"" CapsLock=False NumLock=True ScrollLock=False
            Console.WriteLine("KeyboardInput VirtualKeys=\"\"somewhere\"\" CapsLock=False NumLock=True ScrollLock=False");
            System.Threading.Thread.Sleep(100);
            winElem_LeftClickEditTriggerreg_57_9.SendKeys("somewhere");


            // LeftClick on Edit "Device type:" at (50,15)
            Console.WriteLine("LeftClick on Edit \"Device type:\" at (50,15)");
            string xpath_LeftClickEditDevicetype_50_15 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 1\"]/Window[@Name=\"SnapShot\"][@AutomationId=\"ConfigurationForm\"]/Pane[starts-with(@AutomationId,\"tableLayoutPanel\")]/Group[@Name=\"Device configuration\"][starts-with(@AutomationId,\"groupBox\")]/Edit[@Name=\"Device type:\"][starts-with(@AutomationId,\"textBox\")]";
            var winElem_LeftClickEditDevicetype_50_15 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickEditDevicetype_50_15);
            if (winElem_LeftClickEditDevicetype_50_15 != null)
            {
                winElem_LeftClickEditDevicetype_50_15.Click();
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickEditDevicetype_50_15}");
                return;
            }


            // LeftClick on Button "Save configuration" at (82,16)
            Console.WriteLine("LeftClick on Button \"Save configuration\" at (82,16)");
            string xpath_LeftClickButtonSaveconfig_82_16 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 1\"]/Window[@Name=\"SnapShot\"][@AutomationId=\"ConfigurationForm\"]/Pane[starts-with(@AutomationId,\"tableLayoutPanel\")]/Button[@Name=\"Save configuration\"][starts-with(@AutomationId,\"button\")]";
            var winElem_LeftClickButtonSaveconfig_82_16 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickButtonSaveconfig_82_16);
            if (winElem_LeftClickButtonSaveconfig_82_16 != null)
            {
                winElem_LeftClickButtonSaveconfig_82_16.Click();
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickButtonSaveconfig_82_16}");
                return;
            }


            // LeftClick on RadioButton "Camera 2" at (6,14)
            Console.WriteLine("LeftClick on RadioButton \"Camera 2\" at (6,14)");
            string xpath_LeftClickRadioButtonCamera2_6_14 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 1\"]/Window[@Name=\"SnapShot\"][@AutomationId=\"ConfigurationForm\"]/Pane[starts-with(@AutomationId,\"tableLayoutPanel\")]/Pane[starts-with(@AutomationId,\"panel\")]/RadioButton[@Name=\"Camera 2\"][starts-with(@AutomationId,\"radioButton\")]";
            var winElem_LeftClickRadioButtonCamera2_6_14 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickRadioButtonCamera2_6_14);
            if (winElem_LeftClickRadioButtonCamera2_6_14 != null)
            {
                winElem_LeftClickRadioButtonCamera2_6_14.Click();
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickRadioButtonCamera2_6_14}");
                return;
            }



        }

        [TestMethod]
        public void TestAll3CamerasSave()
        {
            // LeftClick on RadioButton "Camera 2" at (4,16)
            Console.WriteLine("LeftClick on RadioButton \"Camera 2\" at (4,16)");
            string xpath_LeftClickRadioButtonCamera2_4_16 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 1\"]/Window[@Name=\"SnapShot\"][@AutomationId=\"ConfigurationForm\"]/Pane[starts-with(@AutomationId,\"tableLayoutPanel\")]/Pane[starts-with(@AutomationId,\"panel\")]/RadioButton[@Name=\"Camera 2\"][starts-with(@AutomationId,\"radioButton\")]";
            var winElem_LeftClickRadioButtonCamera2_4_16 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickRadioButtonCamera2_4_16);
            if (winElem_LeftClickRadioButtonCamera2_4_16 != null)
            {
                winElem_LeftClickRadioButtonCamera2_4_16.Click();
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickRadioButtonCamera2_4_16}");
                return;
            }


            // LeftClick on RadioButton "Camera 3" at (8,13)
            Console.WriteLine("LeftClick on RadioButton \"Camera 3\" at (8,13)");
            string xpath_LeftClickRadioButtonCamera3_8_13 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 1\"]/Window[@Name=\"SnapShot\"][@AutomationId=\"ConfigurationForm\"]/Pane[starts-with(@AutomationId,\"tableLayoutPanel\")]/Pane[starts-with(@AutomationId,\"panel\")]/RadioButton[@Name=\"Camera 3\"][starts-with(@AutomationId,\"radioButton\")]";
            var winElem_LeftClickRadioButtonCamera3_8_13 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickRadioButtonCamera3_8_13);
            if (winElem_LeftClickRadioButtonCamera3_8_13 != null)
            {
                winElem_LeftClickRadioButtonCamera3_8_13.Click();
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickRadioButtonCamera3_8_13}");
                return;
            }


            // LeftClick on Group "Video configuration" at (135,112)
            Console.WriteLine("LeftClick on Group \"Video configuration\" at (135,112)");
            string xpath_LeftClickGroupVideoconfi_135_112 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 1\"]/Window[@Name=\"SnapShot\"][@AutomationId=\"ConfigurationForm\"]/Pane[starts-with(@AutomationId,\"tableLayoutPanel\")]/Group[@Name=\"Video configuration\"][starts-with(@AutomationId,\"groupBox\")]";
            var winElem_LeftClickGroupVideoconfi_135_112 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickGroupVideoconfi_135_112);
            if (winElem_LeftClickGroupVideoconfi_135_112 != null)
            {
                winElem_LeftClickGroupVideoconfi_135_112.Click();
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickGroupVideoconfi_135_112}");
                return;
            }


            // LeftClick on RadioButton "Camera 2" at (6,3)
            Console.WriteLine("LeftClick on RadioButton \"Camera 2\" at (6,3)");
            string xpath_LeftClickRadioButtonCamera2_6_3 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 1\"]/Window[@Name=\"SnapShot\"][@AutomationId=\"ConfigurationForm\"]/Pane[starts-with(@AutomationId,\"tableLayoutPanel\")]/Pane[starts-with(@AutomationId,\"panel\")]/RadioButton[@Name=\"Camera 2\"][starts-with(@AutomationId,\"radioButton\")]";
            var winElem_LeftClickRadioButtonCamera2_6_3 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickRadioButtonCamera2_6_3);
            if (winElem_LeftClickRadioButtonCamera2_6_3 != null)
            {
                winElem_LeftClickRadioButtonCamera2_6_3.Click();
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickRadioButtonCamera2_6_3}");
                return;
            }


            // LeftClick on RadioButton "Single" at (1,2)
            Console.WriteLine("LeftClick on RadioButton \"Single\" at (1,2)");
            string xpath_LeftClickRadioButtonSingle_1_2 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 1\"]/Window[@Name=\"SnapShot\"][@AutomationId=\"ConfigurationForm\"]/Pane[starts-with(@AutomationId,\"tableLayoutPanel\")]/Group[@Name=\"Capture configuration\"][starts-with(@AutomationId,\"groupBox\")]/Pane[starts-with(@AutomationId,\"panel\")]/RadioButton[@Name=\"Single\"][starts-with(@AutomationId,\"radioButton\")]";
            var winElem_LeftClickRadioButtonSingle_1_2 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickRadioButtonSingle_1_2);
            if (winElem_LeftClickRadioButtonSingle_1_2 != null)
            {
                winElem_LeftClickRadioButtonSingle_1_2.Click();
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickRadioButtonSingle_1_2}");
                return;
            }


            // LeftClick on Button "Save configuration" at (54,24)
            Console.WriteLine("LeftClick on Button \"Save configuration\" at (54,24)");
            string xpath_LeftClickButtonSaveconfig_54_24 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 1\"]/Window[@Name=\"SnapShot\"][@AutomationId=\"ConfigurationForm\"]/Pane[starts-with(@AutomationId,\"tableLayoutPanel\")]/Button[@Name=\"Save configuration\"][starts-with(@AutomationId,\"button\")]";
            var winElem_LeftClickButtonSaveconfig_54_24 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickButtonSaveconfig_54_24);
            if (winElem_LeftClickButtonSaveconfig_54_24 != null)
            {
                winElem_LeftClickButtonSaveconfig_54_24.Click();
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickButtonSaveconfig_54_24}");
                return;
            }


            // LeftClick on RadioButton "Camera 3" at (10,16)
            Console.WriteLine("LeftClick on RadioButton \"Camera 3\" at (10,16)");
            string xpath_LeftClickRadioButtonCamera3_10_16 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 1\"]/Window[@Name=\"SnapShot\"][@AutomationId=\"ConfigurationForm\"]/Pane[starts-with(@AutomationId,\"tableLayoutPanel\")]/Pane[starts-with(@AutomationId,\"panel\")]/RadioButton[@Name=\"Camera 3\"][starts-with(@AutomationId,\"radioButton\")]";
            var winElem_LeftClickRadioButtonCamera3_10_16 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickRadioButtonCamera3_10_16);
            if (winElem_LeftClickRadioButtonCamera3_10_16 != null)
            {
                winElem_LeftClickRadioButtonCamera3_10_16.Click();
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickRadioButtonCamera3_10_16}");
                return;
            }


            // LeftClick on RadioButton "Camera 2" at (6,11)
            Console.WriteLine("LeftClick on RadioButton \"Camera 2\" at (6,11)");
            string xpath_LeftClickRadioButtonCamera2_6_11 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 1\"]/Window[@Name=\"SnapShot\"][@AutomationId=\"ConfigurationForm\"]/Pane[starts-with(@AutomationId,\"tableLayoutPanel\")]/Pane[starts-with(@AutomationId,\"panel\")]/RadioButton[@Name=\"Camera 2\"][starts-with(@AutomationId,\"radioButton\")]";
            var winElem_LeftClickRadioButtonCamera2_6_11 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickRadioButtonCamera2_6_11);
            if (winElem_LeftClickRadioButtonCamera2_6_11 != null)
            {
                winElem_LeftClickRadioButtonCamera2_6_11.Click();
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickRadioButtonCamera2_6_11}");
                return;
            }
        }

    }
}
