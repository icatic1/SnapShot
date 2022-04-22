using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows;
using System;
using Xamarin.Essentials;
using System.IO;
using OpenQA.Selenium;

namespace GUI_Tests
{
    [TestClass]
    public class LocalStorageTest : TestSession
    {
        public const string TESTING_DIRECTORY = @"C:\SnapShotTestingGround";


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
            Directory.Delete(TESTING_DIRECTORY, true);
            
            TearDown();
        }

        [TestMethod]
        public void t1()
        {
            File.Create(TESTING_DIRECTORY + @"\trigger.txt");
        }

        
    }
}
