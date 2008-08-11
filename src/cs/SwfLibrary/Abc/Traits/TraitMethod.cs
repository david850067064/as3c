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
