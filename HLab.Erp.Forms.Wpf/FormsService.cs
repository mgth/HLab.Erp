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
        //public void Register(string source)
    }
}
