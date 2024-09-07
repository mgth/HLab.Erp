using System;
using System.Reactive.Linq;
using System.Windows.Media;
using HLab.Core.Annotations;
using HLab.Erp.Core.ViewModelStates;
using HLab.Mvvm.ReactiveUI;
using ReactiveUI;

namespace HLab.Erp.Core.Wpf.ViewModelStates
{


    public enum ViewModelState
    {
        Default,
        RightHighlighted,
        LeftHighlighted,
        Selected,
        Moving,
        Darken,
        Disabled
    }

    public enum BrushSetUsage
    {
        Front,
        Background,
        Border,
        Text,
        TextBackground
    }

    public class State : ViewModel, ITriggerable
    {
        public State()
        {
            _currentState = this
                .WhenAnyValue(
                e => e.Disabled, 
                e => e.Moving,                 
                e => e.Selected, 
                e => e.RightHighlighted, 
                e => e.LeftHighlighted, 
                e => e.Darken,
                selector: GetCurrentState)
                .ToProperty(this, e => e.CurrentState, scheduler: RxApp.MainThreadScheduler);

            _current = this.WhenAnyValue(e => e.CurrentState)
                .Select(GetBrushSet)
                .ToProperty(this, e => e.Current, scheduler: RxApp.MainThreadScheduler);

            _background = this.WhenAnyValue(e => e.Current.Background)
                .ToProperty(this, e => e.Background, scheduler: RxApp.MainThreadScheduler);

            _front = this.WhenAnyValue(e => e.Current.Front)
                .ToProperty(this, e => e.Front, scheduler: RxApp.MainThreadScheduler);

            _border = this.WhenAnyValue(e => e.Current.Border)
                .ToProperty(this, e => e.Border, scheduler: RxApp.MainThreadScheduler);

            _text = this.WhenAnyValue(e => e.Current.Text)
                .ToProperty(this, e => e.Text, scheduler: RxApp.MainThreadScheduler);

            _textBackground = this.WhenAnyValue(e => e.Current.TextBackground)
                .ToProperty(this, e => e.TextBackground, scheduler: RxApp.MainThreadScheduler);

            _highlighted = this.WhenAnyValue(
                e => e.LeftHighlighted, 
                e => e.RightHighlighted,
                (l, r) => l || r
                )
                .ToProperty(this, e => e.Highlighted, scheduler: RxApp.MainThreadScheduler);
        }

        Func<Color> _getter;
        public State SetColor(Func<Color> getter)
        {
            _getter = getter;
            OnTriggered();
            return this;
        }

        public BrushTheme Theme
        {
            get => _theme;
            set => SetAndRaise(ref _theme, value);
        }
        BrushTheme _theme = BrushTheme.Current;

        public ViewModelState CurrentState => _currentState.Value;
        readonly ObservableAsPropertyHelper<ViewModelState> _currentState;

        static ViewModelState GetCurrentState(bool disabled, bool moving, bool selected, bool rightHighlighted, bool leftHighlighted, bool darken)
        {
            if (disabled) return ViewModelState.Disabled;
            if (moving) return ViewModelState.Moving;
            if (selected) return ViewModelState.Selected;
            if (rightHighlighted) return ViewModelState.RightHighlighted;
            if (leftHighlighted) return ViewModelState.LeftHighlighted;
            if (darken) return ViewModelState.Darken;
            return ViewModelState.Default;
        }

        public BrushSet DefaultBrushSet { get; } = new BrushSet(ViewModelState.Default);

        public BrushSet SelectedBrushSet { get; } = new BrushSet(ViewModelState.Selected);

        public BrushSet LeftHighlightedBrushSet { get; } = new BrushSet(ViewModelState.LeftHighlighted);

        public BrushSet RightHighlightedBrushSet { get; } = new BrushSet(ViewModelState.RightHighlighted);

        public BrushSet DarkenBrushSet { get; } = new BrushSet(ViewModelState.Darken);

        public BrushSet DisabledBrushSet { get; } = new BrushSet(ViewModelState.Disabled);

        public BrushSet MovingBrushSet { get; } = new BrushSet(ViewModelState.Moving);

        public BrushSet Current => _current.Value;
        ObservableAsPropertyHelper<BrushSet> _current;

        BrushSet GetBrushSet(ViewModelState state)
        {
            switch (state)
            {
                case ViewModelState.Default: return DefaultBrushSet;
                case ViewModelState.LeftHighlighted: return LeftHighlightedBrushSet;
                case ViewModelState.RightHighlighted: return RightHighlightedBrushSet;
                case ViewModelState.Selected: return SelectedBrushSet;
                case ViewModelState.Moving: return MovingBrushSet;
                case ViewModelState.Darken: return DarkenBrushSet;
                case ViewModelState.Disabled: return DisabledBrushSet;
                default: return DefaultBrushSet;
            }
        }



        public Brush Background => _background.Value;
        readonly ObservableAsPropertyHelper<Brush> _background;

        public Brush Front => _front.Value;
        readonly ObservableAsPropertyHelper<Brush> _front;


        public Brush Border => _border.Value;
        readonly ObservableAsPropertyHelper<Brush> _border;

        public Brush Text => _text.Value;
        readonly ObservableAsPropertyHelper<Brush> _text;

        public Brush TextBackground => _textBackground.Value;
        readonly ObservableAsPropertyHelper<Brush> _textBackground;


        public Color Color
        {
            get => _color;
            set => SetAndRaise(ref _color, value);
        }

        Color _color;
        public bool Selected { get => _selected; set => SetAndRaise(ref _selected, value); }
        bool _selected;
        public bool LeftHighlighted { get => _leftHighlighted; set => SetAndRaise(ref _leftHighlighted, value); }
        bool _leftHighlighted;
        public bool RightHighlighted { get => _rightHighlighted; set => SetAndRaise(ref _rightHighlighted, value); }
        bool _rightHighlighted;

        public bool Highlighted => _highlighted.Value;
        readonly ObservableAsPropertyHelper<bool> _highlighted;

        public bool Darken { get => _darken; set => SetAndRaise(ref _darken, value); }
        bool _darken;

        public bool Disabled { get => _disabled; set => SetAndRaise(ref _disabled, value); }
        bool _disabled;

        public bool Moving { get => _moving; set => SetAndRaise(ref _moving, value); }
        bool _moving;


        public void OnTriggered()
        {
            Color = _getter();
        }
    }
}
