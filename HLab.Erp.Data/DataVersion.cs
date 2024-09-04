namespace HLab.Erp.Data;

public class DataVersion : Entity
{
    public string Module
    {
        get => _module;
        set => SetAndRaise(ref _module, value);
    }
    string _module = "";

    public string Version
    {
        get => _version;
        set => SetAndRaise(ref _version, value);
    }
    string _version = "";
}
