using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using HLab.Base;
using HLab.Erp.Base.Data;
using HLab.Erp.Data;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Core.Localization
{
    public class LocalizeFromDb : ILocalizationProvider
    {
        
        private IDataService _db;
        public void Inject(IDataService db)
        {
            _db = db;
        }

        private readonly ConcurrentDictionary<string,AsyncDictionary<string,LocalizeEntry>> _cache 
            = new ConcurrentDictionary<string,AsyncDictionary<string,LocalizeEntry>>();

        private async Task<AsyncDictionary<string, LocalizeEntry>> GetDictionaryAsync(string language)
        {
            var created = false;
            var dic = _cache.GetOrAdd(language, t =>
            {
                created = true;
                return new AsyncDictionary<string, LocalizeEntry>();
            });

            if (created)
            {
                var list = _db.FetchWhereAsync<LocalizeEntry>(e => e.Tag == language,e => e.Tag).ConfigureAwait(false);
                await foreach (var e in list)
                {
                    await dic.GetOrAddAsync(e.Code, async x => e).ConfigureAwait(false);
                }
            }
            return dic;
        }
        private AsyncDictionary<string, LocalizeEntry> GetDictionary(string language)
        {
            var created = false;
            var dic = _cache.GetOrAdd(language, t =>
            {
                created = true;
                return new AsyncDictionary<string, LocalizeEntry>();
            });

            if (!created) return dic;

            var list = _db.FetchWhere<LocalizeEntry>(e => e.Tag == language,e => e.Tag);
            foreach (var e in list)
            {
                dic.GetOrAdd(e.Code,   x => e);
            }
            return dic;
        }


        public string Localize(string language, string code)
        {
            try
            {
                var entry = GetLocalizeEntry(language, code);
                return entry?.Value;
            }
            catch
            {
                return code;
            }
        }

        public async Task<string> LocalizeAsync(string language, string code)
        {
            try
            {
                var entry = await GetLocalizeEntryAsync(language, code).ConfigureAwait(false);
                return entry?.Value;
            }
            catch
            {
                return code;
            }
        }

        public ILocalizeEntry GetLocalizeEntry(string language, string code)
        {
            var dic = GetDictionary(language);

            var entry = dic.GetOrAdd(code, t =>
            {
                var en = _db.FetchOne<LocalizeEntry>(e => e.Tag == language && e.Code == code && e.Custom) ??
                         _db.FetchOne<LocalizeEntry>(e => e.Tag == language && e.Code == code);

                if(en!=null && en.BadCode)
                    throw new ArgumentException(en.Code + " is told bad code");
                return en;
            });

            return entry;
        }

        public async Task<ILocalizeEntry> GetLocalizeEntryAsync(string language, string code)
        {
            var dic = await GetDictionaryAsync(language).ConfigureAwait(false);

            var entry = await dic.GetOrAddAsync(code, async t =>
            {
                var en = await _db.FetchOneAsync<LocalizeEntry>(e => e.Tag == language && e.Code == code && e.Custom).ConfigureAwait(false) ??
                        await _db.FetchOneAsync<LocalizeEntry>(e => e.Tag == language && e.Code == code).ConfigureAwait(false);

                if(en!=null && en.BadCode)
                    throw new ArgumentException(en.Code + " is told bad code");
                return en;
            }).ConfigureAwait(false);

            return entry;
        }

        public IEnumerable<ILocalizeEntry> GetLocalizeEntries(string code)
        {
            foreach(var item in _db.FetchWhere<LocalizeEntry>(e => e.Code == code, e => e.Value))
                yield return item;
        }

        public async IAsyncEnumerable<ILocalizeEntry> GetLocalizeEntriesAsync(string code)
        {
            await foreach(var item in _db.FetchWhereAsync<LocalizeEntry>(e => e.Code == code, e => e.Value).ConfigureAwait(false))
                yield return item;
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
