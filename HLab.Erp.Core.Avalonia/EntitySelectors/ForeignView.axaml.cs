using System;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Metadata;
using HLab.Base.Avalonia.DependencyHelpers;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Core.EntitySelectors;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Application;
using HLab.Mvvm.Application.Documents;
using HLab.Mvvm.Avalonia;

namespace HLab.Erp.Core.Avalonia.EntitySelectors;

using H = DependencyHelper<ForeignView>;

/// <summary>
/// Logique d'interaction pour ForeignView.axaml
/// </summary>
public partial class ForeignView : UserControl, IView<IForeignViewModel>, IViewClassForeign, global::HLab.Base.Avalonia.Controls.IMandatoryNotFilled
{
    public AvaloniaProperty MandatoryProperty => ModelProperty;

    public ForeignView()
    {
        InitializeComponent();
        IsVisible = false;
    }

    public static readonly StyledProperty<object?> ModelProperty = H.Property<object?>()
        .BindModeDefault(global::Avalonia.Data.BindingMode.TwoWay)
        .OnChanged((v, a) => v.OnModelChanged(a.NewValue.Value))
        .Register();

    void OnModelChanged(object? newValue)
    {
        Locator.SetValue(ViewLocator.ModelProperty, newValue);
        OpenButton.IsEnabled = newValue != null;
    }

    public static readonly StyledProperty<Type?> ModelClassProperty = H.Property<Type?>()
        .OnChanged((s, a) => s.SetModelClass())
        .Register();

    public static readonly StyledProperty<Type?> ListClassProperty = H.Property<Type?>()
        .OnChanged((s, a) => s.SetList())
        .Register();

    public static readonly StyledProperty<object?> SecondaryModelProperty = H.Property<object?>()
        .BindModeDefault(global::Avalonia.Data.BindingMode.TwoWay)
        .Register();

    public static readonly StyledProperty<Type?> SecondaryModelClassProperty = H.Property<Type?>()
        .OnChanged((s, a) => s.SetModelClass())
        .Register();

    public static readonly StyledProperty<bool> IsReadOnlyProperty = H.Property<bool>()
        .OnChanged((s, a) => s.SetReadOnly(a.NewValue.Value))
        .Register();

    public static readonly StyledProperty<ICommand?> CommandProperty = H.Property<ICommand?>()
        .OnChanged((s, a) => s.SetCommand(a.OldValue.Value, a.NewValue.Value))
        .Register();

    public static readonly StyledProperty<object?> ButtonContentProperty = H.Property<object?>()
        .OnChanged((s, a) =>
        {
            if (s.ButtonContentHost is { } host) host.Content = a.NewValue.Value;
        })
        .Register();

    public static readonly StyledProperty<bool> MandatoryNotFilledProperty = H.Property<bool>()
        .OnChanged((s, a) => s.SetMandatoryNotFilled(a.NewValue.Value))
        .Register();

    void SetCommand(ICommand? oldCommand, ICommand? command)
    {
        if (oldCommand != null) oldCommand.CanExecuteChanged -= Command_CanExecuteChanged;

        if (command == null)
        {
            Locator.IsVisible = true;
            OpenButton.IsVisible = true;
        }
        else
        {
            Locator.IsVisible = false;
            OpenButton.IsVisible = false;
            command.CanExecuteChanged += Command_CanExecuteChanged;
            Button.IsEnabled = command?.CanExecute(null) ?? true;
        }
    }

    void Command_CanExecuteChanged(object? sender, EventArgs e)
    {
        Button.IsEnabled = Command?.CanExecute(null) ?? true;
    }

    public object? Model
    {
        get => GetValue(ModelProperty);
        set => SetValue(ModelProperty, value);
    }

    public Type? ModelClass
    {
        get => GetValue(ModelClassProperty);
        set => SetValue(ModelClassProperty, value);
    }

    public Type? ListClass
    {
        get => GetValue(ListClassProperty);
        set => SetValue(ListClassProperty, value);
    }

    public object? SecondaryModel
    {
        get => GetValue(SecondaryModelProperty);
        set => SetValue(SecondaryModelProperty, value);
    }

    public Type? SecondaryModelClass
    {
        get => GetValue(SecondaryModelClassProperty);
        set => SetValue(SecondaryModelClassProperty, value);
    }

    public bool IsReadOnly
    {
        get => GetValue(IsReadOnlyProperty);
        set => SetValue(IsReadOnlyProperty, value);
    }

    public bool MandatoryNotFilled
    {
        get => GetValue(MandatoryNotFilledProperty);
        set => SetValue(MandatoryNotFilledProperty, value);
    }

    public ICommand? Command
    {
        get => GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    [Content]
    public object? ButtonContent
    {
        get => GetValue(ButtonContentProperty);
        set => SetValue(ButtonContentProperty, value);
    }

    void SetList()
    {
    }

    void SetModelClass()
    {
        IsVisible = ModelClass != null;
    }

    void SetMandatoryNotFilled(bool mnf)
    {
        Mandatory.IsVisible = mnf;
    }

    void SetReadOnly(bool ro)
    {
        Button.IsVisible = !ro;
    }

    void ButtonBase_OnClick(object? sender, RoutedEventArgs e)
    {
        if (Popup.IsOpen) return;

        Popup.IsOpen = true;

        var type = ListClass;
        if (type == null)
        {
            if (typeof(IListableModel).IsAssignableFrom(ModelClass))
                type = typeof(ListableEntityListViewModel<>).MakeGenericType(ModelClass!);
        }
        if (type == null)
        {
            PopupContent.Content = null;
            return;
        }

        var ctx = ViewLocator.GetMvvmContext(this);
        if (ctx == null) return;

        object? vm;

        if (SecondaryModel != null)
        {
            var secondaryType = SecondaryModel.GetType();
            type = typeof(Func<,>).MakeGenericType(secondaryType, type);
            var func = ctx.Locate(type);

            var m = type.GetMethod("Invoke");

            vm = m?.Invoke(func, new[] { SecondaryModel });
        }
        else
        {
            vm = ctx.Locate(type);
        }

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
                    SetCurrentValue(ModelProperty, t);
                    Locator.DataContext = t;
                });
            }
        }

        var view = ctx.GetView(vm, typeof(DefaultViewMode), typeof(IDefaultViewClass));
        PopupContent.Content = view;
    }

    void OpenButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (Model == null) return;
        var ctx = ViewLocator.GetMvvmContext(this);
        var doc = ctx?.Locate<IDocumentService>();
        doc?.OpenDocumentAsync(Model);
    }
}
