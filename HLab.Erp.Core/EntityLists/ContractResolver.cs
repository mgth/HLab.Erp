using HLab.Erp.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NPoco;

namespace HLab.Erp.Core.Wpf.EntityLists
{

    public abstract partial class EntityListViewModel<T> where T : class, IEntity, new()
    {
        private class ContractResolver : DefaultContractResolver
        {
            protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
            {
                var properties = base.CreateProperties(type, memberSerialization);

                List<JsonProperty> outputList = new();

                foreach (var p in properties)
                {
                    if (!p.Writable) continue;
                    if (p.PropertyType == null) continue;
                    if (p.PropertyType == typeof(string))
                    {
                        if (p.AttributeProvider.GetAttributes(true).OfType<IgnoreAttribute>().Any()) continue;
                        outputList.Add(p);
                        continue;
                    }
                    if (p.PropertyType.IsClass)
                    {
                        if (typeof(IEntityWithExportId).IsAssignableFrom(p.PropertyType))
                        {
                            p.ValueProvider = new ExportIdValueProvider(p.ValueProvider);
                            outputList.Add(p);
                            continue;
                        }
                    }
                    if (p.AttributeProvider.GetAttributes(true).OfType<IgnoreAttribute>().Any()) continue;

                    if (p.PropertyType.IsInterface) continue;

                    outputList.Add(p);
                }


                return outputList;
            }
        }
    }
}
