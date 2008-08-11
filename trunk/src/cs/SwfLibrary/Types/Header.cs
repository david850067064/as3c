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
    public class Header : IExternalizeable
    {
        protected byte[] _signature;
        protected byte _version;
        protected uint _fileLength;

        protected bool _compressed;

        public byte[] Signature
        {
            get
            {
                return _signature;
            }
            set
            {
                if ((_signature[0] != 'C' && _signature[0] != 'F') || _signature[1] != 'W' || _signature[2] != 'S')
                {
                    throw new Exception("Invalid signature. Must be either FWS or CWS");
                }

                _signature = value;

                _compressed = _signature[0] == 'C';
            }
        }

        public byte Version
        {
            get { return _version; }
            set { _version = value; }
        }

        public uint FileLength
        {
            get { return _fileLength; }
            set { _fileLength = value; }
        }

        public bool IsCompressed
        {
            get
            {
                return _compressed;
            }
            set
            {
                _signature[0] = (value) ? (byte)'C' : (byte)'F';
                _compressed = value;
            }
        }

        #region IExternalizeable Members

        public void ReadExternal(BinaryReader input)
        {
            _signature = input.ReadBytes(3);
            _version = input.ReadByte();
            _fileLength = input.ReadUInt32();

            _compressed = _signature[0] == 'C';
        }

        public void WriteExternal(BinaryWriter output)
        {
            output.Write(_signature);
            output.Write(_version);
            output.Write(_fileLength);

            System.Diagnostics.Debug.WriteLine("[-] Careful. Do you really want to write the header directly?");
        }

        #endregion
    }
}
