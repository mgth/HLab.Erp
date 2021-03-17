using HLab.DependencyInjection.Annotations;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Data;

namespace HLab.Erp.Core.ListFilters
{
    public class EntityFilter<TClass,TList> : EntityFilterViewModel<TClass>, IEntityFilterViewModel
        where TClass : class, IEntity, IListableModel, new()
        where TList : class, IEntityListViewModel<TClass>
    {
        [Import] public EntityFilter(TList list) : base(list)
        {
            Title = $"{{{typeof(TClass).Name}}}";
            IconPath = $"Icons/Entities/{typeof(TClass).Name}";
        }
    }

    public class EntityFilter<TClass> : EntityFilterViewModel<TClass>, IEntityFilterViewModel
        where TClass : class, IEntity, IListableModel, new()
    {
        [Import]
        public EntityFilter(ListableEntityListViewModel<TClass> list) : base(list)
        {
            Title = $"{{{typeof(TClass).Name}}}";
            IconPath = $"Icons/Entities/{typeof(TClass).Name}";
        }
    }
}