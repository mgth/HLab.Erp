using System.Windows.Media;
using HLab.Mvvm;
using HLab.Notify;
using HLab.Notify.Annotations;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Core.Wpf.ViewModelStates
{
    public class BrushSet : ViewModel<BrushSet>
    {
        public BrushSet Configure(State state, ViewModelState vmState)
        {
            State = state;
            VmState = vmState;                
            return this;
        }
        public State State
        {
            get => _state.Get();
            set => _state.Set(value);
        }
        private IProperty<State> _state = H.Property<State>();

        public ViewModelState VmState
        {
            get => _vmState.Get();
            set => _vmState.Set(value);
        }
        private IProperty<ViewModelState> _vmState = H.Property<ViewModelState>();

        public Brush Background => _background.Get();
        private IProperty<Brush> _background = H.Property<Brush>(c => c
            .On(e => e.State.Theme)
            .On(e => e.State.Color)
            .On(e => e.VmState)
            .Set(e => e.State.Theme.GetBrush(e.State.Color,e.VmState,BrushSetUsage.Background))
        );

        public Brush Front => _front.Get();
        private IProperty<Brush> _front = H.Property<Brush>(c => c
            .On(e => e.State.Theme)
            .On(e => e.State.Color)
            .On(e => e.VmState)
            .Set(e => e.State.Theme.GetBrush(e.State.Color,e.VmState,BrushSetUsage.Front))        
        );

        public Brush Border => _border.Get();
        private IProperty<Brush> _border = H.Property<Brush>(c => c
            .On(e => e.State.Theme)
            .On(e => e.State.Color)
            .On(e => e.VmState)
            .Set(e => e.State.Theme.GetBrush(e.State.Color, e.VmState, BrushSetUsage.Border))
        );

        public Brush Text => _text.Get();
        private IProperty<Brush> _text = H.Property<Brush>(c => c
            .On(e => e.State.Theme)
            .On(e => e.State.Color)
            .On(e => e.VmState)
            .Set(e => e.State.Theme.GetBrush(e.State.Color, e.VmState, BrushSetUsage.Text))
        );

        public Brush TextBackGround => _textBackground.Get();
        private IProperty<Brush> _textBackground = H.Property<Brush>(c => c
            .On(e => e.State.Theme)
            .On(e => e.State.Color)
            .On(e => e.VmState)
            .Set(e => e.State.Theme.GetBrush(e.State.Color, e.VmState, BrushSetUsage.TextBackground))
        );
    }
}
