using System;
using System.Collections.Generic;
using System.Text;
using Classes;
using Logging;

namespace engine
{
    internal class MemLoc
    {
        ushort loc;
        internal MemLoc(ushort _loc)
        {
            loc = _loc;
        }

        public override string ToString()
        {
            return String.Format("0x{0:X}", loc);
        }
    }
}
