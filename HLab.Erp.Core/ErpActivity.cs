using System;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Core
{
    public class ErpActivity
    {
        private IMvvmService _mvvm;

        public ErpActivity(IMvvmService mvvm)
        {
            _mvvm = mvvm;
        }

        public IView View { get; set; }
        public ViewMode ViewMode { get; set; }

        public Action Action { get; set; }

    }
}
