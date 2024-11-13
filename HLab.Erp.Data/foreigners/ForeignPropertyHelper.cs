using System.Runtime.CompilerServices;
using ReactiveUI;

namespace HLab.Erp.Data.foreigners;

public class ForeignPropertyHelper<TObj, TRet>(TObj parent, ObservableAsPropertyHelper<TRet> foreign)
    where TObj : class, IReactiveObject
    where TRet : class, IEntity<int>
{
    int? _id;

    public int? Id => _id;
    public TRet Value => foreign.Value;

    public void SetId(int? id, [CallerMemberName] string? name = null)
    {
        parent.RaiseAndSetIfChanged(ref _id, id, name);
    }
}