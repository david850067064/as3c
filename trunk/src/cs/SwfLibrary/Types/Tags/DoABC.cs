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

using SwfLibrary.Abc;
using System.IO;

namespace SwfLibrary.Types.Tags
{
    public class DoABC : TagBody 
    {
        protected uint _flags;
        protected Abc46 _abc;

        public DoABC(Tag parent) : base(parent) { }

        #region IExternalizeable Members

        public override void ReadExternal(BinaryReader input)
        {
            _flags = input.ReadUInt32();

            _abc = new Abc46();
            _abc.Length = (uint)(_parent.Header.Length - 4);
            _abc.ReadExternal(input);
        }

        public override void WriteExternal(BinaryWriter output)
        {
            output.Write(_flags);

            _abc.WriteExternal(output);
        }

        #endregion

        public Abc46 Abc
        {
            get { return _abc; }
            set { _abc = value; }
        }
    }
}
