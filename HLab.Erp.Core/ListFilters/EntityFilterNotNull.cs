using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;

using HLab.Base;
using HLab.Erp.Data;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Core.Wpf.ListFilters
{
    public class EntityFilter<TClass> : Filter<int>, IEntityFilterNotNull<TClass>
        where TClass : class, IEntity, new()
    {

        private static readonly MethodInfo ContainsMethod = typeof(List<int>).GetMethod("Contains", new[] { typeof(int) });

        public IEntityListViewModel<TClass> Target { get; }


        private SuspenderToken _token;

        public EntityFilter(IEntityListViewModel<TClass> target)
        {
            _token = target.List.Suspender.Get();
            Target = target;

            H<EntityFilter<TClass>>.Initialize(this);

        }

        private ITrigger _ = H<EntityFilter<TClass>>.Trigger(c => c
            .On(e => e.Target.SelectedIds)
            .On(e => e.Target.List.Item())
            .Do(e => e.Update?.Invoke())
        );


        public Type ListClass => Target.GetType();

        public TClass Selected { get; set; }

        protected override void Enable()
        {
            _token?.Dispose();
            _token = null;
        }
        protected override void Disable()
        {
            _token?.Dispose();
            _token = Target.List.Suspender.Get();
        }

        public override Expression<Func<T, bool>> Match<T>(Expression<Func<T, int>> getter)
        {
            var literal = getter.ToString();

            var listId = (Target.SelectedIds != null && Target.SelectedIds.Any())
                ? Target.SelectedIds.ToList()
                : Target.List.Select(e => (int)e.Id).ToList();

            var entity = getter.Parameters[0];


            var value =
                Expression.Constant(listId
                    , typeof(List<int>));

            var ex = Expression.Call(value, ContainsMethod, Expression.Convert(getter.Body, typeof(int)));

            var literal2 = getter.ToString();
            return Expression.Lambda<Func<T, bool>>(ex, entity);
        }


        public override Func<T, bool> PostMatch<T>(Func<T, int> getter)
        {
            var listId = (Target.SelectedIds != null && Target.SelectedIds.Any())
                ? Target.SelectedIds.ToList()
                : Target.List.Select(e => (int)e.Id).ToList();

            return e => listId.Contains(getter(e));
        }

        public override XElement ToXml()
        {
            var element = base.ToXml();

            element.Add(Target.FiltersToXml());

            if(Target.SelectedIds!=null && Target.SelectedIds.Count() > 0)
            {
                var xItems = new XElement("SelectedItems");
                foreach(var item in Target.SelectedIds)
                {
                    var xItem = new XElement("item");
                    xItem.SetAttributeValue("Id", item.ToString());
                    xItems.Add(xItem);
                }
                element.Add(xItems);
            }


            element.SetAttributeValue("Value",Value);

            return element;
        }
        public override void FromXml(XElement element)
        {
            foreach(var child in element.Elements())
            {
                if(child.Name == "Filters")
                {
                    Target.FiltersFromXml(child);
                }
                else if(child.Name == "SelectedItems")
                {
                    Target.SelectedIds = child.Elements().Select(c => int.Parse(c.Attribute("Id")?.Value)).ToArray();
                }
            }
        }    
    }
}