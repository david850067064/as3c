using System;
using System.Collections.Generic;
using System.Text;
using As3c.Swf.Types;

namespace As3c.Swf.Utils
{
    class Capacity
    {
        public static int Max(uint count)
        {
            if (count > int.MaxValue)
                return int.MaxValue;

            return (int)count;
        }

        public static int Max(U30 count)
        {
            return Max((uint)count);
        }
    }
}
