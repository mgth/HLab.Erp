using HLab.Erp.Data;
using HLab.Mvvm.Application;
using HLab.Notify.PropertyChanged;

using NPoco;
using ReactiveUI;
using System.Reactive.Linq;

namespace HLab.Erp.Acl;

public class Profile : Entity, IListableModel
{
    public Profile() 
    { 
        _caption = this
            .WhenAnyValue(e => e.Name)
            .Select(e => string.IsNullOrWhiteSpace(e)?"{New profile}":e)
        .ToProperty(this,nameof(Caption));

    }

    public string Name
    {
        get => _name;
        set => this.RaiseAndSetIfChanged(ref _name, value);
    }
    string _name = "";

    public string Note
    {
        get => _note;
        set =>this.RaiseAndSetIfChanged(ref _note, value);
    }
    string _note = "";

    [Ignore]
    public string Caption => _caption.Value;
    readonly ObservableAsPropertyHelper<string> _caption;// = H.Property<string>(c => c

    public string IconPath => throw new System.NotImplementedException();

}
