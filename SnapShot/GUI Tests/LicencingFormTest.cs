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
    }
}
