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
            var tb1 = desktopSession.FindElementByAccessibilityId("textBox1");
            Assert.IsNotNull(tb1);
            Assert.AreEqual("Licence check has not been performed.", tb1.Text);
        }

        [TestMethod]
        public void TestCheckLicenceStatus()
        {
            var checkLicenceButton = desktopSession.FindElementByAccessibilityId("button1");
            Assert.IsNotNull(checkLicenceButton);
            checkLicenceButton.Click();

            var tb1 = desktopSession.FindElementByAccessibilityId("textBox1");
            Assert.IsNotNull(tb1);
            Assert.AreEqual("Unfortunately, this machine has not been licenced yet. Contact us at icatic1@etf.unsa.ba to get your licence.", tb1.Text);
        }

        [TestMethod]
        public void TestConnectionStatus()
        {
            var label3 = desktopSession.FindElementByAccessibilityId("label3");
            Assert.IsNotNull(label3);
            Assert.AreEqual("Not checked", label3.Text);

            var label7 = desktopSession.FindElementByAccessibilityId("label7");
            Assert.IsNotNull(label7);
            Assert.AreEqual("Disconnected", label7.Text);
        }

        [TestMethod]
        public void TestCheckConnectioStatus()
        {
                        
            var checkConnectionButton = desktopSession.FindElementByAccessibilityId("button1");
            Assert.IsNotNull(checkConnectionButton);
            checkConnectionButton.Click();

            var label3 = desktopSession.FindElementByAccessibilityId("label3");
            Assert.IsNotNull(label3);
            Assert.AreEqual("Demo version", label3.Text);

            var textBox1 = desktopSession.FindElementByAccessibilityId("textBox1");
            Assert.IsNotNull(textBox1);
            Assert.AreEqual("Unfortunately, this machine has not been licenced yet. Contact us at icatic1@etf.unsa.ba to get your licence.", textBox1.Text);

        }

        [TestMethod]
        public void TestLoggingStatus()
        {
            var textBox2 = desktopSession.FindElementByAccessibilityId("textBox2");
            Assert.IsNotNull(textBox2);
            Assert.AreEqual(Environment.MachineName.ToString(), textBox2.Text);

            var checkBox1 = desktopSession.FindElementByAccessibilityId("checkBox1");
            Assert.IsNotNull(checkBox1);
            //Assert not checked

        }

        [TestMethod]
        public void TestDebugLoggingStatus()
        {
            var checkBox1 = desktopSession.FindElementByAccessibilityId("checkBox1");
            Assert.IsNotNull(checkBox1);
            checkBox1.Click();
            //Assert checked
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
