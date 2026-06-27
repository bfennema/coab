using Avalonia.Controls;
using System;

namespace GoldBoxPlayer.Views
{
    public partial class EditEventWindow : Window
    {
        public bool IsOk { get; private set; } = false;
        public int EventNumber => (int)(EventNumberNumeric.Value ?? 0);

        public EditEventWindow()
        {
            InitializeComponent();
        }

        public EditEventWindow(int currentEventNumber) : this()
        {
            EventNumberNumeric.Value = currentEventNumber;

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
