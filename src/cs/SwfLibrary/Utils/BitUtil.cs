/*
Copyright(C) 2007 Joa Ebert

As3c is an ActionScript 3 bytecode compiler for the AVM2.

As3c  is free software; you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation; either version 3 of the License, or
(at your option) any later version.

As3c is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>
*/

using System;
using System.Collections.Generic;
using System.Text;

namespace SwfLibrary.Utils
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
