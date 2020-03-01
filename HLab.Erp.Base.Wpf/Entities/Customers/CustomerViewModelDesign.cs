using HLab.Erp.Base.Data;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Base.Wpf.Entities.Customers
{
    public class CustomerViewModelDesign : CustomerViewModel, IViewModelDesign
    {
        public CustomerViewModelDesign()
        {
            Model = Customer.GetDesignModel();
        }
    }
}
