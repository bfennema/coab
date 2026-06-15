using Avalonia.Controls;

namespace GoldBoxPlayer.Views
{
    public partial class MapWindow : Window
    {
        public MapWindow()
        {
            InitializeComponent();
        }

        public void RefreshMap()
        {
            MapViewControl.InvalidateVisual();
        }
    }
}
