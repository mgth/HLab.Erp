using System;
using System.Windows.Controls;

namespace HLab.Erp.Base.Wpf.Entities
{
    /// <summary>
    /// Logique d'interaction pour DetailView.xaml
    /// </summary>
    public partial class DetailView : UserControl
    {
        public DetailView()
        {
            InitializeComponent();
        }
        private void Populate(Type modelType)
        {
            foreach (var property in modelType.GetProperties())
            {
                if (property.PropertyType == typeof(string))
                {

                }
            }
        }
    }

}
