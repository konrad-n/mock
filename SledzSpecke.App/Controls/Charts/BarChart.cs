using System.Collections;
using System.Collections.Specialized;

namespace SledzSpecke.App.Controls.Charts
{
    public class BarChart : GraphicsView
    {
        public static readonly BindableProperty ItemsSourceProperty =
            BindableProperty.Create(nameof(ItemsSource), typeof(IEnumerable), typeof(BarChart), null,
                propertyChanged: OnItemsSourceChanged);

        public static readonly BindableProperty ValuePathProperty =
            BindableProperty.Create(nameof(ValuePath), typeof(string), typeof(BarChart), null,
                propertyChanged: (bindable, oldValue, newValue) => ((BarChart)bindable).Invalidate());

        public static readonly BindableProperty CategoryPathProperty =
            BindableProperty.Create(nameof(CategoryPath), typeof(string), typeof(BarChart), null,
                propertyChanged: (bindable, oldValue, newValue) => ((BarChart)bindable).Invalidate());

        public static readonly BindableProperty BarColorProperty =
            BindableProperty.Create(nameof(BarColor), typeof(Color), typeof(BarChart), Colors.Blue,
                propertyChanged: (bindable, oldValue, newValue) => ((BarChart)bindable).Invalidate());

        public IEnumerable ItemsSource
        {
            get => (IEnumerable)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public string ValuePath
        {
            get => (string)GetValue(ValuePathProperty);
            set => SetValue(ValuePathProperty, value);
        }

        public string CategoryPath
        {
            get => (string)GetValue(CategoryPathProperty);
            set => SetValue(CategoryPathProperty, value);
        }

        public Color BarColor
        {
            get => (Color)GetValue(BarColorProperty);
            set => SetValue(BarColorProperty, value);
        }

        public BarChart()
        {
            Drawable = new BarChartDrawable(this);
        }

        private static void OnItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var chart = (BarChart)bindable;

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

        internal new void Invalidate()
        {
            Invalidate();
        }

        private class BarChartDrawable : IDrawable
        {
            private readonly BarChart _chart;

            public BarChartDrawable(BarChart chart)
            {
                _chart = chart;
            }

            public void Draw(ICanvas canvas, RectF dirtyRect)
            {
                if (_chart.ItemsSource == null || string.IsNullOrEmpty(_chart.ValuePath) || string.IsNullOrEmpty(_chart.CategoryPath))
                {
                    return;
                }

                // Placeholder implementation - in a real app, you would draw bars based on data
                float margin = 20;
                float barSpacing = 10;
                float chartWidth = dirtyRect.Width - 2 * margin;
                float chartHeight = dirtyRect.Height - 2 * margin;

                canvas.FontColor = Colors.Black;
                canvas.DrawString("Bar Chart Placeholder", dirtyRect.Center.X - 60, dirtyRect.Center.Y, HorizontalAlignment.Center);
            }
        }
    }
}
