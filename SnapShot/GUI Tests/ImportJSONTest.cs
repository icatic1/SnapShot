using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows;
using System;
using Xamarin.Essentials;
using System.IO;
using OpenQA.Selenium;
using Newtonsoft.Json;

namespace GUI_Tests
{
    [TestClass]
    public class ImportJSONTest : TestSession
    {
        public const string TESTING_DIRECTORY = @"C:\SnapShotTestingGround";

        private static string CreateJSON(string triggerFilePath, string triggerRegex, string outputPath,
            int keepCapture, bool motionDetection, string serverVersion, string ip, int serverPort, bool connectionStatus,
            bool imageCapture, bool singleMode, int duration, int burstPeriod)
        {
            string EXPORT = "";
            EXPORT += "{\n";
            EXPORT += "\t\"cameras\":\n";
            EXPORT += "\t[\n";

            for (int i = 0; i < 3; i++)
            {
                EXPORT += "\t\t{\n";

                EXPORT += "\t\t\t\"device_configuration\":\n";
                EXPORT += "\t\t\t{\n";
                EXPORT += "\t\t\t\t\"device_type\": \"" + "USBCamera" + "\",\n";
                EXPORT += "\t\t\t\t\"id\": \"" + "\",\n";
                EXPORT += "\t\t\t\t\"trigger_file_path\": \"" + triggerFilePath + "\",\n";
                EXPORT += "\t\t\t\t\"regex\": \"" + triggerRegex + "\", \n";
                EXPORT += "\t\t\t\t\"output_folder_path\": \"" + outputPath + "\",\n";
                EXPORT += "\t\t\t\t\"output_validity_days\": \"" + keepCapture + "\",\n";
                EXPORT += "\t\t\t\t\"camera_number\": \"" + "0" + "\"\n";
                EXPORT += "\t\t\t},\n";

                EXPORT += "\t\t\t\"video_configuration\":\n";
                EXPORT += "\t\t\t{\n";
                EXPORT += "\t\t\t\t\"resolution\": \"" + "Resolution640x480" + "\",\n";
                EXPORT += "\t\t\t\t\"contrast_level\": \"" + "0" + "\",\n";
                EXPORT += "\t\t\t\t\"image_color\": \"" + "Control" + "\",\n";
                EXPORT += "\t\t\t\t\"motion_detection\": \"" + motionDetection.ToString() + "\"\n";
                EXPORT += "\t\t\t},\n";

                EXPORT += "\t\t\t\"network_configuration\":\n";
                EXPORT += "\t\t\t{\n";
                EXPORT += "\t\t\t\t\"server_version\": \"" + "\",\n";
                EXPORT += "\t\t\t\t\"server_IP_address\": \"" + "\",\n";
                EXPORT += "\t\t\t\t\"server_port\": \"" + serverPort + "\",\n";
                EXPORT += "\t\t\t\t\"connection_status\": \"" + connectionStatus.ToString() + "\"\n";
                EXPORT += "\t\t\t},\n";

                EXPORT += "\t\t\t\"capture_configuration\":\n";
                EXPORT += "\t\t\t{\n";
                EXPORT += "\t\t\t\t\"image_capture\": \"" + imageCapture.ToString() + "\",\n";
                EXPORT += "\t\t\t\t\"single_mode\": \"" + singleMode.ToString() + "\",\n";
                EXPORT += "\t\t\t\t\"duration\": \"" + duration + "\",\n";
                EXPORT += "\t\t\t\t\"burst_period\": \"" + burstPeriod + "\"\n";
                EXPORT += "\t\t\t}\n";

                EXPORT += "\t\t}";
                if (i < 2)
                    EXPORT += ",";
                EXPORT += "\n";
            }
            EXPORT += "\t]\n";
            EXPORT += "}";

            return EXPORT;
        }

