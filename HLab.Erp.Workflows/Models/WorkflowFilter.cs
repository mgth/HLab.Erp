using HLab.Erp.Core.ListFilters;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Xml.Linq;
using DynamicData;
using DynamicData.Binding;
using HLab.Base.ReactiveUI;
using ReactiveUI;
using HLab.Erp.Workflows.Interfaces;

namespace HLab.Erp.Workflows.Models
{
    public class WorkflowFilter<TClass> : Filter<string>, IWorkflowFilter
        where TClass : class, IWorkflow<TClass>
    {
        static readonly MethodInfo ContainsMethod = typeof(List<string>).GetMethod("Contains", new[] { typeof(string) });

        public class StageEntry : ReactiveModel
        {
            public bool Selected
            {
                get => _selected;
                set => SetAndRaise(ref _selected, value);
            }
            bool _selected;

            public IWorkflowStage Stage { get; set; }

            public string IconPath => Stage?.GetIconPath(null);
            public string Caption => Stage?.GetCaption(null);
        }

        public ReadOnlyObservableCollection<StageEntry> List { get; }
        readonly ObservableCollection<StageEntry> _list = new();

        public WorkflowFilter()
        {
            List = new(_list);

            _list.ToObservableChangeSet().AutoRefresh(e => e.Selected).Subscribe(e =>
            {
                Update?.Invoke();
            });

            this.WhenAnyValue(e => e.Enabled).Subscribe(e =>
            {
                Update?.Invoke();
            });

            var properties = typeof(TClass).GetFields(BindingFlags.Static | BindingFlags.Public)
                .Where(p => typeof(IWorkflowStage).IsAssignableFrom(p.FieldType));

            foreach (var p in properties)
            {
                var stage = (IWorkflowStage)p.GetValue(null);
                _list.Add(new StageEntry { Stage = stage });
            }
        }


        public TClass Selected { get; set; }


        public override Expression<Func<T, bool>> Match<T>(Expression<Func<T, string>> getter)
        {
            if (!Enabled) return null;

            var entity = getter.Parameters[0];
            var value = Expression.Constant(List.Where(e => e.Selected).Select(e => e.Stage.Name).ToList(), typeof(List<string>));

            var ex = Expression.Call(value, ContainsMethod, getter.Body);

            return Expression.Lambda<Func<T, bool>>(ex, entity);
        }

        public override Func<TSource, bool> PostMatch<TSource>(Func<TSource, string> getter)
        {
            throw new NotImplementedException();
        }

        public override XElement ToXml()
        {
            var element = base.ToXml();

            foreach (var value in List)
            {
                if (value.Selected)
                {
                    element.Add(new XElement(value.Stage.Name));
                }
            }
            return element;
        }

        public override void FromXml(XElement element)
        {
            foreach (var stage in _list)
            {
                stage.Selected = element.Elements().Any(e => e.Name == stage.Stage.Name);
            }
        }
    }
}