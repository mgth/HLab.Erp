using System;
using System.Reactive.Linq;
using System.Windows.Media;
using HLab.ColorTools.Wpf;
using HLab.Erp.Core.Wpf.ViewModelStates;
using HLab.Erp.Data;
using HLab.Mvvm.ReactiveUI;
using ReactiveUI;

namespace HLab.Erp.Core.Wpf.DragDrops
{
    public class DragDropItemViewModel<T> : ViewModel<T> where T : class, IEntity, IEntityWithColor, IEntityWithIcon
    {

        public DragDropItemViewModel() => this.WhenAnyValue(e => e.Model.Color)
                .Do(c => State.OnTriggered())
                .Subscribe();

        public State State
        {
            get => _state;
            set => _state = (value.SetColor(() => Model?.Color.ToWpfColor() ?? Colors.Transparent));
        }
        State _state;

    }
}
