using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Threading.Tasks;
using HLab.Base;
using HLab.Base.Extensions;
using HLab.Erp.Acl;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Core.ListFilters;
using HLab.Erp.Data;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Core.ListFilterConfigurators
{
    public class ColumnConfigurator<T>(IEntityListViewModel<T> list, ILocalizationService localization, IAclService acl)
       : IColumnConfigurator<T>
       where T : class, IEntity, new()
    {
        public ILocalizationService Localization { get; } = localization;
        public IAclService Acl { get; } = acl;

        public virtual IColumnConfigurator<T, object, IFilter<object>> GetColumnConfigurator()
        {
            return new ColumnConfigurator<T, object, IFilter<object>>(
                new ColumnBuilder<T>(list, new Column<T>())
                ,Localization,Acl
            );
        }

        public IColumnConfigurator<T> BuildList(Action<IEntityListViewModel<T>> build)
        {
            build(list);
            return this;
        }
      
        public IColumnConfigurator<T> List<TT>(out TT list1) where TT : IEntityListViewModel<T>
        {
            list1 = (TT)list;
            return this;
        }

        public virtual void Dispose()
        {
        }
    }

    public class ColumnConfigurator<T, TLink, TFilter> : ColumnConfigurator<T>, IColumnConfigurator<T, TLink, TFilter>

        where T : class, IEntity, new()
        where TFilter : class, IFilter<TLink>
    {
        public IColumnConfigurator<T, TLinkChild, TFilterChild> GetFilterConfigurator<TLinkChild, TFilterChild>()
        where TFilterChild : class, IFilter<TLinkChild>
        {
            return new ColumnConfigurator<T, TLinkChild, TFilterChild>(_builder,Localization,Acl);
        }

        public override IColumnConfigurator<T, object, IFilter<object>> GetColumnConfigurator()
        {
            Dispose();
            return base.GetColumnConfigurator();
        }

        public int OrderByRank { get => _builder.OrderByRank ; set => _builder.OrderByRank = value; }

        public ColumnConfigurator(IColumn<T>.IBuilder builder, ILocalizationService localization, IAclService acl):base(builder.ListViewModel,localization,acl)
        {
            _builder = builder;

            var filter = builder.GetFilter<TFilter>();
            if (filter == null) return;

            var link = builder.GetLinkExpression<TLink>();
            if (link == null) return;

            filter?.Link(builder.ListViewModel.List, link);
        }

        class ColumnBuilder(IColumn<T>.IBuilder builder) : IColumnBuilder<T, TLink, TFilter>
        {
           public IEntityListViewModel<T> ListViewModel => builder.ListViewModel;
            public IColumn<T> Column => builder.Column;
            public TFilter Filter => builder.GetFilter<TFilter>();

            public int OrderByRank
            {
                get => builder.OrderByRank;
                set => builder.OrderByRank = value;
            }
        }

        public IColumnConfigurator<T, TLink, TFilter> Build(Action<IColumnBuilder<T, TLink, TFilter>> builder)
        {

            if(_builder is IColumnBuilder<T, TLink, TFilter> b)
                builder(b);
            else
            {
                builder(new ColumnBuilder(_builder));
            }
            return this;
        }

        public IColumnConfigurator<T, TLink, TFilter> FilterLink(Expression<Func<T, TLink>> getter)
        {
            var filter = _builder.GetFilter<TFilter>();
            filter?.Link(_builder.ListViewModel.List,getter);
            return this;
        }

        public IColumnConfigurator<T, TLink, TFilter> FilterPostLink(Func<T, TLink> getter)
        {
            var filter = _builder.GetFilter<TFilter>();
            filter?.PostLink(_builder.ListViewModel.List,getter);
            return this;
        }

        public IColumnConfigurator<T, TLink, TFilter> AddProperty<TOut>(Expression<Func<T, TOut>> getter, out string name)
        {
            name = _builder.ListViewModel.Columns.AddProperty(getter);
            return this;
        }

        public IColumnConfigurator<T, TLink, TFilter> AddProperty(Func<T, bool> condition, Func<T, object> getter, out string name)
        {
            name = _builder.ListViewModel.Columns.AddProperty( condition, getter);
            return this;
        }

        public IColumnConfigurator<T, TLink, TFilter> AddProperty(string name, Func<T, bool> condition, Func<T, object> getter)
        {
            _builder.ListViewModel.Columns.AddProperty( name, condition, getter);
            return this;
        }

        public IColumnConfigurator<T, TLink, TFilter> DecorateTemplate(string template)
        {
            Debug.Assert(template.Contains(XamlTool.ContentPlaceHolder));
            _builder.DataTemplateSource = template.Replace(XamlTool.ContentPlaceHolder, _builder.DataTemplateSource);
            return this;
        }
        public IColumnConfigurator<T, TLink, TFilter> ContentTemplate(string template)
        {
            _builder.DataTemplateSource = _builder.DataTemplateSource.Replace(XamlTool.ContentPlaceHolder, template);
            return this;
        }

        public TFilter Filter => _builder.GetFilter<TFilter>();
        public Expression<Func<T, TLink>> LinkExpression
        {
            get => _builder.Link as Expression<Func<T, TLink>>;
            set
            {
                _builder.Column.OrderBy ??= value.CastReturn(default(object)).Compile();
                _builder.Link = value;
            }
        }

        public Func<T, TLink> LinkLambda
        {
            get => _builder.PostLink as Func<T, TLink>;
            set
            {
                _builder.Column.OrderBy ??= t => value(t);
                _builder.PostLink = value;
            }
        }
        readonly IColumn<T>.IBuilder _builder;

        public Task<string> Localize(string s) => Localization.LocalizeAsync(s);

        public override void Dispose()
        {
            _builder.Build();
            base.Dispose();
        }
    }
}