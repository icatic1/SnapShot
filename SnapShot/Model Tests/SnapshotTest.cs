using Microsoft.VisualStudio.TestTools.UnitTesting;
using SnapShot;



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
        public void TestMethod1()
        {

        }
    }
}
