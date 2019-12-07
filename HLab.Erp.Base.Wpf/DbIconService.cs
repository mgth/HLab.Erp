using System.Threading.Tasks;
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

        public async Task<object> GetIconAsync(string name)
        {
            var icon = await _db.FetchOneAsync<Icon>(i => i.Name==name);
            if (icon != null)
            {
                return _base.FromSvgStringAsync(icon.Source);
            }

            return _base.GetIconAsync(name);
        }

        public object GetFromHtml(string html) => _base.GetFromHtml(html); 

        public async Task<object> FromSvgStringAsync(string svg) => await _base.FromSvgStringAsync(svg).ConfigureAwait(false);

        public void AddIconProvider(string name, IIconProvider provider) => _base.AddIconProvider(name,provider);

        public dynamic Icon => _base.Icon;
    }
}
