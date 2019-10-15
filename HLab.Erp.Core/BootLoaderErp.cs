using System.Reflection;
using HLab.Core.Annotations;
using HLab.DependencyInjection.Annotations;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Core
{


    public abstract class BootLoaderErp : IPostBootloader
    {
        //[Import]
        //private IMvvmService _mvvm;

        public virtual void Load()
        {
//            _mvvm.Register();
        }

        protected abstract void RegisterAssembly(Assembly assembly);
    }
}
