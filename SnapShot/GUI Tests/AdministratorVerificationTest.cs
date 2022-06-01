using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GUI_Tests
{
    [TestClass]
    public class AdministratorVerificationTest : TestSession
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
        public void Test1_Help()
        {
            var help = desktopSession.FindElementByName("Help");
            Assert.IsNotNull(help);
            help.Click();

            var text = desktopSession.FindElementByName("You cannot exit this form without being licenced first!");
            Assert.IsNotNull(text);
            Assert.AreEqual(text.Text, "You cannot exit this form without being licenced first!");

            var okbtn = desktopSession.FindElementByName("OK");
            Assert.IsNotNull(okbtn);
            okbtn.Click();
        }


        [TestMethod]
        public void Test2_Configuration()
        {
            var config = desktopSession.FindElementByName("Configuration");
            Assert.IsNotNull(config);
            config.Click();

            var gs = desktopSession.FindElementByName("General settings");
            Assert.IsNotNull(gs);
            gs.Click();

            var text = desktopSession.FindElementByName("You cannot access general settings without being licenced first!");
            Assert.IsNotNull(text);
            Assert.AreEqual(text.Text, "You cannot access general settings without being licenced first!");

            var okbtn = desktopSession.FindElementByName("OK");
            Assert.IsNotNull(okbtn);
            okbtn.Click();

            config.Click();

            var cs = desktopSession.FindElementByName("Camera settings");
            Assert.IsNotNull(cs);
            cs.Click();

            var c1 = desktopSession.FindElementByName("Camera 1");
            Assert.IsNotNull(c1);
            c1.Click();

            var text2 = desktopSession.FindElementByName("You cannot access camera settings without being licenced first!");
            Assert.IsNotNull(text2);
            Assert.AreEqual(text2.Text, "You cannot access camera settings without being licenced first!");

            okbtn = desktopSession.FindElementByName("OK");
            Assert.IsNotNull(okbtn);
            okbtn.Click();
        }

        [TestMethod]
        public void Test3_Admin_registration()
        {
            var licence = desktopSession.FindElementByName("Licence");
            Assert.IsNotNull(licence);
            licence.Click();

            var admin_options = desktopSession.FindElementByName("Administrator Options");
            Assert.IsNotNull(admin_options);
            admin_options.Click();

            //Thread.Sleep(1000);
            var tb1 = desktopSession.FindElementByAccessibilityId("textBox1");
            Assert.IsNotNull(tb1);
            tb1.SendKeys("administrator");

            var tb2 = desktopSession.FindElementByAccessibilityId("textBox2");
            Assert.IsNotNull(tb2);
            tb2.SendKeys("administrator");

            var ok = desktopSession.FindElementByAccessibilityId("button2");
            Assert.IsNotNull(ok);
            ok.Click();

            ok.Click();

            var config = desktopSession.FindElementByName("Configuration");
            Assert.IsNotNull(config);
            config.Click();

            var gs = desktopSession.FindElementByName("General settings");
            Assert.IsNotNull(gs);
            gs.Click();

            var combo = desktopSession.FindElementByAccessibilityId("comboBox1");
            Assert.IsNotNull(combo);
            combo.Click();
            
        }

        [TestMethod]
        public void Test4_general_settings()
        {
            var config = desktopSession.FindElementByName("Configuration");
            Assert.IsNotNull(config);
            config.Click();

            var gs = desktopSession.FindElementByName("General settings");
            Assert.IsNotNull(gs);
            gs.Click();

            // LeftClick on CheckBox "File system trigger" at (11,5)
            Console.WriteLine("LeftClick on CheckBox \"File system trigger\" at (11,5)");
            string xpath_LeftClickCheckBoxFilesystem_11_5 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 2\"]/Window[@Name=\"SnapShot\"][@AutomationId=\"GeneralSettingsForm\"]/Pane[starts-with(@AutomationId,\"tableLayoutPanel\")]/Group[@Name=\"Device configuration\"][starts-with(@AutomationId,\"groupBox\")]/Group[@Name=\"Trigger type\"][starts-with(@AutomationId,\"groupBox\")]/CheckBox[@Name=\"File system trigger\"][starts-with(@AutomationId,\"checkBox\")]";
            var winElem_LeftClickCheckBoxFilesystem_11_5 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickCheckBoxFilesystem_11_5);
            if (winElem_LeftClickCheckBoxFilesystem_11_5 != null)
            {
                winElem_LeftClickCheckBoxFilesystem_11_5.Click();
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickCheckBoxFilesystem_11_5}");
                return;
            }


            // KeyboardInput VirtualKeys="Keys.Backspace + Keys.BackspaceKeys.Backspace + Keys.BackspaceKeys.Backspace + Keys.BackspaceKeys.Backspace + Keys.BackspaceKeys.Backspace + Keys.BackspaceKeys.Backspace + Keys.BackspaceKeys.LeftShift + "[" + Keys.LeftShiftKeys.NumberPad0 + Keys.NumberPad0Keys.Subtract + Keys.SubtractKeys.NumberPad9 + Keys.NumberPad9"]"" CapsLock=False NumLock=True ScrollLock=False
            Console.WriteLine("KeyboardInput VirtualKeys=\"Keys.Backspace + Keys.BackspaceKeys.Backspace + Keys.BackspaceKeys.Backspace + Keys.BackspaceKeys.Backspace + Keys.BackspaceKeys.Backspace + Keys.BackspaceKeys.Backspace + Keys.BackspaceKeys.LeftShift + \"[\" + Keys.LeftShiftKeys.NumberPad0 + Keys.NumberPad0Keys.Subtract + Keys.SubtractKeys.NumberPad9 + Keys.NumberPad9\"]\"\" CapsLock=False NumLock=True ScrollLock=False");
            System.Threading.Thread.Sleep(100);
            winElem_LeftClickCheckBoxFilesystem_11_5.SendKeys(Keys.Backspace + Keys.Backspace);
            winElem_LeftClickCheckBoxFilesystem_11_5.SendKeys(Keys.Backspace + Keys.Backspace);
            winElem_LeftClickCheckBoxFilesystem_11_5.SendKeys(Keys.Backspace + Keys.Backspace);
            winElem_LeftClickCheckBoxFilesystem_11_5.SendKeys(Keys.Backspace + Keys.Backspace);
            winElem_LeftClickCheckBoxFilesystem_11_5.SendKeys(Keys.Backspace + Keys.Backspace);
            winElem_LeftClickCheckBoxFilesystem_11_5.SendKeys(Keys.Backspace + Keys.Backspace);
            winElem_LeftClickCheckBoxFilesystem_11_5.SendKeys(Keys.LeftShift + "[" + Keys.LeftShift);
            winElem_LeftClickCheckBoxFilesystem_11_5.SendKeys(Keys.NumberPad0 + Keys.NumberPad0);
            winElem_LeftClickCheckBoxFilesystem_11_5.SendKeys(Keys.Subtract + Keys.Subtract);
            winElem_LeftClickCheckBoxFilesystem_11_5.SendKeys(Keys.NumberPad9 + Keys.NumberPad9);
            winElem_LeftClickCheckBoxFilesystem_11_5.SendKeys("]");


            // LeftClick on Edit "Trigger regex:" at (52,11)
            Console.WriteLine("LeftClick on Edit \"Trigger regex:\" at (52,11)");
            string xpath_LeftClickEditTriggerreg_52_11 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 2\"]/Window[@Name=\"SnapShot\"][@AutomationId=\"GeneralSettingsForm\"]/Pane[starts-with(@AutomationId,\"tableLayoutPanel\")]/Group[@Name=\"Device configuration\"][starts-with(@AutomationId,\"groupBox\")]/Group[@Name=\"Trigger type\"][starts-with(@AutomationId,\"groupBox\")]/Edit[@Name=\"Trigger regex:\"][starts-with(@AutomationId,\"textBox\")]";
            var winElem_LeftClickEditTriggerreg_52_11 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickEditTriggerreg_52_11);
            if (winElem_LeftClickEditTriggerreg_52_11 != null)
            {
                winElem_LeftClickEditTriggerreg_52_11.Click();
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickEditTriggerreg_52_11}");
                return;
            }


            // LeftClick on CheckBox "Face detection trigger" at (61,12)
            Console.WriteLine("LeftClick on CheckBox \"Face detection trigger\" at (61,12)");
            string xpath_LeftClickCheckBoxFacedetect_61_12 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 2\"]/Window[@Name=\"SnapShot\"][@AutomationId=\"GeneralSettingsForm\"]/Pane[starts-with(@AutomationId,\"tableLayoutPanel\")]/Group[@Name=\"Device configuration\"][starts-with(@AutomationId,\"groupBox\")]/Group[@Name=\"Trigger type\"][starts-with(@AutomationId,\"groupBox\")]/CheckBox[@Name=\"Face detection trigger\"][starts-with(@AutomationId,\"checkBox\")]";
            var winElem_LeftClickCheckBoxFacedetect_61_12 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickCheckBoxFacedetect_61_12);
            if (winElem_LeftClickCheckBoxFacedetect_61_12 != null)
            {
                winElem_LeftClickCheckBoxFacedetect_61_12.Click();
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickCheckBoxFacedetect_61_12}");
                return;
            }


            // LeftClick on Button "Up" at (12,7)
            Console.WriteLine("LeftClick on Button \"Up\" at (12,7)");
            string xpath_LeftClickButtonUp_12_7 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 2\"]/Window[@Name=\"SnapShot\"][@AutomationId=\"GeneralSettingsForm\"]/Pane[starts-with(@AutomationId,\"tableLayoutPanel\")]/Group[@Name=\"Device configuration\"][starts-with(@AutomationId,\"groupBox\")]/Spinner[@Name=\"Keep capture for:\"][starts-with(@AutomationId,\"numericUpDown\")]/Spinner[starts-with(@ClassName,\"WindowsForms10\")][@Name=\"UpDown\"]/Button[@Name=\"Up\"]";
            var winElem_LeftClickButtonUp_12_7 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickButtonUp_12_7);
            if (winElem_LeftClickButtonUp_12_7 != null)
            {
                winElem_LeftClickButtonUp_12_7.Click();
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickButtonUp_12_7}");
                return;
            }


            // LeftClick on RadioButton "every" at (1,13)
            Console.WriteLine("LeftClick on RadioButton \"every\" at (1,13)");
            string xpath_LeftClickRadioButtonevery_1_13 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 2\"]/Window[@Name=\"SnapShot\"][@AutomationId=\"GeneralSettingsForm\"]/Pane[starts-with(@AutomationId,\"tableLayoutPanel\")]/Group[@Name=\"Synchronization settings\"][starts-with(@AutomationId,\"groupBox\")]/Group[@Name=\"Configuration synchronization\"][starts-with(@AutomationId,\"groupBox\")]/Pane[starts-with(@AutomationId,\"panel\")]/RadioButton[@Name=\"every\"][starts-with(@AutomationId,\"radioButton\")]";
            var winElem_LeftClickRadioButtonevery_1_13 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickRadioButtonevery_1_13);
            if (winElem_LeftClickRadioButtonevery_1_13 != null)
            {
                winElem_LeftClickRadioButtonevery_1_13.Click();
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickRadioButtonevery_1_13}");
                return;
            }


            // KeyboardInput VirtualKeys="Keys.Backspace + Keys.BackspaceKeys.NumberPad1 + Keys.NumberPad1Keys.NumberPad0 + Keys.NumberPad0Keys.Backspace + Keys.BackspaceKeys.NumberPad0 + Keys.NumberPad0" CapsLock=False NumLock=True ScrollLock=False
            Console.WriteLine("KeyboardInput VirtualKeys=\"Keys.Backspace + Keys.BackspaceKeys.NumberPad1 + Keys.NumberPad1Keys.NumberPad0 + Keys.NumberPad0Keys.Backspace + Keys.BackspaceKeys.NumberPad0 + Keys.NumberPad0\" CapsLock=False NumLock=True ScrollLock=False");
            System.Threading.Thread.Sleep(100);
            winElem_LeftClickRadioButtonevery_1_13.SendKeys(Keys.Backspace + Keys.Backspace);
            winElem_LeftClickRadioButtonevery_1_13.SendKeys(Keys.NumberPad1 + Keys.NumberPad1);
            winElem_LeftClickRadioButtonevery_1_13.SendKeys(Keys.NumberPad0 + Keys.NumberPad0);
            winElem_LeftClickRadioButtonevery_1_13.SendKeys(Keys.Backspace + Keys.Backspace);
            winElem_LeftClickRadioButtonevery_1_13.SendKeys(Keys.NumberPad0 + Keys.NumberPad0);



        }


    }
}
