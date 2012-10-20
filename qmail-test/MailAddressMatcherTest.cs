using qmail;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net.Mail;

namespace qmail_test
{
    
    
    /// <summary>
    ///This is a test class for MailAddressMatcherTest and is intended
    ///to contain all MailAddressMatcherTest Unit Tests
    ///</summary>
    [TestClass()]
    public class MailAddressMatcherTest
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
        ///A test for MailAddressMatcher Constructor
        ///</summary>
        [TestMethod()]
        public void MailAddressMatcherConstructorTest()
        {
            string[] acceptedRecipients = { "valid@domain.dk", "valid@domain.com" }; 
            MailAddressMatcher target = new MailAddressMatcher(acceptedRecipients);
            Assert.IsNotNull(target);
        }

        /// <summary>
        ///A test for MatchingMailAddress
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Project1.exe")]
        public void MatchingMailAddressTest()
        {
            PrivateObject param0 = null; // TODO: Initialize to an appropriate value
            MailAddressMatcher_Accessor target = new MailAddressMatcher_Accessor(param0); // TODO: Initialize to an appropriate value
            string accepted = string.Empty; // TODO: Initialize to an appropriate value
            MailAddress to = null; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.MatchingMailAddress(accepted, to);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for RecipientAccepted
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Project1.exe")]
        public void RecipientAcceptedTest()
        {
            PrivateObject param0 = null; // TODO: Initialize to an appropriate value
            MailAddressMatcher_Accessor target = new MailAddressMatcher_Accessor(param0); // TODO: Initialize to an appropriate value
            MailAddress to = null; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.RecipientAccepted(to);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for RecipientsAccepted
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Project1.exe")]
        public void RecipientsAcceptedTest()
        {
            PrivateObject param0 = null; // TODO: Initialize to an appropriate value
            MailAddressMatcher_Accessor target = new MailAddressMatcher_Accessor(param0); // TODO: Initialize to an appropriate value
            MailAddressCollection tos = null; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.RecipientsAccepted(tos);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
