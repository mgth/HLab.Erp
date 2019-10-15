using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using HLab.Notify;
using HLab.Notify.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace NotifyChangeUnitTest.testsclasses
{
    class NotifierTest : NotifierObject
    {
        public string Test {
            get => N.Get<string>(old=>"");
            set => N.Set(value);
        }
        public string ActionTestResult {
            get => N.Get<string>(old=>"");
            set => N.Set(value);
        }


        [TriggerOn(nameof(Test))]
        public string SharpTest => "#" + Test;

        [TriggerOn(nameof(Test))]
        public string DollarTest => N.Get<string>(old=>"$" + Test);

        [TriggerOn(nameof(Test))]
        public string AndTest {
            get { return N.Get<string>((old) => "&" + Test); }
            set => N.Set(value);
        }

        [TriggerOn(nameof(Test))]
        public void ActionTest()
        {
            ActionTestResult = "Action" + Test;
        }

        public void DoTest()
        {
            PropertyChanged += N_PropertyChanged;

            //Set property value once
            Test = "0";
            Assert.AreEqual(1, _testCount);

            //Set property to same value should not send notification
            Test = "0";
            Assert.AreEqual(1, _testCount);
            Assert.AreEqual("$0", DollarTest);
            Test = "1";
            Assert.AreEqual(2, _testCount);
            Assert.AreEqual(2, _sharpCount);
            Assert.AreEqual(1, _dollarCount);
            Assert.AreEqual(1, _andCount);
            Assert.AreEqual("Action1", ActionTestResult);

            Assert.AreEqual("$1", DollarTest);
        }

        private int _testCount = 0;
        private int _sharpCount = 0;
        private int _dollarCount = 0;
        private int _andCount = 0;

        //public NotifierTest(INotifierService s)
        //{
        //    N = new Notifier(this, s);
        //    N.Subscribe();
        //}

        private void N_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch(e.PropertyName)
            {
                case "Test" : _testCount++; break;
                case "SharpTest": _sharpCount++; break;
                case "DollarTest": _dollarCount++; break;
                case "AndTest": _andCount++; break;
                case "ActionTestResult": break;
                default: Assert.Fail(); break;
            }
        }
    }
}
