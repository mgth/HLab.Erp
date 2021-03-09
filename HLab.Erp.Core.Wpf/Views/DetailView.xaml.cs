using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using HLab.Base.Wpf;

namespace HLab.Erp.Core.Views
{
    using H = DependencyHelper<DetailView>;
    /// <summary>
    /// Logique d'interaction pour DetailView.xaml
    /// </summary>
    [ContentProperty(nameof(Children))]
    public partial class DetailView : UserControl
    {
        public DetailView()
        {
            InitializeComponent();
            Children = PART_Host.Children;
        }

        public bool EditMode
        {
            get => (bool)GetValue(EditModeProperty);
            set => SetValue(EditModeProperty,value);
        }
        public static readonly DependencyProperty EditModeProperty = H.Property<bool>().Register();

        public string IconPath
        {
            get => (string)GetValue(IconPathProperty);
            set => SetValue(IconPathProperty, value);
        }

        public static readonly DependencyProperty IconPathProperty = 
            H.Property<string>()
                .Register();

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public static readonly DependencyProperty TextProperty = 
            H.Property<string>()
                .Register();

        public UIElementCollection Children
        {
            get => (UIElementCollection)GetValue(ChildrenProperty.DependencyProperty);
            private init => SetValue(ChildrenProperty, value);
        }
        public static readonly DependencyPropertyKey ChildrenProperty = H.Property<UIElementCollection>().RegisterReadOnly();

    }
}