        private static string CreateInvalidJSON(string triggerFilePath, string triggerRegex, string outputPath,
            int keepCapture, bool motionDetection, string serverVersion, string ip, int serverPort, bool connectionStatus,
            bool imageCapture, bool singleMode, int duration, int burstPeriod)
        {
            string EXPORT = "";
            EXPORT += "{\n";
            EXPORT += "\t\"cameras\":\n";
            EXPORT += "\t[\n";

            for (int i = 0; i < 3; i++)
            {
                EXPORT += "\t\t{\n";

                //Device configuration is missing

                EXPORT += "\t\t\t\"video_configuration\":\n";
                EXPORT += "\t\t\t{\n";
                EXPORT += "\t\t\t\t\"resolution\": \"" + "Resolution640x480" + "\",\n";
                EXPORT += "\t\t\t\t\"contrast_level\": \"" + "0" + "\",\n";
                EXPORT += "\t\t\t\t\"image_color\": \"" + "Control" + "\",\n";
                EXPORT += "\t\t\t\t\"motion_detection\": \"" + motionDetection.ToString() + "\"\n";
                EXPORT += "\t\t\t},\n";

                EXPORT += "\t\t\t\"network_configuration\":\n";
                EXPORT += "\t\t\t{\n";
                EXPORT += "\t\t\t\t\"server_version\": \"" + "\",\n";
                EXPORT += "\t\t\t\t\"server_IP_address\": \"" + "\",\n";
                EXPORT += "\t\t\t\t\"server_port\": \"" + serverPort + "\",\n";
                EXPORT += "\t\t\t\t\"connection_status\": \"" + connectionStatus.ToString() + "\"\n";
                EXPORT += "\t\t\t},\n";

                EXPORT += "\t\t\t\"capture_configuration\":\n";
                EXPORT += "\t\t\t{\n";
                EXPORT += "\t\t\t\t\"image_capture\": \"" + imageCapture.ToString() + "\",\n";
                EXPORT += "\t\t\t\t\"single_mode\": \"" + singleMode.ToString() + "\",\n";
                EXPORT += "\t\t\t\t\"duration\": \"" + duration + "\",\n";
                EXPORT += "\t\t\t\t\"burst_period\": \"" + burstPeriod + "\"\n";
                EXPORT += "\t\t\t}\n";

                EXPORT += "\t\t}";
                if (i < 2)
                    EXPORT += ",";
                EXPORT += "\n";
            }
            EXPORT += "\t]\n";
            EXPORT += "}";

            return EXPORT;
        }

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            Setup(context);

            if (!Directory.Exists(TESTING_DIRECTORY))
            {
                Directory.CreateDirectory(TESTING_DIRECTORY);
            }

        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            //Directory.Delete(TESTING_DIRECTORY, true);

            TearDown();
        }

