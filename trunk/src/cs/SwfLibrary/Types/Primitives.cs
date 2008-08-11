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
    #region Primitive Types

    /**
     * Primitive types are all using a variable length besides
     * the S24. These types have a special behaviour when en-
     * and decoding them.
     */

    #region S24

    /**
     * The S24 is a three byte signed integer value.
     * Its size is _always_ three byte.
     */

    public struct S24 : IVariableLength
    {
        internal int _value;

        public int Value
        {
            get { return _value; }
            set
            {
                if ((value > 0x7fffff) || (value < -0x800000))
                {
                    throw new OverflowException();
                }

                _value = value;
            }
        }

        public uint Length { get { return 3; } }

        public static S24 operator +(S24 a, S24 b) { return (S24)((int)a + (int)b); }

        public static S24 operator -(S24 a, S24 b) { return (S24)((int)a - (int)b); }

        public static S24 operator *(S24 a, S24 b) { return (S24)((int)a * (int)b); }

        public static S24 operator %(S24 a, S24 b) { return (S24)((int)a % (int)b); }

        public static S24 operator /(S24 a, S24 b) { return (S24)((int)a / (int)b); }

        public static S24 operator &(S24 a, S24 b) { return (S24)((int)a & (int)b); }

        public static S24 operator |(S24 a, S24 b) { return (S24)((int)a | (int)b); }

        public static S24 operator ~(S24 a) { return (S24)((int)~a); }

        public static S24 operator ^(S24 a, S24 b) { return (S24)((int)a ^ (int)b); }

        public static S24 operator <<(S24 a, int b) { return (S24)((int)a << b); }

        public static S24 operator >>(S24 a, int b) { return (S24)((int)a >> b); }

        public static explicit operator int(S24 a) { return a.Value; }

        public static explicit operator S24(int a)
        {
            S24 result = new S24();
            result.Value = a;

            return result;
        }
    }

    #endregion

    #region U30

    /**
     * The U30 is a variable length unsigned integer with a
     * maximum length of 30bit. During en- or decoding 7bits
     * of each byte belong to the U30 while the 8th indicates
     * if another byte sould be read.
     */

    public struct U30 : IVariableLength
    {
        internal uint _length;
        internal uint _value;

        public uint Value
        {
            get { return _value; }
            set
            {
                if (value > 0x3fffffff)
                {
                    throw new OverflowException();
                }

                _value = value;
            }
        }

        public uint Length { get { return _length; } }

        public static U30 operator +(U30 a, U30 b) { return (U30)((uint)a + (uint)b); }

        public static U30 operator -(U30 a, U30 b) { return (U30)((uint)a - (uint)b); }

        public static U30 operator *(U30 a, U30 b) { return (U30)((uint)a * (uint)b); }

        public static U30 operator %(U30 a, U30 b) { return (U30)((uint)a % (uint)b); }

        public static U30 operator /(U30 a, U30 b) { return (U30)((uint)a / (uint)b); }

        public static U30 operator &(U30 a, U30 b) { return (U30)((uint)a & (uint)b); }

        public static U30 operator |(U30 a, U30 b) { return (U30)((uint)a | (uint)b); }

        public static U30 operator ~(U30 a) { return (U30)((uint)~a); }

        public static U30 operator ^(U30 a, U30 b) { return (U30)((uint)a ^ (uint)b); }

        public static U30 operator <<(U30 a, int b) { return (U30)((uint)a << b); }

        public static U30 operator >>(U30 a, int b) { return (U30)((uint)a >> b); }

        public static explicit operator uint(U30 a) { return a.Value; }

        public static explicit operator int(U30 a) { return (int)a.Value; }

        public static explicit operator U30(int a)
        {
            U30 result = new U30();
            result.Value = (uint)a;

            return result;
        }

        public static explicit operator U30(uint a)
        {
            U30 result = new U30();
            result.Value = a;

            return result;
        }
    }

    #endregion

    #region S32

    /**
     * Same as U30 but all 32 bits are used. Last bit indicates
     * the sign.
     */

    public struct S32 : IVariableLength
    {
        internal uint _length;
        internal int _value;

        public int Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public uint Length { get { return _length; } }

        public static S32 operator +(S32 a, S32 b) { return (S32)((int)a + (int)b); }

        public static S32 operator -(S32 a, S32 b) { return (S32)((int)a - (int)b); }

        public static S32 operator *(S32 a, S32 b) { return (S32)((int)a * (int)b); }

        public static S32 operator %(S32 a, S32 b) { return (S32)((int)a % (int)b); }

        public static S32 operator /(S32 a, S32 b) { return (S32)((int)a / (int)b); }

        public static S32 operator &(S32 a, S32 b) { return (S32)((int)a & (int)b); }

        public static S32 operator |(S32 a, S32 b) { return (S32)((int)a | (int)b); }

        public static S32 operator ~(S32 a) { return (S32)((int)~a); }

        public static S32 operator ^(S32 a, S32 b) { return (S32)((int)a ^ (int)b); }

        public static S32 operator <<(S32 a, int b) { return (S32)((int)a << b); }

        public static S32 operator >>(S32 a, int b) { return (S32)((int)a >> b); }

        public static explicit operator int(S32 a) { return a.Value; }

        public static explicit operator S32(int a)
        {
            S32 result = new S32();
            result.Value = a;

            return result;
        }
    }

    #endregion

    #region U32

    /**
     * Same as S32 but unsigned.
     */

    public struct U32 : IVariableLength
    {
        internal uint _length;

        internal uint _value;

        public uint Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public uint Length { get { return _length; } }

        public static U32 operator +(U32 a, U32 b) { return (U32)((uint)a + (uint)b); }

        public static U32 operator -(U32 a, U32 b) { return (U32)((uint)a - (uint)b); }

        public static U32 operator *(U32 a, U32 b) { return (U32)((uint)a * (uint)b); }

        public static U32 operator %(U32 a, U32 b) { return (U32)((uint)a % (uint)b); }

        public static U32 operator /(U32 a, U32 b) { return (U32)((uint)a / (uint)b); }

        public static U32 operator &(U32 a, U32 b) { return (U32)((uint)a & (uint)b); }

        public static U32 operator |(U32 a, U32 b) { return (U32)((uint)a | (uint)b); }

        public static U32 operator ~(U32 a) { return (U32)((uint)~a); }

        public static U32 operator ^(U32 a, U32 b) { return (U32)((uint)a ^ (uint)b); }

        public static U32 operator <<(U32 a, int b) { return (U32)((uint)a << b); }

        public static U32 operator >>(U32 a, int b) { return (U32)((uint)a >> b); }

        public static explicit operator uint(U32 a)
        {
            return a.Value;
        }

        public static explicit operator U32(uint a)
        {
            U32 result = new U32();
            result.Value = a;

            return result;
        }
    }

    #endregion

    #endregion

    public class Primitives
    {
        #region Bitstream Members
        
        private static uint _bitBuffer = 0;
        private static uint _bitPos = 0;
        private static uint _p = 0;

        public static void ResetBuffer()
        {
            _bitBuffer = _bitPos = _p = 0;
        }

        public static void FlushBits(BinaryWriter writer)
        {
            if (0 == _bitPos)
            {
                return;
            }

            writer.Write((byte)_bitBuffer);

            ResetBuffer();
        }

        public static uint BufferPosition
        {
            get { return _p; }
        }

        public static uint ReadUB(BinaryReader reader, uint bitCount)
        {
            if (0 == bitCount)
            {
                return 0;
            }

            uint result = 0;
            uint bitsLeft = bitCount;

            if (0 == _bitPos)
            {
                _bitBuffer = reader.ReadByte();
                _bitPos = 8;
                ++_p;
            }

            while (true)
            {
                int shift = (int)(bitsLeft - _bitPos);

                if (shift > 0)
                {
                    result |= _bitBuffer << shift;
                    bitsLeft -= _bitPos;

                    _bitBuffer = reader.ReadByte();
                    _bitPos = 8;
                    ++_p;
                }
                else
                {
                    result |= _bitBuffer >> (-shift);
                    _bitPos -= bitsLeft;
                    _bitBuffer &= (uint)(0xff >> (int)(8 - _bitPos));

                    return (uint)result;
                }
            }
        }

        public static uint ReadUB(byte[] buffer, uint start, uint bitCount)
        {
            if (0 == bitCount)
            {
                return 0;
            }

            uint result = 0;
            uint bitsLeft = bitCount;

            if (0 == _bitPos)
            {
                _bitBuffer = buffer[start + _p];
                _bitPos = 8;
                ++_p;
            }

            while (true)
            {
                int shift = (int)(bitsLeft - _bitPos);

                if (shift > 0)
                {
                    result |= _bitBuffer << shift;
                    bitsLeft -= _bitPos;

                    _bitBuffer = buffer[start + _p];
                    _bitPos = 8;
                    ++_p;
                }
                else
                {
                    result |= _bitBuffer >> (-shift);
                    _bitPos -= bitsLeft;
                    _bitBuffer &= (uint)(0xff >> (int)(8 - _bitPos));

                    return (uint)result;
                }
            }
        }

        public static void WriteUB(BinaryWriter writer, uint value, uint bitCount)
        {
            if (0 == bitCount)
            {
                return;
            }

            if (_bitPos == 0)
            {
                _bitPos = 8;
            }

            uint bitsLeft = bitCount;

            while (bitsLeft > 0)
            {
                while (_bitPos > 0 && bitsLeft > 0)
                {
                    if (GetBit(bitsLeft, value))
                    {
                        _bitBuffer = SetBit(_bitPos, _bitBuffer);
                    }

                    --bitsLeft;
                    --_bitPos;
                }

                if (0 == _bitPos)
                {
                    writer.Write((byte)_bitBuffer);

                    _bitBuffer = 0;
                
                    if ( bitsLeft > 0 )
                    {
                        _bitPos = 8;
                    }
                }
            }
        }

        public static void WriteUB(byte[] buffer, uint start, uint value, uint bitCount)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public static int ReadSB(BinaryReader reader, uint bitCount)
        {
            if (bitCount > int.MaxValue)
            {
                /* TODO fix this */

                Console.WriteLine("[-] Can not read {0} bits.", bitCount);

                throw new OverflowException();
            }

            if (0 == bitCount) return 0;

            int result = (int)ReadUB(reader, bitCount);

            if ((result & ((uint)1 << (int)(bitCount - 1))) != 0)
            {
                /*TODO checked if this is correct*/
                result = -result;
            }

            return result;
        }

        public static int ReadSB(byte[] buffer, uint start, uint bitCount)
        {
            if (bitCount > int.MaxValue)
            {
                Console.WriteLine("[-] Can not read {0} bits safely.", bitCount);
            }

            if (0 == bitCount) return 0;

            int result = (int)ReadUB(buffer, start, bitCount);

            if ((result & ((uint)1 << (int)(bitCount - 1))) != 0)
            {
                /*TODO checked if this is correct*/
                result = -result;
            }

            return result;
        }

        public static void WriteSB(BinaryWriter writer, int value, uint bitCount)
        {
            uint uvalue = (uint)value & 0x7fffffff;

            if (value < 0)
            {
                /*TODO checked if this is correct*/
                uvalue |= (uint)(1U << (int)(bitCount - 1U));
            }

            WriteUB(writer, uvalue, bitCount);
        }

        public static void WriteSB(byte[] buffer, uint start, int value, uint bitCount)
        {
            throw new Exception("Method not implemented.");
        }
        #endregion

        #region U30

        public static U30 ReadU30(BinaryReader reader)
        {
            U30 result = new U30();

            result._value = reader.ReadByte();
            result._length = 1;

            if (0 == (result._value & 0x00000080))
            {
                return result;
            }

            result._value = (uint)(result._value & 0x0000007f) | (uint)reader.ReadByte() << 7;
            ++result._length;

            if (0 == (result._value & 0x00004000))
            {
                return result;
            }

            result._value = (uint)(result._value & 0x00003fff) | (uint)reader.ReadByte() << 14;
            ++result._length;

            if (0 == (result._value & 0x00200000))
            {
                return result;
            }

            result._value = (uint)(result._value & 0x001fffff) | (uint)reader.ReadByte() << 21;
            ++result._length;

            if (0 == (result._value & 0x10000000))
            {
                return result;
            }

            result._value = (uint)(result._value & 0x0fffffff) | (uint)reader.ReadByte() << 28;
            ++result._length;

            return result;
        }

        public static U30 ReadU30(byte[] buffer, uint start)
        {
            U30 result;

            result._value = buffer[start];
            result._length = 1;

            if (0 == (result._value & 0x00000080))
            {
                return result;
            }

            result._value = (uint)(result._value & 0x0000007f) | (uint)buffer[start + 1] << 7;
            ++result._length;

            if (0 == (result._value & 0x00004000))
            {
                return result;
            }

            result._value = (uint)(result._value & 0x00003fff) | (uint)buffer[start + 2] << 14;
            ++result._length;

            if (0 == (result._value & 0x00200000))
            {
                return result;
            }

            result._value = (uint)(result._value & 0x001fffff) | (uint)buffer[start + 3] << 21;
            ++result._length;

            if (0 == (result._value & 0x10000000))
            {
                return result;
            }

            result._value = (uint)(result._value & 0x0fffffff) | (uint)buffer[start + 4] << 28;
            ++result._length;

            return result;
        }

        public static byte WriteU30(BinaryWriter writer, U30 value)
        {
            if (0 == value._value)
            {
                writer.Write((byte)0);
                return 1;
            }

            uint b0 = value._value;
            byte b1;
            byte p = 0;

            while (b0 != 0)
            {
                b1 = (byte)(b0 & 0x7f);
                b0 >>= 7;

                if (b0 != 0)
                {
                    b1 |= 0x80;
                }

                writer.Write(b1);
                ++p;
            }

            return p;
        }

        public static byte WriteU30(BinaryWriter writer, uint value)
        {
            return WriteU30(writer, (U30)value);
        }

        public static byte WriteU30(byte[] buffer, uint start, U30 value)
        {
            if (0 == value._value)
            {
                buffer[start] = 0;
                return 1;
            }

            uint b0 = value._value;
            byte b1;
            byte p = 0;

            while (b0 != 0)
            {
                b1 = (byte)(b0 & 0x7f);
                b0 >>= 7;

                if (b0 != 0)
                {
                    b1 |= 0x80;
                }

                buffer[start+p] = b1;
                ++p;
            }

            return p;
        }

        public static byte WriteU30(byte[] buffer, uint start, uint value)
        {
            return WriteU30(buffer, start, (U30)value);
        }

        #endregion

        #region S32

        public static S32 ReadS32(BinaryReader reader)
        {
            S32 result;

            result._value = reader.ReadByte();
            result._length = 1;

            if (0 == (result._value & 0x00000080))
            {
                return result;
            }

            result._value = (result._value & 0x0000007f) | (reader.ReadByte() << 7);
            ++result._length;

            if (0 == (result._value & 0x00004000))
            {
                return result;
            }

            result._value = (result._value & 0x00003fff) | (reader.ReadByte() << 14);
            ++result._length;

            if (0 == (result._value & 0x00200000))
            {
                return result;
            }

            result._value = (result._value & 0x001fffff) | (reader.ReadByte() << 21);
            ++result._length;

            if (0 == (result._value & 0x10000000))
            {
                return result;
            }

            result._value = (result._value & 0x0fffffff) | (reader.ReadByte() << 28);
            ++result._length;

            return result;
        }

        public static S32 ReadS32(byte[] buffer, uint start)
        {
            S32 result;

            result._value = buffer[start];
            result._length = 1;

            if (0 == (result._value & 0x00000080))
            {
                return result;
            }

            result._value = (result._value & 0x0000007f) | (buffer[start + 1] << 7);
            ++result._length;

            if (0 == (result._value & 0x00004000))
            {
                return result;
            }

            result._value = (result._value & 0x00003fff) | (buffer[start + 2] << 14);
            ++result._length;

            if (0 == (result._value & 0x00200000))
            {
                return result;
            }

            result._value = (result._value & 0x001fffff) | (buffer[start + 3] << 21);
            ++result._length;

            if (0 == (result._value & 0x10000000))
            {
                return result;
            }

            result._value = (result._value & 0x0fffffff) | (buffer[start + 4] << 28);
            ++result._length;

            return result;
        }

        public static void WriteS32(BinaryWriter writer, S32 value)
        {
            if (0 == value._value)
            {
                writer.Write((byte)0);
                return;
            }

            uint b0 = (uint)value._value;
            byte b1;

            while (b0 != 0)
            {
                b1 = (byte)(b0 & 0x7f);
                b0 >>= 7;

                if (b0 != 0)
                {
                    b1 |= 0x80;
                }

                writer.Write(b1);
            }
        }

        public static void WriteS32(BinaryWriter writer, int value)
        {
            WriteS32(writer, (S32)value);
        }

        #endregion

        #region U32

        public static U32 ReadU32(BinaryReader reader)
        {
            return S32ToU32(ReadS32(reader));
        }

        public static U32 ReadU32(byte[] buffer, uint start)
        {
            return S32ToU32(ReadS32(buffer, start));
        }

        public static void WriteU32(BinaryWriter writer, U32 value)
        {
            WriteS32(writer, U32ToS32(value));
        }

        public static void WriteU32(BinaryWriter writer, uint value)
        {
            WriteS32(writer, U32ToS32((U32)value));
        }

        #endregion

        #region S24

        public static S24 ReadS24(BinaryReader reader)
        {
            S24 result = new S24();

            result._value = ((reader.ReadByte()) | (reader.ReadByte() << 8) | (reader.ReadByte() << 16));

            SetS24Sign(ref result);

            return result;
        }

        public static S24 ReadS24(byte[] buffer, uint start)
        {
            S24 result = new S24();

            result._value = (buffer[start] | (buffer[start + 1] << 8) | (buffer[start + 2] << 16));

            SetS24Sign(ref result);

            return result;
        }

        public static void WriteS24(BinaryWriter writer, int value)
        {
            writer.Write((byte)(value & 0xff));
            writer.Write((byte)((value >> 8) & 0xff));

            byte tmp = (byte)((value >> 16) & 0xff);

            if (value < 0) tmp |= 0x80;

            writer.Write((byte)tmp);
        }

        public static void WriteS24(BinaryWriter writer, S24 value)
        {
            WriteS24(writer, (int)value);
        }

        #endregion

        private static void SetS24Sign(ref S24 result)
        {
            if (0 != (0x800000 & result._value))
            {
                // Is there maybe a better way to do this?
                uint buf = (uint)result._value;
                buf |= 0xff000000;

                result._value = (int)buf;

                /*if (result._value < 0)
                {
                    result._value++;
                }*/
            }
        }

        private static U32 S32ToU32(S32 buffer)
        {
            U32 result;

            result._length = buffer._length;
            result._value = (uint)buffer._value;

            return result;
        }

        private static S32 U32ToS32(U32 buffer)
        {
            S32 result;

            result._length = buffer._length;
            result._value = (int)buffer._value;

            return result;
        }

        private static bool GetBit(uint bit, uint value)
        {
            return (value & (1U << (int)(bit - 1))) != 0;
        }

        private static uint SetBit(uint bit, uint value)
        {
            return value | (uint)(1U << (int)(bit - 1));
        }
    }
}
