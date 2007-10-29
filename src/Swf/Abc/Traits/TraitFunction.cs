using System;
using System.Collections.Generic;
using System.Text;
using As3c.Swf.Types;
using System.IO;

namespace As3c.Swf.Abc.Traits
{
    public class TraitFunction : TraitBody
    {
        protected U30 _slotId;
        protected U30 _function;

        public U30 SlotId
        {
            get { return _slotId; }
            set { _slotId = value; }
        }

        public U30 Function
        {
            get { return _function; }
            set { _function = value; }
        }

        public TraitFunction(TraitInfo parent) : base(parent) { }

        #region IExternalizeable Members

        public override void ReadExternal(BinaryReader input)
        {
            _slotId = Primitives.ReadU30(input);
            _function = Primitives.ReadU30(input);
        }

        public override void WriteExternal(BinaryWriter output)
        {
            Primitives.WriteU30(output, _slotId);
            Primitives.WriteU30(output, _function);
        }

        #endregion
    }
}
