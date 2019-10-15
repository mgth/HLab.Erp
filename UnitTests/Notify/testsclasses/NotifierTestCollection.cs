using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    class NotifierTestCollection : NotifierObject
    {
        private int _col = 0;
        private int _colItem = 0;
        private int _colItemTest = 0;
        private int _colCount = 0;

        public NotifierTestCollection(INotifierService s)
        {
            N = new Notifier(this,s);
            N.Subscribe();
        }

        public ObservableCollection<NotifierTest> Col => this.Get(()=>new ObservableCollection<NotifierTest>());

        public void DoTest()
        {
            var t1 = new NotifierTest(N.NotifierService);
            var t2 = new NotifierTest(N.NotifierService);

            Col.Add(t1);
            Col.Add(t2);

            t1.Test = "0";
            t2.Test = "1";

//            Assert.AreEqual(1,_col);
            Assert.AreEqual(2,_colItem);
            Assert.AreEqual(4,_colItemTest);
        }

        [TriggerOn("Col.Count")]
        public void OnColCount()
        {
            _colCount++;
        }

        [TriggerOn(nameof(Col))]
        public void OnCol()
        {
            _col++;
        }

        [TriggerOn("Col.Item")]
        public void OnColItem()
        {
            _colItem++;
        }

        [TriggerOn("Col.Item.Test")]
        public void OnColItemTest()
        {
            _colItemTest++;
        }
    }
}
