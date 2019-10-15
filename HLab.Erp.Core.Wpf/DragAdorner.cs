// Copyright (C) Josh Smith - January 2007

using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;

namespace HLab.Erp.Core
{
	/// <summary>
	/// Renders a visual which can follow the mouse cursor, 
	/// such as during a drag-and-drop operation.
	/// </summary>
	public class DragAdorner : Adorner
	{
		#region Data

		private Rectangle _child = null;
		private double _offsetLeft = 0;
		private double _offsetTop = 0;

		#endregion // Data

		#region Constructor

		/// <summary>
		/// Initializes a new instance of DragVisualAdorner.
		/// </summary>
		/// <param name="adornedElement">The element being adorned.</param>
		/// <param name="size">The size of the adorner.</param>
		/// <param name="brush">A brush to with which to paint the adorner.</param>
		public DragAdorner( UIElement adornedElement, Size size, Brush brush )
			: base( adornedElement )
		{
		    _child = new Rectangle
		    {
		        Fill = brush,
		        Width = size.Width,
		        Height = size.Height,
		        IsHitTestVisible = false
		    };
		}

		#endregion // Constructor

		#region Access Interface

		#region GetDesiredTransform

		/// <summary>
		/// Override.
		/// </summary>
		/// <param name="transform"></param>
		/// <returns></returns>
		public override GeneralTransform GetDesiredTransform( GeneralTransform transform )
		{
			var result = new GeneralTransformGroup();
			result.Children.Add( base.GetDesiredTransform( transform ) );
			result.Children.Add( new TranslateTransform( this._offsetLeft, this._offsetTop ) );
			return result;
		}

		#endregion // GetDesiredTransform

		#region OffsetLeft

		/// <summary>
		/// Gets/sets the horizontal offset of the adorner.
		/// </summary>
		public double OffsetLeft
		{
            get => this._offsetLeft; set
			{
				this._offsetLeft = value;
				UpdateLocation();
			}
		}

		#endregion // OffsetLeft

		#region SetOffsets

		/// <summary>
		/// Updates the location of the adorner in one atomic operation.
		/// </summary>
		/// <param name="left"></param>
		/// <param name="top"></param>
		public void SetOffsets( double left, double top )
		{
			this._offsetLeft = left;
			this._offsetTop = top;
			this.UpdateLocation();
		}

		#endregion // SetOffsets

		#region OffsetTop

		/// <summary>
		/// Gets/sets the vertical offset of the adorner.
		/// </summary>
		public double OffsetTop
		{
            get => this._offsetTop; set
			{
				this._offsetTop = value;
				UpdateLocation();
			}
		}

		#endregion // OffsetTop

		#endregion // Access Interface

		#region Protected Overrides

		/// <summary>
		/// Override.
		/// </summary>
		/// <param name="constraint"></param>
		/// <returns></returns>
		protected override Size MeasureOverride( Size constraint )
		{
			this._child.Measure( constraint );
			return this._child.DesiredSize;
		}

		/// <summary>
		/// Override.
		/// </summary>
		/// <param name="finalSize"></param>
		/// <returns></returns>
		protected override Size ArrangeOverride( Size finalSize )
		{
			this._child.Arrange( new Rect( finalSize ) );
			return finalSize;
		}

		/// <summary>
		/// Override.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		protected override Visual GetVisualChild( int index )
		{
			return this._child;
		}

		/// <summary>
		/// Override.  Always returns 1.
		/// </summary>
		protected override int VisualChildrenCount => 1;

	    #endregion // Protected Overrides

		#region Private Helpers

		private void UpdateLocation()
		{
			var adornerLayer = this.Parent as AdornerLayer;
			if( adornerLayer != null )
				adornerLayer.Update( this.AdornedElement );
		}

		#endregion // Private Helpers
	}
}