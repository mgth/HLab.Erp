using System;

namespace HLab.Erp.Core.ListFilters
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