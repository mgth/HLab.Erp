using HLab.Base.ReactiveUI;

namespace HLab.Erp.Data;

public class DataVersion : Entity
{
    public string Module
    {
        get => _module;
        set => this.SetAndRaise(ref _module, value);
    }
    string _module = "";

    public string Version
    {
        get => _version;
        set => this.SetAndRaise(ref _version, value);
    }
    string _version = "";
}
