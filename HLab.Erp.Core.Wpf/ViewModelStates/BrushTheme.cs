using System.Windows.Media;
using HLab.Erp.Core.Wpf.ViewModelStates;

namespace HLab.Erp.Core.ViewModelStates
{
    public abstract class BrushTheme
    {
        public static BrushTheme Current { get; set; } = new BlackTheme();


        public abstract Brush GetBrush(Color color, ViewModelState state, BrushSetUsage usage);
    }
}
