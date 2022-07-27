using HLab.Erp.Data;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace HLab.Erp.Acl
{
    public interface IDataLocker : IDisposable
    {
        bool IsActive { get; }
        bool IsEnabled { get; set; }
        bool IsReadOnly { get; }
        ICommand ActivateCommand { get; }
        ICommand SaveCommand { get; }
        ICommand CancelCommand { get; }
        string Message { get; }

        Task<bool> SaveAsync(string caption, string iconPath, bool sign, bool motivate);

        Task SaveAsync(IDataTransaction transaction);
        Task DirtyCancelAsync();
        Task CancelAsync();
        Task<bool> ActivateAsync();

        void AddDependencyLocker(params IDataLocker[] lockers);

    }
}
