using System.Collections;
using System.Collections.Specialized;

namespace HLab.Erp.Core.Wpf.Lists
{
    public static class NotifyCollectionChangedExt
    {
        public static void HandlerInit(this INotifyCollectionChanged col, NotifyCollectionChangedEventHandler handler)
        {
            handler(col,
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, (IList)col));
            col.CollectionChanged += handler;
        }
        public static void HandlerDispose(this INotifyCollectionChanged col, NotifyCollectionChangedEventHandler handler)
        {
            handler(col,
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, (IList)col));
            col.CollectionChanged -= handler;
        }
    }
}
