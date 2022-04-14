using OpenQA.Selenium.Appium.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI_Tests
{
    public class TestSession
    {

        protected static WindowsDriver<WindowsElement> session;
        public static WindowsDriver<WindowsElement> desktopSession;
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
