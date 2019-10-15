using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using HLab.Erp.Data;

namespace HLab.Erp.Core.Views
{
    /// <summary>
    /// Logique d'interaction pour DefaultEntitySelector.xaml
    /// </summary>
    public partial class DefaultSelectorView
    {
        public DefaultSelectorView()
        {
            DataContextChanged += DefaultEntitySelector_DataContextChanged;
            InitializeComponent();
        }

        private void DefaultEntitySelector_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var lvm = (e.NewValue as IListEntityViewModel);
            if (lvm?.LineViewModelType == null) return;
            AddBindings(lvm.LineViewModelType);
            if (lvm.EntityViewModelType == null) return;
            AddBindings(lvm.EntityViewModelType,"ViewModel.");
        }

        private void AddBindings(Type type, string prefix ="")
        {
            foreach (var p in type.GetProperties())
            {
                foreach (var attr in p.GetCustomAttributes(false).OfType<IsListColumn>())
                {
                    var binding = new Binding(prefix + p.Name);
                    binding.Mode = BindingMode.OneWay;

                    var col = new GridViewColumn
                    {
                        Header = attr.Header ?? p.Name,
                        DisplayMemberBinding = binding
                    };

                    GridView.Columns.Add(col);
                    GridView.SetValue(GridViewSort.PropertyNameProperty, p.Name);
                }
            }

        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            //throw new NotImplementedException();
        }
    }
}
