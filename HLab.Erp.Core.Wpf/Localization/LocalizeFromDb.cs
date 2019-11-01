using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HLab.Base;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Base.Data;
using HLab.Erp.Data;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Core.Localization
{
    public class LocalizeFromDb : ILocalizationProvider
    {
        
        private readonly IDataService _db;
        [Import] public LocalizeFromDb(IDataService db)
        {
            _db = db;
        }

        private readonly ConcurrentDictionary<string,AsyncDictionary<string,LocalizeEntry>> _cache 
            = new ConcurrentDictionary<string,AsyncDictionary<string,LocalizeEntry>>();

        public async Task<string> Localize(string tag, string code)
        {
            var dic = _cache.GetOrAdd(tag, t => new AsyncDictionary<string, LocalizeEntry>());

            var entry = await dic.GetOrAdd(code, async t =>
            {
                var e = await _db.FetchOneAsync<LocalizeEntry>(e => e.Tag == tag && e.Code == code).ConfigureAwait(false);

                if(e!=null && e.BadCode)
                    throw new ArgumentException(e.Code + " is told bad code");
                return e;
            }).ConfigureAwait(false);


            return entry?.Value;
        }

        public void Register(string tag, string code, string value, bool quality)
        {
                var entry = _db.AddAsync<LocalizeEntry>(e =>
                {
                    e.Tag = tag;
                    e.Code = code;
                    e.Value = value;
                    e.Todo = !quality;
                });

        }
    }
}
