using System;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Base.Data;
using HLab.Erp.Data;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Core.Wpf.Localization
{
    public class LocalizeFromDb : ILocalizationProvider
    {
        [Import]
        private IDbService _db;
        public string Localize(string tag, string code)
        {
            var entry = _db.FetchOne<LocalizeEntry>(e => e.Tag == tag && e.Code == code);

            if(entry!=null && entry.BadCode)
                throw new ArgumentException(entry.Code + " is told bad code");

            return entry?.Value;
        }

        public void Register(string tag, string code, string value, bool quality)
        {
                var entry = _db.Add<LocalizeEntry>(e =>
                {
                    e.Tag = tag;
                    e.Code = code;
                    e.Value = value;
                    e.Todo = !quality;
                });

        }
    }
}