        [TestMethod]
        public void ImportEmptyJSON()
        {
            //var x = File.Create(TESTING_DIRECTORY + @"\configuration.json");
            File.WriteAllText(TESTING_DIRECTORY + @"\postavke.json", CreateJSON("", "", "", 0, false, "", "", 0, false, true, true, 0, 0));

            // LeftClick on MenuItem "Configuration" at (52,8)
            Console.WriteLine("LeftClick on MenuItem \"Configuration\" at (52,8)");
            string xpath_LeftClickMenuItemConfigurat_52_8 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 2\"]/Window[@Name=\"SnapShot\"][@AutomationId=\"LicencingForm\"]/MenuBar[@Name=\"menuStrip1\"][starts-with(@AutomationId,\"menuStrip\")]/MenuItem[@Name=\"Configuration\"]";
            var winElem_LeftClickMenuItemConfigurat_52_8 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickMenuItemConfigurat_52_8);
            if (winElem_LeftClickMenuItemConfigurat_52_8 != null)
            {
                winElem_LeftClickMenuItemConfigurat_52_8.Click();
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickMenuItemConfigurat_52_8}");
                return;
            }


            // LeftDblClick on MenuItem "Configuration" at (63,14)
            Console.WriteLine("LeftDblClick on MenuItem \"Configuration\" at (63,14)");
            string xpath_LeftDblClickMenuItemConfigurat_63_14 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 2\"]/Window[@Name=\"SnapShot\"][@AutomationId=\"ConfigurationForm\"]/MenuBar[@Name=\"menuStrip1\"][starts-with(@AutomationId,\"menuStrip\")]/MenuItem[@Name=\"Configuration\"]";
            var winElem_LeftDblClickMenuItemConfigurat_63_14 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftDblClickMenuItemConfigurat_63_14);
            if (winElem_LeftDblClickMenuItemConfigurat_63_14 != null)
            {
                desktopSession.Mouse.MouseMove(winElem_LeftDblClickMenuItemConfigurat_63_14.Coordinates);
                desktopSession.Mouse.Click(null);
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftDblClickMenuItemConfigurat_63_14}");
                return;
            }


            // LeftClick on MenuItem "Import from JSON" at (63,5)
            Console.WriteLine("LeftClick on MenuItem \"Import from JSON\" at (63,5)");
            string xpath_LeftClickMenuItemImportfrom_63_5 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 2\"]/Window[@Name=\"SnapShot\"][@AutomationId=\"ConfigurationForm\"]/Menu[starts-with(@ClassName,\"WindowsForms10\")][@Name=\"Configuration\"]/MenuItem[@Name=\"Import from JSON\"]";
            var winElem_LeftClickMenuItemImportfrom_63_5 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickMenuItemImportfrom_63_5);
            if (winElem_LeftClickMenuItemImportfrom_63_5 != null)
            {
                winElem_LeftClickMenuItemImportfrom_63_5.Click();
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickMenuItemImportfrom_63_5}");
                return;
            }

            // LeftDblClick on Edit "Name" at (62,8)
            Console.WriteLine("LeftDblClick on Edit \"Name\" at (62,8)");
            string xpath_LeftDblClickEditName_62_8 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 2\"]/Window[@Name=\"SnapShot\"][@AutomationId=\"ConfigurationForm\"]/Window[@ClassName=\"#32770\"][@Name=\"Open\"]/Pane[@ClassName=\"DUIViewWndClassName\"]/Pane[@Name=\"Shell Folder View\"][@AutomationId=\"listview\"]/List[@ClassName=\"UIItemsView\"][@Name=\"Items View\"]/ListItem[@ClassName=\"UIItem\"][@Name=\"SnapShotTestingGround\"]/Edit[@Name=\"Name\"][@AutomationId=\"System.ItemNameDisplay\"]";
            var winElem_LeftDblClickEditName_62_8 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftDblClickEditName_62_8);
            if (winElem_LeftDblClickEditName_62_8 != null)
            {
                desktopSession.Mouse.MouseMove(winElem_LeftDblClickEditName_62_8.Coordinates);
                desktopSession.Mouse.DoubleClick(null);
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftDblClickEditName_62_8}");
                return;
            }

            // LeftDblClick on Edit "Name" at (47,5)
            Console.WriteLine("LeftDblClick on Edit \"Name\" at (47,5)");
            string xpath_LeftDblClickEditName_47_5 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 2\"]/Window[@Name=\"SnapShot\"][@AutomationId=\"ConfigurationForm\"]/Window[@ClassName=\"#32770\"][@Name=\"Open\"]/Pane[@ClassName=\"DUIViewWndClassName\"]/Pane[@Name=\"Shell Folder View\"][@AutomationId=\"listview\"]/List[@ClassName=\"UIItemsView\"][@Name=\"Items View\"]/ListItem[@ClassName=\"UIItem\"][@Name=\"postavke\"]/Edit[@Name=\"Name\"][@AutomationId=\"System.ItemNameDisplay\"]";
            var winElem_LeftDblClickEditName_47_5 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftDblClickEditName_47_5);
            if (winElem_LeftDblClickEditName_47_5 != null)
            {
                desktopSession.Mouse.MouseMove(winElem_LeftDblClickEditName_47_5.Coordinates);
                desktopSession.Mouse.DoubleClick(null);
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftDblClickEditName_47_5}");
                return;
            }

            var textBox1 = desktopSession.FindElementByAccessibilityId("textBox1");
            Assert.IsNotNull(textBox1);
            Assert.AreEqual("", textBox1.Text.Trim());

            var textBox6 = desktopSession.FindElementByAccessibilityId("textBox6");
            Assert.IsNotNull(textBox6);
            Assert.AreEqual("", textBox6.Text.Trim());

            var textBox2 = desktopSession.FindElementByAccessibilityId("textBox2");
            Assert.IsNotNull(textBox2);
            Assert.AreEqual("", textBox2.Text.Trim());


        }

        [TestMethod]
        public void ImportNonEmptyJSON()
        {
            //var x = File.Create(TESTING_DIRECTORY + @"\configuration.json");
            File.WriteAllText(TESTING_DIRECTORY + @"\postavke.json", CreateJSON("Just", "testing", "stuff", 3, true, "", "", 4, true, false, true, 4, 2));
            Console.WriteLine("LeftClick on MenuItem \"Configuration\" at (52,8)");
            string xpath_LeftClickMenuItemConfigurat_52_8 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 2\"]/Window[@Name=\"SnapShot\"][@AutomationId=\"LicencingForm\"]/MenuBar[@Name=\"menuStrip1\"][starts-with(@AutomationId,\"menuStrip\")]/MenuItem[@Name=\"Configuration\"]";
            var winElem_LeftClickMenuItemConfigurat_52_8 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickMenuItemConfigurat_52_8);
            if (winElem_LeftClickMenuItemConfigurat_52_8 != null)
            {
                winElem_LeftClickMenuItemConfigurat_52_8.Click();
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickMenuItemConfigurat_52_8}");
                return;
            }


            // LeftDblClick on MenuItem "Configuration" at (63,14)
            Console.WriteLine("LeftDblClick on MenuItem \"Configuration\" at (63,14)");
            string xpath_LeftDblClickMenuItemConfigurat_63_14 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 2\"]/Window[@Name=\"SnapShot\"][@AutomationId=\"ConfigurationForm\"]/MenuBar[@Name=\"menuStrip1\"][starts-with(@AutomationId,\"menuStrip\")]/MenuItem[@Name=\"Configuration\"]";
            var winElem_LeftDblClickMenuItemConfigurat_63_14 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftDblClickMenuItemConfigurat_63_14);
            if (winElem_LeftDblClickMenuItemConfigurat_63_14 != null)
            {
                desktopSession.Mouse.MouseMove(winElem_LeftDblClickMenuItemConfigurat_63_14.Coordinates);
                desktopSession.Mouse.Click(null);
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftDblClickMenuItemConfigurat_63_14}");
                return;
            }


            // LeftClick on MenuItem "Import from JSON" at (63,5)
            Console.WriteLine("LeftClick on MenuItem \"Import from JSON\" at (63,5)");
            string xpath_LeftClickMenuItemImportfrom_63_5 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 2\"]/Window[@Name=\"SnapShot\"][@AutomationId=\"ConfigurationForm\"]/Menu[starts-with(@ClassName,\"WindowsForms10\")][@Name=\"Configuration\"]/MenuItem[@Name=\"Import from JSON\"]";
            var winElem_LeftClickMenuItemImportfrom_63_5 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickMenuItemImportfrom_63_5);
            if (winElem_LeftClickMenuItemImportfrom_63_5 != null)
            {
                winElem_LeftClickMenuItemImportfrom_63_5.Click();
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickMenuItemImportfrom_63_5}");
                return;
            }


            // LeftDblClick on Edit "Name" at (62,8)
            Console.WriteLine("LeftDblClick on Edit \"Name\" at (62,8)");
            string xpath_LeftDblClickEditName_62_8 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 2\"]/Window[@Name=\"SnapShot\"][@AutomationId=\"ConfigurationForm\"]/Window[@ClassName=\"#32770\"][@Name=\"Open\"]/Pane[@ClassName=\"DUIViewWndClassName\"]/Pane[@Name=\"Shell Folder View\"][@AutomationId=\"listview\"]/List[@ClassName=\"UIItemsView\"][@Name=\"Items View\"]/ListItem[@ClassName=\"UIItem\"][@Name=\"SnapShotTestingGround\"]/Edit[@Name=\"Name\"][@AutomationId=\"System.ItemNameDisplay\"]";
            var winElem_LeftDblClickEditName_62_8 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftDblClickEditName_62_8);
            if (winElem_LeftDblClickEditName_62_8 != null)
            {
                desktopSession.Mouse.MouseMove(winElem_LeftDblClickEditName_62_8.Coordinates);
                desktopSession.Mouse.DoubleClick(null);
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftDblClickEditName_62_8}");
                return;
            }


            // LeftDblClick on Edit "Name" at (47,5)
            Console.WriteLine("LeftDblClick on Edit \"Name\" at (47,5)");
            string xpath_LeftDblClickEditName_47_5 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 2\"]/Window[@Name=\"SnapShot\"][@AutomationId=\"ConfigurationForm\"]/Window[@ClassName=\"#32770\"][@Name=\"Open\"]/Pane[@ClassName=\"DUIViewWndClassName\"]/Pane[@Name=\"Shell Folder View\"][@AutomationId=\"listview\"]/List[@ClassName=\"UIItemsView\"][@Name=\"Items View\"]/ListItem[@ClassName=\"UIItem\"][@Name=\"postavke\"]/Edit[@Name=\"Name\"][@AutomationId=\"System.ItemNameDisplay\"]";
            var winElem_LeftDblClickEditName_47_5 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftDblClickEditName_47_5);
            if (winElem_LeftDblClickEditName_47_5 != null)
            {
                desktopSession.Mouse.MouseMove(winElem_LeftDblClickEditName_47_5.Coordinates);
                desktopSession.Mouse.DoubleClick(null);
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftDblClickEditName_47_5}");
                return;
            }

            var textBox1 = desktopSession.FindElementByAccessibilityId("textBox1");
            Assert.IsNotNull(textBox1);
            Assert.AreEqual("Just", textBox1.Text.Trim());

            var textBox6 = desktopSession.FindElementByAccessibilityId("textBox6");
            Assert.IsNotNull(textBox6);
            Assert.AreEqual("testing", textBox6.Text.Trim());

            var textBox2 = desktopSession.FindElementByAccessibilityId("textBox2");
            Assert.IsNotNull(textBox2);
            Assert.AreEqual("stuff", textBox2.Text.Trim());
        }

        [TestMethod]
        public void ImportInvalidJSON()
        {
            //var x = File.Create(TESTING_DIRECTORY + @"\configuration.json");
            File.WriteAllText(TESTING_DIRECTORY + @"\postavke.json", CreateInvalidJSON("This", "shouldn't", "appear", 0, false, "", "", 0, false, true, true, 0, 0));

            // LeftClick on MenuItem "Configuration" at (52,8)
            Console.WriteLine("LeftClick on MenuItem \"Configuration\" at (52,8)");
            string xpath_LeftClickMenuItemConfigurat_52_8 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 2\"]/Window[@Name=\"SnapShot\"][@AutomationId=\"LicencingForm\"]/MenuBar[@Name=\"menuStrip1\"][starts-with(@AutomationId,\"menuStrip\")]/MenuItem[@Name=\"Configuration\"]";
            var winElem_LeftClickMenuItemConfigurat_52_8 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickMenuItemConfigurat_52_8);
            if (winElem_LeftClickMenuItemConfigurat_52_8 != null)
            {
                winElem_LeftClickMenuItemConfigurat_52_8.Click();
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickMenuItemConfigurat_52_8}");
                return;
            }


            // LeftDblClick on MenuItem "Configuration" at (63,14)
            Console.WriteLine("LeftDblClick on MenuItem \"Configuration\" at (63,14)");
            string xpath_LeftDblClickMenuItemConfigurat_63_14 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 2\"]/Window[@Name=\"SnapShot\"][@AutomationId=\"ConfigurationForm\"]/MenuBar[@Name=\"menuStrip1\"][starts-with(@AutomationId,\"menuStrip\")]/MenuItem[@Name=\"Configuration\"]";
            var winElem_LeftDblClickMenuItemConfigurat_63_14 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftDblClickMenuItemConfigurat_63_14);
            if (winElem_LeftDblClickMenuItemConfigurat_63_14 != null)
            {
                desktopSession.Mouse.MouseMove(winElem_LeftDblClickMenuItemConfigurat_63_14.Coordinates);
                desktopSession.Mouse.Click(null);
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftDblClickMenuItemConfigurat_63_14}");
                return;
            }


            // LeftClick on MenuItem "Import from JSON" at (63,5)
            Console.WriteLine("LeftClick on MenuItem \"Import from JSON\" at (63,5)");
            string xpath_LeftClickMenuItemImportfrom_63_5 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 2\"]/Window[@Name=\"SnapShot\"][@AutomationId=\"ConfigurationForm\"]/Menu[starts-with(@ClassName,\"WindowsForms10\")][@Name=\"Configuration\"]/MenuItem[@Name=\"Import from JSON\"]";
            var winElem_LeftClickMenuItemImportfrom_63_5 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickMenuItemImportfrom_63_5);
            if (winElem_LeftClickMenuItemImportfrom_63_5 != null)
            {
                winElem_LeftClickMenuItemImportfrom_63_5.Click();
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickMenuItemImportfrom_63_5}");
                return;
            }

            // LeftDblClick on Edit "Name" at (62,8)
            Console.WriteLine("LeftDblClick on Edit \"Name\" at (62,8)");
            string xpath_LeftDblClickEditName_62_8 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 2\"]/Window[@Name=\"SnapShot\"][@AutomationId=\"ConfigurationForm\"]/Window[@ClassName=\"#32770\"][@Name=\"Open\"]/Pane[@ClassName=\"DUIViewWndClassName\"]/Pane[@Name=\"Shell Folder View\"][@AutomationId=\"listview\"]/List[@ClassName=\"UIItemsView\"][@Name=\"Items View\"]/ListItem[@ClassName=\"UIItem\"][@Name=\"SnapShotTestingGround\"]/Edit[@Name=\"Name\"][@AutomationId=\"System.ItemNameDisplay\"]";
            var winElem_LeftDblClickEditName_62_8 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftDblClickEditName_62_8);
            if (winElem_LeftDblClickEditName_62_8 != null)
            {
                desktopSession.Mouse.MouseMove(winElem_LeftDblClickEditName_62_8.Coordinates);
                desktopSession.Mouse.DoubleClick(null);
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftDblClickEditName_62_8}");
                return;
            }

            // LeftDblClick on Edit "Name" at (47,5)
            Console.WriteLine("LeftDblClick on Edit \"Name\" at (47,5)");
            string xpath_LeftDblClickEditName_47_5 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 2\"]/Window[@Name=\"SnapShot\"][@AutomationId=\"ConfigurationForm\"]/Window[@ClassName=\"#32770\"][@Name=\"Open\"]/Pane[@ClassName=\"DUIViewWndClassName\"]/Pane[@Name=\"Shell Folder View\"][@AutomationId=\"listview\"]/List[@ClassName=\"UIItemsView\"][@Name=\"Items View\"]/ListItem[@ClassName=\"UIItem\"][@Name=\"postavke\"]/Edit[@Name=\"Name\"][@AutomationId=\"System.ItemNameDisplay\"]";
            var winElem_LeftDblClickEditName_47_5 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftDblClickEditName_47_5);
            if (winElem_LeftDblClickEditName_47_5 != null)
            {
                desktopSession.Mouse.MouseMove(winElem_LeftDblClickEditName_47_5.Coordinates);
                desktopSession.Mouse.DoubleClick(null);
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftDblClickEditName_47_5}");
                return;
            }

            var textBox1 = desktopSession.FindElementByAccessibilityId("textBox1");
            Assert.IsNotNull(textBox1);
            Assert.AreEqual("", textBox1.Text.Trim());

            var textBox6 = desktopSession.FindElementByAccessibilityId("textBox6");
            Assert.IsNotNull(textBox6);
            Assert.AreEqual("", textBox6.Text.Trim());

            var textBox2 = desktopSession.FindElementByAccessibilityId("textBox2");
            Assert.IsNotNull(textBox2);
            Assert.AreEqual("", textBox2.Text.Trim());
        }
    }
}