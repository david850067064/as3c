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
    public class NamespaceInfo : IExternalizeable
    {
        public const byte Namespace = 0x08;
        public const byte PackageNamespace = 0x16;
        public const byte PackageInternalNs = 0x17;
        public const byte ProtectedNamespace = 0x18;
        public const byte ExplicitNamespace = 0x19;
        public const byte StaticProtectedNs = 0x1a;
        public const byte PrivateNs = 0x05;

        protected byte _kind;
        protected U30 _name;

        public byte Kind
        {
            get { return _kind; }
            set { _kind = value; }
        }

        public U30 Name
        {
            get { return _name; }
            set { _name = value; }
        }

        #region IExternalizeable Members

        public void ReadExternal(BinaryReader input)
        {
            _kind = input.ReadByte();
            _name = Primitives.ReadU30(input);
        }

        public void WriteExternal(BinaryWriter output)
        {
            output.Write(_kind);
            Primitives.WriteU30(output, _name);
        }

        #endregion
    }
}
