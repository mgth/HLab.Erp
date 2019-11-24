using HLab.Mvvm.Annotations;

namespace HLab.Erp.Core.ListFilters
{
    public class FilterTextDesignViewModel : FilterTextViewModel, IViewModelDesign
    {
        public FilterTextDesignViewModel()
        {
            Value = "DummySearch";
        }
    }
}