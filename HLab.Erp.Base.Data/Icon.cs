using HLab.Erp.Data;
using HLab.Mvvm.Application;
using HLab.Notify.PropertyChanged;

using System.Linq;

namespace HLab.Erp.Base.Data
{
    using H = H<Icon>;

    public class Icon : Entity, IListableModel
    {
        public Icon() => H.Initialize(this);

        public string Path
        {
            get => _path.Get();
            set => _path.Set(value);
        }

        readonly IProperty<string> _path = H.Property<string>(c => c.Default(""));

        public string SourceSvg
        {
            get => _sourceSvg.Get();
            set => _sourceSvg.Set(value);
        }

        readonly IProperty<string> _sourceSvg = H.Property<string>(c => c.Default(""));

        public string SourceXaml
        {
            get => _sourceXaml.Get();
            set => _sourceXaml.Set(value);
        }

        readonly IProperty<string> _sourceXaml = H.Property<string>(c => c.Default(""));

        public int? Foreground
        {
            get => _foreground.Get();
            set => _foreground.Set(value);
        }

        readonly IProperty<int?> _foreground = H.Property<int?>(c => c.Default((int?)null));

        public string Caption => _caption.Get();

        readonly IProperty<string> _caption = H.Property<string>(c => c
            .On(e => e.Path)
            .Set(e => string.IsNullOrWhiteSpace(e.Path)?"{New icon}":e.Path.Split('/').Last())
        );
        public string IconPath => _iconPath.Get();

        readonly IProperty<string> _iconPath = H.Property<string>(c => c
            .On(e => e.Path)
            .Set(e => string.IsNullOrWhiteSpace(e.Path)?"Icons/Entities/Icon":e.Path)
        );
    }
}
