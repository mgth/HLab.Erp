using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;
using HLab.Base.Wpf;

namespace HLab.Erp.Core.Views
{
    using H = DependencyHelper<DetailView>;
    /// <summary>
    /// Logique d'interaction pour DetailView.xaml
    /// </summary>
    [ContentProperty(nameof(Children))]
    public partial class DetailView : UserControl
    {
        public DetailView()
        {
            InitializeComponent();
            Children = PART_Host.Children;
        }

        public bool EditMode
        {
            get => (bool)GetValue(EditModeProperty);
            set => SetValue(EditModeProperty, value);
        }
        public static readonly DependencyProperty EditModeProperty = H.Property<bool>().Register();

        public string IconPath
        {
            get => (string)GetValue(IconPathProperty);
            set => SetValue(IconPathProperty, value);
        }

        public static readonly DependencyProperty IconPathProperty =
            H.Property<string>()
                .Register();

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public static readonly DependencyProperty TextProperty =
            H.Property<string>()
                .Register();

        public UIElementCollection Children
        {
            get => (UIElementCollection)GetValue(ChildrenProperty.DependencyProperty);
            private init => SetValue(ChildrenProperty, value);
        }
        public static readonly DependencyPropertyKey ChildrenProperty = H.Property<UIElementCollection>().RegisterReadOnly();

    }

    public class UniformGridAuto : Grid
    {
        protected override Size MeasureOverride(Size constraint)
        {
            var columns = Math.Max(1, Math.Round(constraint.Width / 500.0, 0));
            columns = Math.Min(Children.Count, columns);
            var rows = Math.Round(0.5 + Children.Count / columns, 0);

            ColumnDefinitions.Clear();
            RowDefinitions.Clear();
            RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            for (int i = 0; i < columns; i++)
            {
                ColumnDefinitions.Add(new ColumnDefinition());
            }

            var c = 0;
            var r = 0;
            foreach (var e in Children.OfType<DependencyObject>())
            {
                e.SetValue(ColumnProperty, c);
                e.SetValue(RowProperty, r);

                c++;
                if (c >= columns)
                {
                    c = 0;
                    r++;
                    RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                }
            }
            return base.MeasureOverride(constraint);
        }
    }
}
