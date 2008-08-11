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
using SwfLibrary.Utils;
using System.IO;

namespace SwfLibrary.Abc.Traits
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
