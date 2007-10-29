using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using As3c.Swf.Utils;
using As3c.Swf.Types;

namespace As3c.Swf.Abc.Constants
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
