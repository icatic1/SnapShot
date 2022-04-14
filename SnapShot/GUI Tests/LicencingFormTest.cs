using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows;
using System;
using Xamarin.Essentials;

namespace GUI_Tests
{
    [TestClass]
    public class LicencingFormTest : TestSession
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
        public void TestCheckLicenceStatus()
        {
            // RightClick on Edit "" at (201,14)
            Console.WriteLine("RightClick on Edit \"\" at (201,14)");
            string xpath_RightClickEdit_201_14 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 2\"]/Window[@Name=\"SnapShot\"][@AutomationId=\"LicencingForm\"]/Group[@Name=\"Licencing information\"][starts-with(@AutomationId,\"groupBox\")]/Edit[starts-with(@AutomationId,\"textBox\")]";
            var winElem_RightClickEdit_201_14 = desktopSession.FindElementByAbsoluteXPath(xpath_RightClickEdit_201_14);
            if (winElem_RightClickEdit_201_14 != null)
            {
                desktopSession.Mouse.MouseMove(winElem_RightClickEdit_201_14.Coordinates);
                desktopSession.Mouse.ContextClick(null);
            }
            else
            {
                string message = $"Failed to find element using xpath: {xpath_RightClickEdit_201_14}";
                Console.WriteLine(message);
                Assert.Fail(message);
                return;
            }

            //winElem_RightClickEdit_201_14.GetAttribute;

            // LeftClick on MenuItem "Copy" at (39,1)
            Console.WriteLine("LeftClick on MenuItem \"Copy\" at (39,1)");
            string xpath_LeftClickMenuItemCopy_39_1 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 2\"]/Menu[@ClassName=\"#32768\"][@Name=\"Context\"]/MenuItem[@Name=\"Copy\"]";
            var winElem_LeftClickMenuItemCopy_39_1 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickMenuItemCopy_39_1);
            if (winElem_LeftClickMenuItemCopy_39_1 != null)
            {
                winElem_LeftClickMenuItemCopy_39_1.Click();
            }
            else
            {
                string message = $"Failed to find element using xpath: {xpath_LeftClickMenuItemCopy_39_1}";
                Console.WriteLine(message);
                Assert.Fail(message);
                return;
            }




            /*if (Clipboard.HasText)
            {
                var text = Clipboard.GetTextAsync();
                Assert.AreEqual("Licence check has not been performed.", text);

            }
            else
            {
                Assert.Fail("Empty clipboard");
            }*/
        }

        [TestMethod]
        public void TestCheckConnectioStatus()
        {
            // LeftClick on Button "Connect" at (82,19)
            Console.WriteLine("LeftClick on Button \"Connect\" at (82,19)");
            string xpath_LeftClickButtonConnect_82_19 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 2\"]/Window[@Name=\"SnapShot\"][@AutomationId=\"LicencingForm\"]/Group[@Name=\"Connection information\"][starts-with(@AutomationId,\"groupBox\")]/Button[@Name=\"Connect\"][starts-with(@AutomationId,\"button\")]";
            var winElem_LeftClickButtonConnect_82_19 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickButtonConnect_82_19);
            if (winElem_LeftClickButtonConnect_82_19 != null)
            {
                winElem_LeftClickButtonConnect_82_19.Click();
            }
            else
            {
                string message = $"Failed to find element using xpath: {xpath_LeftClickButtonConnect_82_19}";
                Console.WriteLine(message);
                Assert.Fail(message);
                return;
            }

        }

        [TestMethod]
        public void TestDebugLoggingStatus()
        {
            // LeftClick on CheckBox "Debug logging" at (5,7)
            Console.WriteLine("LeftClick on CheckBox \"Debug logging\" at (5,7)");
            string xpath_LeftClickCheckBoxDebugloggi_5_7 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 2\"]/Window[@Name=\"SnapShot\"][@AutomationId=\"LicencingForm\"]/Group[@Name=\"Terminal information\"][starts-with(@AutomationId,\"groupBox\")]/CheckBox[@Name=\"Debug logging\"][starts-with(@AutomationId,\"checkBox\")]";
            var winElem_LeftClickCheckBoxDebugloggi_5_7 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickCheckBoxDebugloggi_5_7);
            if (winElem_LeftClickCheckBoxDebugloggi_5_7 != null)
            {
                winElem_LeftClickCheckBoxDebugloggi_5_7.Click();
            }
            else
            {
                string message = $"Failed to find element using xpath: {xpath_LeftClickCheckBoxDebugloggi_5_7}";
                Console.WriteLine(message);
                Assert.Fail(message);
                return;
            }

        }

        [TestMethod]
        public void TestOpenConfigurationForm()
        {
            // LeftClick on MenuItem "Configuration" at (39,16)
            Console.WriteLine("LeftClick on MenuItem \"Configuration\" at (39,16)");
            string xpath_LeftClickMenuItemConfigurat_39_16 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 2\"]/Window[@Name=\"SnapShot\"][@AutomationId=\"LicencingForm\"]/MenuBar[@Name=\"menuStrip1\"][starts-with(@AutomationId,\"menuStrip\")]/MenuItem[@Name=\"Configuration\"]";
            var winElem_LeftClickMenuItemConfigurat_39_16 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickMenuItemConfigurat_39_16);
            if (winElem_LeftClickMenuItemConfigurat_39_16 != null)
            {
                winElem_LeftClickMenuItemConfigurat_39_16.Click();
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickMenuItemConfigurat_39_16}");
                return;
            }


            // LeftClick on MenuItem "Licence" at (44,13)
            Console.WriteLine("LeftClick on MenuItem \"Licence\" at (44,13)");
            string xpath_LeftClickMenuItemLicence_44_13 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 2\"]/Window[@Name=\"SnapShot\"][@AutomationId=\"ConfigurationForm\"]/MenuBar[@Name=\"menuStrip1\"][starts-with(@AutomationId,\"menuStrip\")]/MenuItem[@Name=\"Licence\"]";
            var winElem_LeftClickMenuItemLicence_44_13 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickMenuItemLicence_44_13);
            if (winElem_LeftClickMenuItemLicence_44_13 != null)
            {
                winElem_LeftClickMenuItemLicence_44_13.Click();
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickMenuItemLicence_44_13}");
                return;
            }








        }

        [TestMethod]
        public void TestOpenHelpForm()
        {
            // LeftClick on MenuItem "Help" at (9,2)
            Console.WriteLine("LeftClick on MenuItem \"Help\" at (9,2)");
            string xpath_LeftClickMenuItemHelp_9_2 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 2\"]/Window[@Name=\"SnapShot\"][@AutomationId=\"LicencingForm\"]/MenuBar[@Name=\"menuStrip1\"][starts-with(@AutomationId,\"menuStrip\")]/MenuItem[@Name=\"Help\"]";
            var winElem_LeftClickMenuItemHelp_9_2 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickMenuItemHelp_9_2);
            if (winElem_LeftClickMenuItemHelp_9_2 != null)
            {
                winElem_LeftClickMenuItemHelp_9_2.Click();
            }
            else
            {
                string message = $"Failed to find element using xpath: {xpath_LeftClickMenuItemHelp_9_2}";
                Console.WriteLine(message);
                Assert.Fail(message);
                return;
            }



        }
    }
}
