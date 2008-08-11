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
using SwfLibrary.Types;

namespace SwfLibrary.Abc.Constants
{
    public class MultinameInfo : IExternalizeable
    {
        public const byte QName = 0x07;
        public const byte QNameA = 0x0d;
        public const byte RTQName = 0x0f;
        public const byte RTQNameA = 0x10;
        public const byte RTQNameL = 0x11;
        public const byte RTQNameLA = 0x12;
        public const byte Multiname_ = 0x09;
        public const byte MultinameA = 0x0e;
        public const byte MultinameL = 0x1b;
        public const byte MultinameLA = 0x1c;

        protected byte _kind;
        protected U30[] _data;

        public byte Kind
        {
            get { return _kind; }
            set { _kind = value; }
        }

        public U30[] Data
        {
            get { return _data; }
            set { _data = value; }
        }

        #region IExternalizeable Members

        public void ReadExternal(BinaryReader input)
        {
            _kind = input.ReadByte();

            switch (_kind)
            {
                case QName:
                case QNameA:
                case Multiname_:
                case MultinameA:
                    _data = new U30[] { Primitives.ReadU30(input), Primitives.ReadU30(input) };
                    break;

                case RTQNameL:
                case RTQNameLA:
                    break;

                case RTQName:
                case RTQNameA:
                case MultinameL:
                case MultinameLA:
                    _data = new U30[] { Primitives.ReadU30(input) };
                    break;
            }
        }

        public void WriteExternal(BinaryWriter output)
        {
            output.Write(_kind);

            switch (_kind)
            {
                case QName:
                case QNameA:
                case Multiname_:
                case MultinameA:
                    Primitives.WriteU30(output, _data[0]);
                    Primitives.WriteU30(output, _data[1]);
                    break;

                case RTQNameL:
                case RTQNameLA:
                    break;

                case RTQName:
                case RTQNameA:
                case MultinameL:
                case MultinameLA:
                    Primitives.WriteU30(output, _data[0]);
                    break;
            }
        }

        #endregion
    }
}
