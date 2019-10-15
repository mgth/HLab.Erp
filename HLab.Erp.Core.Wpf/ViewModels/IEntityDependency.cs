using HLab.Erp.Data;

namespace HLab.Erp.Core.ViewModels
{
    interface IEntityDependency<T> where T:IEntity
    {
        IEntity Entity { get; }
    }
}
