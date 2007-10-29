using System;
using System.Collections.Generic;
using System.Text;
using As3c.Swf.Utils;
using System.IO;

namespace As3c.Swf.Abc.Traits
{
    public class TraitBody : IExternalizeable
    {
        protected TraitInfo _parent;

        public TraitBody(TraitInfo parent)
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
