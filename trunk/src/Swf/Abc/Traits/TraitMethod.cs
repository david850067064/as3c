using System;
using System.Collections.Generic;
using System.Text;
using As3c.Swf.Types;
using System.IO;

namespace As3c.Swf.Abc.Traits
{
    public class TraitMethod : TraitBody
    {
        protected U30 _dispId;
        protected U30 _method;

        public U30 DispId
        {
            get { return _dispId; }
            set { _dispId = value; }
        }

        public U30 Method
        {
            get { return _method; }
            set { _method = value; }
        }

        public TraitMethod(TraitInfo parent) : base(parent) { }

        #region IExternalizeable Members

        public override void ReadExternal(BinaryReader input)
        {
            _dispId = Primitives.ReadU30(input);
            _method = Primitives.ReadU30(input);
        }

        public override void WriteExternal(BinaryWriter output)
        {
            Primitives.WriteU30(output, _dispId);
            Primitives.WriteU30(output, _method);
        }

        #endregion
    }
}
