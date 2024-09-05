using HLab.Erp.Data;
using HLab.Mvvm.Application;
using ReactiveUI;
using System.Linq;

namespace HLab.Erp.Base.Data;

public class Icon : Entity, IListableModel
{
    public Icon()
    {
        _caption = this.WhenAnyValue(e => e.Path, selector: e => string.IsNullOrWhiteSpace(e)?"{New icon}":e.Split('/').Last())
            .ToProperty(this, nameof(Caption));

        _iconPath = this.WhenAnyValue(e => e.Path, selector: e => string.IsNullOrWhiteSpace(e)?"Icons/Entities/Icon":e)
            .ToProperty(this, nameof(IconPath));
    }

    public string Path
    {
        get => _path;
        set => this.RaiseAndSetIfChanged(ref _path,value);
    }

    string _path = "";

    public string SourceSvg
    {
        get => _sourceSvg;
        set => this.RaiseAndSetIfChanged(ref _sourceSvg,value);
    }

    string _sourceSvg = "";

    public string SourceXaml
    {
        get => _sourceXaml;
        set => this.RaiseAndSetIfChanged(ref _sourceXaml,value);
    }

    string _sourceXaml = "";

    public int? Foreground
    {
        get => _foreground;
        set => this.RaiseAndSetIfChanged(ref _foreground,value);
    }

    int? _foreground = (int?)null;

    public string Caption => _caption.Value;

    readonly ObservableAsPropertyHelper<string> _caption;

    public string IconPath => _iconPath.Value;
    readonly ObservableAsPropertyHelper<string> _iconPath;

}
