using System;
using HLab.DependencyInjection.Annotations;

namespace HLab.Erp.Core
{
    [Export(typeof(IApplicationInfoService))]
    public class ApplicationInfoService : IApplicationInfoService
    {
        public Version Version { get; set; }
        public String Name { get; set; }
    }
}
