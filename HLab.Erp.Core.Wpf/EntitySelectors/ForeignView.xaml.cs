using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using HLab.Base;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Core.ViewModels;
using HLab.Mvvm;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Core.EntitySelectors
{
    using H = DependencyHelper<ForeignView>;
    /// <summary>
    /// Logique d'interaction pour ForeignView.xaml
    /// </summary>
    public partial class ForeignView : UserControl, IView<IForeignViewModel>, IViewClassForeign
    {
        [Import]
        private IMvvmService Mvvm;
        
        [Import]
        private IExportLocatorScope Scope;

        public ForeignView()
        {
            InitializeComponent();
        }

        private static readonly DependencyProperty ModelProperty = H.Property<object>()
            .BindsTwoWayByDefault
            .OnChange((s,a) => s.Locator.SetValue(ViewLocator.ModelProperty, a.NewValue))
            .Register();

        private static readonly DependencyProperty ModelClassProperty = H.Property<Type>()
//            .OnChange( (s,a) => s.SetList() )
            .Register();

        private static readonly DependencyProperty ListClassProperty = H.Property<Type>()
            .OnChange( (s,a) => s.SetList() )
            .Register();

        private static readonly DependencyProperty MvvmContextProperty = H.Property<IMvvmContext>()
//            .OnChange( (s,a) => s.SetList() )
            .Register();

        private static readonly DependencyProperty IsReadOnlyProperty = H.Property<bool>()
            .OnChange( (s,a) => s.SetReadOnly(a.NewValue) )
            .Register();

        public object Model
        {
            get => GetValue(ModelProperty);
            set => SetValue(ModelProperty, value);
        }
        public Type ModelClass
        {
            get => (Type)GetValue(ModelClassProperty);
            set => SetValue(ModelClassProperty, value);
        }
        public Type ListClass
        {
            get => (Type)GetValue(ListClassProperty);
            set => SetValue(ListClassProperty, value);
        }
        public IMvvmContext MvvmContext
        {
            get => (IMvvmContext)GetValue(MvvmContextProperty);
            set => SetValue(MvvmContextProperty, value);
        }
        public bool IsReadOnly
        {
            get => (bool)GetValue(IsReadOnlyProperty);
            set => SetValue(IsReadOnlyProperty, value);
        }

        private void SetList()
        {
        }
        private void SetReadOnly(bool ro)
        {
            Button.Visibility = ro ? Visibility.Collapsed : Visibility.Visible;
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            Popup.IsOpen = !Popup.IsOpen;
            if (Popup.IsOpen)
            {
                if (ListClass == null)
                {
                    PopupContent.Content = null;
                    return;
                }

                var vm = MvvmContext.Scope.Locate(ListClass,this);

                if(vm is IListViewModel lvm)
                    lvm.SetOpenAction(t =>
                    {
                        Popup.IsOpen = false;
                        Model = t;
                    });


                var view = MvvmContext.GetView(vm,typeof(ViewModeDefault),typeof(IViewClassDefault));
                PopupContent.Content = view;
            }
        }
    }
}
