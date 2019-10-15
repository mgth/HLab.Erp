using System;
using System.Diagnostics;
using System.Windows.Media;
using ErpCore.ViewModelStates;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ErpSystem;

namespace ErpCoreUnitTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            bool result = "TEST".Like("TEST");
            Assert.AreEqual(true,result);


            result = "TEST".Like("TE*");
            Assert.AreEqual(true,result);

            result = "TEST".Like("*ST");
            Assert.AreEqual(true,result);

            result = "TEST".Like("TE*T");
            Assert.AreEqual(true,result);

            result = "TEST".Like("TE*ST");
            Assert.AreEqual(true, result);

            result = "ErpCore".Like("Er*C*e");
            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public void TestState()
        {        
            var state = new State();

            state.PropertyChanged += N_PropertyChanged;


            state.Color = Colors.Black;
            state.Color = Colors.Red;
            state.Selected = true;
        }

        public void N_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {

            Debug.Print(e.PropertyName);
        }
    }
}
