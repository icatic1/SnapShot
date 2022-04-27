using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows;
using System;
using Xamarin.Essentials;
using System.IO;
using OpenQA.Selenium;

namespace GUI_Tests
{
    [TestClass]
    public class HelpFormTest : TestSession
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
        public void Test1_CheckHelpButton()
        {
            // LeftClick on MenuItem "Help" at (34,12)
            Console.WriteLine("LeftClick on MenuItem \"Help\" at (34,12)");
            string xpath_LeftClickMenuItemHelp_34_12 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 1\"]/Window[@Name=\"SnapShot\"][@AutomationId=\"LicencingForm\"]/MenuBar[@Name=\"menuStrip1\"][starts-with(@AutomationId,\"menuStrip\")]/MenuItem[@Name=\"Help\"]";
            var winElem_LeftClickMenuItemHelp_34_12 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickMenuItemHelp_34_12);
            if (winElem_LeftClickMenuItemHelp_34_12 != null)
            {
                winElem_LeftClickMenuItemHelp_34_12.Click();
                // LeftClick on MenuItem "Licence" at (34,13)
                Console.WriteLine("LeftClick on MenuItem \"Licence\" at (34,13)");
                string xpath_LeftClickMenuItemLicence_34_13 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 1\"]/Window[@Name=\"SnapShot\"][@AutomationId=\"InformationForm\"]/MenuBar[@Name=\"menuStrip1\"][starts-with(@AutomationId,\"menuStrip\")]/MenuItem[@Name=\"Licence\"]";
                var winElem_LeftClickMenuItemLicence_34_13 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickMenuItemLicence_34_13);
                winElem_LeftClickMenuItemLicence_34_13.Click();
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickMenuItemHelp_34_12}");
                Assert.Fail();
                return;
            }
        }


        [TestMethod]
        public void Test2_CheckTitle()
        {
            // LeftClick on MenuItem "Help" at (23,19)
            Console.WriteLine("LeftClick on MenuItem \"Help\" at (23,19)");
            string xpath_LeftClickMenuItemHelp_23_19 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 1\"]/Window[@Name=\"SnapShot\"][@AutomationId=\"InformationForm\"]/MenuBar[@Name=\"menuStrip1\"][starts-with(@AutomationId,\"menuStrip\")]/MenuItem[@Name=\"Help\"]";
            var winElem_LeftClickMenuItemHelp_23_19 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickMenuItemHelp_23_19);
            if (winElem_LeftClickMenuItemHelp_23_19 != null)
            {
                winElem_LeftClickMenuItemHelp_23_19.Click();
                var label2 = desktopSession.FindElementByAccessibilityId("label2");
                Assert.AreEqual("SnapShot, version: 0.2", label2.Text);
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickMenuItemHelp_23_19}");
                return;
            }
        }

        [TestMethod]
        public void Test3_CheckLabel()
        {
            // LeftClick on MenuItem "Help" at (34,12)
            Console.WriteLine("LeftClick on MenuItem \"Help\" at (34,12)");
            string xpath_LeftClickMenuItemHelp_34_12 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 1\"]/Window[@Name=\"SnapShot\"][@AutomationId=\"LicencingForm\"]/MenuBar[@Name=\"menuStrip1\"][starts-with(@AutomationId,\"menuStrip\")]/MenuItem[@Name=\"Help\"]";
            var winElem_LeftClickMenuItemHelp_34_12 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickMenuItemHelp_34_12);
            if (winElem_LeftClickMenuItemHelp_34_12 != null)
            {
                winElem_LeftClickMenuItemHelp_34_12.Click();
                var label1 = desktopSession.FindElementByAccessibilityId("label1");
                string text = @"This app was created by a team of students at the Faculty of Electrical Engineering Sarajevo.
Contact us at sbehic2@etf.unsa.ba if you encounter any problems with the application.";
                Assert.AreEqual( text, label1.Text);
                // LeftClick on MenuItem "Licence" at (34,13)
                Console.WriteLine("LeftClick on MenuItem \"Licence\" at (34,13)");
                string xpath_LeftClickMenuItemLicence_34_13 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 1\"]/Window[@Name=\"SnapShot\"][@AutomationId=\"InformationForm\"]/MenuBar[@Name=\"menuStrip1\"][starts-with(@AutomationId,\"menuStrip\")]/MenuItem[@Name=\"Licence\"]";
                var winElem_LeftClickMenuItemLicence_34_13 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickMenuItemLicence_34_13);
                winElem_LeftClickMenuItemLicence_34_13.Click();
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickMenuItemHelp_34_12}");
                Assert.Fail();
                return;
            }
        }
        
    }
}
