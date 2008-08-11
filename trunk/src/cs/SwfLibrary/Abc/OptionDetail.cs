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

namespace SwfLibrary.Abc
{
    public class OptionDetail : IExternalizeable
    {
        public const int KInt = 0x03;
        public const int KUInt = 0x04;
        public const int KDouble = 0x06;
        public const int KUtf8 = 0x01;
        public const int KTrue = 0x0b;
        public const int KFalse = 0x0a;
        public const int KNull = 0x0c;
        public const int KUndefined = 0x00;
        public const int KNamespace = 0x08;
        public const int KPackageNamespace = 0x16;
        public const int KPackageInternalNs = 0x17;
        public const int KProtectedNamespace = 0x18;
        public const int KExplicitNamespace = 0x19;
        public const int KStaticProtectedNs = 0x1a;
        public const int KPrivateNs = 0x05;
        
        protected U30 _val;
        protected byte _kind;

        public U30 Value
        {
            get { return _val; }
            set { _val = value; }
        }

        public byte Kind
        {
            get { return _kind; }
            set { _kind = value; }
        }

        #region IExternalizeable Members

        public void ReadExternal(BinaryReader input)
        {
            _val = Primitives.ReadU30(input);
            _kind = input.ReadByte();
        }

        public void WriteExternal(BinaryWriter output)
        {
            Primitives.WriteU30(output, _val);
            output.Write(_kind);
        }

        #endregion
    }
}
