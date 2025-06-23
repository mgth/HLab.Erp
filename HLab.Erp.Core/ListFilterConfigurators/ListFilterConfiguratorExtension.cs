﻿#nullable enable
using System;
using System.Linq.Expressions;
using System.Reflection;
using HLab.Base;
using HLab.Base.Extensions;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Core.ListFilters;
using HLab.Erp.Data;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Application;

namespace HLab.Erp.Core.ListFilterConfigurators;

public static class ListFilterConfiguratorExtension
{
    /// <summary>
    /// Start un new column
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="@this"></param>
    /// <param name="this"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    public static IColumnConfigurator<T,object,IFilter<object>> Column<T>(this IColumnConfigurator<T> @this, string? id=null)
        where T : class, IEntity, new()
        =>
            @this.GetColumnConfigurator().Name(id);
    public static IColumnConfigurator<T> HideMenu<T>(this IColumnConfigurator<T> @this)
        where T : class, IEntity, new()
        =>
            @this.ShowMenu(false);
    public static IColumnConfigurator<T> HideFilters<T>(this IColumnConfigurator<T> @this)
        where T : class, IEntity, new()
        =>
            @this.ShowFilters(false);
    public static IColumnConfigurator<T> ShowMenu<T>(this IColumnConfigurator<T> @this, bool value=true)
        where T : class, IEntity, new()
        =>
            @this.BuildList(l => l.ShowMenu = value);
    public static IColumnConfigurator<T> ShowFilters<T>(this IColumnConfigurator<T> @this, bool value=true)
        where T : class, IEntity, new()
        =>
            @this.BuildList(l => l.ShowFilters = value);

    /// <summary>
    /// Define a static filter that will not be editable
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="this"></param>
    /// <param name="filter"></param>
    /// <returns></returns>
    public static IColumnConfigurator<T> StaticFilter<T>(this IColumnConfigurator<T> @this, Expression<Func<T, bool>> filter)
        where T : class, IEntity, new()
        =>
            @this.BuildList(l => l.List.AddFilter(filter));

    public static IColumnConfigurator<T, string, IFilter<string>> Link<T, TLink, TFilter>(this IColumnConfigurator<T, TLink, TFilter> c, Expression<Func<T, string>> getter)
        where T : class, IEntity, new()
        where TFilter : class, IFilter<TLink>
    {
        var result = c
            .GetFilterConfigurator<string, IFilter<string>>()
            .AddProperty(getter,out var content)
            .ContentTemplate($@"<TextBlock Text=""{{Binding {content}}}""/>")
            .OrderByDefault(getter.Compile());

        result.LinkExpression = getter;

        return result.UpdateOn(getter);
    }

    public static IColumnConfigurator<T, bool?, IFilter<bool?>> Link<T, TLink, TFilter>(this IColumnConfigurator<T, TLink, TFilter> c, Expression<Func<T, bool?>> getter)
        where T : class, IEntity, new()
        where TFilter : class, IFilter<TLink>
    {
        var result = c.GetFilterConfigurator<bool?, IFilter<bool?>>();
        result.LinkExpression = getter;

        return result;
    }

    public static IColumnConfigurator<T, DateTime?, IFilter<DateTime?>> Link<T, TLink, TFilter>(this IColumnConfigurator<T, TLink, TFilter> c, Expression<Func<T, DateTime?>> getter)
        where T : class, IEntity, new()
        where TFilter : class, IFilter<TLink>
    {
        var result = c
            .GetFilterConfigurator<DateTime?, IFilter<DateTime?>>()
            .AddProperty(getter,out var content)
            .ContentTemplate($@"<TextBlock Text=""{{Binding {content}, StringFormat='dd/MM/yyyy'}}""/>");
        ;
        result.LinkExpression = getter;

        return result;
    }

    //1
    public static IColumnConfigurator<T, TLinkOut, IFilter<TLinkOut>> LinkGeneric<T, TLink, TFilter, TLinkOut>(this IColumnConfigurator<T, TLink, TFilter> c, Expression<Func<T, TLinkOut>> getter)
        where T : class, IEntity, new()
        where TFilter : class, IFilter<TLink>
    {
        var result = c.GetFilterConfigurator<TLinkOut, IFilter<TLinkOut>>();
        result.LinkExpression = getter;

        return result;
    }

