using System.Windows.Media;

namespace HLab.Erp.Core.Wpf.ViewModelStates
{
    public abstract class BrushTheme
    {
        public static BrushTheme Current { get; set; } = new BlackTheme();


        public abstract Brush GetBrush(Color color, ViewModelState state, BrushSetUsage usage);
    }
}
