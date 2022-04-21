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
        public void TestLicenceStatus()
        {
            var tb = desktopSession.FindElementByAccessibilityId("textBox1");
            Assert.IsNotNull(tb);
            Assert.AreEqual("Licence check has not been performed.", tb.Text);
        }

        [TestMethod]
        public void TestCheckLicenceStatus()
        {
            // LeftClick on Button "Check my licence status" at (62,10)
            /*Console.WriteLine("LeftClick on Button \"Check my licence status\" at (62,10)");
            string xpath_LeftClickButtonCheckmylic_62_10 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 2\"]/Window[@Name=\"SnapShot\"][@AutomationId=\"LicencingForm\"]/Group[@Name=\"Licencing information\"][starts-with(@AutomationId,\"groupBox\")]/Button[@Name=\"Check my licence status\"][starts-with(@AutomationId,\"button\")]";
            var winElem_LeftClickButtonCheckmylic_62_10 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickButtonCheckmylic_62_10);
            if (winElem_LeftClickButtonCheckmylic_62_10 != null)
            {
                winElem_LeftClickButtonCheckmylic_62_10.Click();
            }
            else
            {
                string message = $"Failed to find element using xpath: {xpath_LeftClickButtonCheckmylic_62_10}";
                Console.WriteLine(message);
                Assert.Fail(message);
                return;
            }*/

            var checkLicenceButton = desktopSession.FindElementByAccessibilityId("button1");
            Assert.IsNotNull(checkLicenceButton);
            checkLicenceButton.Click();

            var tb = desktopSession.FindElementByAccessibilityId("textBox1");
            Assert.IsNotNull(tb);
            Assert.AreEqual("Unfortunately, this machine has not been licenced yet. Contact us at icatic1@etf.unsa.ba to get your licence.", tb.Text);
        }

        [TestMethod]
        public void TestCheckConnectioStatus()
        {

            var label7 = desktopSession.FindElementByAccessibilityId("label7");
            Assert.IsNotNull(label7);
            Assert.AreEqual("Disconnected", label7.Text);


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

            
            Assert.AreEqual("Connected", label7.Text);

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