    //2
    public static IColumnConfigurator<T, int, EntityFilter<TE>> Link<T, TE, TLink, TFilter>(this IColumnConfigurator<T, TLink, TFilter> c, Expression<Func<T, TE>> getter)
        where T : class, IEntity, new()
        where TFilter : class, IFilter<TLink>
        where TE : class, IEntity<int>, new()
    {
        var result = c.GetFilterConfigurator<int, EntityFilter<TE>>();
        result.LinkExpression = GetterIdFromGetter(getter);

        return result.UpdateOn(getter);
    }

    public static IColumnConfigurator<T, int?, EntityFilterNullable<TE>> LinkNullable<T, TE, TLink, TFilter>(this IColumnConfigurator<T, TLink, TFilter> c, Expression<Func<T, TE>> getter)
        where T : class, IEntity, new()
        where TFilter : class, IFilter<TLink>
        where TE : class, IEntity<int>, new()
    {
        var result =
            c.GetFilterConfigurator<int?, EntityFilterNullable<TE>>();

        result.LinkExpression = GetterIdNullableFromGetter(getter);

        return result.UpdateOn(getter);
    }
    public static IColumnConfigurator<T, int?, EntityFilterNullable<TE>> LinkNullable<T, TE, TLink, TFilter>(this IColumnConfigurator<T, TLink, TFilter> c, Expression<Func<T, TE>> getter, Expression<Func<T,int?>> idGetter)
        where T : class, IEntity, new()
        where TFilter : class, IFilter<TLink>
        where TE : class, IEntity<int>, new()
    {
        var result =
            c.GetFilterConfigurator<int?, EntityFilterNullable<TE>>();

        result.LinkExpression = GetterIdNullableFromGetter(getter);

        return result.UpdateOn(getter);
    }

    public static IColumnConfigurator<T, TLink, TFilterOut> Filter<T, TLink, TFilterIn, TFilterOut>(this IColumnConfigurator<T, TLink, TFilterIn> @this, TFilterOut filter)
        where T : class, IEntity, new()
        where TFilterOut : class, IFilter<TLink>
        where TFilterIn : class, IFilter<TLink>
    {
        return @this.LinkExpression!=null 
            ? @this.GetFilterConfigurator<TLink, TFilterOut>().FilterLink(@this.LinkExpression) 

            : @this.GetFilterConfigurator<TLink, TFilterOut>().FilterPostLink(@this.LinkLambda);
    }

    public static IColumnConfigurator<T, string, TextFilter> Filter<T, TFilter>(this IColumnConfigurator<T, string, TFilter> @this)
        where T : class, IEntity, new()
        where TFilter : class, IFilter<string>
    {
        return @this.GetFilterConfigurator<string, TextFilter>().Link(@this.LinkExpression);
    }

    public static IColumnConfigurator<T, bool?, BoolFilter> Filter<T, TFilter>(this IColumnConfigurator<T, bool?, TFilter> @this)
        where T : class, IEntity, new()
        where TFilter : class, IFilter<bool?>
    {
        return @this.GetFilterConfigurator<bool?, BoolFilter>().FilterLink(@this.LinkExpression);
    }

    public static IColumnConfigurator<T, DateTime?, DateFilterNullable> Filter<T, TFilter>(this IColumnConfigurator<T, DateTime?, TFilter> @this)
        where T : class, IEntity, new()
        where TFilter : class, IFilter<DateTime?>
    {
        return @this.GetFilterConfigurator<DateTime?, DateFilterNullable>().FilterLink(@this.LinkExpression);
    }

    public static IColumnConfigurator<T, DateTime, DateFilter> Filter<T, TFilter>(this IColumnConfigurator<T, DateTime, TFilter> @this)
        where T : class, IEntity, new()
        where TFilter : class, IFilter<DateTime>
    {
        return @this.GetFilterConfigurator<DateTime, DateFilter>().FilterLink(@this.LinkExpression);
    }

