using System;
using System.Windows;
using System.Windows.Controls;
using HLab.Base.Wpf;
using HLab.Erp.Data;

namespace HLab.Erp.Core.Wpf.EntityLists
{

    using H = DependencyHelper<ColumnHeaderView>;



    public class SortDirectionEventArg : RoutedEventArgs
    {
        public SortDirectionEventArg(
            RoutedEvent routedEvent,
            object source, 
            SortDirection sortDirection
            ):base(routedEvent,source)
        {
            SortDirection = sortDirection;
        }

        public SortDirection SortDirection { get; }
    }

    public delegate void SortDirectionEventHandler(object sender, SortDirectionEventArg e);

    /// <summary>
    /// Logique d'interaction pour ColumnHeaderView.xaml
    /// </summary>
    public partial class ColumnHeaderView : UserControl
    {
        public ColumnHeaderView()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty SortDirectionProperty = H.Property<SortDirection>()
            .OnChange((e,a) => e.OnSortDirectionChange(a.NewValue))
            .BindsTwoWayByDefault
            .Default(SortDirection.None)
            .Register();

        public static readonly DependencyProperty OrderByOrderProperty = H.Property<int>().BindsTwoWayByDefault.Register();


        public static readonly RoutedEvent SortDirectionChangedEvent = H.Event<SortDirectionEventHandler>().Bubble.Register();
        public event SortDirectionEventHandler SortDirectionChanged
        {
            add => AddHandler(SortDirectionChangedEvent, value);
            remove => RemoveHandler(SortDirectionChangedEvent, value);
        }
            
        private void OnSortDirectionChange(SortDirection value)
        {
            SortingIcon.Path = value switch
            {
                SortDirection.Ascending => "icons/sort/ascending",
                SortDirection.Descending => "icons/sort/descending",
                SortDirection.None => "icons/sort/none",
                _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
            };
            RaiseEvent(new SortDirectionEventArg(SortDirectionChangedEvent,this,value));
        }


        public SortDirection SortDirection
        {
            get => (SortDirection) GetValue(SortDirectionProperty);
            set => SetValue(SortDirectionProperty, value);
        }
        public int OrderByOrder
        {
            get => (int) GetValue(OrderByOrderProperty);
            set => SetValue(OrderByOrderProperty, value);
        }


        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            switch (SortDirection)
            {
                case SortDirection.None:
                    SortDirection = SortDirection.Ascending;
                    break;
                case SortDirection.Ascending:
                    SortDirection = SortDirection.Descending;
                    break;
                case SortDirection.Descending:
                    SortDirection = SortDirection.None;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            ;
        }
    }
}
