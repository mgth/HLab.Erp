using System.Windows;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Core.Wpf.Extensions
{
    public static class ViewExt
    {
        public static Window AsWindow(this IView view)
        {
            if (view is Window win) return win;

            var w = new Mvvm.Wpf.DefaultWindow
            {
                DataContext = (view as FrameworkElement)?.DataContext,
                Content = view,
                //SizeToContent = SizeToContent.WidthAndHeight,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
            };

            //if(view is FrameworkElement e)
            //{
            //    w.Height = e.Height;
            //    w.Width = e.Width;
            //}

            return w;
        }
    }
}
