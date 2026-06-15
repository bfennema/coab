using System;
using System.Collections.Generic;

namespace Classes
{
    public class Debug
    {
        public static HashSet<ushort> Breakpoints { get; } = new HashSet<ushort>();
        public static Dictionary<ushort, ushort> Watchpoints { get; } = new Dictionary<ushort, ushort>();
        public static HashSet<ushort> MonitoredAddresses { get; } = new HashSet<ushort>();
        public static readonly SortedSet<ushort> ExecutionHistory = new();
        private const int HistoryCapacity = 200;
        private static byte CurrentBlockId = 0;

        public static readonly object syncLock = new object();
        private static bool isStepMode = false;
        private static bool isPaused = false;
        private static ushort currentOffset = 0;
        private static ushort? watchpointHitAddress = null;
        private static readonly System.Threading.AutoResetEvent resumeEvent = new System.Threading.AutoResetEvent(false);

        public static bool IsPaused
        {
            get { lock (syncLock) return isPaused; }
        }

        public static bool IsPausing
        {
            get { lock (syncLock) return isStepMode && !isPaused; }
        }

        public static ushort CurrentOffset
        {
            get { lock (syncLock) return currentOffset; }
        }

        public static ushort? WatchpointHitAddress
        {
            get { lock (syncLock) return watchpointHitAddress; }
            set { lock (syncLock) watchpointHitAddress = value; }
        }

        public static event System.Action<ushort>? Paused;
        public static event System.Action? Resumed;
        public static event System.Action? StateChanged;
        public static event System.Action? EclChanged;

        private static void TriggerUpdate()
        {
            StateChanged?.Invoke();
        }

        public static void NotifyEclChanged()
        {
            EclChanged?.Invoke();
        }

        public static void ClearBreakpoints()
        {
            lock (syncLock)
            {
                Breakpoints.Clear();
            }
            TriggerUpdate();
        }

        public static void AddBreakpoint(ushort addr)
        {
            lock (syncLock)
            {
                Breakpoints.Add(addr);
            }
            TriggerUpdate();
        }

        public static bool RemoveBreakpoint(ushort addr)
        {
            bool removed;
            lock (syncLock)
            {
                removed = Breakpoints.Remove(addr);
            }
            TriggerUpdate();
            return removed;
        }

        public static void ClearWatchpoints()
        {
            lock (syncLock)
            {
                Watchpoints.Clear();
            }
            TriggerUpdate();
        }

        public static void AddWatchpoint(ushort addr, ushort initialValue)
        {
            lock (syncLock)
            {
                Watchpoints[addr] = initialValue;
            }
            TriggerUpdate();
        }

        public static bool RemoveWatchpoint(ushort addr)
        {
            bool removed;
            lock (syncLock)
            {
                removed = Watchpoints.Remove(addr);
            }
            TriggerUpdate();
            return removed;
        }

        public static void ClearMonitoredAddresses()
        {
            lock (syncLock)
            {
                MonitoredAddresses.Clear();
            }
            TriggerUpdate();
        }

        public static void AddMonitoredAddress(ushort addr)
        {
            lock (syncLock)
            {
                MonitoredAddresses.Add(addr);
            }
            TriggerUpdate();
        }

        public static bool RemoveMonitoredAddress(ushort addr)
        {
            bool removed;
            lock (syncLock)
            {
                removed = MonitoredAddresses.Remove(addr);
            }
            TriggerUpdate();
            return removed;
        }

        public static bool CheckBreakpoint(ushort addr)
        {
            OnStep(addr);
            lock (syncLock)
            {
                return Breakpoints.Contains(addr);
            }
        }

        public static void OnStep(ushort offset)
        {
            bool shouldPause = false;
            lock (syncLock)
            {
                currentOffset = offset;

                if (gbl.EclBlockId == CurrentBlockId)
                {
                    // Record history here, on every step
                    if (ExecutionHistory.Count >= HistoryCapacity)
                        ExecutionHistory.Remove(ExecutionHistory.Min);
                    ExecutionHistory.Add(offset);
                }
                else
                {
                    ExecutionHistory.Clear();
                    CurrentBlockId = gbl.EclBlockId;
                }

                if (isStepMode || Breakpoints.Contains(offset) || watchpointHitAddress.HasValue)
                {
                    shouldPause = true;
                    isPaused = true;
                    isStepMode = false;
                }
            }

            if (shouldPause)
            {
                Paused?.Invoke(offset);
                resumeEvent.WaitOne();

                lock (syncLock)
                {
                    isPaused = false;
                }
                Resumed?.Invoke();
            }
        }

        public static void OnMemoryWrite(ushort addr, ushort newValue)
        {
            bool shouldBreak = false;
            lock (syncLock)
            {
                if (Watchpoints.TryGetValue(addr, out ushort oldValue))
                {
                    if (oldValue != newValue)
                    {
                        Watchpoints[addr] = newValue;
                        watchpointHitAddress = addr;
                        shouldBreak = true;
                    }
                }
            }

            if (shouldBreak)
            {
                OnStep(currentOffset);
            }
        }

        public static void Step()
        {
            lock (syncLock)
            {
                if (isPaused)
                {
                    watchpointHitAddress = null;
                    isStepMode = true;
                    resumeEvent.Set();
                }
            }
        }

        public static void Resume()
        {
            lock (syncLock)
            {
                if (isPaused)
                {
                    watchpointHitAddress = null;
                    isStepMode = false;
                    resumeEvent.Set();
                }
                else if (IsPausing)
                {
                    isStepMode = false;
                }
            }
        }

        public static void Pause()
        {
            lock (syncLock)
            {
                if (!isPaused)
                {
                    isStepMode = true;
                }
            }
        }
    }
}

