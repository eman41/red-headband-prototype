using TileEngine.Engine.Platforms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Microsoft.Xna.Framework;

namespace RedHeadbandTests
{
    
    
    /// <summary>
    ///This is a test class for PlatformControllerTest and is intended
    ///to contain all PlatformControllerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class PlatformControllerTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for PointBetween
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Red Headband Prototype.exe")]
        public void PointBetweenTest()
        {
            Vector2 p1 = new Vector2(1,1); // TODO: Initialize to an appropriate value
            Vector2 p2 = new Vector2(1,10); // TODO: Initialize to an appropriate value
            Vector2 pos1 = new Vector2(1,11); // FALSE
            Vector2 pos2 = new Vector2(1, 5); // TRUE
            Vector2 pos3 = new Vector2(1, 1); // TRUE
            Vector2 pos4 = new Vector2(1, 10); // TRUE

            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = PlatformController.PointBetween(p1, p2, pos1);
            Assert.AreEqual(expected, actual);

            expected = true;
            actual = PlatformController.PointBetween(p1, p2, pos2);
            Assert.AreEqual(expected, actual, "1,5");
            actual = PlatformController.PointBetween(p1, p2, pos3);
            Assert.AreEqual(expected, actual, "1,1");
            actual = PlatformController.PointBetween(p1, p2, pos4);
            Assert.AreEqual(expected, actual, "1,10");
        }
    }
}
