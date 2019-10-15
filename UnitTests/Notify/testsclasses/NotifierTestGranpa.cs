using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HLab.Notify;
using HLab.Notify.Extentions;
using HLab.Notify.Triggers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NotifyChangeUnitTest.testsclasses
{
    class NotifierTestGranpa : NotifierObject
    {
         public int Result = 0;

        public NotifierTestGranpa(INotifierService s)
        {
            N = new Notifier(this, s);
            N.Subscribe();
        }

        public void DoTest()
        {
            TestGranpa.TestParent.Test = "0";

            var r = TestGranpa.TestParent.Test;

            Assert.AreEqual("0", r);
            Assert.AreEqual(1, Result);
        }

        public NotifierTestParent TestGranpa => this.Get(

            () => 
            new NotifierTestParent(N.NotifierService)
            );

        [TriggerOn(nameof(TestGranpa),"TestParent","Test")]
        public void TestMeGranpa()
        {
            Result++;
        }
    }
}

