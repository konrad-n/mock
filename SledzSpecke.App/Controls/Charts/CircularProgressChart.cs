namespace SledzSpecke.App.Controls.Charts
{
    public class CircularProgressChart : GraphicsView
    {
        public static readonly BindableProperty ProgressProperty =
            BindableProperty.Create(nameof(Progress), typeof(double), typeof(CircularProgressChart), 0.0,
                propertyChanged: (bindable, oldValue, newValue) => ((CircularProgressChart)bindable).Invalidate());

        public static readonly BindableProperty ProgressColorProperty =
            BindableProperty.Create(nameof(ProgressColor), typeof(Color), typeof(CircularProgressChart), Colors.Blue,
                propertyChanged: (bindable, oldValue, newValue) => ((CircularProgressChart)bindable).Invalidate());

        public static readonly BindableProperty TrackColorProperty =
            BindableProperty.Create(nameof(TrackColor), typeof(Color), typeof(CircularProgressChart), Colors.LightGray,
                propertyChanged: (bindable, oldValue, newValue) => ((CircularProgressChart)bindable).Invalidate());

        public static readonly BindableProperty StrokeWidthProperty =
            BindableProperty.Create(nameof(StrokeWidth), typeof(float), typeof(CircularProgressChart), 10f,
                propertyChanged: (bindable, oldValue, newValue) => ((CircularProgressChart)bindable).Invalidate());

        public double Progress
        {
            get => (double)GetValue(ProgressProperty);
            set => SetValue(ProgressProperty, value);
        }

        public Color ProgressColor
        {
            get => (Color)GetValue(ProgressColorProperty);
            set => SetValue(ProgressColorProperty, value);
        }

        public Color TrackColor
        {
            get => (Color)GetValue(TrackColorProperty);
            set => SetValue(TrackColorProperty, value);
        }

        public float StrokeWidth
        {
            get => (float)GetValue(StrokeWidthProperty);
            set => SetValue(StrokeWidthProperty, value);
        }

        public CircularProgressChart()
        {
            Drawable = new CircularProgressDrawable(this);
        }

        internal void Invalidate()
        {
            Invalidate();
        }

        private class CircularProgressDrawable : IDrawable
        {
            private readonly CircularProgressChart _chart;

            public CircularProgressDrawable(CircularProgressChart chart)
            {
                _chart = chart;
            }

            public void Draw(ICanvas canvas, RectF dirtyRect)
            {
                float centerX = dirtyRect.Width / 2;
                float centerY = dirtyRect.Height / 2;
                float radius = Math.Min(centerX, centerY) - _chart.StrokeWidth / 2;

                // Draw track
                canvas.StrokeColor = _chart.TrackColor;
                canvas.StrokeSize = _chart.StrokeWidth;
                canvas.DrawCircle(centerX, centerY, radius);

                // Draw progress
                if (_chart.Progress > 0)
                {
                    canvas.StrokeColor = _chart.ProgressColor;
                    float startAngle = -90; // Start from top
                    float sweepAngle = (float)(_chart.Progress * 360);

                    canvas.SaveState();
                    canvas.DrawArc(centerX - radius, centerY - radius, centerX + radius, centerY + radius, startAngle, sweepAngle, false, false);
                    canvas.RestoreState();
                }
            }
        }
    }
}
