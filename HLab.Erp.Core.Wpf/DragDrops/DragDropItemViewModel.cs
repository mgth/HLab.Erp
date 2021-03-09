using System.Windows.Media;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Core.ViewModelStates;
using HLab.Erp.Data;
using HLab.Icons.Annotations.Icons;
using HLab.Mvvm;
using HLab.Mvvm._Colors;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Core.DragDrops
{
    public class DragDropItemViewModel<T> :  ViewModel<T> where T : IEntity, IEntityWithColor, IEntityWithIcon
    {
        [Import]
        private IIconService _icons;

        public DragDropItemViewModel() => H<DragDropItemViewModel<T>>.Initialize(this);

        [Import]
        public State State
        {
            get => _state.Get();
            set => _state.Set(value.SetColor(() => Model?.Color.ToColor()??Colors.Transparent));
        }
        private readonly IProperty<State> _state = H<DragDropItemViewModel<T>>.Property<State>(c => c
            .On(e => e.Model.Color)
            .Do((a,p) => p.Get().Color = a.Model.Color.ToColor())
            //.Set((a,p) => p.Color = a.Model.Color.ToColor())
        );


        public object Icon => _icon.Get();
        private readonly IProperty<object> _icon = H<DragDropItemViewModel<T>>.Property<object>(c => c
            .On(e => e.Model.IconPath)
            .On(e => e.IconPath)
            .Set(async e => await e._icons.GetIconAsync(e.Model?.IconPath ?? e.IconPath))
        );

        public virtual string IconPath => "IconMicroscope";

    }
}
