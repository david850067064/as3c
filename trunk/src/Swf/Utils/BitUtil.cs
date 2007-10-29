using System;
using System.Collections.Generic;
using System.Text;

namespace As3c.Swf.Utils
{
    class BitUtil
    {
        public static byte LengthUB(uint value)
        {
            for (byte i = 32; i > 0; --i)
            {
                if (0 != (value & (1 << (i - 1))))
                {
                    return i;
                }
            }

            return 1;
        }

        public static byte LengthSB(int value)
        {
            byte i = 31;//ignore sign bit for now

            for (; i > 0; --i)
            {
                if (0 != (value & (1 << (i - 1))))
                {
                    break;
                }
            }

            // +1 bit for the sign
            return (byte)(i + 1);
        }
    }
}
