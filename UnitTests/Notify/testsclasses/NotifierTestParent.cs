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
    class NotifierTestParent : NotifierObject
    {
        public int Result = 0;

        public NotifierTestParent(INotifierService s)
        {
            N = new Notifier(this, s);
            N.Subscribe();
        }

        public void DoTest()
        {
            this.GetNotifier();

            TestParent.Test = "0";
            Assert.AreEqual(1,Result);
            TestParent = new NotifierTest(N.NotifierService);
            Assert.AreEqual(2,Result);
        }

        public NotifierTest TestParent
        {
            get => this.Get(() => new NotifierTest(N.NotifierService));
            set => this.Set(value);
        }

        [TriggerOn("TestParent","Test")]
        public void TestMe()
        {
            Result++;
        }
    }
}
