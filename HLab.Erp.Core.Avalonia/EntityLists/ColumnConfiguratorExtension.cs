using System;
using System.Linq.Expressions;
using System.Reflection;
using HLab.Base;
using HLab.Base.Avalonia.Controls;
using HLab.Base.Extensions;
using HLab.Erp.Core.Avalonia.EntityLists;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Core.ListFilterConfigurators;
using HLab.Erp.Core.ListFilters;
using HLab.Erp.Data;
using HLab.Icons.Avalonia.Icons;
using HLab.Localization.Avalonia.Lang;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Application;
using HLab.Mvvm.Avalonia;

namespace HLab.Erp.Core.Avalonia.EntityLists
{
    public static class ColumnConfiguratorExtension
    {

        public static IColumnConfigurator<T> AllowDelete<T>(this IColumnConfigurator<T> @this, Expression<Func<T, bool>> getter)
            where T : class, IEntity, new()
        {
            return @this.BuildList(l => { });
        }



        public static IColumnConfigurator<T, TLink, TFilter> Date<T, TLink, TFilter>(this IColumnConfigurator<T, TLink, TFilter> @this, Expression<Func<T, DateTime?>> getter)
            where T : class, IEntity, new()
            where TFilter : class, IFilter<TLink>
        {
            return @this
                .OrderByDefault(getter.CastReturn(default(object)).Compile())
                .AddProperty(getter, out var content)
                .ContentTemplate($@"<{XamlTool.Type<DateColumnItem>()} HorizontalAlignment=""Right"" Date=""{{Binding {content}}}"" DayValid=""True""/>");
        }

        public static IColumnConfigurator<T, TLink, TFilter> Date<T, TLink, TFilter>(this IColumnConfigurator<T, TLink, TFilter> @this
            , Expression<Func<T, DateTime?>> dateGetter
            , Expression<Func<T, bool>> dayValidGetter
            )
            where T : class, IEntity, new()
            where TFilter : class, IFilter<TLink>
        {
            return @this
                .OrderByDefault(dateGetter.CastReturn(default(object)).Compile())
                .AddProperty(dateGetter, out var date)
                .AddProperty(dayValidGetter, out var dayValid)
                .ContentTemplate($@"<{XamlTool.Type<DateColumnItem>()} HorizontalAlignment=""Right"" Date=""{{Binding {date}}}"" DayValid=""{{Binding {dayValid}}}""/>");
        }

        /// <summary>
        /// Icon
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TLink"></typeparam>
        /// <typeparam name="TFilter"></typeparam>
        /// <param name="c"></param>
        /// <param name="getPath"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static IColumnConfigurator<T, TLink, TFilter> Icon<T, TLink, TFilter>(this IColumnConfigurator<T, TLink, TFilter> c, Expression<Func<T, string>> getPath, double size = 30.0)
            where T : class, IEntity, new()
            where TFilter : class, IFilter<TLink> => c
                .AddProperty(getPath.CastReturn(default(object)), out var iconPath)
                .DecorateTemplate($@"<{XamlTool.Type<IconView>(out var ns)} Path=""{{Binding {iconPath}}}"" IconMaxHeight =""{size}"" IconMaxWidth = ""{size}"">{XamlTool.ContentPlaceHolder}</{XamlTool.Type<IconView>(ns)}>");

        public static IColumnConfigurator<T, TLink, TFilter> Localize<T, TLink, TFilter>(this IColumnConfigurator<T, TLink, TFilter> @this, Expression<Func<T, string>> getter)
            where T : class, IEntity, new()
            where TFilter : class, IFilter<TLink>
        {
            return @this
                .OrderByDefault(getter.Compile())
                .AddProperty(getter, out var content)
                .ContentTemplate($@"<{XamlTool.Type<Localize>()} Id=""{{Binding {content}}}""/>");
        }

        //public static IColumnConfigurator<T,TLink,TFilter> Localize<T, TLink, TFilter>(this IColumnConfigurator<T,TLink,TFilter> c)
        //    where T : class, IEntity, new()
        //    where TFilter : class, IFilter<TLink> => c
        //    .DecorateTemplate(@$"<{XamlTool.Type<Localize>(out var ns)}>{XamlTool.ContentPlaceHolder}</{XamlTool.Type<Localize>(ns)}>");

        public static IColumnConfigurator<T, TLink, TFilter> Center<T, TLink, TFilter>(this IColumnConfigurator<T, TLink, TFilter> c)
            where T : class, IEntity, new()
            where TFilter : class, IFilter<TLink> =>
            c.DecorateTemplate(@$"<ContentPresenter HorizontalAlignment=""Center"">{XamlTool.ContentPlaceHolder}</ContentPresenter>");

        public static IColumnConfigurator<T, TLink, TFilter> Mvvm<T, TLink, TFilter, TViewClass>(this IColumnConfigurator<T, TLink, TFilter> c, TViewClass viewClass)
            where T : class, IEntity, new()
            where TFilter : class, IFilter<TLink>
            where TViewClass : IViewClass
        {
            var viewClassType = typeof(TViewClass);

            return c
                //                .AddValue(c.Column.Getter,out var model)
                .DecorateTemplate(@$"
                <{XamlTool.Type<ViewLocator>(out var ns1)}
                    {XamlTool.Namespace<TViewClass>(out var ns2)}
                    ViewClass=""{{x:Type {XamlTool.Type<TViewClass>(ns2)}}}""
                >
                    <{XamlTool.Type<ViewLocator>(ns1)}.Model>{XamlTool.ContentPlaceHolder}</{XamlTool.Type<ViewLocator>(ns1)}.Model>
                </{XamlTool.Type<ViewLocator>(ns1)}>");
        }
        //        {TH.ContentPlaceHolder}            DataContext=""{{Binding Value}}""
        public static IColumnConfigurator<T, TLink, TFilter> Mvvm<T, TLink, TFilter>(this IColumnConfigurator<T, TLink, TFilter> c)
            where T : class, IEntity, new()
            where TFilter : class, IFilter<TLink>
        {
            return c.Mvvm(default(IListItemViewClass));
        }


        static readonly MemberInfo CaptionMember = typeof(IListableModel).GetProperty("Caption") ?? throw new InvalidOperationException();
        static readonly MemberInfo IconPathMember = typeof(IListableModel).GetProperty("IconPath") ?? throw new InvalidOperationException();

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

            var caption = Expression.Lambda<Func<T, string>>(Expression.MakeMemberAccess(body, CaptionMember), getter.Parameters);
            var iconPath = Expression.Lambda<Func<T, string>>(Expression.MakeMemberAccess(body, IconPathMember), getter.Parameters);

            var lambda = getter.Compile();
            //            getterId ??= GetterIdNullableFromGetter(getter);

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


    }
}