    static readonly MethodInfo GetIdMethod = typeof(IEntity<int>).GetProperty("Id")?.GetMethod ?? throw new InvalidOperationException();

    static Expression<Func<T, int>> GetIdExpression<T, TE>(Expression<Func<T, TE>> getter)
        where T : class, IEntity, new()
        where TE : class, IListableModel, IEntity<int>, new()
    {
        var entity = getter.Parameters[0];
        var ex = Expression.Call(getter.Body, GetIdMethod);
        return Expression.Lambda<Func<T, int>>(ex, entity);
    }

    static readonly MethodInfo PropertyHasValue = typeof(int?).GetProperty("HasValue")?.GetMethod ?? throw new InvalidOperationException();
    static readonly MethodInfo PropertyValue = typeof(int?).GetProperty("Value")?.GetMethod ?? throw new InvalidOperationException();

    // TODO : may return int?
    static Expression<Func<T, int>> GetterIdFromGetter<T, TE>(Expression<Func<T, TE>> getter)
        where T : class, IEntity, new()
        where TE : class, IEntity<int>, new()
    {
        if (getter.Body is MemberExpression member)
        {
            var name = member.Member.Name;
            var method = typeof(T).GetProperty($"{name}Id");//?.GetMethod;

            var entity = Expression.Parameter(typeof(T), "e");
            if (method != null)
            {
                var property = Expression.Property(entity, method);

                if (method.PropertyType.IsGenericType &&
                    method.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    return Expression.Lambda<Func<T, int>>(
                        Expression.Condition(
                            Expression.Call(property, PropertyHasValue),
                            Expression.Call(property, PropertyValue),
                            Expression.Constant(-1,typeof(int))), entity);
                }

                return Expression.Lambda<Func<T, int>>(property, entity);
            }
        }

        return t => -1;
    }

    static Expression<Func<T, int?>> GetterIdNullableFromGetter<T, TE>(Expression<Func<T, TE>> getter)
        where T : class, IEntity, new()
        where TE : class, IEntity<int>, new()
    {
        if (getter.Body is not MemberExpression member) return t => -1;

        var name = member.Member.Name;
        var method = member.Member.DeclaringType?.GetProperty($"{name}Id")?.GetMethod;

        if (method?.ReturnType != typeof(int?)) throw new InvalidOperationException("Entity should have been nullable");

        var entity = getter.Parameters[0];
        var property = Expression.Property(member.Expression, method);
        return Expression.Lambda<Func<T, int?>>(property, entity);

    }


    public static IColumnConfigurator<T, int, EntityFilter<TE>> Filter<T, TE>(this IColumnConfigurator<T, int, EntityFilter<TE>> c)
        where T : class, IEntity, new()
        where TE : class, IEntity<int>, new()

    {
        //var getter = GetterIdFromGetter(c.Getter);

        return  c.GetFilterConfigurator<int, EntityFilter<TE>>();
    }

    public static IColumnConfigurator<T, int?, EntityFilterNullable<TE>> Filter<T, TE>(this IColumnConfigurator<T, int?, EntityFilterNullable<TE>> c)
        where T : class, IEntity, new()
        where TE : class, IEntity<int>, new()

    {
        //var getter = GetterIdFromGetter(c.Getter);

        return c.GetFilterConfigurator<int?, EntityFilterNullable<TE>>();
    }
    public static IColumnConfigurator<T, int?, EntityFilterNullable<TE>> Column<T, TLink, TFilter, TE>(this IColumnConfigurator<T> c,
        Expression<Func<T, TE>> getter,
        string? id = null,
//            Expression<Func<T, int?>> getterId = null,
        double width = double.NaN
    )
        where T : class, IEntity, new()
        where TFilter : class, IFilter<TLink>
        where TE : class, IListableModel, IEntity<int>, new()
    {
        var lambda = getter.Compile();
//            getterId ??= GetterIdNullableFromGetter(getter);

        return c.Column(id)
                .Header($"{{{typeof(TE).Name}}}")
                .Width(width)
                .LinkNullable(getter)
                .Content( e => lambda(e))
                .OrderBy(e => c.Localize(lambda(e)?.Caption))
                // TODO                .Icon(e => lambda(e)?.IconPath)
                .Filter().IconPath($"Icons/Entities/{typeof(TE).Name}")
            ;
    }

