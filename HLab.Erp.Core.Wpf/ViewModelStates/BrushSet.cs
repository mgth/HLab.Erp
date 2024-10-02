using System;
using System.Windows.Media;
using HLab.Base.ReactiveUI;
using HLab.Erp.Core.ViewModelStates;
using HLab.Mvvm.ReactiveUI;
using ReactiveUI;

namespace HLab.Erp.Core.Wpf.ViewModelStates;

public class BrushSet : ViewModel //, IChildObject
{
    public BrushSet(ViewModelState vmState)
    {
        VmState = vmState;

        _background = this.WhenAnyValue(
            e => e.State.Theme,
            e => e.State.Color,
            e => e.VmState,
            selector: (theme, color, state) => theme.GetBrush(color, state, BrushSetUsage.Background)
            )
            .ToProperty(this, e => e.Background);

        _front = this.WhenAnyValue(
            e => e.State.Theme,
            e => e.State.Color,
            e => e.VmState,
            selector: (theme, color, state) => theme.GetBrush(color, state, BrushSetUsage.Front)
            )
            .ToProperty(this, e => e.Front);

        _border = this.WhenAnyValue(
            e => e.State.Theme,
            e => e.State.Color,
            e => e.VmState,
            selector: (theme, color, state) => theme.GetBrush(color, state, BrushSetUsage.Border)
            )
            .ToProperty(this, e => e.Border);

        _text = this.WhenAnyValue(
            e => e.State.Theme,
            e => e.State.Color,
            e => e.VmState,
            selector: (theme, color, state) => theme.GetBrush(color, state, BrushSetUsage.Text)
            )
            .ToProperty(this, e => e.Text);

        _textBackground = this.WhenAnyValue(
            e => e.State.Theme,
            e => e.State.Color,
            e => e.VmState,
            selector: (theme, color, state) => theme.GetBrush(color, state, BrushSetUsage.TextBackground)
            )
            .ToProperty(this, e => e.TextBackground);


    }

    public State State
    {
        get => _state;
        set => this.SetAndRaise(ref _state,value);
    }
    State _state;

    public ViewModelState VmState { get; }

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

    //public INotifyPropertyChangedWithHelper Parent
    //{
    //    get => State;
    //    set
    //    {
    //        if(value is State state)
    //            State = state;
    //    }
    //}

    //public void OnDispose(Action action)
    //{
    //}
}