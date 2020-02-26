using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using HLab.Base;

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
                //.OnChange((s, e) => s.IconView.Path = e.NewValue)
                .Register();

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }
        public static readonly DependencyProperty TextProperty = 
            H.Property<string>()
                //.OnChange((s, e) => s.Localize.Id = e.NewValue)
                .Register();

        public UIElementCollection Children
        {
            get => (UIElementCollection)GetValue(ChildrenProperty.DependencyProperty);
            private set => SetValue(ChildrenProperty, value);
        }
        public static readonly DependencyPropertyKey ChildrenProperty = H.Property<UIElementCollection>().RegisterReadOnly();

    }
}