    public static IColumnConfigurator<T> Header<T>(this IColumnConfigurator<T> @this, object caption)
        where T : class, IEntity, new()
    {
        if(caption is string s) caption = @this.Localize(s);

        return @this.BuildList(l =>
        {
            //TODO :
        });
    }

    public static IColumnConfigurator<T, TLink, TFilter> Header<T, TLink, TFilter>(this IColumnConfigurator<T, TLink, TFilter> @this, object caption)
        where T : class, IEntity, new()
        where TFilter : class, IFilter<TLink>
    {
        if(caption is string s) caption = @this.Localize(s);

        return @this.Build(b =>
        {
            b.Column.Header = caption;
            if (b.Filter != null) b.Filter.Header = caption;
        });
    }
    public static IColumnConfigurator<T, TLink, TFilter> UpdateOn<T, TLink, TFilter>(this IColumnConfigurator<T, TLink, TFilter> @this, Expression trigger)
        where T : class, IEntity, new()
        where TFilter : class, IFilter<TLink>
    {
        return @this.Build(b => b.Column.AddTrigger(trigger));
    }
    public static IColumnConfigurator<T, TLink, TFilter> UpdateOn<T, TLink, TFilter>(this IColumnConfigurator<T, TLink, TFilter> @this, Expression<Func<T,object>> trigger)
        where T : class, IEntity, new()
        where TFilter : class, IFilter<TLink>
    {
        return @this.Build(b => b.Column.AddTrigger(trigger));
    }

    public static IColumnConfigurator<T, TLink, TFilter> Width<T, TLink, TFilter>(this IColumnConfigurator<T, TLink, TFilter> @this, double width)
        where T : class, IEntity, new()
        where TFilter : class, IFilter<TLink>
    {
        return @this.Build(b => b.Column.Width = width);
    }
    public static IColumnConfigurator<T, TLink, TFilter> Hidden<T, TLink, TFilter>(this IColumnConfigurator<T, TLink, TFilter> @this)
        where T : class, IEntity, new()
        where TFilter : class, IFilter<TLink>
    {
        return @this.Build(b => b.Column.Hidden = true);
    }
    public static IColumnConfigurator<T, TLink, TFilter> Height<T, TLink, TFilter>(this IColumnConfigurator<T, TLink, TFilter> @this, double height)
        where T : class, IEntity, new()
        where TFilter : class, IFilter<TLink>
    {
        return @this.DecorateTemplate($"<Grid Height=\"{height}\">{XamlTool.ContentPlaceHolder}</Grid>");
    }
    public static IColumnConfigurator<T, TLink, TFilter> OrderBy<T, TLink, TFilter>(this IColumnConfigurator<T, TLink, TFilter> @this, Func<T, object?> orderBy)
        where T : class, IEntity, new()
        where TFilter : class, IFilter<TLink>
    {
        return @this.Build(b => b.Column.OrderBy = orderBy);
    }
    public static IColumnConfigurator<T, TLink, TFilter> OrderByAsc<T, TLink, TFilter>(this IColumnConfigurator<T, TLink, TFilter> @this, int orderByRank = -1)
        where T : class, IEntity, new()
        where TFilter : class, IFilter<TLink>
    {
        if (orderByRank < 0) orderByRank = @this.OrderByRank++;
        else @this.OrderByRank = orderByRank;

        return @this.Build(b =>
        {
            b.Column.OrderByRank = orderByRank;
            b.Column.SortDirection = SortDirection.Ascending;
        });
    }
    public static IColumnConfigurator<T, TLink, TFilter> OrderByDesc<T, TLink, TFilter>(this IColumnConfigurator<T, TLink, TFilter> @this, int orderByRank = -1)
        where T : class, IEntity, new()
        where TFilter : class, IFilter<TLink>
    {
        if (orderByRank < 0) orderByRank = @this.OrderByRank++;
        else @this.OrderByRank = orderByRank;

        return @this.Build(b =>
        {
            b.Column.OrderByRank = orderByRank;
            b.Column.SortDirection = SortDirection.Descending;
        });
    }
    public static IColumnConfigurator<T, TLink, TFilter> OrderByDefault<T, TLink, TFilter>(this IColumnConfigurator<T, TLink, TFilter> @this, Func<T, object?> orderBy)
        where T : class, IEntity, new()
        where TFilter : class, IFilter<TLink>
    {
        return @this.Build(b => b.Column.OrderBy ??= orderBy);
    }

