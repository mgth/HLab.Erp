using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using HLab.Erp.Data;
using HLab.Erp.Data.Observables;
using Newtonsoft.Json.Serialization;

namespace HLab.Erp.Core.EntityLists;

public interface IEntityListHelper<T> : IEntityListHelper where T : class, IEntity, new()
{
    void Populate(object grid, IColumnsProvider<T> provider);

    Task ExportAsync(IObservableQuery<T> list, IContractResolver resolver);
    public Task<IEnumerable<T>> ImportAsync();
}

public interface IEntityListHelper
{
    object GetListView(IList list);
    void DoOnDispatcher(object grid, Action action);
}