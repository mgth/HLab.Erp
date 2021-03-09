using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using HLab.Core.Annotations;
using HLab.DependencyInjection.Annotations;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using Panel = System.Windows.Controls.Panel;

namespace HLab.Erp.Core.DragDrops
{
    //public class ErpDragDropEventArg
    //{
    //    public ErpDragDropEventArg(ErpDragDrop source)
    //    {
    //        Query = source;
    //    }
    //    public ErpDragDrop Query { get;}
    //    public FrameworkElement FrameworkElement { get; set; }
    //    public bool IsDragging { get; set; }
    //}

    public interface IDropViewModel
    {
        bool DragEnter(ErpDragDrop data);
        bool DragLeave(ErpDragDrop data);
        bool DropIn(ErpDragDrop data);
        void DropOut(ErpDragDrop data);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns>true if object can deal with the drop</returns>
        bool DragStart(ErpDragDrop data);
    }


    public class ErpDragDrop
    {
        [Import]
        public IMessageBus MessageBus { get; set; }

        public delegate void DragDropEventHandler(ErpDragDrop data);

        public event DragDropEventHandler Start;
        public event DragDropEventHandler Drop;
        public event DragDropEventHandler Move;

        private Point _startPoint;
        private IDropViewModel _currentTarget = null;
        public Panel Panel { get; }
        private FrameworkElement Source { get; }
        private readonly bool _sendMessages = false;


        //private readonly ErpDragDropEventArg _args;

            /// <summary>
            /// 
            /// </summary>
        public FrameworkElement DraggedElement
        {
            get => _draggedElement; set
            {
                EndDrag(true);

                _draggedElement = value;
                if (DraggedElement == null) return;

                StartDrag();
            }
        }

        public bool Canceled { get; internal set; }
        public bool Positioned { get; set; } = false;


        public ErpDragDrop(Panel p, FrameworkElement source, bool sendMessages=false)
        {
            Panel = p;
            Source = source;
            _sendMessages = sendMessages;
            Source.PreviewMouseLeftButtonDown += OnMouseDown;
        }

        public MouseEventArgs MouseEventArgs { get; private set; }




        public bool HitTest(FrameworkElement e)
        {
            if (MouseEventArgs == null) return false;

            Point p = MouseEventArgs.GetPosition(e);

            if (p.X >= 0 && p.Y > 0 && e.ActualWidth > p.X && e.ActualHeight > p.Y)
            {
                return true;
            }
            return false;
        }

        public bool IsDragging => DraggedElement != null && DraggedElement.IsMouseCaptured;

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            _startPoint = e.GetPosition(Source);
            MouseEventArgs = e;

