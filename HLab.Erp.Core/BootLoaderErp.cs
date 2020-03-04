using System.Reflection;
using HLab.Core.Annotations;
using HLab.DependencyInjection.Annotations;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Core
{


    public abstract class BootLoaderErp : IBootloader //postboot
    {
        public virtual void Load()
        {
        }
    }
}
