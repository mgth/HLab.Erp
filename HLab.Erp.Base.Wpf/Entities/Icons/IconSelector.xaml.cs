using System.Windows;
using System.Windows.Controls;
using HLab.Base.Wpf;
using HLab.Erp.Base.Data;

namespace HLab.Erp.Base.Wpf.Entities.Icons
{
    using H = DependencyHelper<IconSelector>;
    /// <summary>
    /// Logique d'interaction pour IconSelector.xaml
    /// </summary>
    public partial class IconSelector : UserControl
    {
        public IconSelector()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty IconPathProperty = H.Property<string>()
            .OnChange( (s,a) => s.OnIconPathChanged(a) )
            .Register();

        void OnIconPathChanged(DependencyPropertyChangedEventArgs<string> dependencyPropertyChangedEventArgs)
        {
//            throw new NotImplementedException();
        }

        public static readonly DependencyProperty IconProperty = H.Property<Icon>()
            .OnChange( (s,a) => s.OnIconChanged(a) )
            .Register();

        void OnIconChanged(DependencyPropertyChangedEventArgs<Icon> a)
        {
            IconPath = a.NewValue.Path;
        }


        public string IconPath
        {
            get => (string)GetValue(IconPathProperty);
            set => SetValue(IconPathProperty, value);
        }
        public Icon Icon
        {
            get => (Icon)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }
    }
}
