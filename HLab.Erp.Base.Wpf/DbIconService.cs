using HLab.DependencyInjection.Annotations;
using HLab.Erp.Base.Data;
using HLab.Erp.Data;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Icons;

namespace HLab.Erp.Base.Wpf
{
    //[Export(typeof(IIconService))]
    class DbIconService : IIconService
    {
        [Import]
        private IconService _base;
        [Import]
        private IDataService _db;

        public object GetIcon(string name,string forMatch, string backMatch)
        {
            var icon = _db.FetchOne<Icon>(i => i.Name==name);
            if (icon != null)
            {
                return _base.FromSvgString(icon.Source,forMatch,backMatch);
            }

            return _base.GetIcon(name,forMatch,backMatch);
        }

        public object GetFromHtml(string html) => _base.GetFromHtml(html); 

        public object FromSvgString(string svg, string forMatch, string backMatch) => _base.FromSvgString(svg, forMatch, backMatch);

        public void AddIconProvider(string name, IIconProvider provider) => _base.AddIconProvider(name,provider);

        public dynamic Icon => _base.Icon;
    }
}
