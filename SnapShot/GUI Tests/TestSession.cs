using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Windows;
using System;


namespace GUI_Tests
{
    public class TestSession
    {

        protected static WindowsDriver<WindowsElement> session;
        public static WindowsDriver<WindowsElement> desktopSession;

        private const string WINAPP_DRIVER_URL = "http://127.0.0.1:4723/";
        //private const string TEST_APP = @"C:\Windows\System32\calc.exe";
        //private const string TEST_APP = "Notepad";
        private const string TEST_APP = @"C:\Set 1\Snapshot\SnapShot.exe";

        public static void Setup(TestContext context)
        {
            if (session == null || desktopSession == null)
            {
                TearDown();

                // Create a new session to bring up the test application
                AppiumOptions options = new AppiumOptions();
                options.AddAdditionalCapability("app", TEST_APP);
                options.AddAdditionalCapability("deviceName", "WindowsPC");
                options.AddAdditionalCapability("platformName", "Windows");

                session = new WindowsDriver<WindowsElement>(new Uri(WINAPP_DRIVER_URL), options);

                Assert.IsNotNull(session);
                Assert.IsNotNull(session.SessionId);

                // Set implicit timeout to 1.5 seconds to make element search to retry every 500 ms for at most three times
                session.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(1.5);

                AppiumOptions optionsDesktop = new AppiumOptions();
                optionsDesktop.AddAdditionalCapability("app", "Root");
                optionsDesktop.AddAdditionalCapability("deviceName", "WindowsPC");
                optionsDesktop.AddAdditionalCapability("ms:experimental-webdriver", true);
                desktopSession = new WindowsDriver<WindowsElement>(new Uri(WINAPP_DRIVER_URL), optionsDesktop);
            }
        }

        public static void TearDown()
        {
            if (session != null)
            {
                session.Quit();
                session = null;
            }

            if (desktopSession != null)
            {
                desktopSession.Quit();
                desktopSession = null;
            }
        }
    }
}
