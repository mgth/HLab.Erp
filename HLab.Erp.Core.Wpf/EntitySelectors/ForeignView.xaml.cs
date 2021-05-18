using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using Grace.DependencyInjection;
using HLab.Base.Wpf;
using HLab.Erp.Core.EntityLists;
using HLab.Mvvm;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Application;

namespace HLab.Erp.Core.EntitySelectors
{

    using H = DependencyHelper<ForeignView>;
    /// <summary>
    /// Logique d'interaction pour ForeignView.xaml
    /// </summary>
    [ContentProperty(nameof(ButtonContent))]
    public partial class ForeignView : UserControl, IView<IForeignViewModel>, IViewClassForeign, IMandatoryNotFilled
    {
        public static DependencyInjectionContainer Container {get; set;}

        public ForeignView()
        {
            InitializeComponent();
            Locator.SetValue(ViewLocator.ModelProperty,null);
        }

        public static readonly DependencyProperty ModelProperty = H.Property<object>()
            .BindsTwoWayByDefault
            .OnChange((v,a) => v.OnModelChanged(a))
            .Register();

        private void OnModelChanged(DependencyPropertyChangedEventArgs<object> args)
        {
            var value = args.NewValue;
            Locator.SetValue(ViewLocator.ModelProperty, args.NewValue);
            OpenButton.IsEnabled = value!=null;
        }

        public static readonly DependencyProperty ModelClassProperty = H.Property<Type>()
            .Register();

        public static readonly DependencyProperty ListClassProperty = H.Property<Type>()
            .OnChange( s => s.SetList() )
            .Register();

        public static readonly DependencyProperty IsReadOnlyProperty = H.Property<bool>()
            .OnChange( (s,a) => s.SetReadOnly(a.NewValue) )
            .Register();

        public static readonly DependencyProperty CommandProperty = H.Property<ICommand>()
            .OnChange( (s,a) => s.SetCommand(a.OldValue,a.NewValue) )
            .Register();

        public static readonly DependencyProperty ButtonContentProperty = H.Property<object>()
            .OnChange( (s,a) => s.SetButtonContent(a.NewValue) )
            .Register();

        public static readonly DependencyProperty MandatoryNotFilledProperty = H.Property<bool>()
            .OnChange( (s,a) => s.SetMandatoryNotFilled(a.NewValue) )
            .Register();

        private void SetCommand(ICommand oldCommand,ICommand command)
        {
            if(oldCommand!=null) oldCommand.CanExecuteChanged -= Command_CanExecuteChanged;

            if (command == null)
            {
                Locator.Visibility = Visibility.Visible;
                OpenButton.Visibility = Visibility.Visible;

            }
            else
            {
                Locator.Visibility = Visibility.Collapsed;
                OpenButton.Visibility = Visibility.Collapsed;
                command.CanExecuteChanged += Command_CanExecuteChanged;
                Button.IsEnabled = command?.CanExecute(null)??true;
            }
        }

        private void Command_CanExecuteChanged(object sender, EventArgs e)
        {
            Button.IsEnabled = Command?.CanExecute(null)??true;
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

        public DependencyProperty MandatoryProperty => ForeignView.ModelProperty;

        public bool MandatoryNotFilled
        {
            get => (bool)GetValue(MandatoryNotFilledProperty);
            set => SetValue(MandatoryNotFilledProperty, value);
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
        private void SetMandatoryNotFilled(bool mnf)
        {
            Mandatory.Visibility = mnf ? Visibility.Visible : Visibility.Collapsed;
        }
        private void SetReadOnly(bool ro)
        {
            Button.Visibility = ro ? Visibility.Collapsed : Visibility.Visible;
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            if (!Popup.IsOpen)
            {
                Popup.IsOpen = true;
                var type = ListClass;
                if (type == null)
                {
                    if (typeof(IListableModel).IsAssignableFrom(ModelClass))
                        type = typeof(ListableEntityListViewModel<>).MakeGenericType(ModelClass);
                        //type = typeof(IEntityListViewModel<>).MakeGenericType(ModelClass);
                }
                if(type==null)
                { 
                    PopupContent.Content = null;
                    return;
                }

                var ctx = ViewLocator.GetMvvmContext(this);

                var test = ctx.Scope.WhatDoIHave();//.Locate(type);
                var vm = ctx.Scope.Locate(type);

                //                ctx.Scope.Inject(vm);

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
            if(Model == null) return;
            ViewLocator.GetMvvmContext(this).Scope.Locate<IDocumentService>().OpenDocumentAsync(Model);
        }
    }
}
