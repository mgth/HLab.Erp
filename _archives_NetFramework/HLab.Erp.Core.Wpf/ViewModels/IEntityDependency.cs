using HLab.Erp.Data;

namespace HLab.Erp.Core.Wpf.ViewModels
{
    interface IEntityDependency<T> where T:IEntity
    {
        IEntity Entity { get; }
    }
}
