using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using ams.sf;


namespace TestProject2
{
    [TestClass]
    public class ActivityTest
    {
        [TestMethod]
        public void TestEnvironmentActivity()
        {
            Activity a = Activity.Parse("KT1: 2012-2");
            Assert.IsInstanceOfType(a, typeof(EnvironmentActivity));
            EnvironmentActivity ea = (EnvironmentActivity)a;
            Assert.AreEqual("KT1", ea.Environment());
        }

        [TestMethod]
        public void TestInstallation()
        {
            Installation i = (Installation)Activity.Parse("KT1: 2012-1.0.2");
            Assert.AreEqual("KT1", i.Environment());
            Assert.AreEqual("2012-1.0.2", i.Version());
        }
    
    }
}
