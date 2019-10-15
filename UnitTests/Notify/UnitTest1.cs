using System;
using System.Diagnostics;
using HLab.Notify;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NotifyChangeUnitTest.testsclasses;

namespace NotifyChangeUnitTest
{
    [TestClass]
    public class UnitTest1
    {

        [TestMethod]
        public void TestMethod1()
        {
            var s = new NotifierService();
            new NotifierTest(s).DoTest();
        }

        [TestMethod]
        public void TestMethod2()
        {
            var s = new NotifierService();
            new NotifierTestParent(s).DoTest();
        }

        [TestMethod]
        public void TestMethod3()
        {
            var s = new NotifierService();
            new NotifierTestGranpa(s).DoTest();
        }

        [TestMethod]
        public void TestMethod4()
        {
            var s = new NotifierService();
            new NotifierTestCollection(s).DoTest();
        }
    }
}
