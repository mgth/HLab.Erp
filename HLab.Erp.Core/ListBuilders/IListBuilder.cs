using System.Collections.Generic;
using System.Dynamic;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Core.ListFilters;
using Ubiety.Dns.Core;

namespace HLab.Erp.Core.ListBuilders
{
    public class ListBuilderHelper<T>
    {
        Dictionary<string, IFilter> _filters;
        Dictionary<string, IColumn> _column;


        public object Header { get; set; }
        public string IconPath { get; set; }
       // IColumn<T> Column(string id);
       // IFilter<T> Filter<TFilter>(string id)
    }

    public interface IListBuilder
    {
        IListBuilder Column();
        IListBuilder Header();
        IListBuilder IconPath();
        IListBuilder Filter();
        IListBuilder OrderBy();
        IListBuilder Width();
        IListBuilder AddValue();
    }


}
