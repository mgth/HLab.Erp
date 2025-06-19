using HLab.Erp.Data;
using HLab.Mvvm.Application;

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

    public string Name { get; set => this.RaiseAndSetIfChanged(ref field, value); } = "";

    public string Note { get; set => this.RaiseAndSetIfChanged(ref field, value); } = "";

    [Ignore]
    public string Caption => _caption.Value;
    readonly ObservableAsPropertyHelper<string> _caption;// = H.Property<string>(c => c

    public string IconPath { get; set => this.RaiseAndSetIfChanged(ref field, value); } = "";

}
