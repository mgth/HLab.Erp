using System.Reflection;
using HLab.Core.Annotations;
using HLab.DependencyInjection.Annotations;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Core
{


    public abstract class BootLoaderErp : IPostBootloader
    {
        public virtual void Load()
        {
        }
    }
}
