using System;
using System.Collections.Generic;
using System.Text;
using HLab.DependencyInjection.Annotations;

namespace HLab.Erp.Forms.Wpf
{
    public interface IFormsService
    {

    }
    [Export(typeof(IFormsService))]
    public class FormsService
    {
        private static Dictionary<string, Func<object>> _dict = new();

        public void Register(string source)
        { }
    }
}
