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
        public void TestCheckConnectioStatus ()
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
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickButtonConnect_82_19}");
                return;
            }


            // LeftClick on Button "Close" at (5,15)
            Console.WriteLine("LeftClick on Button \"Close\" at (5,15)");
            string xpath_LeftClickButtonClose_5_15 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 2\"]/Window[@Name=\"SnapShot\"][@AutomationId=\"LicencingForm\"]/TitleBar[@AutomationId=\"TitleBar\"]/Button[@Name=\"Close\"]";
            var winElem_LeftClickButtonClose_5_15 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickButtonClose_5_15);
            if (winElem_LeftClickButtonClose_5_15 != null)
            {
                winElem_LeftClickButtonClose_5_15.Click();
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickButtonClose_5_15}");
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
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickCheckBoxDebugloggi_5_7}");
                return;
            }


            // LeftClick on Button "Close" at (32,10)
            Console.WriteLine("LeftClick on Button \"Close\" at (32,10)");
            string xpath_LeftClickButtonClose_32_10 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 2\"]/Window[@Name=\"SnapShot\"][@AutomationId=\"LicencingForm\"]/TitleBar[@AutomationId=\"TitleBar\"]/Button[@Name=\"Close\"]";
            var winElem_LeftClickButtonClose_32_10 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickButtonClose_32_10);
            if (winElem_LeftClickButtonClose_32_10 != null)
            {
                winElem_LeftClickButtonClose_32_10.Click();
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickButtonClose_32_10}");
                return;
            }



        }
    }
}
