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
using HLab.Mvvm.Icons;

namespace HLab.Erp.Base.Wpf.Entities
{
        using H = DependencyHelper<DetailSeparator>;
    /// <summary>
    /// Logique d'interaction pour DetailSeparator.xaml
    /// </summary>
    public partial class DetailSeparator : UserControl
    {
        public DetailSeparator()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty IconPathProperty = 
            H.Property<string>()
                .OnChange((s, e) => s.IconView.Path = e.NewValue)
                .Register();
        public string IconPath
        {
            get => (string)GetValue(IconPathProperty);
            set => SetValue(IconPathProperty, value);
        }

        public static readonly DependencyProperty TextProperty = 
            H.Property<string>()
                .OnChange((s, e) => s.Localize.Id = e.NewValue)
                .Register();
        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }
    }
}
