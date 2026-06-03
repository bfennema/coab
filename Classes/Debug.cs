using System.Collections.Generic;

namespace Classes
{
    public class Debug
    {
        static List<ushort> breakpoints = [];

        public static void ClearBreakpoints()
        {
            breakpoints.Clear();
        }

        public static void AddBreakpoint(ushort addr)
        {
            breakpoints.Add(addr);
        }

        public static bool RemoveBreakpoint(ushort addr)
        {
            if (breakpoints.Contains(addr))
            {
                breakpoints.Remove(addr);
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool CheckBreakpoint(ushort addr)
        {
            if (breakpoints.Contains(addr))
            {
                System.Diagnostics.Debugger.Break();
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
