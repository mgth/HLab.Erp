using HLab.Erp.Data;
using HLab.Mvvm.Application;
using HLab.Notify.PropertyChanged;

using System.Linq;

namespace HLab.Erp.Base.Data
{
    using H = H<Icon>;

    public class Icon : Entity, IListableModel
    {
        public Icon() { }

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

        public string Caption => _caption.Get();

        string _caption = H.Property<string>(c => c
            .On(e => e.Path)
            .Set(e => string.IsNullOrWhiteSpace(e.Path)?"{New icon}":e.Path.Split('/').Last())
        );
        public string IconPath => _iconPath.Get();

        string _iconPath = H.Property<string>(c => c
            .On(e => e.Path)
            .Set(e => string.IsNullOrWhiteSpace(e.Path)?"Icons/Entities/Icon":e.Path)
        );
    }
}
