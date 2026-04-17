using System;
using System.Windows;
using System.Windows.Controls;

namespace FluentFlyout.Controls
{
    public class WrappingGrid : Grid
    {
        /// <summary> Index of the item whose width should not drop below the threshold </summary>
        public int StaticItemIndex
        {
            get => (int)GetValue(StaticItemIndexProperty);
            set => SetValue(StaticItemIndexProperty, value);
        }

        public static readonly DependencyProperty StaticItemIndexProperty =
            DependencyProperty.Register(nameof(StaticItemIndex), typeof(int), typeof(WrappingGrid),
                new PropertyMetadata(0, OnLayoutPropertyChanged));

        /// <summary> Index of the item which is wrapped the next row if width is constrained (wrapping means dropping it into column 0, last row, not wrapped means last column, row 0) </summary>
        public int WrappingItemIndex
        {
            get => (int)GetValue(WrappingItemIndexProperty);
            set => SetValue(WrappingItemIndexProperty, value);
        }

        public static readonly DependencyProperty WrappingItemIndexProperty =
            DependencyProperty.Register(nameof(WrappingItemIndex), typeof(int), typeof(WrappingGrid),
                new PropertyMetadata(1, OnLayoutPropertyChanged));

        /// <summary> Threshold for wrapping logic </summary>
        public double WrapThreshold
        {
            get => (double)GetValue(WrapThresholdProperty);
            set => SetValue(WrapThresholdProperty, value);
        }

        public static readonly DependencyProperty WrapThresholdProperty =
            DependencyProperty.Register(nameof(WrapThreshold), typeof(double), typeof(WrappingGrid),
                new PropertyMetadata(220.0, OnLayoutPropertyChanged));
        
        /// <summary> When specified, determines the index of the item whose desired width is used instead of the threshold </summary>
        public int DesiredSizeIndex
        {
            get => (int)GetValue(DesiredSizeIndexProperty);
            set => SetValue(DesiredSizeIndexProperty, value);
        }

        public static readonly DependencyProperty DesiredSizeIndexProperty =
            DependencyProperty.Register(nameof(DesiredSizeIndex), typeof(int), typeof(WrappingGrid),
                new PropertyMetadata(-1, OnLayoutPropertyChanged));

        /// <summary> Row spacing when wrapped (value < 0 idicates *) </summary>
        public double RowSpacing
        {
            get => (double)GetValue(RowSpacingProperty);
            set => SetValue(RowSpacingProperty, value);
        }

        public static readonly DependencyProperty RowSpacingProperty =
            DependencyProperty.Register(nameof(RowSpacing), typeof(double), typeof(WrappingGrid),
                new PropertyMetadata(16.0, OnLayoutPropertyChanged));

        /// <summary> Column spacing when not wrapped (value < 0 idicates *) </summary>
        public double ColumnSpacing
        {
            get => (double)GetValue(ColumnSpacingProperty);
            set => SetValue(ColumnSpacingProperty, value);
        }

        public static readonly DependencyProperty ColumnSpacingProperty =
            DependencyProperty.Register(nameof(ColumnSpacing), typeof(double), typeof(WrappingGrid),
                new PropertyMetadata(0.0, OnLayoutPropertyChanged));

        public WrappingGrid()
        {
            RowDefinitions.Add(new RowDefinition());
            RowDefinitions.Add(new RowDefinition());

            ColumnDefinitions.Add(new ColumnDefinition());
            ColumnDefinitions.Add(new ColumnDefinition());
        }

        private static void OnLayoutPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is WrappingGrid grid)
            {
                grid.UpdateLayoutLogic();
            }
        }

        protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
        {
            base.OnVisualChildrenChanged(visualAdded, visualRemoved);
            UpdateLayoutLogic();
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            UpdateLayoutLogic();
        }

        private void UpdateLayoutLogic()
        {
            if (Children.Count > 2)
                throw new InvalidOperationException("WrappingGrid supports only 2 children.");

            if (Children.Count != 2)
                return;

            var nonWrappingChild = Children[1 - WrappingItemIndex];

            // Place non wrapping child
            Grid.SetRow(nonWrappingChild, 0);
            Grid.SetColumn(nonWrappingChild, 1 - WrappingItemIndex);

            if (Children.Count == 1)
                return;

            var wrappingChild = Children[WrappingItemIndex];

            if (ActualWidth > 0)
            {
                // Measure children
                Children[0].Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                Children[1].Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

                double staticWidth = Children[StaticItemIndex].DesiredSize.Width;
                double remainingWidth = ActualWidth - staticWidth - ((ColumnSpacing < 0) ? 0 : ColumnSpacing);

                if (((DesiredSizeIndex < 0) && (remainingWidth < WrapThreshold)) || ((-1 < DesiredSizeIndex) && (remainingWidth < Children[DesiredSizeIndex].DesiredSize.Width)))
                {
                    // Wrap wrapping child to row 1, column 0
                    Grid.SetRow(wrappingChild, 2);
                    Grid.SetColumn(wrappingChild, 0);
                    Grid.SetColumnSpan(wrappingChild, 2);
                    Grid.SetColumnSpan(nonWrappingChild, 2);
                    ColumnDefinitions[1].Width = new GridLength(0);
                    RowDefinitions[1].Height = (RowSpacing < 0) ? new GridLength(1, GridUnitType.Star) : new GridLength(RowSpacing);
                }
                else
                {
                    Grid.SetRow(wrappingChild, 0);
                    Grid.SetColumn(wrappingChild, 2);
                    Grid.SetColumnSpan(wrappingChild, 1);
                    Grid.SetColumnSpan(nonWrappingChild, 1);
                    ColumnDefinitions[1].Width = (ColumnSpacing < 0) ? new GridLength(1, GridUnitType.Star) : new GridLength(ColumnSpacing);
                    RowDefinitions[1].Height = new GridLength(0);
                }
            }
            else
            {
                // Default placement
                Grid.SetRow(wrappingChild, 0);
                Grid.SetColumn(wrappingChild, 2);
            }
        }
    }
}