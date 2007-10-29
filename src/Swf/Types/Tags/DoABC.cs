using System;
using System.Collections.Generic;
using System.Text;

using As3c.Swf.Abc;
using System.IO;

namespace As3c.Swf.Types.Tags
{
    class DoABC : TagBody 
    {
        protected uint _flags;
        protected Abc46 _abc;

        public DoABC(Tag parent) : base(parent) { }

        #region IExternalizeable Members

        public override void ReadExternal(BinaryReader input)
        {
            _flags = input.ReadUInt32();

            _abc = new Abc46();
            _abc.Length = (uint)(_parent.Header.Length - 4);
            _abc.ReadExternal(input);
        }

        public override void WriteExternal(BinaryWriter output)
        {
            output.Write(_flags);

            _abc.WriteExternal(output);
        }

        #endregion

        public Abc46 Abc
        {
            get { return _abc; }
            set { _abc = value; }
        }
    }
}
