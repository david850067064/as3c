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
using System.IO;

using SwfLibrary.Utils;

namespace SwfLibrary.Types
{
    public class RECT : IExternalizeable
    {
        public enum StringFormat
        {
            TWIPS,
            Pixel
        };

        protected int _minX;
        protected int _maxX;
        protected int _minY;
        protected int _maxY;

        public int MinX
        {
            get { return _minX; }
            set { _minX = value; }
        }

        public int MaxX
        {
            get { return _maxX; }
            set { _maxX = value; }
        }

        public int MinY
        {
            get { return _minY; }
            set { _minY = value; }
        }

        public int MaxY
        {
            get { return _maxY; }
            set { _maxY = value; }
        }

        public RECT(int minX, int maxX, int minY, int maxY)
        {
            Init(minX, maxX, minY, maxY);
        }

        public RECT()
        {
            Init(0, 0, 0, 0);
        }

        protected void Init(int minX, int maxX, int minY, int maxY)
        {
            _minX = minX;
            _maxX = maxX;
            _minY = minY;
            _maxY = maxY;
        }

        #region IExternalizeable Members

        public virtual void ReadExternal(BinaryReader input)
        {
            Primitives.ResetBuffer();

            uint bitCount = Primitives.ReadUB(input, 5);

            _minX = Primitives.ReadSB(input, bitCount);
            _maxX = Primitives.ReadSB(input, bitCount);
            _minY = Primitives.ReadSB(input, bitCount);
            _maxY = Primitives.ReadSB(input, bitCount);
        }

        public virtual void WriteExternal(BinaryWriter output)
        {
            Primitives.ResetBuffer();

            uint b0 = 0, b1 = 0, b2 = 0, b3 = 0;

            // Find only if needed
            b0 = BitUtil.LengthSB(_maxX);
            if (b0 < 32)
            {
                b1 = BitUtil.LengthSB(_maxY);
                if (b1 < 32)
                {
                    b2 = BitUtil.LengthSB(_minX);
                    if (b2 < 32)
                    {
                        b3 = BitUtil.LengthSB(_minY);
                    }
                }
            }
            
            // Find maximum
            uint bitCount = Math.Max(Math.Max(b0,b1),Math.Max(b2,b3));

            // Make sure we did not do anything wrong...
            if (bitCount > 0x20)
            {
                throw new Exception("RECT overflow error.");
            }

            // Write it to the stream
            Primitives.WriteUB(output, bitCount, 5);
            Primitives.WriteSB(output, _minX, bitCount);
            Primitives.WriteSB(output, _maxX, bitCount);
            Primitives.WriteSB(output, _minY, bitCount);
            Primitives.WriteSB(output, _maxY, bitCount);

            Primitives.FlushBits(output);
        }

        #endregion

        public override string ToString()
        {
            return "[RECT MinX: " + _minX + ", MaxX: " + _maxX + ", MinY: " + _minY + ", MaxY: " + _maxY + "]";
        }

        public string ToString(StringFormat format)
        {
            if (StringFormat.TWIPS == format)
            {
                return ToString();
            }

            return "[RECT MinX: " + (_minX / 20) + ", MaxX: " + (_maxX / 20) + ", MinY: " + (_minY / 20) + ", MaxY: " + (_maxY / 20) + "]";
        }
    }
}
