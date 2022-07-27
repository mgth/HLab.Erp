using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using HLab.Base.Wpf;
using HLab.Notify.PropertyChanged;
using HLab.Notify.PropertyChanged.PropertyHelpers;

namespace HLab.Erp.Acl
{
    /// <summary>
    /// Logique d'interaction pour TextBoxCFR.xaml
    /// </summary>
    public partial class CfrContainer : UserControl
    {
        public CfrContainer()
        {
            InitializeComponent();
        }

        class H : DependencyHelper<CfrContainer> { }

        public static DependencyProperty PropertyProperty = H.Property<IProperty>().OnChange((o, a) =>
        {
            if (a.OldValue is NotifierBase old)
            {
                old.PropertyChanged -= o.N_PropertyChanged;
            }
            if (a.NewValue is NotifierBase n)
            {
                n.PropertyChanged += o.N_PropertyChanged;
            }
        }).Register();

        public IProperty Property
        {
            get => (IProperty) GetValue(PropertyProperty);
            set => SetValue(PropertyProperty, value);
        }

        void N_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is IPropertyHolderN<string> p)
            {
                switch (e.PropertyName)
                {
                    case "Value":
                        //TextBox.Text = p.Value;
                        break;

                    case "Enabled":
                        //TextBox.IsEnabled = p.Enabled;
                        break;

                }

            }
        }
    }
}
