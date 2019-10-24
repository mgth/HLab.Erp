using System.Threading.Tasks;
using System.Windows.Media;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Core.ViewModelStates;
using HLab.Erp.Data;
using HLab.Mvvm;
using HLab.Mvvm._Colors;
using HLab.Mvvm.Annotations;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Core.DragDrops
{
    public class DragDropItemViewModel<TClass,T> :  ViewModel<TClass,T> where T : IEntity, IEntityWithColor, IEntityWithIcon
        where TClass : DragDropItemViewModel<TClass,T>
    {
        [Import]
        private IIconService _icons;

        [Import]
        public State State
        {
            get => _state.Get();
            set => _state.Set(value.SetColor(() => Model?.Color.ToColor()??Colors.Transparent));
        }
        private readonly IProperty<State> _state = H.Property<State>(c => c
            .On(e => e.Model.Color)
            .Do((a,p) => p.Get().Color = a.Model.Color.ToColor())
            //.Set((a,p) => p.Color = a.Model.Color.ToColor())
        );


        public object Icon => _icon.Get();
        private readonly IProperty<object> _icon = H.Property<object>(c => c
            .On(e => e.Model.IconPath)
            .On(e => e.IconPath)
            .Set(async e => await e._icons.GetIcon(e.Model?.IconPath ?? e.IconPath))
        );

        public virtual string IconPath => "IconMicroscope";

    }
}
