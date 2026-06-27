using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;

namespace GoldBoxPlayer.Views
{
    public partial class MapWindow : Window
    {
        private const double DefaultCellSize = 25.0;
        private const double MinCellSize = 20.0;
        private const double MaxCellSize = 80.0;
        private const double ZoomStep = 5.0;

        private bool _adjusting = false;
        private double _lastWidth;
        private double _lastHeight;

        public MapWindow()
        {
            InitializeComponent();
            _lastWidth  = Width;
            _lastHeight = Height;
            this.SizeChanged += OnSizeChanged;

            // Zoom buttons
            ZoomInButton.Click    += (_, _) => AdjustZoom(+ZoomStep);
            ZoomOutButton.Click   += (_, _) => AdjustZoom(-ZoomStep);
            ZoomResetButton.Click += (_, _) => SetZoom(DefaultCellSize);

            // Ctrl+Scroll on the ScrollViewer
            MapScroller.PointerWheelChanged += OnScrollerWheel;

            // Show Event Numbers CheckBox
            ShowEventNumbersCheckBox.IsCheckedChanged += (s, e) =>
            {
                MapViewControl.ShowEventNumbers = ShowEventNumbersCheckBox.IsChecked ?? false;
            };

            UpdateZoomLabel();
        }

        // ── Zoom helpers ──────────────────────────────────────────────────────

        private void AdjustZoom(double delta) => SetZoom(MapViewControl.CellSize + delta);

        private void SetZoom(double newSize)
        {
            // Clamp
            if (newSize < MinCellSize) newSize = MinCellSize;
            if (newSize > MaxCellSize) newSize = MaxCellSize;

            MapViewControl.CellSize = newSize;
            MapViewControl.InvalidateMeasure();
            MapViewControl.InvalidateVisual();
            UpdateZoomLabel();
        }

        private void UpdateZoomLabel()
        {
            int pct = (int)(MapViewControl.CellSize / DefaultCellSize * 100);
            ZoomLabel.Text = $"{pct}%";
        }

        private void OnScrollerWheel(object? sender, PointerWheelEventArgs e)
        {
            // Only intercept Ctrl+Scroll for zoom; plain scroll is handled by the ScrollViewer
            if (e.KeyModifiers.HasFlag(KeyModifiers.Control))
            {
                double delta = e.Delta.Y > 0 ? ZoomStep : -ZoomStep;
                AdjustZoom(delta);
                e.Handled = true;
            }
        }

        // ── Square resize enforcement ─────────────────────────────────────────

        private void OnSizeChanged(object? sender, SizeChangedEventArgs e)
        {
            /*
            if (_adjusting) return;
            _adjusting = true;

            if (e.NewSize.Width != _lastWidth)
                Height = e.NewSize.Width;
            else if (e.NewSize.Height != _lastHeight)
                Width = e.NewSize.Height;

            _lastWidth  = Width;
            _lastHeight = Height;
            _adjusting  = false;
            */
        }

        // ── Map refresh ───────────────────────────────────────────────────────

        public void RefreshMap()
        {
            MapViewControl.InvalidateMeasure();
            MapViewControl.InvalidateVisual();
        }
    }
}
