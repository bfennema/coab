using Avalonia.Controls;
using System;

namespace GoldBoxPlayer.Views
{
    public partial class EditEventWindow : Window
    {
        public bool IsOk { get; private set; } = false;
        public int EventNumber => (int)(EventNumberNumeric.Value ?? 0);
        public bool IsIndoor => IndoorFlagCheckBox.IsChecked ?? false;

        public EditEventWindow()
        {
            InitializeComponent();
        }

        public EditEventWindow(int currentEventNumber, bool isIndoor) : this()
        {
            EventNumberNumeric.Value = currentEventNumber;
            IndoorFlagCheckBox.IsChecked = isIndoor;

            OkButton.Click += (s, e) =>
            {
                IsOk = true;
                Close();
            };

            CancelButton.Click += (s, e) =>
            {
                Close();
            };
        }
    }
}
