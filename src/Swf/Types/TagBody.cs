using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using As3c.Swf.Utils;

namespace As3c.Swf.Types
{
    public class TagBody : IExternalizeable
    {
        protected Tag _parent;

        public TagBody(Tag parent)
        {
            _parent = parent;
        }

        #region IExternalizeable Members

        public virtual void ReadExternal(BinaryReader input)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public virtual void WriteExternal(BinaryWriter output)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}
