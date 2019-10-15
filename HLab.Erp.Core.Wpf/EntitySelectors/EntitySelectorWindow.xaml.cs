using System;
using System.Windows;

namespace HLab.Erp.Core.EntitySelectors
{
    /// <summary>
    /// Logique d'interaction pour EntitySelectorView.xaml
    /// </summary>
    public partial class EntitySelectorWindow
    {
        public EntitySelectorWindow()
        {
            InitializeComponent();
        }


        public static readonly DependencyProperty ItemProperty =
            DependencyProperty.RegisterAttached(
                "Model",
                typeof(Object),
                typeof(EntitySelectorWindow),
                new UIPropertyMetadata(null));

        public object Item
        {
            get => GetValue(ItemProperty); set => SetValue(ItemProperty, value);
        }


        private void EntitySelector_OnOkClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void EntitySelector_OnCancelClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
