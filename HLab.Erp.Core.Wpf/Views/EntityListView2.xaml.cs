using System.Windows.Controls;

namespace HLab.Erp.Core.Views
{
    /// <summary>
    /// Logique d'interaction pour EntityListView2.xaml
    /// </summary>
    public partial class EntityListView2 : UserControl, 
        //IView<ViewModeDefault, IListViewModel>,
        IViewClassDocument

    {
        public EntityListView2()
        {
            InitializeComponent();
        }

        public string ContentId => nameof(EntityListView3);
    }
}
