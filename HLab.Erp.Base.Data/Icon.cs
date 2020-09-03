using System;
using System.Collections.Generic;
using System.Text;
using HLab.Erp.Core;
using HLab.Erp.Data;
using HLab.Notify.PropertyChanged;
using NPoco;

namespace HLab.Erp.Base.Data
{
    using H = H<Icon>;

    public class Icon : Entity, IListableModel
    {
        public Icon() => H.Initialize(this);

        [Column]
        public string Path
        {
            get => _path.Get();
            set => _path.Set(value);
        }
        private readonly IProperty<string> _path = H.Property<string>(c => c.Default(""));

        [Column]
        public string SourceSvg
        {
            get => _sourceSvg.Get();
            set => _sourceSvg.Set(value);
        }
        private readonly IProperty<string> _sourceSvg = H.Property<string>(c => c.Default(""));

        [Column]
        public string SourceXaml
        {
            get => _sourceXaml.Get();
            set => _sourceXaml.Set(value);
        }
        private readonly IProperty<string> _sourceXaml = H.Property<string>(c => c.Default(""));

        [Ignore]
        public string Caption => Path;
        [Ignore]
        public string IconPath => Path;
    }
}
