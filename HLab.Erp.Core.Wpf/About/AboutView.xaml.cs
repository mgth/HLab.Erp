using System.Windows.Controls;
using System.Windows.Navigation;

namespace HLab.Erp.Core
{
    /// <summary>
    /// Logique d'interaction pour AboutView.xaml
    /// </summary>
    public partial class AboutView : UserControl
    {
        public AboutView()
        {
            InitializeComponent();
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Uri.ToString());        }
    }
}