    public static IColumnConfigurator<T, string, TextFilter> PostLink<T>(this IColumnConfigurator<T, string, TextFilter> @this, Func<T, string> getter)
        where T : class, IEntity, new()
    {
        @this.LinkLambda = getter;
        return @this.FilterPostLink(getter);
    }

    public static IColumnConfigurator<T, string, TextFilter> Link<T>(this IColumnConfigurator<T, string, TextFilter> @this, Expression<Func<T, string>> getter)
        where T : class, IEntity, new()
    {
        @this.LinkExpression = getter;
        //@this.LinkLambda = getter.Compile();
        return @this.FilterLink(getter);
    }

    public static IColumnConfigurator<T,TLink,TFilter> Name<T,TLink,TFilter>(this IColumnConfigurator<T,TLink,TFilter> @this, string? name)
        where T : class, IEntity, new() where TFilter : class, IFilter<TLink>
    {
        return string.IsNullOrWhiteSpace(name)?@this:@this.Build(b => b.Column.Name = name);
    }

    public static IColumnConfigurator<T, TLink, TFilter> IconPath<T,TLink,TFilter>(this IColumnConfigurator<T, TLink, TFilter> @this, string path)
        where T : class, IEntity, new()
        where TFilter : class, IFilter<TLink>
    {
        return @this
            .Build(b => b.Column.IconPath ??= path)
            .Build(b =>
            {
                if(b.Filter != null) b.Filter.IconPath ??= path;
            }) ;
    }

    public static IColumnConfigurator<T,TLink,TFilter> Content<T, TLink, TFilter>(this IColumnConfigurator<T,TLink,TFilter> @this, Expression<Func<T, string>> getter)
        where T : class, IEntity, new()
        where TFilter : class, IFilter<TLink> 
    {
        return @this
            .OrderByDefault(getter.Compile())
            .AddProperty(getter,out var content)
            .ContentTemplate($@"<TextBlock Text=""{{Binding {content}}}""/>");
    }


    public static IColumnConfigurator<T,TLink,TFilter> Content<T, TLink, TFilter>(this IColumnConfigurator<T,TLink,TFilter> @this, Expression<Func<T, object>> getter)
        where T : class, IEntity, new()
        where TFilter : class, IFilter<TLink> 
    {
        var result = @this
            .OrderByDefault(getter.Compile())
            .AddProperty(getter,out var content)
            .ContentTemplate(@$"<ContentPresenter Content=""{{Binding {content}}}""/>");

        return result;
    }

    public static IColumnConfigurator<T,TLink,TFilter> 
        Localize<T, TLink, TFilter>(this IColumnConfigurator<T,TLink,TFilter> @this, Expression<Func<T, string>> getter)

        where T : class, IEntity, new()
        where TFilter : class, IFilter<TLink> 
    {
        return Implementation?.Localize(@this
            .OrderByDefault(getter.Compile())
            .AddProperty(getter,out var content)
            , content)??@this;
    }

    public static IColumnConfigurator<T,TLink,TFilter> Date<T, TLink, TFilter>(this IColumnConfigurator<T,TLink,TFilter> @this, Expression<Func<T, DateTime?>> getter)
        where T : class, IEntity, new()
        where TFilter : class, IFilter<TLink> 
    {
        return Implementation?.Date( @this
            .OrderByDefault(getter.CastReturn(default(object)).Compile())
            .AddProperty(getter,out var content)
            , content) ?? @this;
    }

