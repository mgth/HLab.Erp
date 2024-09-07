using HLab.Erp.Data;
using HLab.Mvvm.Application;

namespace HLab.Erp.Core.EntityLists;

public interface IListableEntityListViewModel<T>
    where T : class, IEntity, IListableModel, new()
{

}