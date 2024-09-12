﻿using System.Windows;
using System.Windows.Controls;
using HLab.Base.Wpf.Extensions;
using HLab.Erp.Core.ListFilters;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Core.Wpf.ListFilters
{

    /// <summary>
    /// Logique d'interaction pour FilterView.xaml
    /// </summary>
    public partial class FilterView : UserControl, IView<DefaultViewMode, IFilter>
    {
        public FilterView()
        {
            InitializeComponent();

            ToggleButton.Checked += ToggleButton_Checked;
        }

        void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            var content = ContentControl.FindVisualChildren<IFilterContentViewClass>();
            foreach (var control in content)
            {
                control.SetFocus();
            }

            //if(content is UIElement element)
            //    element?.Focus();
        }
    }
}
