using System.Collections.Generic;
using System.Linq;
using HLab.Erp.Data;
using HLab.Erp.Data.Observables;

namespace HLab.Erp.Base.Data;

public interface IUnitService
{
        IObservableQuery<Unit> Units { get; }
        IEnumerable<Unit> GetGroup(string group);

        Unit Default(string group);
}

public class UnitService : IUnitService
{
    IDataService _db;

    public UnitService(IDataService db, IObservableQuery<Unit> units)
    {
        _db = db;
        Units = units;
    }

    public IObservableQuery<Unit> Units { get; private set;}

    public IEnumerable<Unit> GetGroup(string group)
    {
        return Units.Where(u => u.UnitClass.Symbol == group);
    }

    public Unit Default(string @group)
    {
        return Units.FirstOrDefault(u => u.UnitClass.Symbol == group && u.Default);
    }
}
