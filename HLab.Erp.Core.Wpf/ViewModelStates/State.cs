using System;
using System.Windows.Media;
using HLab.Mvvm;
using HLab.Notify.Annotations;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Core.ViewModelStates
{
    using H = H<State>;

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
        public State() => H.Initialize(this);

        Func<Color> _getter;
        public State SetColor(Func<Color> getter)
        {
            _getter = getter;
            OnTriggered();
            return this;
        }

        public BrushTheme Theme
        {
            get => _theme.Get();
            set => _theme.Set(value);
        }

        readonly IProperty<BrushTheme> _theme = H.Property<BrushTheme>(c => c
            .Set(e => BrushTheme.Current)
        );

        public ViewModelState CurrentState => _currentState.Get();

        readonly IProperty<ViewModelState> _currentState = H.Property<ViewModelState>(c => c
            .Set(e =>
                e.Disabled ? ViewModelState.Disabled :
                e.Moving ? ViewModelState.Moving :
                e.Selected ? ViewModelState.Selected :
                e.RightHighlighted ? ViewModelState.RightHighlighted :
                e.LeftHighlighted ? ViewModelState.RightHighlighted :
                e.Darken ? ViewModelState.Darken :
                    ViewModelState.Default)
            .On(e => e.Selected)
            .On(e => e.Highlighted)
            .On(e => e.RightHighlighted)
            .On(e => e.LeftHighlighted)
            .On(e => e.Disabled)
            .On(e => e.Moving)
            .On(e => e.Darken)
            .Update()
        );


        public BrushSet DefaultBrushSet { get; } = new BrushSet(ViewModelState.Default);

        public BrushSet SelectedBrushSet { get; } = new BrushSet(ViewModelState.Selected);

        public BrushSet LeftHighlightedBrushSet { get; } = new BrushSet(ViewModelState.LeftHighlighted);

        public BrushSet RightHighlightedBrushSet { get; } = new BrushSet(ViewModelState.RightHighlighted);

        public BrushSet DarkenBrushSet { get; } = new BrushSet(ViewModelState.Darken);

        public BrushSet DisabledBrushSet { get; } = new BrushSet(ViewModelState.Disabled);

        public BrushSet MovingBrushSet { get; } = new BrushSet(ViewModelState.Moving);

        public BrushSet Current => _current.Get();

        readonly IProperty<BrushSet> _current = H.Property<BrushSet>(c => c
           .Set(e =>
           {
               switch (e.CurrentState)
               {
                   case ViewModelState.Default: return e.DefaultBrushSet;
                   case ViewModelState.LeftHighlighted: return e.LeftHighlightedBrushSet;
                   case ViewModelState.RightHighlighted: return e.RightHighlightedBrushSet;
                   case ViewModelState.Selected: return e.SelectedBrushSet;
                   case ViewModelState.Moving: return e.MovingBrushSet;
                   case ViewModelState.Darken: return e.DarkenBrushSet;
                   case ViewModelState.Disabled: return e.DisabledBrushSet;
                   default: return e.DefaultBrushSet;
               }
           })
           .On(e => e.CurrentState).Update()
     );

        public Brush Background => _background.Get();

        readonly IProperty<Brush> _background = H.Property<Brush>(c => c
            .On(e => e.Current.Background)
            .Set(e => e.Current.Background)
            .Update()
        );

        public Brush Front => _front.Get();

        readonly IProperty<Brush> _front = H.Property<Brush>(c => c
            .Set(e => e.Current.Front)
            .On(e => e.Current.Front)
            .Update()
        );

        public Brush Border => _border.Get();

        readonly IProperty<Brush> _border = H.Property<Brush>(c => c
            .Set(e => e.Current.Border)
            .On(e => e.Current.Border)
            .Update()
        );

        public Brush Text => _text.Get();

        readonly IProperty<Brush> _text = H.Property<Brush>(c => c
            .Set(e => e.Current.Text)
            .On(e => e.Current.Text)
            .Update()
        );

        public Brush TextBackground => _textBackground.Get();

        readonly IProperty<Brush> _textBackground = H.Property<Brush>(c => c
            .Set(e => e.Current.TextBackground)
            .On(e => e.Current.TextBackground)
            .Update()
        );


        public Color Color
        {
            get => _color.Get();
            set => _color.Set(value);
        }

        readonly IProperty<Color> _color = H.Property<Color>();
        public bool Selected { get => _selected.Get(); set => _selected.Set(value); }
        readonly IProperty<bool> _selected = H.Property<bool>();
        public bool LeftHighlighted { get => _leftHighlighted.Get(); set => _leftHighlighted.Set(value); }
        readonly IProperty<bool> _leftHighlighted = H.Property<bool>();
        public bool RightHighlighted { get => _rightHighlighted.Get(); set => _rightHighlighted.Set(value); }
        readonly IProperty<bool> _rightHighlighted = H.Property<bool>();

        public bool Highlighted => _highlighted.Get();

        readonly IProperty<bool> _highlighted = H.Property<bool>(c => c
            .Set(e => e.LeftHighlighted || e.RightHighlighted)
            .On(e => e.LeftHighlighted)
            .On(e => e.RightHighlighted)
            .Update()
        );
        public bool Darken { get => _darken.Get(); set => _darken.Set(value); }
        readonly IProperty<bool> _darken = H.Property<bool>();

        public bool Disabled { get => _disabled.Get(); set => _disabled.Set(value); }
        readonly IProperty<bool> _disabled = H.Property<bool>();

        public bool Moving { get => _moving.Get(); set => _moving.Set(value); }
        readonly IProperty<bool> _moving = H.Property<bool>();


        public void OnTriggered()
        {
            Color = _getter();
        }
    }
}
