using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows;
using System;
using Xamarin.Essentials;
using System.IO;
using OpenQA.Selenium;
using Newtonsoft.Json;
using OpenQA.Selenium.Appium.Windows;
using System.Threading;

namespace GUI_Tests
{
    [TestClass]
    public class ExportJSONTest : TestSession
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
        public void Test1_CheckDeviceConfigurationAllCamerasDifferent()
        {

            //Click on configuration
            var configuration = session.FindElementByName("Configuration");
            configuration.Click();
            Thread.Sleep(1000);

            //Enter data for Camera 1
            var Camera1textBox1 = desktopSession.FindElementByAccessibilityId("textBox1");
            var Camera1textBox6 = desktopSession.FindElementByAccessibilityId("textBox6");
            var Camera1textBox2 = desktopSession.FindElementByAccessibilityId("textBox2");
            var Camera1KeepCapture = desktopSession.FindElementByName("Keep capture for:");
            Assert.IsNotNull(Camera1textBox6);
            const string Camera1regex = "+[a-z]";
            const string Camera1triggerPath = TESTING_DIRECTORY + @"\trigger1.txt";
            const string Camera1storagePath = TESTING_DIRECTORY;
            const string Camera1KeepCaptureLength = "11";

            Camera1textBox1.SendKeys(Camera1triggerPath);
            Camera1textBox6.SendKeys(Camera1regex);
            Camera1textBox2.SendKeys(Camera1storagePath);
            Camera1KeepCapture.SendKeys(Keys.Delete);
            Camera1KeepCapture.SendKeys(Camera1KeepCaptureLength);

            //Click configure 
            var button6 = desktopSession.FindElementByAccessibilityId("button6");
            button6.Click();

       //     configuration.Click();

            //Select Camera 2
            var secondCameraRadioBtn = desktopSession.FindElementByAccessibilityId("radioButton6");
            secondCameraRadioBtn.Click();

            var Camera2textBox1 = desktopSession.FindElementByAccessibilityId("textBox1");
            var Camera2textBox6 = desktopSession.FindElementByAccessibilityId("textBox6");
            var Camera2textBox2 = desktopSession.FindElementByAccessibilityId("textBox2");
            var Camera2KeepCapture = desktopSession.FindElementByName("Keep capture for:");
            Assert.IsNotNull(Camera2textBox6);
            const string Camera2regex = "+[a-z]";
            const string Camera2triggerPath = TESTING_DIRECTORY + @"\trigger2.txt";
            const string Camera2storagePath = TESTING_DIRECTORY;
            const string Camera2KeepCaptureLength = "22";

            Camera2textBox1.SendKeys(Camera2triggerPath);
            Camera2textBox6.SendKeys(Camera2regex);
            Camera2textBox2.SendKeys(Camera2storagePath);
            Camera2KeepCapture.SendKeys(Keys.Delete);
            Camera2KeepCapture.SendKeys(Camera2KeepCaptureLength);

            //Click configure 
            button6.Click();

         //   configuration.Click();

            //Select Camera 3
            var thirdCameraRadioBtn = desktopSession.FindElementByAccessibilityId("radioButton7");
            thirdCameraRadioBtn.Click();

            var Camera3textBox1 = desktopSession.FindElementByAccessibilityId("textBox1");
            var Camera3textBox6 = desktopSession.FindElementByAccessibilityId("textBox6");
            var Camera3textBox2 = desktopSession.FindElementByAccessibilityId("textBox2");
            var Camera3KeepCapture = desktopSession.FindElementByName("Keep capture for:");
            Assert.IsNotNull(Camera3textBox6);
            const string Camera3regex = "+[a-z]";
            const string Camera3triggerPath = TESTING_DIRECTORY + @"\trigger3.txt";
            const string Camera3storagePath = TESTING_DIRECTORY;
            const string Camera3KeepCaptureLength = "33";

            Camera3textBox1.SendKeys(Camera3triggerPath);
            Camera3textBox6.SendKeys(Camera3regex);
            Camera3textBox2.SendKeys(Camera3storagePath);
            Camera3KeepCapture.SendKeys(Keys.Delete);
            Camera3KeepCapture.SendKeys(Camera3KeepCaptureLength);

            //Click configure 
            button6.Click();

       //     configuration.Click();


            // LeftClick on MenuItem "Export to JSON" at (54,19)
            Console.WriteLine("LeftClick on MenuItem \"Export to JSON\" at (54,19)");
            string xpath_LeftClickMenuItemExporttoJS_54_19 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 1\"]/Window[@Name=\"SnapShot\"][@AutomationId=\"ConfigurationForm\"]/Menu[starts-with(@ClassName,\"WindowsForms10\")][@Name=\"Configuration\"]/MenuItem[@Name=\"Export to JSON\"]";
            var winElem_LeftClickMenuItemExporttoJS_54_19 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickMenuItemExporttoJS_54_19);
            if (winElem_LeftClickMenuItemExporttoJS_54_19 != null)
            {
                winElem_LeftClickMenuItemExporttoJS_54_19.Click();
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickMenuItemExporttoJS_54_19}");
                return;
            }


            // LeftDblClick on Edit "Name" at (62,13)
            Console.WriteLine("LeftDblClick on Edit \"Name\" at (62,13)");
            string xpath_LeftDblClickEditName_62_13 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 2\"]/Window[@Name=\"SnapShot\"][@AutomationId=\"ConfigurationForm\"]/Window[@ClassName=\"#32770\"][@Name=\"Open\"]/Pane[@ClassName=\"DUIViewWndClassName\"]/Pane[@Name=\"Shell Folder View\"][@AutomationId=\"listview\"]/List[@ClassName=\"UIItemsView\"][@Name=\"Items View\"]/ListItem[@ClassName=\"UIItem\"][@Name=\"SnapShotTestingGround\"]/Edit[@Name=\"Name\"][@AutomationId=\"System.ItemNameDisplay\"]";
            var winElem_LeftDblClickEditName_62_13 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftDblClickEditName_62_13);
            if (winElem_LeftDblClickEditName_62_13 != null)
            {
                desktopSession.Mouse.MouseMove(winElem_LeftDblClickEditName_62_13.Coordinates);
                desktopSession.Mouse.DoubleClick(null);
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftDblClickEditName_62_13}");
                return;
            }


            // LeftClick on Button "Open" at (31,3)
            Console.WriteLine("LeftClick on Button \"Open\" at (31,3)");
            string xpath_LeftClickButtonOpen_31_3 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 2\"]/Window[@Name=\"SnapShot\"][@AutomationId=\"ConfigurationForm\"]/Window[@ClassName=\"#32770\"][@Name=\"Open\"]/Button[@ClassName=\"Button\"][@Name=\"Open\"]";
            var winElem_LeftClickButtonOpen_31_3 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickButtonOpen_31_3);
            if (winElem_LeftClickButtonOpen_31_3 != null)
            {
                winElem_LeftClickButtonOpen_31_3.Click();
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickButtonOpen_31_3}");
                return;
            }

            Thread.Sleep(1000);
            Assert.IsTrue(File.Exists(TESTING_DIRECTORY + @"\configuration.json"));

            string json = File.ReadAllText(TESTING_DIRECTORY + @"\configuration.json");


            //Check if values are in JSON

            Assert.IsTrue(json.Contains(Camera1regex));
            Assert.IsTrue(json.Contains(Camera1triggerPath));
            Assert.IsTrue(json.Contains(Camera1storagePath));

            Assert.IsTrue(json.Contains(Camera2regex));
            Assert.IsTrue(json.Contains(Camera2triggerPath));
            Assert.IsTrue(json.Contains(Camera2storagePath));

            Assert.IsTrue(json.Contains(Camera3regex));
            Assert.IsTrue(json.Contains(Camera3triggerPath));
            Assert.IsTrue(json.Contains(Camera3storagePath));

            Assert.IsTrue(json.Contains(Camera1KeepCaptureLength));
            Assert.IsTrue(json.Contains(Camera2KeepCaptureLength));
            Assert.IsTrue(json.Contains(Camera3KeepCaptureLength));
        }

        [TestMethod]
        public void Test2_CheckCameraConfiguration()
        {

            //Click on configuration
           // var configuration = session.FindElementByName("Configuration");
           // configuration.Click();
            Thread.Sleep(1000);

            //Enter data for Camera 1
            var Camera1textBox1 = desktopSession.FindElementByAccessibilityId("textBox1");
            var Camera1textBox6 = desktopSession.FindElementByAccessibilityId("textBox6");
            var Camera1textBox2 = desktopSession.FindElementByAccessibilityId("textBox2");
            var ResolutionContrastDropdown = desktopSession.FindElementByAccessibilityId("comboBox3");
            var contrastSlideBar = desktopSession.FindElementByAccessibilityId("trackBar1");
            var Camera1ImageColorBtn = desktopSession.FindElementByAccessibilityId("button4");

            Assert.IsNotNull(Camera1textBox6);
            const string Camera1regex = "+[a-z]";
            const string Camera1triggerPath = TESTING_DIRECTORY + @"\trigger1.txt";
            const string Camera1storagePath = TESTING_DIRECTORY;
            const string Camera1Resolution = "Resolution1920x1080";
            const string Camera1Contrast = "3";
            const string Camera1ImageColor = "Black";
            const string Camera1MotionDetection = "False";

            //Values for these fields are necesarry even if we're not testing them
            Camera1textBox1.SendKeys(Camera1triggerPath);
            Camera1textBox6.SendKeys(Camera1regex);
            Camera1textBox2.SendKeys(Camera1storagePath);

            //Choose item from dropdown
            ResolutionContrastDropdown.Click();
            ResolutionContrastDropdown.SendKeys(Keys.ArrowDown);
            ResolutionContrastDropdown.SendKeys(Keys.ArrowDown);
            ResolutionContrastDropdown.SendKeys(Keys.ArrowDown);
            ResolutionContrastDropdown.SendKeys(Keys.ArrowDown);
            ResolutionContrastDropdown.SendKeys(Keys.Enter);

            //Move the slider
            contrastSlideBar.Click();
            contrastSlideBar.SendKeys(Keys.ArrowRight);
            contrastSlideBar.SendKeys(Keys.ArrowRight);

            //Pick a color
            Camera1ImageColorBtn.Click();
            Camera1ImageColorBtn.SendKeys(Keys.Enter);

            //Click configure 
            var button6 = desktopSession.FindElementByAccessibilityId("button6");
            button6.Click();

        //    configuration.Click();


            // LeftClick on MenuItem "Export to JSON" at (54,19)
            Console.WriteLine("LeftClick on MenuItem \"Export to JSON\" at (54,19)");
            string xpath_LeftClickMenuItemExporttoJS_54_19 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 1\"]/Window[@Name=\"SnapShot\"][@AutomationId=\"ConfigurationForm\"]/Menu[starts-with(@ClassName,\"WindowsForms10\")][@Name=\"Configuration\"]/MenuItem[@Name=\"Export to JSON\"]";
            var winElem_LeftClickMenuItemExporttoJS_54_19 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickMenuItemExporttoJS_54_19);
            if (winElem_LeftClickMenuItemExporttoJS_54_19 != null)
            {
                winElem_LeftClickMenuItemExporttoJS_54_19.Click();
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickMenuItemExporttoJS_54_19}");
                return;
            }


            // LeftDblClick on Edit "Name" at (62,13)
            Console.WriteLine("LeftDblClick on Edit \"Name\" at (62,13)");
            string xpath_LeftDblClickEditName_62_13 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 2\"]/Window[@Name=\"SnapShot\"][@AutomationId=\"ConfigurationForm\"]/Window[@ClassName=\"#32770\"][@Name=\"Open\"]/Pane[@ClassName=\"DUIViewWndClassName\"]/Pane[@Name=\"Shell Folder View\"][@AutomationId=\"listview\"]/List[@ClassName=\"UIItemsView\"][@Name=\"Items View\"]/ListItem[@ClassName=\"UIItem\"][@Name=\"SnapShotTestingGround\"]/Edit[@Name=\"Name\"][@AutomationId=\"System.ItemNameDisplay\"]";
            var winElem_LeftDblClickEditName_62_13 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftDblClickEditName_62_13);
            if (winElem_LeftDblClickEditName_62_13 != null)
            {
                desktopSession.Mouse.MouseMove(winElem_LeftDblClickEditName_62_13.Coordinates);
                desktopSession.Mouse.DoubleClick(null);
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftDblClickEditName_62_13}");
                return;
            }


            // LeftClick on Button "Open" at (31,3)
            Console.WriteLine("LeftClick on Button \"Open\" at (31,3)");
            string xpath_LeftClickButtonOpen_31_3 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 2\"]/Window[@Name=\"SnapShot\"][@AutomationId=\"ConfigurationForm\"]/Window[@ClassName=\"#32770\"][@Name=\"Open\"]/Button[@ClassName=\"Button\"][@Name=\"Open\"]";
            var winElem_LeftClickButtonOpen_31_3 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickButtonOpen_31_3);
            if (winElem_LeftClickButtonOpen_31_3 != null)
            {
                winElem_LeftClickButtonOpen_31_3.Click();
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickButtonOpen_31_3}");
                return;
            }

            Thread.Sleep(1000);
            Assert.IsTrue(File.Exists(TESTING_DIRECTORY + @"\configuration.json"));

            string json = File.ReadAllText(TESTING_DIRECTORY + @"\configuration.json");


            //Check if values are in JSON

            Assert.IsTrue(json.Contains(Camera1Resolution));
            Assert.IsTrue(json.Contains(Camera1Contrast));
            Assert.IsTrue(json.Contains(Camera1ImageColor));
            Assert.IsTrue(json.Contains(Camera1MotionDetection));
        }
    }
}
