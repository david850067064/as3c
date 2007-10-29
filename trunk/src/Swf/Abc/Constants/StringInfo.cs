using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using As3c.Swf.Utils;
using As3c.Swf.Types;

namespace As3c.Swf.Abc.Constants
{
    public class StringInfo : IExternalizeable
    {
        protected byte[] _utf8;

        public byte[] Data
        {
            get { return _utf8; }
            set { _utf8 = value; }
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
