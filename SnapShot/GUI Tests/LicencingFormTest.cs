using Microsoft.VisualStudio.TestTools.UnitTesting;


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
        public void Test1()
        {

        }
    }
}
