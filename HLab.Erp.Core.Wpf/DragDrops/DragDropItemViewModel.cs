using System.Windows.Media;
using HLab.ColorTools.Wpf;
using HLab.Erp.Core.ViewModelStates;
using HLab.Erp.Data;
using HLab.Mvvm;
using HLab.Notify.PropertyChanged;


namespace HLab.Erp.Core.DragDrops
{
    public class DragDropItemViewModel<T> : ViewModel<T> where T : IEntity, IEntityWithColor, IEntityWithIcon
    {

        public DragDropItemViewModel()
        {
            H<DragDropItemViewModel<T>>.Initialize(this);
        }

        public State State
        {
            get => _state.Get();
            set => _state.Set(value.SetColor(() => Model?.Color.ToColor() ?? Colors.Transparent));
        }

        readonly IProperty<State> _state = H<DragDropItemViewModel<T>>.Property<State>(c => c
            .On(e => e.Model.Color)
            //.Do((a,p) => p.Get().Color = a.Model.Color.ToColor())
            .Do(e=>e.State.OnTriggered())
        );

    }
}
