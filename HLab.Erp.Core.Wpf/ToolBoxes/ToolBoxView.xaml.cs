using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using HLab.Erp.Core.DragDrops;
using HLab.Erp.Data;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Application.Documents;

namespace HLab.Erp.Core.ToolBoxes
{
    /// <summary>
    /// Logique d'interaction pour SearchTestView.xaml
    /// </summary>
    public partial class ToolBoxView : UserControl, IAnchorableViewClass
        , IView<DefaultViewMode, IToolListViewModel>
    {
        readonly IMvvmService _mvvm;
        readonly IDragDropService _dragDrop;


        void SetDragDrop()
        {
            var drag = _dragDrop.Get(ListViewTest, true);

            drag.Start += drag_Start;
            drag.Drop += drag_Drop;
        }
        
        public ToolBoxView(IMvvmService mvvm, IDragDropService dragDrop)
        {
            _mvvm = mvvm;
            _dragDrop = dragDrop;
            InitializeComponent();
            // TODO : SetDragDrop();
        }

        void drag_Drop(object source)
        {

        }

        void drag_Start(ErpDragDrop source)
        {
            //            var vm = DataContext as 

            var item = ListViewTest.SelectedItem;

            var i = ListViewTest.ItemContainerGenerator.ContainerFromItem(item) as IInputElement;

            source.DragShift = new Point(0, 0) - source.MouseEventArgs.GetPosition(i);

            // TODO : 
            Task.Run(async () =>
            {
                source.DraggedElement = (FrameworkElement)await _mvvm.ViewHelperFactory.Get(this).Context.GetViewAsync(
                    ListViewTest.SelectedValue as IEntity,
                    typeof(DefaultViewMode),
                    typeof(IViewClassDraggable)
                    );
            });
       }

        public string ContentId => GetType().Name;

    }
}
