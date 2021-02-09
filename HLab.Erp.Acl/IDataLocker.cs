using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace HLab.Erp.Acl
{
    public interface IDataLocker : IDisposable
    {
        bool IsActive { get; }
        ICommand ActivateCommand { get; }
        ICommand SaveCommand { get; }
        ICommand CancelCommand { get; }
        Task<bool> SaveAsync(string caption, string iconPath, bool sign, bool motivate);
    }
}
