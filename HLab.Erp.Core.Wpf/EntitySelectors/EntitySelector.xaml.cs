using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Input;

namespace HLab.Erp.Core.EntitySelectors
{
    /// <summary>
    /// Logique d'interaction pour EntitySelector.xaml
    /// </summary>
    public partial class EntitySelector : UserControl
    {
        public EntitySelector()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            SearchTextBox.Focus();
        }

        void Control_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ButtonOk.DoClick();
        }

        public bool OkOnSingleClick { get; set; }
        public bool SearchEnabled { get; set; }

        public Visibility SearchVisibility => SearchEnabled ? Visibility.Visible : Visibility.Hidden;

        public static readonly DependencyProperty OkCommandProperty =
            DependencyProperty.RegisterAttached(
                nameof(OkCommand),
                typeof(ICommand),
                typeof(EntitySelector),
                new UIPropertyMetadata(null));

        public static readonly DependencyProperty CancelCommandProperty =
            DependencyProperty.RegisterAttached(
                nameof(CancelCommand),
                typeof(ICommand),
                typeof(EntitySelector),
                new UIPropertyMetadata(null));

        public ICommand OkCommand
        {
            get => (ICommand)GetValue(OkCommandProperty); set => SetValue(OkCommandProperty, value);
        }
        public ICommand CancelCommand
        {
            get => (ICommand)GetValue(CancelCommandProperty); set => SetValue(CancelCommandProperty, value);
        }

        public static readonly RoutedEvent OkClickEvent = EventManager.RegisterRoutedEvent(nameof(OkClick), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(EntitySelector));

        public event RoutedEventHandler OkClick
        {
            add => AddHandler(OkClickEvent, value);
            remove => RemoveHandler(OkClickEvent, value);
        }
        public static readonly RoutedEvent CancelClickEvent = EventManager.RegisterRoutedEvent(nameof(CancelClick), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(EntitySelector));

        public event RoutedEventHandler CancelClick
        {
            add => AddHandler(CancelClickEvent, value);
            remove => RemoveHandler(CancelClickEvent, value);
        }

        void OkOnClick(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(OkClickEvent));
        }

        void CancelOnClick(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(CancelClickEvent));
        }
    }

    public static class ButtonExtention
    {
        public static void DoClick(this Button button)
        {
            var peer = new ButtonAutomationPeer(button);
            var invokeProv = peer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
            invokeProv?.Invoke();
        }
    }
}
