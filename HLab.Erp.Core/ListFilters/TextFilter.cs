﻿using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Xml.Linq;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Core.ListFilters
{
    public class TextFilter : Filter<string>
    {
        public TextFilter()
        {
        }

        static readonly MethodInfo ContainsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
        static readonly MethodInfo ToLowerMethod = typeof(string).GetMethod("ToLower", new Type[] { });

        public override Expression<Func<T, bool>> Match<T>(Expression<Func<T, string>> getter)
        {
            if (string.IsNullOrEmpty(Value)) return t => true;

            var entity = getter.Parameters[0];
            var value = Expression.Constant(Value.ToLower(), typeof(string));

            var ex1 = Expression.Call(getter.Body, ToLowerMethod);
            var ex = Expression.Call(ex1, ContainsMethod, value);

            return Expression.Lambda<Func<T, bool>>(ex, entity);
        }

        public override Func<TSource, bool> PostMatch<TSource>(Func<TSource, string> getter)
            => s => Value == null || getter(s).ToLower().Contains(Value.ToLower());

        public override XElement ToXml()
        {
            var element = base.ToXml();

            element.SetAttributeValue("Value",Value);

            return element;
        }
        public override void FromXml(XElement element)
        {
            var value = element.Attribute("Value");
            if (value != null)
            {
                Value = value.Value;
            }
        }    
    }
}