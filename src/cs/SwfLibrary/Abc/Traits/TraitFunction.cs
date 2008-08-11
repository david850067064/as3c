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
using SwfLibrary.Types;
using System.IO;

namespace SwfLibrary.Abc.Traits
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
