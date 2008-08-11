/*
Copyright(C) 2007 Joa Ebert

As3c is an ActionScript 3 bytecode compiler for the AVM2.

As3c  is free software; you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation; either version 3 of the License, or
(at your option) any later version.

As3c is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>
*/

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using SwfLibrary.Types;

namespace SwfLibrary.Abc.Traits
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
