using HLab.Erp.Data;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace HLab.Erp.Core.Wpf.EntitySelectors
{
    /// <summary>
    /// Logique d'interaction pour EntitySelectorView.xaml
    /// </summary>
    public partial class EntitySelectorButton : UserControl
    {

        public EntitySelectorButton()
        {
            InitializeComponent();

            this.Loaded += EntitySelectorButton_Loaded;
            
        }

        private void EntitySelectorButton_Loaded(object sender, RoutedEventArgs e)
        {
            var parentWindow = Window.GetWindow(this);
            if (parentWindow != null)
            {
                parentWindow.LocationChanged += ParentWindow_LocationChanged;
                parentWindow.StateChanged += ParentWindow_LocationChanged;
            }
        }

        public static readonly DependencyProperty EntityProperty =
            DependencyProperty.RegisterAttached(
                "Model",
                typeof(Object),
                typeof(EntitySelectorButton),
                new UIPropertyMetadata(null));

        public object Entity
        {
            get => GetValue(EntityProperty); set => SetValue(EntityProperty, value);
        }

        public object ButtonContent
        {
            get => Button.Content; set => Button.Content = value;
        }

        public static readonly DependencyProperty SelectorDataContextProperty =
            DependencyProperty.RegisterAttached(
                nameof(SelectorDataContext),
                typeof(IListEntityViewModel),
                typeof(EntitySelectorButton),
                new FrameworkPropertyMetadata(
                    null,
                    FrameworkPropertyMetadataOptions.None,
                    new PropertyChangedCallback(SelectorDataContextPropertyChanged)
                    //new CoerceValueCallback(CoerceCurrentReading)
                )

        );

        private static void SelectorDataContextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            EntitySelectorButton button = d as EntitySelectorButton;
            if (button != null)
            {
                button.EntitySelector.DataContext = e.NewValue as IListEntityViewModel;
            }
        }

        public IListEntityViewModel SelectorDataContext
        {
            get => (IListEntityViewModel)GetValue(SelectorDataContextProperty); set => SetValue(SelectorDataContextProperty, value);
        }

        public Type ViewModelType { get; set; }

        private IListEntityViewModel ViewModel 
            => EntitySelector.DataContext as IListEntityViewModel;

        private void OnDragStarted(object sender, DragStartedEventArgs e)
        {
            var t = (Thumb)sender;
            t.Cursor = Cursors.SizeNESW;
        }

        private void OnDragDelta(object sender, DragDeltaEventArgs e)
        {
             var h = this.Popup.Height + e.VerticalChange;
            var w = this.Popup.Width - e.HorizontalChange;
            this.Popup.Width = w>100?w:100;
            this.Popup.Height = h>100?h:100;
        }

        private void OnDragCompleted(object sender, DragCompletedEventArgs e)
        {
            var t = (Thumb)sender;
            t.Cursor = null;
        }


        private void ParentWindow_LocationChanged(object sender, EventArgs e)
        {
            if (Popup.IsOpen)
            {
                var mi = typeof(Popup).GetMethod("UpdatePosition", global::System.Reflection.BindingFlags.NonPublic | global::System.Reflection.BindingFlags.Instance);
                mi.Invoke(Popup, null);
            }
        }

        private void Popup_OnClosed(object sender, EventArgs e)
        {
            //var old = EntitySelector.DataContext as IDisposable;
            //EntitySelector.DataContext = null;
            //old?.Dispose();
        }

        private void EntitySelector_OnCancelClick(object sender, RoutedEventArgs e)
        {
            Popup.IsOpen = false;
        }

        private void OnOkClick(object sender, RoutedEventArgs e)
        {
            Entity = ViewModel?.SelectedObjectEntity;
            Popup.IsOpen = false;
        }

        private void Button_OnChecked(object sender, RoutedEventArgs e)
        {
            //if (EntitySelector.DataContext == null)
            //EntitySelector.DataContext 
            //    = Activator.CreateInstance(ViewModelType);
        }

        private void EntitySelectorButton_OnLostFocus(object sender, RoutedEventArgs e)
        {
            //Button.IsChecked = false;
        }

    }
}
