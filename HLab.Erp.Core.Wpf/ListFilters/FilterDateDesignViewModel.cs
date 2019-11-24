using System;

namespace HLab.Erp.Core.ListFilters
{
    public class FilterDateDesignViewModel : FilterDateViewModel
    {
        public FilterDateDesignViewModel()
        {
            MinDate = DateTime.Now.AddDays(-10);
            MaxDate = DateTime.Now;
        }
    }
}