using Microsoft.VisualStudio.TestTools.UnitTesting;
using SnapShot;
using System;
using System.Collections.Generic;

namespace Model_Tests
{
    [TestClass]
    public class SnapshotTest
    {
        private static Snapshot snapshot;

        [TestInitialize]
        public void InitializeSnapshotInstance()
        {
            snapshot = new Snapshot();
        }

        [TestMethod]
        public void Test1()
        {
            Assert.AreEqual("", snapshot.TerminalName);
            Assert.IsFalse(snapshot.DebugLog);
            Assert.IsFalse(snapshot.Licenced);
        }

        [TestMethod]
        public void Test2()
        {
            var configList = new List<Configuration>()
            {
                new Configuration(),
                new Configuration(),
                new Configuration()
            };
            Assert.AreEqual(configList.Count, snapshot.Camera.Count);
        }

        [TestMethod]
        public void Test3()
        {
            try
            {
                snapshot.DebugLog = true;
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.ToString());
            }
            Assert.IsTrue(snapshot.DebugLog);
            Assert.IsFalse(snapshot.Licenced);
        }

        [TestMethod]
        public void Test4()
        {
            try
            {
                snapshot.Licenced = true;
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.ToString());
            }
            Assert.IsTrue(snapshot.Licenced);
            Assert.IsFalse(snapshot.DebugLog);
        }
    }
}
