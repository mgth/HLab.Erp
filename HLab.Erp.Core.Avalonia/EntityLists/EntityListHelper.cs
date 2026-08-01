using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Data;
using HLab.Erp.Data.Observables;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace HLab.Erp.Core.Avalonia.EntityLists;

public class EntityListHelper<T> : IEntityListHelper<T>
    where T : class, IEntity, new()
{
    public void Populate(object grid, IColumnsProvider<T> provider)
    {
        provider.Populate(grid);
    }

    public object GetListView(IList list) => list;

    public void DoOnDispatcher(object grid, Action action)
    {
        if (Dispatcher.UIThread.CheckAccess()) action();
        else Dispatcher.UIThread.Invoke(action);
    }

    static IStorageProvider? GetStorageProvider()
        => global::Avalonia.Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime { MainWindow: { } w }
            ? w.StorageProvider
            : null;

    public async Task ExportAsync(IObservableQuery<T> list, IContractResolver resolver)
    {
        var storage = GetStorageProvider();
        if (storage is null) return;

        var date = DateTime.Now.ToString("u").Replace(':', '-');

        var file = await storage.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            SuggestedFileName = $"Export-{date}.{typeof(T).Name}.gz",
            DefaultExtension = $"{typeof(T).Name}.gz",
            FileTypeChoices = [new FilePickerFileType(typeof(T).Name) { Patterns = [$"*.{typeof(T).Name}.gz"] }]
        });
        if (file is null) return;

        var text = JsonConvert.SerializeObject(
            list.ToList(),
            Formatting.Indented,
            new JsonSerializerSettings { ContractResolver = resolver });

        await using var sourceStream = new MemoryStream(Encoding.UTF8.GetBytes(text));
        await using var fileStream = await file.OpenWriteAsync();
        await using var gzipStream = new GZipStream(fileStream, CompressionMode.Compress);
        try
        {
            await sourceStream.CopyToAsync(gzipStream);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    public async Task<IEnumerable<T>> ImportAsync()
    {
        var storage = GetStorageProvider();
        if (storage is null) return [];

        var files = await storage.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            AllowMultiple = false,
            FileTypeFilter = [new FilePickerFileType(typeof(T).Name) { Patterns = [$"*.{typeof(T).Name}.gz"] }]
        });
        if (files.Count == 0) return [];

        await using var fileStream = await files[0].OpenReadAsync();
        await using var resultStream = new MemoryStream();
        await using var gzipStream = new GZipStream(fileStream, CompressionMode.Decompress);
        try
        {
            await gzipStream.CopyToAsync(resultStream);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        var text = Encoding.UTF8.GetString(resultStream.ToArray());
        return JsonConvert.DeserializeObject<List<T>>(text) ?? [];
    }
}
