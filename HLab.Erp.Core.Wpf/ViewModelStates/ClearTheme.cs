using System;
using System.Windows;
using System.Windows.Media;
using HLab.ColorTools;
using HLab.ColorTools.Wpf;
using HLab.Erp.Core.ViewModelStates;

namespace HLab.Erp.Core.Wpf.ViewModelStates;

internal class ClearTheme : BrushTheme
{
    public override Brush GetBrush(Color color, ViewModelState state, BrushSetUsage usage)
    {
        var c = color.ToColor<double>().ToHSL();

        c = state switch
        {
            ViewModelState.Default => c.Highlight(0.8).Desaturate(0.5),
            ViewModelState.Moving => c.Highlight(0.8).Desaturate(0.5),
            ViewModelState.LeftHighlighted => c.Darken(0.3).Saturate(0.6),
            ViewModelState.RightHighlighted => c.Darken(0.3).Saturate(0.6),
            ViewModelState.Selected => c.Highlight(0.2),
            ViewModelState.Disabled => c.Desaturate(0.2),
            _ => throw new ArgumentOutOfRangeException(nameof(state), state, null)
        };

        switch (usage)
        {
            case BrushSetUsage.Front:
                break;
            case BrushSetUsage.Background:
                c = c.Highlight(0.5).Desaturate(0.3)/*.Transparent(0.5)*/;
                if (state == ViewModelState.Moving) c = c.LessOpacity(0.5);
                break;
            case BrushSetUsage.Border:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(usage), usage, null);
        }

        var c2 = c.Darken(0.6);

        return new LinearGradientBrush(c.ToWpfColor(), c2.ToWpfColor(), new Point(0, 0), new Point(0, 1));
        //return new SolidColorBrush(c);
    }
}