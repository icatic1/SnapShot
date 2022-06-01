using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GUI_Tests
{
    [TestClass]
    public class AdministratorVerificationTest : TestSession
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
        public void Test1_Help()
        {
            var help = desktopSession.FindElementByName("Help");
            Assert.IsNotNull(help);
            help.Click();

            var text = desktopSession.FindElementByName("You cannot exit this form without being licenced first!");
            Assert.IsNotNull(text);
            Assert.AreEqual(text.Text, "You cannot exit this form without being licenced first!");

            var okbtn = desktopSession.FindElementByName("OK");
            Assert.IsNotNull(okbtn);
            okbtn.Click();
        }


        [TestMethod]
        public void Test2_Configuration()
        {
            var config = desktopSession.FindElementByName("Configuration");
            Assert.IsNotNull(config);
            config.Click();

            var gs = desktopSession.FindElementByName("General settings");
            Assert.IsNotNull(gs);
            gs.Click();

            var text = desktopSession.FindElementByName("You cannot access general settings without being licenced first!");
            Assert.IsNotNull(text);
            Assert.AreEqual(text.Text, "You cannot access general settings without being licenced first!");

            var okbtn = desktopSession.FindElementByName("OK");
            Assert.IsNotNull(okbtn);
            okbtn.Click();

            config.Click();

            var cs = desktopSession.FindElementByName("Camera settings");
            Assert.IsNotNull(cs);
            cs.Click();

            var c1 = desktopSession.FindElementByName("Camera 1");
            Assert.IsNotNull(c1);
            c1.Click();

            var text2 = desktopSession.FindElementByName("You cannot access camera settings without being licenced first!");
            Assert.IsNotNull(text2);
            Assert.AreEqual(text2.Text, "You cannot access camera settings without being licenced first!");

            okbtn = desktopSession.FindElementByName("OK");
            Assert.IsNotNull(okbtn);
            okbtn.Click();
        }

        [TestMethod]
        public void Test3_Admin_registration()
        {
            var licence = desktopSession.FindElementByName("Licence");
            Assert.IsNotNull(licence);
            licence.Click();

            var admin_options = desktopSession.FindElementByName("Administrator Options");
            Assert.IsNotNull(admin_options);
            admin_options.Click();

            //Thread.Sleep(1000);
            var tb1 = desktopSession.FindElementByAccessibilityId("textBox1");
            Assert.IsNotNull(tb1);
            tb1.SendKeys("administrator");

            var tb2 = desktopSession.FindElementByAccessibilityId("textBox2");
            Assert.IsNotNull(tb2);
            tb2.SendKeys("administrator");

            var ok = desktopSession.FindElementByAccessibilityId("button2");
            Assert.IsNotNull(ok);
            ok.Click();

            ok.Click();

            var config = desktopSession.FindElementByName("Configuration");
            Assert.IsNotNull(config);
            config.Click();
        }

    }
}