            Source.PreviewMouseLeftButtonUp += OnMouseUp;
            Source.PreviewMouseMove += OnMouseMove;
        }

        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            //Debug.Print("OnMouseUp");
            DraggedElement=null;
        }
        public static IEnumerable<IDropViewModel> FindDropEnabledModels(DependencyObject depObj)
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);

                    HashSet<IDropViewModel> cache = new HashSet<IDropViewModel>();

                    if (child is FrameworkElement && ((FrameworkElement)child).DataContext is IDropViewModel)
                    {
                        var item = (IDropViewModel)((FrameworkElement)child).DataContext;
                        if (!cache.Contains(item))
                        {
                            cache.Add(item);
                            yield return item;
                        }                       
                    }

                    foreach (IDropViewModel item in FindDropEnabledModels(child))
                    {
                        if (!cache.Contains(item))
                        {
                            cache.Add(item);
                            yield return item;
                        }
                    }
                }
            }
        }

        private HashSet<IDropViewModel> _currentViewModels = null;
        private HashSet<IDropViewModel> _allViewModels = null;

        private void DragInvokeViewModels()
        {
            var w = Application.Current.MainWindow;

            _allViewModels = new HashSet<IDropViewModel>(FindDropEnabledModels(w));

            _currentViewModels = new HashSet<IDropViewModel>();
            foreach (var vm in _allViewModels)
            {
                if (vm.DragStart(this)) _currentViewModels.Add(vm);
            }
        }


        public void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (Math.Abs(e.GetPosition(Source).X - _startPoint.X) >
                SystemParameters.MinimumHorizontalDragDistance || Math.Abs(e.GetPosition(Source).Y - _startPoint.Y) >
                SystemParameters.MinimumVerticalDragDistance)
            {
                Source.PreviewMouseMove -= OnMouseMove;
                MouseEventArgs = e;

                Start?.Invoke(this);

                if (DraggedElement != null)
                {
                    DragInvokeViewModels();
                    if (_sendMessages) MessageBus.Publish(this);                   
                }
            }
        }

        private VerticalAlignment _bckVerticalAlignment;
        private HorizontalAlignment _bckHorizontalAlignment;
        private bool _isHitTestVisible;
        private Thickness _bckMargin;
        private Binding _bckMarginBinding;
        private FrameworkElement _draggedElement;

        public Vector DragShift { get; set; }


        public void OnMouseMoveDragged(object sender, MouseEventArgs e)
        {
                DragMove(e);           
        }


        private void StartDrag()
        {
            Canceled = false;

            _bckVerticalAlignment = DraggedElement.VerticalAlignment;
            _bckHorizontalAlignment = DraggedElement.HorizontalAlignment;
            _isHitTestVisible = DraggedElement.IsHitTestVisible;
            _bckMargin = DraggedElement.Margin;
            _bckMarginBinding = DraggedElement.GetBindingExpression(FrameworkElement.MarginProperty)?.ParentBinding;


            //DraggedElement.VerticalAlignment = VerticalAlignment.Top;
            //DraggedElement.HorizontalAlignment = HorizontalAlignment.Left;
            DraggedElement.IsHitTestVisible = false;

            DraggedElement.MouseMove += OnMouseMoveDragged;
            DraggedElement.MouseLeftButtonUp += OnMouseUp;

            (DraggedElement.Parent as Panel)?.Children.Remove(DraggedElement);
            Panel.Children.Add(DraggedElement);

            DraggedElement.CaptureMouse();
            DraggedElement.LostMouseCapture += OnLostMouseCapture;

            DragMove(MouseEventArgs);
        }

        private void EndDrag(bool cancel=false)
        {
            //Debug.Print("EndDrag");

            Canceled = cancel;

            if (IsDragging)
            {
                DragMove(MouseEventArgs);

                (DraggedElement.Parent as Panel)?.Children.Remove(DraggedElement);

                DraggedElement.VerticalAlignment = _bckVerticalAlignment;
                DraggedElement.HorizontalAlignment = _bckHorizontalAlignment;
                DraggedElement.IsHitTestVisible = _isHitTestVisible;
                DraggedElement.Margin = _bckMargin;
                if (_bckMarginBinding != null)
                    DraggedElement.SetBinding(FrameworkElement.MarginProperty, _bckMarginBinding);

                DraggedElement.MouseMove -= OnMouseMoveDragged;
                DraggedElement.MouseLeftButtonUp -= OnMouseUp;
                DraggedElement.LostMouseCapture -= OnLostMouseCapture;

                DraggedElement.ReleaseMouseCapture();

                foreach (var vm in _allViewModels)
                {
                    if (!ReferenceEquals(vm, _currentTarget)) vm.DropOut(this);
                }
                _currentTarget?.DropIn(this);
                Drop?.Invoke(this);
            }

            Source.MouseLeftButtonUp -= OnMouseUp;
            Source.PreviewMouseMove -= OnMouseMove;
        }

        private void OnLostMouseCapture(object sender, MouseEventArgs e)
        {
            DraggedElement.CaptureMouse();
            //Debug.Print("LostCapture");
            return;
        }

        private void SetPosition()
        {
            Point p = MouseEventArgs.GetPosition(Panel);

            DraggedElement.Margin
                //= new Thickness(p.X - DraggedElement.ActualWidth / 2, p.Y - DraggedElement.ActualHeight / 2, 0, 0);
                = new Thickness(p.X + DragShift.X, p.Y + DragShift.Y, 0, 0);
        }

        private void DragMove(MouseEventArgs e)
        {
            if (!IsDragging) return;

            MouseEventArgs = e;

            Positioned = false;


            if (_currentViewModels != null)
            {
                _newTarget = null;
                
                Window w = Application.Current.MainWindow;

                var pt = e.GetPosition(w);
                VisualTreeHelper.HitTest(w, null,
                    HitTest,
                    new PointHitTestParameters(pt));

                if (!ReferenceEquals(_newTarget, _currentTarget))
                {
                    _currentTarget?.DragLeave(this);
                    _currentTarget = _newTarget;
                    if (!(_currentTarget?.DragEnter(this) ?? false))
                    {
                        _currentViewModels.Remove(_currentTarget);
                        _currentTarget?.DragLeave(this);
                        _currentTarget = null;
                    }
                }

            }



            SetPosition();
            Move?.Invoke(this);

            if(!Positioned) SetPosition();     
                   
            //if (_sendMessages) MessageBus.D.Publish(this);
        }

        private IDropViewModel _newTarget;

        private HitTestResultBehavior HitTest(HitTestResult result)
        {
            var fe = result?.VisualHit as FrameworkElement;
            var vm = fe?.DataContext as IDropViewModel;

            if (_currentViewModels.Contains(vm))
            {
                _newTarget = vm;
                return HitTestResultBehavior.Stop;
            }
            return HitTestResultBehavior.Continue;

            //if (!ReferenceEquals(vm, _currentTarget))
            //{
            //    if (_currentViewModels.Contains(vm))
            //    _currentTarget?.DragLeave(this);
            //    _currentTarget = vm;
            //    if (_currentTarget?.DragEnter(this)??false)
            //        return HitTestResultBehavior.Stop;
            //}
            //return HitTestResultBehavior.Continue;
        }
    }
}
