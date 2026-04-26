using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Flower.Helpers
{
    public class WaterfallPanel : Panel
    {
        // 期望列宽（实际列宽将尽量匹配此值，但会缩放以填满容器）
        public static readonly DependencyProperty ColumnWidthProperty =
            DependencyProperty.Register(nameof(ColumnWidth), typeof(double), typeof(WaterfallPanel),
                new FrameworkPropertyMetadata(200.0, FrameworkPropertyMetadataOptions.AffectsMeasure));

        public double ColumnWidth
        {
            get => (double)GetValue(ColumnWidthProperty);
            set => SetValue(ColumnWidthProperty, value);
        }

        public static readonly DependencyProperty SpacingProperty =
            DependencyProperty.Register(nameof(Spacing), typeof(double), typeof(WaterfallPanel),
                new FrameworkPropertyMetadata(4.0, FrameworkPropertyMetadataOptions.AffectsMeasure));

        public double Spacing
        {
            get => (double)GetValue(SpacingProperty);
            set => SetValue(SpacingProperty, value);
        }

        // 记录本次布局的实际列宽
        private double actualColumnWidth;

        protected override Size MeasureOverride(Size availableSize)
        {
            if (double.IsInfinity(availableSize.Width) || availableSize.Width <= 0)
                return new Size(0, 0);

            // 计算实际列宽：让列宽尽可能接近 ColumnWidth，但拉伸以填满宽度
            int columnCount = Math.Max(1, (int)((availableSize.Width + Spacing) / (ColumnWidth + Spacing)));
            actualColumnWidth = (availableSize.Width + Spacing) / columnCount - Spacing;

            double[] columnHeights = new double[columnCount];

            foreach (UIElement child in Children)
            {
                child.Measure(new Size(actualColumnWidth, double.PositiveInfinity));
                int minCol = MinHeightColumn(columnHeights);
                columnHeights[minCol] += child.DesiredSize.Height + Spacing;
            }

            double totalHeight = columnHeights.Length > 0 ? columnHeights.Max() : 0;
            totalHeight = totalHeight > 0 ? totalHeight - Spacing : 0;
            return new Size(availableSize.Width, Math.Max(0, totalHeight));
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            int columnCount = Math.Max(1, (int)((finalSize.Width + Spacing) / (ColumnWidth + Spacing)));
            actualColumnWidth = (finalSize.Width + Spacing) / columnCount - Spacing;
            double[] columnHeights = new double[columnCount];
            double[] columnX = new double[columnCount];
            for (int i = 0; i < columnCount; i++)
                columnX[i] = i * (actualColumnWidth + Spacing);

            foreach (UIElement child in Children)
            {
                int minCol = MinHeightColumn(columnHeights);
                double x = columnX[minCol];
                double y = columnHeights[minCol];
                child.Arrange(new Rect(x, y, actualColumnWidth, child.DesiredSize.Height));
                columnHeights[minCol] += child.DesiredSize.Height + Spacing;
            }

            return finalSize;
        }

        private int MinHeightColumn(double[] heights)
        {
            int min = 0;
            for (int i = 1; i < heights.Length; i++)
                if (heights[i] < heights[min]) min = i;
            return min;
        }
    }
}