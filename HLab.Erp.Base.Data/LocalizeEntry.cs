using HLab.Erp.Data;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Application;
using ReactiveUI;

namespace HLab.Erp.Base.Data;

public class LocalizeEntry : Entity, ILocalizeEntry, IListableModel
{
    public LocalizeEntry()
    {
        _caption = this.WhenAnyValue(
            e => e.Code, 
            e => e.Tag, 
            selector: (code,tag) => string.IsNullOrWhiteSpace(code)?"{New localize entry}":$"{tag} - {code}")
        .ToProperty(this, nameof(BadCode));
    }

    public string Tag
    {
        get => _tag;
        set => SetAndRaise(ref _tag,value);
    }

    string _tag = "en-us";

    public string Code
    {
        get => _code;
        set => this.RaiseAndSetIfChanged(ref _code,value);
    }

    string _code = "";

    public string Value
    {
        get => _value;
        set => this.RaiseAndSetIfChanged(ref _value,value);
    }

    string _value = "";

    public bool Todo
    {
        get => _todo;
        set => this.RaiseAndSetIfChanged(ref _todo,value);
    }

    bool _todo = true;

    public bool BadCode
    {
        get => _badCode;
        set => this.RaiseAndSetIfChanged(ref _badCode,value);
    }
    bool _badCode = false;

    public bool Custom
    {
        get => _custom;
        set => this.RaiseAndSetIfChanged(ref _custom,value);
    }
    bool _custom = false;

    public string Caption => _caption.Value;
    readonly ObservableAsPropertyHelper<string> _caption;
}
