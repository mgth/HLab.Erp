using HLab.Erp.Core.ListFilters;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Core.Wpf.ListFilters
{
    public class TextFilterDesignViewModel : TextFilter, IViewModelDesign
    {
        public TextFilterDesignViewModel()
        {
            Value = "DummySearch";
        }
    }
}