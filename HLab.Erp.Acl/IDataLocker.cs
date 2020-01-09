using System;
using System.Windows.Input;

namespace HLab.Erp.Acl
{
    public interface IDataLocker : IDisposable
    {
        bool IsActive { get; }
        ICommand ActivateCommand { get; }
        ICommand SaveCommand { get; }
        ICommand CancelCommand { get; }
    }
}
