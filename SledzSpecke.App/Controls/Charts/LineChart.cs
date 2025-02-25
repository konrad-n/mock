using System.Collections;
using System.Collections.Specialized;

namespace SledzSpecke.App.Controls.Charts
{
    public class LineChart : GraphicsView
    {
        public static readonly BindableProperty ItemsSourceProperty =
            BindableProperty.Create(nameof(ItemsSource), typeof(IEnumerable), typeof(LineChart), null,
                propertyChanged: OnItemsSourceChanged);

        public static readonly BindableProperty XValuePathProperty =
            BindableProperty.Create(nameof(XValuePath), typeof(string), typeof(LineChart), null,
                propertyChanged: (bindable, oldValue, newValue) => ((LineChart)bindable).Invalidate());

        public static readonly BindableProperty YValuePathProperty =
            BindableProperty.Create(nameof(YValuePath), typeof(string), typeof(LineChart), null,
                propertyChanged: (bindable, oldValue, newValue) => ((LineChart)bindable).Invalidate());

        public static readonly BindableProperty LineColorProperty =
            BindableProperty.Create(nameof(LineColor), typeof(Color), typeof(LineChart), Colors.Blue,
                propertyChanged: (bindable, oldValue, newValue) => ((LineChart)bindable).Invalidate());

        public static readonly BindableProperty PointColorProperty =
            BindableProperty.Create(nameof(PointColor), typeof(Color), typeof(LineChart), Colors.Red,
                propertyChanged: (bindable, oldValue, newValue) => ((LineChart)bindable).Invalidate());

        public IEnumerable ItemsSource
        {
            get => (IEnumerable)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public string XValuePath
        {
            get => (string)GetValue(XValuePathProperty);
            set => SetValue(XValuePathProperty, value);
        }

        public string YValuePath
        {
            get => (string)GetValue(YValuePathProperty);
            set => SetValue(YValuePathProperty, value);
        }

        public Color LineColor
        {
            get => (Color)GetValue(LineColorProperty);
            set => SetValue(LineColorProperty, value);
        }

        public Color PointColor
        {
            get => (Color)GetValue(PointColorProperty);
            set => SetValue(PointColorProperty, value);
        }

        public LineChart()
        {
            Drawable = new LineChartDrawable(this);
        }

        private static void OnItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var chart = (LineChart)bindable;

            if (oldValue is INotifyCollectionChanged oldCollection)
            {
                oldCollection.CollectionChanged -= chart.OnCollectionChanged;
            }

            if (newValue is INotifyCollectionChanged newCollection)
            {
                newCollection.CollectionChanged += chart.OnCollectionChanged;
            }

            chart.Invalidate();
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Invalidate();
        }

        internal void Invalidate()
        {
            Invalidate();
        }

        private class LineChartDrawable : IDrawable
        {
            private readonly LineChart _chart;

            public LineChartDrawable(LineChart chart)
            {
                _chart = chart;
            }

            public void Draw(ICanvas canvas, RectF dirtyRect)
            {
                if (_chart.ItemsSource == null || string.IsNullOrEmpty(_chart.XValuePath) || string.IsNullOrEmpty(_chart.YValuePath))
                {
                    return;
                }

                // Placeholder implementation - in a real app, you would draw lines based on data
                float margin = 20;
                float chartWidth = dirtyRect.Width - 2 * margin;
                float chartHeight = dirtyRect.Height - 2 * margin;

                canvas.FontColor = Colors.Black;
                canvas.DrawString("Line Chart Placeholder", dirtyRect.Center.X - 60, dirtyRect.Center.Y, HorizontalAlignment.Center);
            }
        }
    }
}
