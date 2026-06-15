using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.OpenGL;
using Avalonia.Threading;
using Avalonia.VisualTree;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using Command = global::Classes.Command;
using EclBlock = global::Classes.EclBlock;
using EclDebug = global::Classes.Debug;
using EclDisassembler = global::Classes.EclDisassembler;
using gbl = global::Classes.gbl;
using Vm = global::Classes.Vm;

namespace GoldBoxPlayer.Views
{
    public class DisassemblyItem : INotifyPropertyChanged
    {
        private IBrush _color = Brushes.White;
        //private string _color = "#e0e0e0";
        private string _prefix = "       ";

        public string RawText { get; init; } = "";
        public ushort Address { get; init; }

        public string Text => _prefix + RawText;

        public string Prefix
        {
            get => _prefix;
            set
            {
                if (_prefix == value) return;
                _prefix = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Text)));
            }
        }

        //public string Color
        public IBrush Color
        {
            get => _color;
            set
            {
                if (_color == value) return;
                _color = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Color)));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }

    public class DisassemblyItem2
    {
        public string Text { get; set; } = "";
        public string Color { get; set; } = "#e0e0e0";
    }

    public partial class DebuggerWindow : Window
    {
        private DispatcherTimer? _refreshTimer;
        private double _cachedItemHeight = 0;
        private double _currentScrollOffset = 0;

        // Tab management
        private Button[] _tabButtons = null!;
        private Control[] _tabPanels = null!;

        public DebuggerWindow()
        {
            InitializeComponent();
        }

        protected override void OnOpened(EventArgs e)
        {
            base.OnOpened(e);

            // Skip all game-state access when running inside the XAML designer
            if (Avalonia.Controls.Design.IsDesignMode) return;

            // Wire up custom tab system
            _tabButtons = new Button[] { TabBtnCallStack, TabBtnBreakpoints, TabBtnMemory };
            _tabPanels = new Control[] { PanelCallStack, PanelBreakpoints, PanelMemory };

            EclDebug.Paused += OnPaused;
            EclDebug.Resumed += OnResumed;
            EclDebug.StateChanged += OnStateChanged;
            EclDebug.EclChanged += OnEclChanged;

            _refreshTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(300) };
            _refreshTimer.Tick += RefreshTimer_Tick;
            _refreshTimer.Start();

            var sv = DisassemblyListBox.GetVisualDescendants().OfType<ScrollViewer>().FirstOrDefault();
            if (sv != null)
            {
                sv.ScrollChanged += (sender, e) =>
                {
                    if (e.ExtentDelta.Y != 0 && e.OffsetDelta.Y != 0)
                        sv.Offset = new Avalonia.Vector(sv.Offset.X, _currentScrollOffset);
                };

            }
            DisassemblyListBox.ItemsSource = _disassemblyItems;
            BuildDisassembly();
            UpdateUI();
        }

        protected override void OnClosing(WindowClosingEventArgs e)
        {
            _refreshTimer?.Stop();
            EclDebug.Paused -= OnPaused;
            EclDebug.Resumed -= OnResumed;
            EclDebug.StateChanged -= OnStateChanged;
            EclDebug.EclChanged -= OnEclChanged;
            base.OnClosing(e);
        }

        // ── Tab switching ──────────────────────────────────────────────────────

        private void TabBtn_Click(object? sender, RoutedEventArgs e)
        {
            if (sender is not Button clicked) return;
            ActivateTab(clicked);
        }

        private void ActivateTab(Button target)
        {
            for (int i = 0; i < _tabButtons.Length; i++)
            {
                bool active = _tabButtons[i] == target;
                _tabPanels[i].IsVisible = active;
                if (active)
                    _tabButtons[i].Classes.Add("tabActive");
                else
                    _tabButtons[i].Classes.Remove("tabActive");
            }
        }

        private void ShowMemoryTab()
        {
            ActivateTab(TabBtnMemory);
        }

        // ── Timer / Event callbacks ────────────────────────────────────────────

        private void RefreshTimer_Tick(object? sender, EventArgs e)
        {
            if (!EclDebug.IsPaused)
            {
                UpdateMonitorsOnly();
                UpdateDisassembly();
            }
        }

        private void OnPaused(ushort offset) => Dispatcher.UIThread.Post(() => UpdateUI());
        private void OnResumed()                => Dispatcher.UIThread.Post(() => UpdateUI());
        private void OnStateChanged()           => Dispatcher.UIThread.Post(() =>
        {
            UpdateBreakpointsList();
            UpdateWatchpointsList();
            UpdateMonitorsOnly();
            UpdateDisassembly();
        });
        private void OnEclChanged() => Dispatcher.UIThread.Post(() =>
        {
            BuildDisassembly();
            UpdateUI();
        });

        // ── Full UI update ─────────────────────────────────────────────────────

        private void UpdateUI()
        {
            bool isPaused = EclDebug.IsPaused;
            bool isPausing = EclDebug.IsPausing;
            PauseButton.IsEnabled  = !isPaused && !isPausing;
            StepButton.IsEnabled   =  isPaused;
            ResumeButton.IsEnabled =  isPaused || isPausing;

            if (EclDebug.WatchpointHitAddress is ushort wpAddr)
            {
                StatusTextBlock.Text       = $"Status: Paused (Watchpoint hit on ${wpAddr:X4})";
                StatusTextBlock.Foreground = Avalonia.Media.Brushes.OrangeRed;
            }
            else if (isPaused)
            {
                StatusTextBlock.Text       = $"Status: Paused at ${EclDebug.CurrentOffset:X4}";
                StatusTextBlock.Foreground = Avalonia.Media.Brushes.Gold;
            }
            else if (isPausing)
            {
                StatusTextBlock.Text = $"Status: Pausing";
                StatusTextBlock.Foreground = Avalonia.Media.Brushes.Gold;
            }
            else
            {
                StatusTextBlock.Text       = "Status: Running";
                StatusTextBlock.Foreground = Avalonia.Media.Brushes.LightGreen;
            }

            // Skip all game-state access when running inside the XAML designer
            if (Avalonia.Controls.Design.IsDesignMode) return;

            UpdateDisassembly();
            UpdateBreakpointsList();
            UpdateWatchpointsList();
            UpdateCallStack();
            UpdateInstructionAccesses();
            UpdateMonitorsOnly();
        }

        public double GetUniformItemHeight(ListBox listBox)
        {
            if (_cachedItemHeight > 0) return _cachedItemHeight;

            if (listBox.ItemsSource == null) return 0;

            // Grab the first data item from the source
            var firstItem = listBox.ItemsSource.Cast<object>().FirstOrDefault();
            if (firstItem == null) return 0;

            // Fetch its visual container container
            listBox.ScrollIntoView(firstItem);
            listBox.UpdateLayout();

            if (listBox.ContainerFromItem(firstItem) is ListBoxItem container && container.Bounds.Height > 0)
                _cachedItemHeight = container.Bounds.Height;

            return _cachedItemHeight;
        }

        public void ScrollToKeepVisible(ListBox listBox, int firstIdx, int lastIdx)
        {
            if (listBox?.ItemsSource == null) return;

            var scrollViewer = listBox.GetVisualDescendants().OfType<ScrollViewer>().FirstOrDefault();
            if (scrollViewer == null) return;

            double itemHeight = GetUniformItemHeight(listBox);
            double viewportTop = _currentScrollOffset;  // use our tracked value
            double viewportBottom = viewportTop + scrollViewer.Viewport.Height;

            double firstY = firstIdx * itemHeight;
            double lastY = (lastIdx + 1) * itemHeight;

            // Already fully visible — don't move
            if (firstY >= viewportTop && lastY <= viewportBottom)
                return;

            double maxOffset = scrollViewer.Extent.Height - scrollViewer.Viewport.Height;
            double blockHeight = lastY - firstY;

            double newOffset;
            if (lastY > viewportBottom)
            {
                newOffset = lastY - scrollViewer.Viewport.Height + itemHeight;
                double itemPositionInViewport = lastY - newOffset;
                if (itemPositionInViewport > scrollViewer.Viewport.Height * 0.90)
                    newOffset = firstY - (scrollViewer.Viewport.Height / 2) + (blockHeight / 2);
            }
            else // above viewport
            {
                newOffset = firstY - itemHeight;
                double itemPositionInViewport = firstY - newOffset;
                if (itemPositionInViewport < scrollViewer.Viewport.Height * 0.10)
                    newOffset = firstY - (scrollViewer.Viewport.Height / 2) + (blockHeight / 2);

            }

            _currentScrollOffset = Math.Clamp(newOffset, 0, maxOffset);
            scrollViewer.Offset = new Avalonia.Vector(scrollViewer.Offset.X, _currentScrollOffset);
        }

        // ── Disassembly ────────────────────────────────────────────────────────
        private ObservableCollection<DisassemblyItem> _disassemblyItems = new();
        private Dictionary<ushort, List<DisassemblyItem>> _itemsByAddress = new();

        private static SortedSet<ushort> FindReachableAddresses(IEnumerable<ushort> entryPoints)
        {
            var visited = new SortedSet<ushort>();
            var worklist = new Stack<(ushort addr, bool afterIf)>();

            foreach (var entryPoint in entryPoints)
                worklist.Push((entryPoint, false));

            while (worklist.Count > 0)
            {
                var (addr, afterIf) = worklist.Pop();
                if (visited.Contains(addr)) continue;

                int index = addr - gbl.initial_ecl_offset;
                if (index < 0 || index >= EclBlock.ecl_struct_size) continue;

                visited.Add(addr);

                byte commandId = gbl.ecl_ptr[index];
                if (!Command.Table.TryGetValue(commandId, out var cmdItem)) continue;

                var (_, branchAddrs) = EclDisassembler.Disassemble(addr, out ushort nextAddr);

                // If we were the instruction targeted by an IF skip,
                // the instruction after us is always reachable (the false path)
                if (afterIf)
                    worklist.Push((nextAddr, false));

                switch (commandId)
                {
                    case 0x00: // EXIT
                    case 0x13: // RETURN
                    case 0x20: // NEWECL
                               // terminate this path (afterIf case already handled above)
                        break;

                    case 0x01: // GOTO - unconditional, no fallthrough
                        if (branchAddrs.Count > 0)
                            worklist.Push((branchAddrs[0], false));
                        break;

                    case 0x02: // GOSUB - follow target and continue after
                        if (branchAddrs.Count > 0)
                            worklist.Push((branchAddrs[0], false));
                        worklist.Push((nextAddr, false));
                        break;

                    case 0x16: // IF =
                    case 0x17: // IF <>
                    case 0x18: // IF 
                    case 0x19: // IF >
                    case 0x1A: // IF <=
                    case 0x1B: // IF >=
                               // nextAddr is the instruction to execute if true,
                               // and it is marked afterIf so its successor is also reachable
                        worklist.Push((nextAddr, true));
                        break;

                    case 0x25: // ON GOTO - unconditional, no fallthrough
                        foreach (var target in branchAddrs)
                            worklist.Push((target, false));
                        break;

                    case 0x26: // ON GOSUB - follow all targets and continue after
                        foreach (var target in branchAddrs)
                            worklist.Push((target, false));
                        worklist.Push((nextAddr, false));
                        break;

                    default:
                        worklist.Push((nextAddr, false));
                        break;
                }
            }

            return visited;
        }

        private void BuildDisassembly()
        {
            _disassemblyItems.Clear();
            _itemsByAddress.Clear();

            if (gbl.ecl_ptr == null || gbl.ecl_initial_entryPoint == 0) return;

            // Seed with the 5 header GOTOs - their targets will be followed automatically
            var entryPoints = new List<ushort>();
            ushort tableAddr = gbl.initial_ecl_offset;
            for (int i = 0; i < 5; i++)
            {
                entryPoints.Add(tableAddr);
                EclDisassembler.Disassemble(tableAddr, out ushort nextAddr);
                if (nextAddr <= tableAddr) break;
                tableAddr = nextAddr;
            }

            var reachable = FindReachableAddresses(entryPoints);

            foreach (ushort addr in reachable)
            {
                var (lines, _) = EclDisassembler.Disassemble(addr, out _);
                //var (lines, _) = EclDisassembler.Disassemble(addr, out ushort nextAddr);
                var itemsForAddr = new List<DisassemblyItem>();

                foreach (var line in lines)
                {
                    var item = new DisassemblyItem { Address = addr, RawText = line };
                    _disassemblyItems.Add(item);
                    itemsForAddr.Add(item);
                }

                _itemsByAddress[addr] = itemsForAddr;

                //if (nextAddr <= addr) break;
                //addr = nextAddr;
            }
        }

        private void UpdateDisassembly()
        {
            if (gbl.ecl_ptr == null) return;

            ushort currentOffset = EclDebug.CurrentOffset;
            bool isEnginePaused = EclDebug.IsPaused;

            lock (EclDebug.syncLock)
            {
                foreach (var item in _disassemblyItems)
                {
                    bool isCurrent = isEnginePaused && item.Address == currentOffset;
                    bool hasBreakpoint = EclDebug.Breakpoints.Contains(item.Address);
                    bool wasExecuted = EclDebug.ExecutionHistory.Contains(item.Address);

                    item.Prefix = isCurrent ? "=> " : "   ";
                    item.Prefix += hasBreakpoint ? "[●] " : "    ";

                    //item.Color = isCurrent ? "#f9a825"
                    //           : hasBreakpoint ? "#f44336"
                    //           : wasExecuted ? "#888888"
                    //           : "#e0e0e0";
                    item.Color = isCurrent ? Brushes.Gold
                                : hasBreakpoint ? Brushes.OrangeRed
                                : wasExecuted ? new SolidColorBrush(Color.Parse("#888888"))
                                : new SolidColorBrush(Color.Parse("#e0e0e0"));
                }
            }

            if (_itemsByAddress.TryGetValue(currentOffset, out var currentItems))
            {
                int firstIdx = _disassemblyItems.IndexOf(currentItems.First());
                int lastIdx = _disassemblyItems.IndexOf(currentItems.Last());
                ScrollToKeepVisible(DisassemblyListBox, firstIdx, lastIdx);
            }
        }

        private void UpdateDisassembly2()
        {
            var items = new List<DisassemblyItem2>();
            ushort currentOffset = EclDebug.CurrentOffset;
            bool isEnginePaused = EclDebug.IsPaused;

            if (gbl.ecl_ptr == null)
            {
                items.Add(new DisassemblyItem2 { Text = "No ECL Loaded", Color = "#888888" });
                DisassemblyListBox.ItemsSource = items;
                return;
            }
            lock (EclDebug.syncLock)
            {
                var history = EclDebug.ExecutionHistory.GetViewBetween(0, (ushort)(currentOffset - 1));
                ushort? prevAddr = null;

                // ── History (dimmed) ──────────────────────────────────────────────
                foreach (var addr in history)
                {
                    if (prevAddr.HasValue && prevAddr.Value != addr)
                    {
                        items.Add(new DisassemblyItem2
                        {
                            Text = $"   ... jumped from ${prevAddr:X4} to ${addr:X4} (skipped {addr - prevAddr} bytes)",
                            Color = "#555577"
                        });
                    }
                    var (lines, _) = EclDisassembler.Disassemble(addr, out ushort nextAddr);
                    bool hasBreakpoint = EclDebug.Breakpoints.Contains(addr);
                    string prefix = "   ";
                    prefix += hasBreakpoint ? "[●] " : "    ";
                    string color = hasBreakpoint ? "#f44336"
                                 : "#555555";
                    foreach (var line in lines)
                    {
                        items.Add(new DisassemblyItem2 { Text = prefix + line, Color = color });
                    }
                    prevAddr = nextAddr;
                }

                if (prevAddr.HasValue && prevAddr.Value != currentOffset)
                {
                    items.Add(new DisassemblyItem2
                    {
                        Text = $"   ... jumped from ${prevAddr:X4} to ${currentOffset:X4} (skipped {currentOffset - prevAddr} bytes)",
                        Color = "#555577"
                    });
                }
            }

            // ── Current + lookahead ───────────────────────────────────────────
            ushort current = currentOffset;
            for (int i = 0; i < 120; i++)
            {
                if (current - gbl.initial_ecl_offset >= EclBlock.ecl_struct_size || current - gbl.initial_ecl_offset < 0)
                    break;

                bool isCurrent = (current == currentOffset && isEnginePaused);
                bool hasBreakpoint = EclDebug.Breakpoints.Contains(current);

                var (lines, _) = EclDisassembler.Disassemble(current, out ushort next);
                string prefix = isCurrent ? "=> " : "   ";
                prefix += hasBreakpoint ? "[●] " : "    ";

                string color = isCurrent ? "#f9a825"
                             : hasBreakpoint ? "#f44336"
                             : "#e0e0e0";

                foreach (var line in lines)
                {
                    items.Add(new DisassemblyItem2 { Text = prefix + line, Color = color });
                }

                if (next <= current) break;
                current = next;
            }

            DisassemblyListBox.ItemsSource = items;

            int curIdx = items.FindIndex(it => it.Text.StartsWith("=>"));
            int curLastIdx = items.FindLastIndex(it => it.Text.StartsWith("=>"));
            if (curIdx >= 0)
            {
                // lastIdx is the last item belonging to the current instruction
                // if single-line instructions, firstIdx == lastIdx == curIdx
                ScrollToKeepVisible(DisassemblyListBox, curIdx, curLastIdx);
            }
        }

        // ── Sidebar lists ──────────────────────────────────────────────────────

        private void UpdateBreakpointsList()
        {
            var list = new List<string>();
            lock (EclDebug.syncLock)
            {
                foreach (var bp in EclDebug.Breakpoints)
                    list.Add($"${bp:X4}");
            }
            list.Sort();
            BreakpointsListBox.ItemsSource = list;
        }

        private void UpdateWatchpointsList()
        {
            var list = new List<string>();
            lock (EclDebug.syncLock)
            {
                foreach (var kvp in EclDebug.Watchpoints)
                    list.Add($"${kvp.Key:X4} (Value: {kvp.Value})");
            }
            list.Sort();
            WatchpointsListBox.ItemsSource = list;
        }

        private void UpdateCallStack()
        {
            var list = new List<string>();
            try
            {
                var stack = gbl.vmCallStack;
                if (stack != null)
                {
                    foreach (var addr in stack)
                        list.Add($"${addr:X4}");
                }
            }
            catch { /* ignore concurrent access */ }
            CallStackListBox.ItemsSource = list;
        }

        private void UpdateInstructionAccesses()
        {
            var list = new List<string>();
            if (!EclDebug.IsPaused || gbl.ecl_ptr == null)
            {
                list.Add("(only available when paused)");
                InstructionAccessListBox.ItemsSource = list;
                return;
            }

            ushort offset = EclDebug.CurrentOffset;
            var (_,addrs) = EclDisassembler.Disassemble(offset, out _);

            if (addrs.Count == 0)
            {
                list.Add("None");
            }
            else
            {
                foreach (var addr in addrs)
                {
                    try
                    {
                        ushort val = Vm.GetMemoryValue(addr);
                        list.Add($"${addr:X4}: {val} (0x{val:X4})");
                    }
                    catch
                    {
                        list.Add($"${addr:X4}: (error)");
                    }
                }
            }
            InstructionAccessListBox.ItemsSource = list;
        }

        private void UpdateMonitorsOnly()
        {
            var list      = new List<string>();
            var monitored = new List<ushort>();
            lock (EclDebug.syncLock)
            {
                foreach (var addr in EclDebug.MonitoredAddresses)
                    monitored.Add(addr);
            }
            monitored.Sort();

            foreach (var addr in monitored)
            {
                try
                {
                    ushort val = Vm.GetMemoryValue(addr);
                    list.Add($"${addr:X4}: {val} (0x{val:X4})");
                }
                catch
                {
                    list.Add($"${addr:X4}: (error)");
                }
            }
            MonitorListBox.ItemsSource = list;
        }

        // ── Toolbar button handlers ────────────────────────────────────────────

        private void PauseButton_Click(object? sender, RoutedEventArgs e)
        {
            EclDebug.Pause();
            UpdateUI();
        }

        private void StepButton_Click(object? sender, RoutedEventArgs e)  => EclDebug.Step();
        private void ResumeButton_Click(object? sender, RoutedEventArgs e)
        {
            EclDebug.Resume();
            UpdateUI();
        }

        // ── Breakpoint handlers ────────────────────────────────────────────────

        private void AddBreakpointButton_Click(object? sender, RoutedEventArgs e)
        {
            if (TryParseHex(BreakpointTextBox.Text, out ushort addr))
            {
                EclDebug.AddBreakpoint(addr);
                BreakpointTextBox.Text = "";
                UpdateUI();
            }
        }

        private void ClearBreakpointsButton_Click(object? sender, RoutedEventArgs e)
        {
            EclDebug.ClearBreakpoints();
            UpdateUI();
        }

        private void RemoveBreakpointButton_Click(object? sender, RoutedEventArgs e)
        {
            if (sender is Button { Tag: string tag } && TryParseHex(tag, out ushort addr))
            {
                EclDebug.RemoveBreakpoint(addr);
                UpdateUI();
            }
        }

        // ── Watchpoint handlers ────────────────────────────────────────────────

        private void AddWatchpointButton_Click(object? sender, RoutedEventArgs e)
        {
            if (TryParseHex(WatchpointTextBox.Text, out ushort addr))
            {
                ushort currentVal = 0;
                try { currentVal = Vm.GetMemoryValue(addr); } catch { }
                EclDebug.AddWatchpoint(addr, currentVal);
                WatchpointTextBox.Text = "";
                UpdateUI();
            }
        }

        private void ClearWatchpointsButton_Click(object? sender, RoutedEventArgs e)
        {
            EclDebug.ClearWatchpoints();
            UpdateUI();
        }

        private void RemoveWatchpointButton_Click(object? sender, RoutedEventArgs e)
        {
            if (sender is Button { Tag: string tag } && TryParseHex(tag[..6], out ushort addr))
            {
                EclDebug.RemoveWatchpoint(addr);
                UpdateUI();
            }
        }

        // ── Memory monitor handlers ────────────────────────────────────────────

        private void AddMonitorButton_Click(object? sender, RoutedEventArgs e)
        {
            if (TryParseHex(MonitorTextBox.Text, out ushort addr))
            {
                EclDebug.AddMonitoredAddress(addr);
                MonitorTextBox.Text = "";
                UpdateMonitorsOnly();
            }
        }

        private void ClearMonitorsButton_Click(object? sender, RoutedEventArgs e)
        {
            EclDebug.ClearMonitoredAddresses();
            UpdateMonitorsOnly();
        }

        private void RemoveMonitorButton_Click(object? sender, RoutedEventArgs e)
        {
            if (sender is Button { Tag: string tag } && TryParseHex(tag[..6], out ushort addr))
            {
                EclDebug.RemoveMonitoredAddress(addr);
                UpdateMonitorsOnly();
            }
        }

        /// <summary>
        /// Clicking an instruction-ref entry adds its address to the watch list
        /// and switches to the Memory tab.
        /// </summary>
        private void InstructionRef_Click(object? sender, RoutedEventArgs e)
        {
            if (sender is not Button btn) return;
            string text = btn.Content as string ?? "";

            // Format: "$ADDR: val ($VAL)"  or "(only available when paused)" / "None"
            if (text.StartsWith("$") && text.Length >= 5)
            {
                if (TryParseHex(text[..5], out ushort addr))
                {
                    EclDebug.AddMonitoredAddress(addr);
                    ShowMemoryTab();
                    UpdateMonitorsOnly();
                }
            }
        }

        // ── Disassembly double-tap ─────────────────────────────────────────────

        private void DisassemblyListBox_DoubleTapped(object? sender, Avalonia.Input.TappedEventArgs e)
        {
            if (DisassemblyListBox.SelectedItem is not DisassemblyItem item) return;

            lock (EclDebug.syncLock)
            {
                if (EclDebug.Breakpoints.Contains(item.Address))
                    EclDebug.RemoveBreakpoint(item.Address);
                else
                    EclDebug.AddBreakpoint(item.Address);
            }
            UpdateDisassembly();
        }

        private void DisassemblyListBox_DoubleTapped2(object? sender, Avalonia.Input.TappedEventArgs e)
        {
            var item = DisassemblyListBox.SelectedItem as DisassemblyItem;
            if (item == null) return;

            string text   = item.Text;
            int    hexIdx = text.IndexOf("$");
            if (hexIdx != -1 && hexIdx + 5 <= text.Length)
            {
                if (TryParseHex(text.Substring(hexIdx, 5), out ushort addr))
                {
                    lock (EclDebug.syncLock)
                    {
                        if (EclDebug.Breakpoints.Contains(addr))
                            EclDebug.RemoveBreakpoint(addr);
                        else
                            EclDebug.AddBreakpoint(addr);
                    }
                    UpdateUI();
                }
            }
        }

        // ── Helpers ───────────────────────────────────────────────────────────

        /// <summary>
        /// Parses a hex address in the form "$ABCD" or "ABCD".
        /// </summary>
        private static bool TryParseHex(string? text, out ushort addr)
        {
            addr = 0;
            if (string.IsNullOrWhiteSpace(text)) return false;
            string s = text.Trim();
            if (s.StartsWith("$", StringComparison.OrdinalIgnoreCase))
                s = s.Substring(1);
            return ushort.TryParse(s, System.Globalization.NumberStyles.HexNumber, null, out addr);
        }
    }
}
