using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using As3c.Swf.Types;

namespace As3c.Swf.Abc.Traits
{
    public class TraitSlot : TraitBody
    {
        protected U30 _slotId;
        protected U30 _typeName;
        protected bool _hasKind;
        protected U30 _vIndex;
        private byte _vKind;

        public U30 SlotId
        {
            get { return _slotId; }
            set { _slotId = value; }
        }

        public U30 TypeName
        {
            get { return _typeName; }
            set { _typeName = value; }
        }

        public U30 VIndex
        {
            get { return _vIndex; }
            set
            {
                _hasKind = (0 == value.Value);
                _vIndex = value;
            }
        }

        public byte VKind
        {
            get { return _vKind; }
            set { _vKind = value; }
        }

        public TraitSlot(TraitInfo parent) : base(parent) { }

        #region IExternalizeable Members

        public override void ReadExternal(BinaryReader input)
        {
            _slotId = Primitives.ReadU30(input);
            _typeName = Primitives.ReadU30(input);
            _vIndex = Primitives.ReadU30(input);

            if (0 != _vIndex.Value)
            {
                _vKind = input.ReadByte();
                _hasKind = true;
            }
        }

        public override void WriteExternal(BinaryWriter output)
        {
            Primitives.WriteU30(output, _slotId);
            Primitives.WriteU30(output, _typeName);
            Primitives.WriteU30(output, _vIndex);

            if (0 != _vIndex.Value || _hasKind)
            {
                output.Write(_vKind);
            }
        }

        #endregion
    }
}
