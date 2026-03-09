using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace AssetManager.Controls
{
    public class MasonryPanel : Panel
    {
        public double MinColumnWidth
        {
            get => (double)GetValue(MinColumnWidthProperty);
            set => SetValue(MinColumnWidthProperty, value);
        }

        public static readonly DependencyProperty MinColumnWidthProperty =
            DependencyProperty.Register(
                nameof(MinColumnWidth),
                typeof(double),
                typeof(MasonryPanel),
                new FrameworkPropertyMetadata(200.0, FrameworkPropertyMetadataOptions.AffectsMeasure));

        protected override Size MeasureOverride(Size availableSize)
        {
            if (InternalChildren.Count == 0)
                return new Size(0, 0);

            double width = double.IsInfinity(availableSize.Width) ? 800 : availableSize.Width;

            int columnCount = Math.Max(1, (int)(width / MinColumnWidth));

            double columnWidth = width / columnCount;

            double[] columnHeights = new double[columnCount];

            foreach (UIElement child in InternalChildren)
            {
                child.Measure(new Size(columnWidth, double.PositiveInfinity));

                int column = 0;
                double min = columnHeights[0];

                for (int i = 1; i < columnCount; i++)
                {
                    if (columnHeights[i] < min)
                    {
                        column = i;
                        min = columnHeights[i];
                    }
                }

                columnHeights[column] += child.DesiredSize.Height;
            }

            return new Size(width, columnHeights.Max());
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            int columnCount = Math.Max(1, (int)(finalSize.Width / MinColumnWidth));

            double columnWidth = finalSize.Width / columnCount;

            double[] columnHeights = new double[columnCount];

            foreach (UIElement child in InternalChildren)
            {
                int column = 0;
                double min = columnHeights[0];

                for (int i = 1; i < columnCount; i++)
                {
                    if (columnHeights[i] < min)
                    {
                        column = i;
                        min = columnHeights[i];
                    }
                }

                double x = column * columnWidth;
                double y = columnHeights[column];

                child.Arrange(new Rect(x, y, columnWidth, child.DesiredSize.Height));

                columnHeights[column] += child.DesiredSize.Height;
            }

            return new Size(finalSize.Width, columnHeights.Max());
        }
    }
}