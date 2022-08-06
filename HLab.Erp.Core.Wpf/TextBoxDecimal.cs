using System.Diagnostics;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Input;

namespace HLab.Erp.Core.Wpf
{
    public class TextBoxDecimal : TextBox
    {
        protected override void OnKeyUp(KeyEventArgs e)
        {
            var decSep = Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;

            if (e.Key != Key.Decimal || decSep == ".") return;

            if (!(e.OriginalSource is TextBox tb)) return;
            var sText = tb.Text;

            var iPos = tb.SelectionStart - 1;
            if (iPos < 0) return;
            Debug.Assert(sText.Substring(iPos, 1) == ".");

            tb.Text = sText.Substring(0, iPos) + decSep + sText.Substring(iPos + 1);
            tb.SelectionStart = iPos + 1; // reposition cursor
            base.OnKeyUp(e);
        }
    }

    public class ComboBoxDecimal : ComboBox
    {
        protected override void OnKeyUp(KeyEventArgs e)
        {
            var decSep = Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;

            if (e.Key != Key.Decimal || decSep == ".") return;

            if (!(e.OriginalSource is TextBox tb)) return;
            var sText = tb.Text;

            var iPos = tb.SelectionStart - 1;
            if (iPos < 0) return;
            Debug.Assert(sText.Substring(iPos, 1) == ".");

            tb.Text = sText.Substring(0, iPos) + decSep + sText.Substring(iPos + 1);
            tb.SelectionStart = iPos + 1; // reposition cursor
            base.OnKeyUp(e);
        }
    }

}
