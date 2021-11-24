using System;
using System.Linq.Expressions;
using System.Xml.Linq;

using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Core.Wpf.ListFilters
{
    public class BoolFilter : Filter<bool?>
    {
        public BoolFilter() => H<BoolFilter>.Initialize(this);


        public override Expression<Func<T, bool>> Match<T>(Expression<Func<T, bool?>> getter)
        {
            if (!Value.HasValue) return t => true;

            var entity = getter.Parameters[0];
            var value = Expression.Constant(Value, typeof(bool?));

            var ex = Expression.Equal(getter.Body, value); //Expression.Call(ex1, ContainsMethod, value);

            return Expression.Lambda<Func<T, bool>>(ex, entity);
        }

        public override Func<TSource, bool> PostMatch<TSource>(Func<TSource, bool?> getter)
            => s => Value == null || getter(s) == Value.Value;
        public override XElement ToXml()
        {
            var element = base.ToXml();

            if(Value.HasValue)
                element.SetAttributeValue("Value",Value.Value?"1":"0");
            else
                element.SetAttributeValue("Value","N");

            return element;
        }
        public override void FromXml(XElement element)
        {
            var value = element.Attribute("Value");
            if (value != null)
            {
                Value = value.Value switch
                {
                    "0" => false,
                    "1" => true,
                    _ => null,
                };
            }
        }

    }
}