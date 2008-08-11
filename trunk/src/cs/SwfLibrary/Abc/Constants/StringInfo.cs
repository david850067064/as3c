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
    public class StringInfo : IExternalizeable
    {
        protected byte[] _utf8;

        public byte[] Data
        {
            get { return _utf8; }
            set { _utf8 = value; }
        }

        public StringInfo()
        {
            _utf8 = new byte[0];
        }

        #region IExternalizeable Members

        public void ReadExternal(BinaryReader input)
        {
            uint n = Primitives.ReadU30(input).Value;
            _utf8 = new byte[n];

            for (uint i = 0; i < n; ++i)
                _utf8[i] = input.ReadByte();
        }

        public void WriteExternal(BinaryWriter output)
        {
            Primitives.WriteU30(output, (uint)_utf8.Length);
            output.Write(_utf8);
        }

        #endregion

        public override string ToString()
        {
            string result = "";

            for (int i = 0; i < _utf8.Length; ++i)
            {
                switch (_utf8[i])
                {
                    case (byte)'"':
                        result += "\\\"";
                        break;

                    case (byte)'\\':
                        result += "\\\\";
                        break;

                    default:
                        result += (char)_utf8[i];
                        break;
                }
            }

            return result;
        }
    }
}
