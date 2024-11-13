using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using HLab.Erp.Data;
using HLab.Mvvm.Application;
using NPoco;

namespace HLab.Erp.Acl;

public class AclRight : Entity, IListableModel
{
    public static IDataService? Data { get; set; }
    AclRight(string name, string description)
    {
        Name = name;
        Description = description;
    }
    public string Description { get; set; }

    static readonly ConcurrentDictionary<string, AclRight?> Cache = new();
    public static AclRight? Create(string description ="", [CallerMemberName] string? name = null)
    {
        if(name == null) throw new ArgumentException("name is null");
        return Cache.GetOrAdd(name, e => CreateFromDb(e, description));
    }

    static AclRight? CreateFromDb(string name, string description)
    {
        return Data?.GetOrAdd<AclRight>(
            e => e.Name == name,
            e => e.Name = name)
            ?? new AclRight (name, description);
    }

    public string Name {get; set; }

    [Ignore]
    public string Caption => $"{{{Name}}}";

    [Ignore]
    public string IconPath => "icons/approved";

    public override string ToString() => Name;
}
