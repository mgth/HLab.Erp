using System;

namespace HLab.Erp.Core.ListFilters
{
    public class FilterDateDesignViewModel : DateFilter
    {
        public FilterDateDesignViewModel()
        {
            MinDate = DateTime.Now.AddDays(-10);
            MaxDate = DateTime.Now;
        }
    }
}