    public static IColumnConfigurator<T,TLink,TFilter> Date<T, TLink, TFilter>(this IColumnConfigurator<T,TLink,TFilter> @this
        , Expression<Func<T, DateTime?>> dateGetter
        , Expression<Func<T, bool>> dayValidGetter
    )
        where T : class, IEntity, new()
        where TFilter : class, IFilter<TLink> 
    {
        return Implementation?.Date( @this
            .OrderByDefault(dateGetter.CastReturn(default(object)).Compile())
            .AddProperty(dateGetter,out var date)
            .AddProperty(dayValidGetter,out var dayValid)
            , date, dayValid) ?? @this;
    }

    public static IColumnConfigurator<T, TLink, TFilter> Icon<T, TLink, TFilter>(this IColumnConfigurator<T, TLink, TFilter> @this, Expression<Func<T, string>> getPath, double size = 30.0)
        where T : class, IEntity, new()
        where TFilter : class, IFilter<TLink> => Implementation?.Icon( @this
        .AddProperty(getPath.CastReturn(default(object)), out var iconPath)
            , iconPath, size) ?? @this;

    public static IColumnConfigurator<T, TLink, TFilter> Center<T, TLink, TFilter>(this IColumnConfigurator<T, TLink, TFilter> @this)
        where T : class, IEntity, new()
        where TFilter : class, IFilter<TLink> =>
        @this.DecorateTemplate($"""<ContentPresenter HorizontalAlignment="Center">{XamlTool.ContentPlaceHolder}</ContentPresenter>""");

    public static IColumnConfigurator<T, TLink, TFilter> Mvvm<T, TLink, TFilter,TViewClass>(this IColumnConfigurator<T, TLink, TFilter> @this, TViewClass viewClass) 
        where T : class, IEntity, new()
        where TFilter : class, IFilter<TLink>
        where TViewClass : IViewClass
    {
        var viewClassType = typeof(TViewClass);

        return Implementation?.Mvvm(@this, viewClass)??@this;
    }

    public static IColumnConfigurator<T, TLink, TFilter> Mvvm<T, TLink, TFilter>(this IColumnConfigurator<T, TLink, TFilter> c) 
        where T : class, IEntity, new()
        where TFilter : class, IFilter<TLink>
    {
        return c.Mvvm(default(IListItemViewClass));
    }

    static readonly MemberInfo CaptionMember = typeof(IListableModel).GetProperty("Caption")?? throw new InvalidOperationException();
    static readonly MemberInfo IconPathMember = typeof(IListableModel).GetProperty("IconPath")?? throw new InvalidOperationException();

    public static IColumnConfigurator<T, int?, EntityFilterNullable<TE>> ColumnListable<T, TE>(this IColumnConfigurator<T> c,
        Expression<Func<T, TE>> getter,
        string id = null,
//            Expression<Func<T, int?>> getterId = null,
        double width = double.NaN
    )
        where T : class, IEntity, new()
        where TE : class, IListableModel, IEntity<int>, new()
    {
        var body = getter.Body;

        var caption = Expression.Lambda<Func<T,string>>(Expression.MakeMemberAccess(body, CaptionMember),getter.Parameters) ;
        var iconPath = Expression.Lambda<Func<T,string>>(Expression.MakeMemberAccess(body, IconPathMember),getter.Parameters) ;

        var lambda = getter.Compile();
//            getterId ??= GetterIdNullableFromGetter(getter);

      var c1 = c.Column(id);
      var c2 = c1.Header($"{{{typeof(TE).Name}}}");
      var c3 = c2.Width(width);
      var c4 = c3.LinkNullable(getter);
      var c5 = c4.Localize(caption);
      var c6 = c5.Icon(iconPath);
      var c7 = c6.OrderBy(e => c.Localize(lambda(e)?.Caption));
      var c8 = c7.Filter();
      var c9 = c8.IconPath($"Icons/Entities/{typeof(TE).Name}");
      
      return c9;

      return c.Column(id)
                .Header($"{{{typeof(TE).Name}}}")
                .Width(width)
                .LinkNullable(getter)
                .Localize(caption)
                .Icon(iconPath)
                .OrderBy(e => c.Localize(lambda(e)?.Caption))
                // TODO                .Icon(e => lambda(e)?.IconPath)
                .Filter().IconPath($"Icons/Entities/{typeof(TE).Name}")
//                    .Template($@"<{XamlTool.Type<IconView>(out var ns)} Path=""{{Binding Model.IconPath}}"" Caption=""{{Binding Model.Caption}}"" Height =""30"" IconMaxHeight =""20"" IconMaxWidth = ""20""/>")
            ;
    }

