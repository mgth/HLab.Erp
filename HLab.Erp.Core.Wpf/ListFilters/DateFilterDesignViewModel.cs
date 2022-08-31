using System;
using HLab.Erp.Core.ListFilters;

namespace HLab.Erp.Core.Wpf.ListFilters
{
    public class DateFilterDesignViewModel : DateFilter
    {
        public DateFilterDesignViewModel()
        {
            MinDate = DateTime.Now.AddDays(-10);
            MaxDate = DateTime.Now;
        }
    }
}