using System;
using System.Windows;
using System.Windows.Media;
using HLab.ColorTools.Wpf;

namespace HLab.Erp.Core.ViewModelStates
{
    public class BlackTheme : BrushTheme
    {
        public override Brush GetBrush(Color color, ViewModelState state, BrushSetUsage usage)
        {
            if (usage == BrushSetUsage.Text || usage == BrushSetUsage.TextBackground)
            {
                return GetBrushText(color, state, usage);
            }

            HSL c = color.ToHSL();

            switch (state)
            {
                case ViewModelState.Default:
                    c = c.Darken(0.3).Saturate(0.6);
                    break;
                case ViewModelState.Moving:
                    c = c.Darken(0.3).Saturate(0.6);
                    break;
                case ViewModelState.LeftHighlighted:
                    c = c.Highlight(0.8).Desaturate(0.5);
                    break;
                case ViewModelState.RightHighlighted:
                    c = c.Highlight(0.8).Desaturate(0.5);
                    break;
                case ViewModelState.Selected:
                    c = c.Highlight(0.2);
                    break;
                case ViewModelState.Disabled:
                    c = c.Desaturate(0.2);
                    break;
                    case ViewModelState.Darken:
                    c = c.Transparent(0.3).Desaturate(0.5);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }

            switch (usage)
            {
                case BrushSetUsage.Front:
                    break;
                case BrushSetUsage.Background:
                    c = c.Highlight(0.5).Desaturate(0.3) /*.Transparent(0.5)*/;
                    if (state == ViewModelState.Moving) c = c.Transparent(0.5);
                    break;
                case BrushSetUsage.Border:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(usage), usage, null);
            }

            HSL c2 = c.Darken(0.6);

            return new LinearGradientBrush(c, c2, new Point(0, 0), new Point(0, 1));
        }
        public Brush GetBrushText(Color color, ViewModelState state, BrushSetUsage usage)
        {
            //if (usage == BrushSetUsage.Text && usage == BrushSetUsage.TextBackground)
            //{
            //    return GetBrushText(color, state, usage);
            //}

            HSL c = color.ToHSL();

            switch (state)
            {
                case ViewModelState.Default:
                    c = c.Highlight(0.8).Desaturate(0.5);
                    break;
                case ViewModelState.Moving:
                    c = c.Highlight(0.8).Desaturate(0.5);
                    break;
                case ViewModelState.Selected:
                    c = c.Darken(0.3).Saturate(0.6);
                    break;
                case ViewModelState.LeftHighlighted:
                case ViewModelState.RightHighlighted:
                    c = c.Darken(0.3)/*.Saturate(0.6)*/;
                    break;
               case ViewModelState.Disabled:
                    c = c.Desaturate(0.2);
                    break;
                case ViewModelState.Darken:
                    c = c.Transparent(0.5).Desaturate(0.5);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }

            switch (usage)
            {
                case BrushSetUsage.Text:
                    break;
                case BrushSetUsage.TextBackground:
                    c = c.Highlight(0.5).Desaturate(0.3)/*.Transparent(0.5)*/;
                    if (state == ViewModelState.Moving) c = c.Transparent(0.5);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(usage), usage, null);
            }

            //HSL c2 = c.Darken(0.6);

            //return new LinearGradientBrush(c, c2, new Point(0, 0), new Point(0, 1));
            return new SolidColorBrush(c);
        }
    }
}
