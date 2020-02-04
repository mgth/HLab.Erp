using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using HLab.Base;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Core.ViewModels;
using HLab.Mvvm;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Core.EntitySelectors
{
    using H = DependencyHelper<ForeignView>;
    /// <summary>
    /// Logique d'interaction pour ForeignView.xaml
    /// </summary>
    [ContentProperty(nameof(ButtonContent))]
    public partial class ForeignView : UserControl, IView<IForeignViewModel>, IViewClassForeign
    {
        [Import]
        private IMvvmService Mvvm;
        
        [Import]
        private IExportLocatorScope Scope;

        public ForeignView()
        {
            InitializeComponent();
            Locator.SetValue(ViewLocator.ModelProperty,null);
        }

        public static readonly DependencyProperty ModelProperty = H.Property<object>()
            .BindsTwoWayByDefault
            .OnChange((s,a) => s.Locator.SetValue(ViewLocator.ModelProperty, a.NewValue))
            .Register();

        public static readonly DependencyProperty ModelClassProperty = H.Property<Type>()
            .Register();

        public static readonly DependencyProperty ListClassProperty = H.Property<Type>()
            .OnChange( (s,a) => s.SetList() )
            .Register();

        public static readonly DependencyProperty IsReadOnlyProperty = H.Property<bool>()
            .OnChange( (s,a) => s.SetReadOnly(a.NewValue) )
            .Register();

        public static readonly DependencyProperty CommandProperty = H.Property<ICommand>()
            .OnChange( (s,a) => s.SetCommand(a.NewValue) )
            .Register();

        public static readonly DependencyProperty ButtonContentProperty = H.Property<object>()
            .OnChange( (s,a) => s.SetButtonContent(a.NewValue) )
            .Register();

        private void SetCommand(ICommand command)
        {
            Locator.Visibility = command == null ? Visibility.Visible : Visibility.Collapsed;
            OpenButton.Visibility = command == null ? Visibility.Visible : Visibility.Collapsed;
        }

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

        public bool IsReadOnly
        {
            get => (bool)GetValue(IsReadOnlyProperty);
            set => SetValue(IsReadOnlyProperty, value);
        }
        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }
        public object ButtonContent
        {
            get => (object)GetValue(ButtonContentProperty);
            set => SetValue(ButtonContentProperty, value);
        }

        private void SetButtonContent(object content)
        {
            Label.Content = content;
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
                var type = ListClass;
                if (type == null)
                {
                    if (typeof(IListableModel).IsAssignableFrom(ModelClass))
                        type = typeof(ListableEntityListViewModel<>).MakeGenericType(ModelClass);
                }
                if(type==null)
                { 
                    PopupContent.Content = null;
                    return;
                }

                var ctx = ViewLocator.GetMvvmContext(this);

                var vm = ctx.Scope.Locate(type,this);

                if (vm is IEntityListViewModel lvm)
                {
                    if (Command != null)
                    {

                        lvm.SetSelectAction(t =>
                        {
                            Popup.IsOpen = false;
                            Command.Execute(t);
                        });
                    }
                    else
                    {
                        lvm.SetSelectAction(t =>
                        {
                            Popup.IsOpen = false;
                            Model = t;
                        });
                    }
                }


                var view = ctx.GetView(vm,typeof(ViewModeDefault),typeof(IViewClassDefault));
                PopupContent.Content = view;
            }
        }

        private void OpenButton_OnClick(object sender, RoutedEventArgs e)
        {
            ViewLocator.GetMvvmContext(this).Scope.Locate<IDocumentService>().OpenDocumentAsync(Model);
        }
    }
}
