using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using HLab.Base.Wpf;
using HLab.Icons.Wpf.Icons;

namespace HLab.Erp.Core.Wpf.EntityLists
{
    using H = DependencyHelper<AsyncContentView>;
    /// <summary>
    /// Logique d'interaction pour AsyncContentView.xaml
    /// </summary>
    public partial class AsyncContentView : UserControl
    {
        public AsyncContentView()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty PathProperty =
            H.Property<Func<Task<object>>>()
                .OnChange(async (s, e) => await s.UpdateAsync(e.NewValue).ConfigureAwait(true))
                .Register();

        public async Task UpdateAsync(Func<Task<object>> getter)
        {
            AsyncContent.Content = await getter();
        }

    }
}
