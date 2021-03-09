using HLab.Mvvm.Annotations;

namespace HLab.Erp.Core.ListFilters
{
    public class FilterTextDesignViewModel : TextFilter, IViewModelDesign
    {
        public FilterTextDesignViewModel()
        {
            Value = "DummySearch";
        }
    }
}