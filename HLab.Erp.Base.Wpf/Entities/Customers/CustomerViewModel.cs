using HLab.Erp.Base.Data;

namespace HLab.Erp.Base.Wpf.Entities.Customers
{
    public interface ICorporationViewModel
    {
        ICorporation Model { get; }
    }

    public class CustomerViewModel : CorporationViewModel<CustomerViewModel, Customer>
    {


    }
}
