using System.Windows.Media;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Core.Wpf.ViewModelStates;
using HLab.Erp.Data;
using HLab.Mvvm;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Wpf.Icons;
using HLab.Mvvm.Wpf._Colors;
using HLab.Notify;
using HLab.Notify.Annotations;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Core.Wpf.DragDrops
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
        private IProperty<State> _state = H.Property<State>(c => c
            .On(e => e.Model.Color)
            .Update()
        );


        public object Icon => 
            _icons.GetIcon(Model?.IconName ?? IconName);
        private IProperty<object> _icon = H.Property<object>(c => c
            .On(e => e.Model.IconName)
            .On(e => e.IconName)
            .Set(e => e.Model?.IconName ?? e.IconName)
        );

        public virtual string IconName => "IconMicroscope";

    }
}
