using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
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

        private readonly ConcurrentDictionary<Tuple<string,string>,LocalizeEntry> _cache 
            = new ConcurrentDictionary<Tuple<string, string>, LocalizeEntry>();

        public string Localize(string tag, string code)
        {
            var entry = _cache.GetOrAdd(Tuple.Create(tag, code), t =>
            {
                var entries = _db.FetchWhere<LocalizeEntry>(e => e.Tag == tag && e.Code == code);
                var e =  entries.FirstOrDefault();

                if(e!=null && e.BadCode)
                    throw new ArgumentException(e.Code + " is told bad code");
                return e;
            });


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
