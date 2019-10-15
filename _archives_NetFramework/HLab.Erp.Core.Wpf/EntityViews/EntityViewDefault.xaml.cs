using System.Windows.Controls;
using HLab.Erp.Data;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Core.Wpf.EntityViews
{
    /// <summary>
    /// Logique d'interaction pour EntityViewDefault.xaml
    /// </summary>
    public partial class EntityViewDefault : UserControl, IView<ViewModeDefault,IEntity>
    {
        public EntityViewDefault()
        {
            InitializeComponent();
        }
    }
}
