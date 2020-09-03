using System;
using System.Windows.Media;
using HLab.DependencyInjection.Annotations;
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

        private Func<Color> _getter;
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
        private IProperty<BrushTheme> _theme = H.Property<BrushTheme>(c => c
            //TODO : .On(e => BrushTheme.Current)
            .Set(e => BrushTheme.Current)
        );

        [TriggerOn(nameof(Selected))]
        [TriggerOn(nameof(Highlighted))]
        [TriggerOn(nameof(RightHighlighted))]
        [TriggerOn(nameof(LeftHighlighted))]
        [TriggerOn(nameof(Disabled))]
        [TriggerOn(nameof(Moving))]
        [TriggerOn(nameof(Darken))]
        public ViewModelState CurrentState => _curentState.Get();
        private IProperty<ViewModelState> _curentState = H.Property<ViewModelState>(c => c
            .On(e => e.Selected)
            .On(e => e.Highlighted)
            .On(e => e.RightHighlighted)
            .On(e => e.LeftHighlighted)
            .On(e => e.Disabled)
            .On(e => e.Moving)
            .On(e => e.Darken)
            .Set(e => 
                e.Disabled?ViewModelState.Disabled : 
                e.Moving ? ViewModelState.Moving : 
                e.Selected ? ViewModelState.Selected : 
                e.RightHighlighted ? ViewModelState.RightHighlighted : 
                e.LeftHighlighted ? ViewModelState.RightHighlighted : 
                e.Darken ? ViewModelState.Darken : 
                    ViewModelState.Default)
        );


        [Import]
        public BrushSet DefaultBrushSet
        {
            get => _defaultBrushSet.Get();
            set => _defaultBrushSet.Set(value.Configure(this, ViewModelState.Default));
        }
        private IProperty<BrushSet> _defaultBrushSet = H.Property<BrushSet>();

        [Import]
        public BrushSet SelectedBrushSet
        {
            get => _selectedBrushSet.Get();
            set => _selectedBrushSet.Set(value.Configure(this, ViewModelState.Selected));
        }
        private IProperty<BrushSet> _selectedBrushSet = H.Property<BrushSet>();

        [Import]
       public BrushSet LeftHighlightedBrushSet 
        {
            get => _leftHighlightedBrushSet.Get();
            set => _leftHighlightedBrushSet.Set(value.Configure(this, ViewModelState.LeftHighlighted));
        }
        private IProperty<BrushSet> _leftHighlightedBrushSet = H.Property<BrushSet>();

        [Import]
        public BrushSet RightHighlightedBrushSet 
        {
            get => _rightHighlightedBrushSet.Get();
            set => _rightHighlightedBrushSet.Set(value.Configure(this, ViewModelState.RightHighlighted));
        }
        private IProperty<BrushSet> _rightHighlightedBrushSet = H.Property<BrushSet>();

        [Import]
        public BrushSet DarkenBrushSet 
        {
            get => _darkenBrushSet.Get();
            set => _darkenBrushSet.Set(value.Configure(this, ViewModelState.Darken));
        }
        private IProperty<BrushSet> _darkenBrushSet = H.Property<BrushSet>();

        [Import]
        public BrushSet DisabledBrushSet 
        {
            get => _disabledBrushSet.Get();
            set => _disabledBrushSet.Set(value.Configure(this, ViewModelState.Disabled));
        }
        private IProperty<BrushSet> _disabledBrushSet = H.Property<BrushSet>();

        [Import]
        public BrushSet MovingBrushSet 
        {
            get => _movingBrushSet.Get();
            set => _movingBrushSet.Set(value.Configure(this, ViewModelState.Moving));
        }
        private IProperty<BrushSet> _movingBrushSet = H.Property<BrushSet>();

        public BrushSet Current => _current.Get();
        private IProperty<BrushSet> _current = H.Property<BrushSet>( c => c
            .On(e => e.CurrentState)
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
            }
        ));

        public Brush Background => _background.Get();
        private IProperty<Brush> _background = H.Property<Brush>(c=>c
            .On(e => e.Current.Background)
            .Set(e => e.Current.Background)
        );

        public Brush Front => _front.Get();
        private IProperty<Brush> _front = H.Property<Brush>(c => c
            .On(e => e.Current.Front)
            .Set(e => e.Current.Front)
        );

        public Brush Border => _border.Get();
        private IProperty<Brush> _border = H.Property<Brush>(c => c
            .On(e => e.Current.Border)
            .Set(e => e.Current.Border)
        );

        public Brush Text => _text.Get();
        private IProperty<Brush> _text = H.Property<Brush>(c => c
            .On(e => e.Current.Text)
            .Set(e => e.Current.Text)
        );

        public Brush TextBackground => _textBackground.Get();
        private IProperty<Brush> _textBackground = H.Property<Brush>(c => c
            .On(e => e.Current.TextBackGround)
            .Set(e => e.Current.TextBackGround)
        );


        public Color Color {
            get => _color.Get();
            set => _color.Set(value);
        }
        private IProperty<Color> _color = H.Property<Color>();
        public bool Selected { get => _selected.Get(); set => _selected.Set(value); }
        private IProperty<bool> _selected = H.Property<bool>();
        public bool LeftHighlighted { get => _leftHighlighted.Get(); set => _leftHighlighted.Set(value); }
        private IProperty<bool> _leftHighlighted = H.Property<bool>();
        public bool RightHighlighted { get => _rightHighlighted.Get(); set => _rightHighlighted.Set(value); }
        private IProperty<bool> _rightHighlighted = H.Property<bool>();

        public bool Highlighted => _highlighted.Get();
        private IProperty<bool> _highlighted = H.Property<bool>(c => c
            .On(e => e.LeftHighlighted)
            .On(e => e.RightHighlighted)
            .Set(e => e.LeftHighlighted || e.RightHighlighted)
        );
        public bool Darken { get => _darken.Get(); set => _darken.Set(value); }
        private IProperty<bool> _darken = H.Property<bool>();

        public bool Disabled { get => _disabled.Get(); set => _disabled.Set(value); }
        private IProperty<bool> _disabled = H.Property<bool>();

        public bool Moving { get => _moving.Get(); set => _moving.Set(value); }
        private IProperty<bool> _moving = H.Property<bool>();


        public void OnTriggered()
        {
            Color = _getter();
        }
    }
}
