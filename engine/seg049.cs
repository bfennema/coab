using System.Collections.Generic;
using System.Threading;

namespace engine
{
    public class seg049
    {
        private static int SleepUntil = System.Environment.TickCount;
        private static readonly int minTick = 16;

        internal static void SysDelay(int milliseconds)
        {
            int TickCount = System.Environment.TickCount;

            // If the last sleep was in the past, reset
            if (TickCount > SleepUntil)
            {
                SleepUntil = TickCount;
            }

            // If the sleep is long enough, or the accumulated skipped sleeps are long enough, actually sleep
            if ( milliseconds >= minTick || SleepUntil - TickCount >= minTick - milliseconds)
            {
                System.Threading.Thread.Sleep(milliseconds);
                SleepUntil = System.Environment.TickCount;
            }
            else // otherwise track how much sleep time we skipped
            {
                SleepUntil += milliseconds;
            }
        }
    }
}
