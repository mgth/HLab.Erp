using System.ComponentModel;
using HLab.Erp.Acl;
using HLab.Erp.Base.Data;
using HLab.Erp.Base.Wpf.Entities.Customers;
using HLab.Erp.Data;
using HLab.Mvvm.Application;

namespace HLab.Erp.Base.Customers;

public abstract class CorporationViewModel<T>(EntityViewModel<T>.Injector i)
    : ListableEntityViewModel<T>(i), ICorporationViewModel
    where T : class, IEntity<int>, INotifyPropertyChanged, ICorporation, IListableModel
{
    ICorporation ICorporationViewModel.Model => Model;
}