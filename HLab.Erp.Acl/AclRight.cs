using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using HLab.Erp.Data;
using HLab.Mvvm.Application;
using NPoco;
using ReactiveUI;

namespace HLab.Erp.Acl;

public class AclRight : Entity, IListableModel
{
    public static IDataService Data { get; set; }

    static readonly ConcurrentDictionary<string, AclRight> _cache = new();
    public static AclRight Get([CallerMemberName] string name = null)
    {
        return _cache.GetOrAdd(name, e => GetFromDb(name));
    }

    static AclRight GetFromDb(string name)
    {
        return Data?.GetOrAdd<AclRight>(
            e => e.Name == name,
            e => e.Name = name)
            ?? new AclRight { Name = name }
            ;
    }

    public string Name
    {
        get => _name;
        set => this.RaiseAndSetIfChanged(ref _name, value);
    }
    string _name;

    [Ignore]
    public string Caption => $"{{{Name}}}";//_caption.Get();

    //string _caption = HD<AclRight>.Property<string>(c => c.Set(e => $"{{{e.Name}}}").On(e => e.Name).Update());

    [Ignore]
    public string IconPath => "icons/approved";

    public override string ToString() => Name;
}
