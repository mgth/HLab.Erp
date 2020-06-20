using System;
using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Windows;
using System.Windows.Markup;
using HLab.Base;
using HLab.Core.Annotations;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Acl;
using HLab.Erp.Core.DragDrops;
using HLab.Erp.Core.Update;
using HLab.Mvvm;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Views;

namespace HLab.Erp.Core.ApplicationServices
{
    public class BootLoaderErpWpf : BootLoaderErp
    {

        public IUpdater Updater { get; set; }
        [Import] private readonly Func<MainWpfViewModel> _getVm;

        [Import] private readonly IErpServices _erp;

        public void SetMainViewMode(Type vm)
        {
            MainViewMode = vm;
        }
        public Type MainViewMode { get; private set; }

        public MainWpfViewModel ViewModel { get; set; } //= new ApplicationViewModel().SetParentMvvmContext(MvvmService.MainContext);
        public Window MainWindow { get; protected set; }
 
        private static void InitializeCultures()
        {
            FrameworkElement.LanguageProperty.OverrideMetadata(
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(
                    XmlLanguage.GetLanguage(
                        CultureInfo.CurrentCulture.IetfLanguageTag)));
        }


        [Import]
        public Func<ILoginViewModel> GetLoginViewModel { get; set; }

        [Import]
        public Func<ProgressLoadingViewModel> GetProgressLoadingViewModel { get; set; }



        public override void Load(IBootContext b)
        {
            if (b.Contains("LocalizeBootloader"))
            {
                b.Requeue();
                return;
            }

            base.Load(b);

            MainWindow = new /*Default*/Window()
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                WindowState =  WindowState.Maximized
            };

            MainWindow.Closing += (sender, args) => Application.Current.Shutdown();
            MainWindow.Show();

            _erp.Info.Version = Assembly.GetCallingAssembly().GetName().Version;

            InitializeCultures();


            var vm = GetLoginViewModel();
            var loginWindow = _erp.Mvvm.MainContext.GetView(vm,MainViewMode).AsWindow();
            //loginWindow.WindowState = WindowState.Maximized;

            loginWindow.ShowDialog();

            if (_erp.Acl.Connection == null)
            {
                Application.Current.Shutdown();
                return;
            }

            if (Updater != null )
            {
                Updater.CheckVersion();

                if (Updater.NewVersionFound)
                {
                    var updaterView = new ApplicationUpdateView
                    {
                        DataContext = Updater
                    };
                    // TODO : updaterView.ShowDialog();

                    if (Updater.Updated)
                    {
                        Application.Current.Shutdown();
                        return;;
                    }
                }
            }




            
            //Dispatcher progressDisptacher = null;
            //var uiThread = new Thread(() =>
            //{
            //    var vm2 = GetProgressLoadingViewModel();
            //    vm2.Title = _info.Name;

            //    var progressWindow = new ProgressLoadingView
            //    {
            //        DataContext = vm2,
            //        Topmost = true
            //    };
            //    progressWindow.Show();
            //    progressDisptacher = progressWindow.Dispatcher;

            //    // allowing the main UI thread to proceed 
            //    Dispatcher.Run();
            //});
            //uiThread.SetApartmentState(ApartmentState.STA);
            //uiThread.IsBackground = true;
            //uiThread.Start();


            //progressDisptacher?.BeginInvokeShutdown(DispatcherPriority.Send);



            ViewModel = _getVm();
            var w = _erp.Mvvm.MainContext.GetView(ViewModel,MainViewMode);


            _erp.Menu.RegisterMenu("file", "{File}", null, null);
            _erp.Menu.RegisterMenu("data", "{Data}", null, null);
            _erp.Menu.RegisterMenu("param", "{Parameters}", null, null);
            _erp.Menu.RegisterMenu("tools", "{Tools}", null, null);
            _erp.Menu.RegisterMenu("help", "{_?}", null, null);


            _erp.Menu.RegisterMenu("file/exit","{Exit}", ViewModel.Exit,null);

            MainWindow.Content = w;

            return;
        }
    }
}