    public static IColumnConfigurator<T, object, IFilter<object>> ProgressColumn<T, TLink, TFilter>(this IColumnConfigurator<T, TLink, TFilter> @this,
        Expression<Func<T, double?>> getProgress)

        where T : class, IEntity, new()
        where TFilter : class, IFilter<TLink>
    {
        var getter = getProgress.Compile();

        if(Implementation == null) throw new("Platform not set");

        return Implementation.Progress( @this.Column("Progress")
            .Header("{Progress}").Width(80)
            .AddProperty(getProgress,out var progress), progress)
            .UpdateOn(getProgress)
            .OrderBy(s => getter(s));
    }

    public static IColumnConfigurator<T, object, IFilter<object>> 
        DescriptionColumn<T>(
        this IColumnConfigurator<T> @this,
        Expression<Func<T, object>> getTitle,
        Expression<Func<T, object>> getDescription,
        string id = null
    )

        where T : class, IEntity, new()
        => Implementation.Description( @this.Column(id)
            .AddProperty(getTitle, out var title)
            .AddProperty(getDescription, out var description)
            ,title, description);



    public static IListFilterConfiguratorPlatformImplementation? Implementation { get; set; }
}

public interface IListFilterConfiguratorPlatformImplementation
{
    IColumnConfigurator<T,TLink,TFilter> 
        Localize<T, TLink, TFilter>
        (
            IColumnConfigurator<T,TLink,TFilter> @this,
            string content
        )
        where T : class, IEntity, new()
        where TFilter : class, IFilter<TLink>;

    IColumnConfigurator<T,TLink,TFilter> 
        Progress<T, TLink, TFilter>
        (
            IColumnConfigurator<T,TLink,TFilter> @this,
            string content
        )
        where T : class, IEntity, new()
        where TFilter : class, IFilter<TLink>;

    IColumnConfigurator<T,TLink,TFilter> 
        Description<T, TLink, TFilter>
        (
            IColumnConfigurator<T,TLink,TFilter> @this,
            string title,
            string description
        )
        where T : class, IEntity, new()
        where TFilter : class, IFilter<TLink>;

    IColumnConfigurator<T, TLink, TFilter> 
        Date<T, TLink, TFilter>(IColumnConfigurator<T, TLink, TFilter> @this, string content) 
        where T : class, IEntity, new() 
        where TFilter : class, IFilter<TLink>;

    IColumnConfigurator<T, TLink, TFilter> 
        Date<T, TLink, TFilter>(IColumnConfigurator<T, TLink, TFilter> @this, string date, string dayValid) 
        where T : class, IEntity, new() 
        where TFilter : class, IFilter<TLink>;

    IColumnConfigurator<T, TLink, TFilter> 
        Icon<T, TLink, TFilter>(IColumnConfigurator<T, TLink, TFilter> @this, string iconPath, double size) 
        where T : class, IEntity, new() 
        where TFilter : class, IFilter<TLink>;

    IColumnConfigurator<T, TLink, TFilter> 
        Mvvm<T, TLink, TFilter, TViewClass>(IColumnConfigurator<T, TLink, TFilter> @this, TViewClass viewClass) 
        where T : class, IEntity, new() 
        where TFilter : class, IFilter<TLink>
        where TViewClass : IViewClass;

}