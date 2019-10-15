using System;
using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Windows;
using System.Windows.Markup;

using HLab.Base;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Acl;
using HLab.Erp.Acl.Wpf.LoginServices;
using HLab.Erp.Core.Wpf.DragDrops;
using HLab.Erp.Core.Wpf.Extensions;
using HLab.Erp.Core.Wpf.Update;
using HLab.Mvvm;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Core.Wpf.ApplicationServices
{
    public class BootLoaderErpWpf : BootLoaderErp
    {

        public IUpdater Updater { get; set; }
        [Import]
        private IApplicationInfoService _info;
        [Import]
        private Func<MainWpfViewModel> _getVm;
        [Import]
        private IMenuService _menu;
        [Import]
        private IAclService _acl;
        [Import]
        private IMvvmService _mvvm;
        [Import]
        private IDragDropService _dragdrop;


        public void SetMainViewMode(Type vm)
        {
            MainViewMode = vm;
        }
        public Type MainViewMode { get; private set; }

        public MainWpfViewModel ViewModel { get; set; } //= new ApplicationViewModel().SetParentMvvmContext(MvvmService.MainContext);
        public Window MainWindow { get; protected set; }
 
        private void PreRegisterPlugin(IErpService erpService)
        {
            var assembly = erpService.GetType().Assembly;

            var types = assembly.GetTypesSafe();

            foreach (var type in types)
            {
                foreach (var m in type.GetMethods())
                {
                    if (m.GetCustomAttribute<OnRegisterPlugin>() != null)
                    {
                        if (m.IsStatic)
                        {
                            //m.Invoke(null,null);
                            var action = (Action)Delegate.CreateDelegate(typeof(Action), m);
                            action();
                        }
                        else throw new InvalidOperationException("OnRegisterPlugin should be static");
                    }
                }
            }
        }
        private static void InitializeCultures()
        {
            FrameworkElement.LanguageProperty.OverrideMetadata(
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(
                    XmlLanguage.GetLanguage(
                        CultureInfo.CurrentCulture.IetfLanguageTag)));
        }


        protected override void RegisterAssembly(Assembly assembly)
        {
            RegisterIcons(assembly);

        }

        public static void RegisterIcons(Assembly assembly)
        {
            var rm = new ResourceManager(assembly.GetName().Name + ".g", assembly);
            try
            {
                var dict = new ResourceDictionary();
                var list = rm.GetResourceSet(CultureInfo.CurrentCulture, true, true);
                foreach (DictionaryEntry item in list)
                {
                    //resourceNames.Add((string)item.Key);
                    string s = item.Key.ToString();
                    if (s.StartsWith("icons/"))
                    {
                        var uri = new Uri("/" + assembly.GetName().Name + ";component/" + s.Replace(".baml", ".xaml"), UriKind.RelativeOrAbsolute);
                        var obj = Application.LoadComponent(uri);

                        string key = s.Replace("icons/", "").Replace(".baml", "");

                        if (obj.GetType() == typeof(ResourceDictionary))
                        {
                            Application.Current.Resources.MergedDictionaries.Add(obj as ResourceDictionary);
                        }
                        else
                            dict.Add(key, obj);
                    }
                }
                Application.Current.Resources.MergedDictionaries.Add(dict);
            }
            catch (MissingManifestResourceException) { }
            finally
            {
                rm.ReleaseAllResources();
            }
        }

        [Import]
        public Func<LoginViewModel> GetLoginViewModel { get; set; }

        [Import]
        public Func<ProgressLoadingViewModel> GetProgressLoadingViewModel { get; set; }



        public override void Load()
        {
            MainWindow = new Mvvm.Wpf.DefaultWindow()
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                WindowState =  WindowState.Maximized
            };

            MainWindow.Closing += (sender, args) => Application.Current.Shutdown();
            MainWindow.Show();

            base.Load();

            _info.Version = Assembly.GetCallingAssembly().GetName().Version;

            InitializeCultures();


            var vm = GetLoginViewModel();
            var loginWindow = _mvvm.MainContext.GetView(vm,MainViewMode).AsWindow();
            loginWindow.WindowState = WindowState.Maximized;

            loginWindow.ShowDialog();

            if (_acl.Connection == null)
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
                        return;
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
            var w = _mvvm.MainContext.GetView(ViewModel,MainViewMode);// new MainWindow{DataContext = ViewModel};

            //MainWindow = (MainWindow)MvvmService.D.MainViewModeContext.GetView<ViewModeDefault>(ViewModel);// new MainWindow{DataContext = ViewModel};

            _menu.RegisterMenu(null, "file", "File", null, null);
            _menu.RegisterMenu(null, "data", "Data", null, null);
            _menu.RegisterMenu(null, "tools", "Tools", null, null);
            _menu.RegisterMenu(null, "help", "_?", null, null);

            //_menu.RegisterMenu("file", "add", "_Ajouter", null, null);
            //_menu.RegisterMenu("file", "open", "_Ouvrir", null, null);


            _menu.RegisterMenu("file","exit","Exit", ViewModel.Exit,null);


            //w.Show();
            MainWindow.Content = w;
        }
    }
}
