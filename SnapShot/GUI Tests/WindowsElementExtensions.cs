using OpenQA.Selenium.Appium.Windows;
using OpenQA.Selenium.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI_Tests
{
    public static class WindowsElementExtensions
    {
        public static WindowsElement FindElementByAbsoluteXPath(this WindowsDriver<WindowsElement> desktopSession, string xPath, int nTryCount = 3)
        {
            WindowsElement uiTarget = null;
            while (nTryCount-- > 0)
            {
                try
                {
                    uiTarget = desktopSession.FindElementByXPath(xPath);
                }
                catch
                {
                }
                if (uiTarget != null)
                {
                    break;
                }
                else
                {
                    System.Threading.Thread.Sleep(400);
                }
            }
            return uiTarget;
        }

        public static void DoubleClick(this WindowsElement element, int offsetX = 0, int offsetY = 0)
        {
            Actions actions = new Actions(element.WrappedDriver);
            actions.Build();
            actions.MoveToElement(element);
            actions.MoveByOffset(offsetX, offsetY);
            actions.DoubleClick();
            actions.Perform();
        }

        public static void Click(this WindowsElement element, int offsetX = 0, int offsetY = 0)
        {
            Actions actions = new Actions(element.WrappedDriver);
            actions.Build();
            actions.MoveToElement(element);
            actions.MoveByOffset(offsetX, offsetY);
            actions.Click();
            actions.Perform();
        }

        public static void RightClick(this WindowsElement element, int offsetX = 0, int offsetY = 0)
        {
            Actions actions = new Actions(element.WrappedDriver);
            actions.Build();
            actions.MoveToElement(element);
            actions.MoveByOffset(offsetX, offsetY);
            actions.ContextClick();
            actions.Perform();
        }

    }
}

