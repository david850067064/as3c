using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using As3c.Swf.Utils;

namespace As3c.Swf.Types
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
