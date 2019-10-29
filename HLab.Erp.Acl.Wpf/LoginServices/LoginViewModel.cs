using System;
using System.ComponentModel;
using System.Net;
using System.Runtime.CompilerServices;
using System.Security;
using System.Windows;
using System.Windows.Input;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Data.Observables;
using HLab.Mvvm;
using HLab.Mvvm.Annotations;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Acl.LoginServices
{
    [Export(typeof(LoginViewModel))]
    public class LoginViewModel : AuthenticationViewModel<LoginViewModel>
    {
        public string Title => "Connexion";

        [Import]
        public ILocalizationService LocalizationService { get; }

        public string PinView
        {
            get => _pinView.Get();
            set => _pinView.Set(value);
        }
        private IProperty<string> _pinView = H.Property<string>();

        private string _pin = "";
        public ICommand NumPadCommand { get; } = H.Command(c => c
            
            .CanExecute((e) => (e.Login?.Length ?? 0) > 0)
            .Action(
            async (e,n) =>
            {
                if (e._pin.Length > 4) e._pin = "";
                e._pin += (string)n;

                e.PinView = new String('.', e._pin.Length);

                if (e._pin.Length == 4)
                {
                    e.Message = await e.Acl.Login(new NetworkCredential(e.Credential.UserName, e._pin), true);
                    e._pin = "";
                    e.PinView = "";
                }
            }
            )
            .On(e => e.Login)
            .On(e => e.Password)
            .CheckCanExecute()
        );

        public ICommand LoginCommand { get; } = H.Command( c => c
            .CanExecute(e => (e.Login?.Length??0)>0)
            .Action(async e =>
                {
                    e.Message = await e.Acl.Login(e.Credential);
                })                      
            .On(e => e.Login).CheckCanExecute()
        );
    }
}
