using OAuthLib.Providers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using OAuthLib.Core;

namespace UnitTests
{
    
    
    /// <summary>
    ///This is a test class for YahooProviderTest and is intended
    ///to contain all YahooProviderTest Unit Tests
    ///</summary>
    [TestClass()]
    public class YahooProviderTest {

        private const string CONSUMER_KEY = "dj0yJmk9M0tUNzNZVUxvcWhvJmQ9WVdrOWVFeDNRbkp6TkhVbWNHbzlNVGcyTURVME1UWXkmcz1jb25zdW1lcnNlY3JldCZ4PWRm";
        private const string CONSUMER_SECRET = "4a5594f053844a91a19b6a2ab85c0e0bd82ebff5";

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext {
            get {
                return testContextInstance;
            }
            set {
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
        ///A test for Authenticate
        ///</summary>
        [TestMethod()]
        public void AuthenticateTest() {
            string consumerKey = CONSUMER_KEY; // TODO: Initialize to an appropriate value
            string consumerSecret = CONSUMER_SECRET; // TODO: Initialize to an appropriate value
            string appUrl = string.Empty; // TODO: Initialize to an appropriate value
            string authCallback = "oob"; // TODO: Initialize to an appropriate value
            YahooProvider target = new YahooProvider(consumerKey, consumerSecret, appUrl, authCallback); // TODO: Initialize to an appropriate value
            Func<string> userAuthCallback = () => { return Microsoft.VisualBasic.Interaction.InputBox("Enter key:"); }; // TODO: Initialize to an appropriate value
            AccessToken actual;
            actual = target.Authenticate(userAuthCallback);

        }
    }
}
