using HLab.Erp.Base.Customers;
using HLab.Erp.Base.Data;
using HLab.Erp.Core;

namespace HLab.Erp.Base.Wpf.Entities.Customers;

public interface ICorporationViewModel
{
    ICorporation Model { get; }
}

public class CustomerViewModel : CorporationViewModel<Customer>
{
    public CustomerViewModel(Injector i) : base(i)
    {
    }
}