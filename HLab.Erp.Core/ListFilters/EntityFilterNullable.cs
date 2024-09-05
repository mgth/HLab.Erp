﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Xml.Linq;
using HLab.Erp.Data;
using HLab.Notify.PropertyChanged;
using ReactiveUI;

namespace HLab.Erp.Core.ListFilters
{
    public class EntityFilterNullable<TClass> : Filter<int?>, IEntityFilterNullable<TClass>
    where TClass : class, IEntity, new()
    {
        static readonly MethodInfo ContainsMethod = typeof(List<int?>).GetMethod("Contains", new[] { typeof(int?) });

        public IEntityListViewModel<TClass> Target { get; }

        public virtual string Name => typeof(TClass).Name;

        public EntityFilterNullable(IEntityListViewModel<TClass> target)
        {
            Target = target;
            this.WhenAnyValue(
                e => e.Target.SelectedIds
                // TODO : , e => e.Target.List.Item()
            ).Subscribe(e => Update?.Invoke());

        }

        public Type ListClass => Target.GetType();

        public TClass Selected { get; set; }

        public override Expression<Func<T, bool>> Match<T>(Expression<Func<T, int?>> getter)
        {
            var listId = (Target.SelectedIds != null && Target.SelectedIds.Any())
                ? Target.SelectedIds.Cast<int?>().ToList()
                : Target.List.Select(e => (int?)e.Id).ToList();

            var entity = getter.Parameters[0];

            var value =
                Expression.Constant(listId
                    , typeof(List<int?>));

            var ex = Expression.Call(value, ContainsMethod, Expression.Convert(getter.Body, typeof(int?)));

            //Expression<Func<T, bool>> test = e => e == null;
            //var visitor = new SubstExpressionVisitor { Subst = { [test.Parameters[0]] = entity } };

            //var ex1 = Expression.Condition(visitor.Visit(test.Body), Expression.Constant(false, typeof(bool)), ex);

            return Expression.Lambda<Func<T, bool>>(ex, entity);
        }

        public override Func<T, bool> PostMatch<T>(Func<T, int?> getter)
        {
            var listId = (Target.SelectedIds != null && Target.SelectedIds.Any())
                ? Target.SelectedIds.Cast<int?>().ToList()
                : Target.List.Select(e => (int?)e.Id).ToList();

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
