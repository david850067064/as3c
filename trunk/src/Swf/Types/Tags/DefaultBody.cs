using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace As3c.Swf.Types.Tags
{
    public class DefaultBody : TagBody
    {
        protected byte[] _body;

        public DefaultBody(Tag parent) : base(parent) { }

        #region IExternalizeable Members

        public override void ReadExternal(BinaryReader input)
        {
            _body = input.ReadBytes(_parent.Header.Length);
        }

        public override void WriteExternal(BinaryWriter output)
        {
            output.Write(_body);
        }

        #endregion
    }
}
