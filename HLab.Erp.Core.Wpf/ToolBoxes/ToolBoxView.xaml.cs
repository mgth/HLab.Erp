using System.Windows;
using System.Windows.Controls;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Core.DragDrops;
using HLab.Erp.Data;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Core.ToolBoxes
{
    /// <summary>
    /// Logique d'interaction pour SearchTestView.xaml
    /// </summary>
    public partial class ToolBoxView : UserControl, IViewClassAnchorable
        , IView<ViewModeDefault, IToolListViewModel>
    {
        [Import] private readonly IMvvmService _mvvm;
        [Import] private readonly IDragDropService _dragDrop;


        private void SetDragDrop()
        {
            var drag = _dragDrop.Get(ListViewTest, true);

            drag.Start += drag_Start;
            drag.Drop += drag_Drop;
        }
        
        public ToolBoxView()
        {
            InitializeComponent();
            SetDragDrop();
        }
        private void drag_Drop(object source)
        {

        }

        private void drag_Start(ErpDragDrop source)
        {
            //            var vm = DataContext as 

            var item = ListViewTest.SelectedItem;

            var i = ListViewTest.ItemContainerGenerator.ContainerFromItem(item) as IInputElement;

            source.DragShift = new Point(0, 0) - source.MouseEventArgs.GetPosition(i);


            source.DraggedElement = (FrameworkElement)_mvvm.ViewHelperFactory.Get(this).Context.GetView(
                ListViewTest.SelectedValue as IEntity,
                typeof(ViewModeDefault),
                typeof(IViewClassDraggable)
                );
        }

        public string ContentId => GetType().Name;

    }
}
