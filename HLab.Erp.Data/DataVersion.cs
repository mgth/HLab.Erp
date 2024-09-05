using ReactiveUI;
using System.Reflection;

namespace HLab.Erp.Data
{
    public class DataVersion : Entity
    {
        public string Module
        {
            get => _module;
            set =>  this.RaiseAndSetIfChanged(ref _module, value);
        }
        string _module = "";

        public string Version
        {
            get => _version;
            set => this.RaiseAndSetIfChanged(ref _version, value);
        }
        string _version;

    }
}
