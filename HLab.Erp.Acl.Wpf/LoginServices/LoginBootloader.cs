using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HLab.Core.Annotations;
using HLab.DependencyInjection.Annotations;
using HLab.Mvvm;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Views;

namespace HLab.Erp.Acl.LoginServices
{
    public class LoginBootloader : IBootloader
    {
        public ILoginViewModel _viewModel;
        public IMvvmService _mvvm;
        public IAclService _acl;

        [Import]public LoginBootloader(IMvvmService mvvm, ILoginViewModel viewModel, IAclService acl)
        {
            _mvvm = mvvm;
            _viewModel = viewModel;
            _acl = acl;
        }

        public void Load(IBootContext bootstrapper)
        {
            var loginWindow = _mvvm.MainContext.GetView(_viewModel,typeof(ViewModeDefault)).AsWindow();
            ////loginWindow.WindowState = WindowState.Maximized;

            loginWindow.ShowDialog();

            if (_acl.Connection == null)
            {
                System.Windows.Application.Current.Shutdown();
                return;
            }
        }
    }
}
