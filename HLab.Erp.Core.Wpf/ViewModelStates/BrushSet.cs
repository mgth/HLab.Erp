using System;
using System.Windows.Media;
using HLab.Mvvm;
using HLab.Notify.Annotations;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Core.ViewModelStates
{
    using H = H<BrushSet>;

    public class BrushSet : ViewModel, IChildObject
    {
        public BrushSet(ViewModelState vmState)
        {
            VmState = vmState;
            H.Initialize(this);
        }

        public State State
        {
            get => _state.Get();
            set => _state.Set(value);
        }
        private readonly IProperty<State> _state = H.Property<State>();

        public ViewModelState VmState { get; }

        public Brush Background => _background.Get();
        private readonly IProperty<Brush> _background = H.Property<Brush>(c => c
            .NotNull(e => e.State?.Theme)
            .Set(e => e.State.Theme.GetBrush(e.State.Color, e.VmState, BrushSetUsage.Background))
            .On(e => e.State.Theme)
            .On(e => e.State.Color)
            .On(e => e.VmState)
            .Update()
        );

        public Brush Front => _front.Get();
        private readonly IProperty<Brush> _front = H.Property<Brush>(c => c
            .NotNull(e => e.State?.Theme)
            .Set(e => e.State.Theme.GetBrush(e.State.Color, e.VmState, BrushSetUsage.Front))
            .On(e => e.State.Theme)
            .On(e => e.State.Color)
            .On(e => e.VmState)
            .Update()
        );

        public Brush Border => _border.Get();
        private readonly IProperty<Brush> _border = H.Property<Brush>(c => c
            .NotNull(e => e.State?.Theme)
            .Set(e => e.State.Theme.GetBrush(e.State.Color, e.VmState, BrushSetUsage.Border))
            .On(e => e.State.Theme)
            .On(e => e.State.Color)
            .On(e => e.VmState)
            .Update()
        );

        public Brush Text => _text.Get();
        private readonly IProperty<Brush> _text = H.Property<Brush>(c => c
            .NotNull(e => e.State?.Theme)
            .Set(e => e.State.Theme.GetBrush(e.State.Color, e.VmState, BrushSetUsage.Text))
            .On(e => e.State.Theme)
            .On(e => e.State.Color)
            .On(e => e.VmState)
            .Update()
        );

        public Brush TextBackground => _textBackground.Get();
        private readonly IProperty<Brush> _textBackground = H.Property<Brush>(c => c
            .NotNull(e => e.State?.Theme)
            .Set(e => e.State.Theme.GetBrush(e.State.Color, e.VmState, BrushSetUsage.TextBackground))
            .On(e => e.State.Theme)
            .On(e => e.State.Color)
            .On(e => e.VmState)
            .Update()
        );

        public INotifyPropertyChangedWithHelper Parent
        {
            get => State;
            set
            {
                if(value is State state)
                    State = state;
            }
        }

        public void OnDispose(Action action)
        {
        }
    }
}
