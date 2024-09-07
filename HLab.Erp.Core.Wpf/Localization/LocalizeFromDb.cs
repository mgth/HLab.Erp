using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HLab.Base;
using HLab.Erp.Base.Data;
using HLab.Erp.Data;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Core.Wpf.Localization;

public class LocalizeFromDb(IDataService db) : ILocalizationProvider
{
    readonly ConcurrentDictionary<string,AsyncDictionary<string,LocalizeEntry>> _cache = new();

    async Task<AsyncDictionary<string, LocalizeEntry>> GetDictionaryAsync(string language)
    {
        var created = false;
        var dic = _cache.GetOrAdd(language, t =>
        {
            created = true;
            return new();
        });

        if (!created) return dic;

        var list = db.FetchWhereAsync<LocalizeEntry>(e => e.Tag == language,e => e.Tag).ConfigureAwait(false);
        await foreach (var e in list)
        {
            await dic.GetOrAddAsync(e.Code,  x => Task.FromResult(e)).ConfigureAwait(false);
        }
        return dic;
    }

    AsyncDictionary<string, LocalizeEntry> GetDictionary(string language)
    {
        var created = false;
        var dic = _cache.GetOrAdd(language, t =>
        {
            created = true;
            return new();
        });

        if (!created) return dic;

        var list = db.FetchWhere<LocalizeEntry>(e => e.Tag == language,e => e.Tag);
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
            return entry?.Value??"";
        }
        catch
        {
            return code;
        }
    }

    public async Task<string> LocalizeAsync(string language, string code, CancellationToken token = default)
    {
        try
        {
            var entry = await GetLocalizeEntryAsync(language, code).ConfigureAwait(false);
            return entry?.Value??"";
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
            var en = db.FetchOne<LocalizeEntry>(e => e.Tag == language && e.Code == code && e.Custom) ??
                     db.FetchOne<LocalizeEntry>(e => e.Tag == language && e.Code == code);

            if(en is { BadCode: true })
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
            var en = await db.FetchOneAsync<LocalizeEntry>(e => e.Tag == language && e.Code == code && e.Custom).ConfigureAwait(false) ??
                     await db.FetchOneAsync<LocalizeEntry>(e => e.Tag == language && e.Code == code).ConfigureAwait(false);

            if(en is { BadCode: true })
                throw new ArgumentException(en.Code + " is told bad code");
            return en;
        }).ConfigureAwait(false);

        return entry;
    }

    public IEnumerable<ILocalizeEntry> GetLocalizeEntries(string code)
    {
        return db
            .FetchWhere<LocalizeEntry>(e => e.Code == code, e => e.Value);
    }

    public async IAsyncEnumerable<ILocalizeEntry> GetLocalizeEntriesAsync(string code, CancellationToken token = default)
    {
        await foreach(var item in db.FetchWhereAsync<LocalizeEntry>(e => e.Code == code, e => e.Value).WithCancellation(token).ConfigureAwait(false))
            yield return item;
    }

    public void Register(string tag, string code, string value, bool quality)
    {
        var entry = db.AddAsync<LocalizeEntry>(e =>
        {
            e.Tag = tag;
            e.Code = code;
            e.Value = value;
            e.Todo = !quality;
        });

    }
}