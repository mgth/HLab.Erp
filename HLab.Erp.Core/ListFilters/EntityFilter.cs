using HLab.Erp.Data;
using HLab.Mvvm.Application;

namespace HLab.Erp.Core.ListFilters
{
    public class EntityFilter<TClass, TList> : EntityFilter<TClass>, IEntityFilterNotNull<TClass>
        where TClass : class, IEntity, IListableModel, new()
        where TList : class, IEntityListViewModel<TClass>
    {
        public EntityFilter(IEntityListViewModel<TClass> target) : base(target)
        {
            Header = $"{{{typeof(TClass).Name}}}";
            IconPath = $"Icons/Entities/{typeof(TClass).Name}";
        }
    }

    public class EntityFilterNullable<TClass, TList> : EntityFilterNullable<TClass>, IEntityFilterNullable<TClass>
        where TClass : class, IEntity, IListableModel, new()
        where TList : class, IEntityListViewModel<TClass>
    {
        public EntityFilterNullable(IEntityListViewModel<TClass> target) : base(target)
        {
            Header = $"{{{typeof(TClass).Name}}}";
            IconPath = $"Icons/Entities/{typeof(TClass).Name}";
        }
    }

    public class EntityFilterListable<TClass> : EntityFilter<TClass>, IEntityFilterNotNull<TClass>
        where TClass : class, IEntity, IListableModel, new()
    {
        public EntityFilterListable(IEntityListViewModel<TClass> target) : base(target)
        {
            Header = $"{{{typeof(TClass).Name}}}";
            IconPath = $"Icons/Entities/{typeof(TClass).Name}";
        }
    }

    public class EntityFilterNullableListable<TClass> : EntityFilterNullable<TClass>, IEntityFilterNullable<TClass>
        where TClass : class, IEntity, IListableModel, new()
    {
        public EntityFilterNullableListable(IEntityListViewModel<TClass> target) : base(target)
        {
            Header = $"{{{typeof(TClass).Name}}}";
            IconPath = $"Icons/Entities/{typeof(TClass).Name}";
        }
    }
}