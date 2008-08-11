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

using SwfLibrary.Utils;
using SwfLibrary.Types;

namespace SwfLibrary.Abc
{
    public class ExceptionInfo : IExternalizeable
    {
        protected U30 _from;
        protected U30 _to;
        protected U30 _target;
        protected U30 _execType;
        protected U30 _varName;

        public U30 From
        {
            get { return _from; }
            set { _from = value; }
        }

        public U30 To
        {
            get { return _to; }
            set { _to = value; }
        }

        public U30 Target
        {
            get { return _target; }
            set { _target = value; }
        }

        public U30 ExecutionType
        {
            get { return _execType; }
            set { _execType = value; }
        }

        public U30 VariableName
        {
            get { return _varName; }
            set { _varName = value; }
        }

        #region IExternalizeable Members

        public void ReadExternal(BinaryReader input)
        {
            _from = Primitives.ReadU30(input);
            _to = Primitives.ReadU30(input);
            _target = Primitives.ReadU30(input);
            _execType = Primitives.ReadU30(input);
            _varName = Primitives.ReadU30(input);
        }

        public void WriteExternal(BinaryWriter output)
        {
            Primitives.WriteU30(output, _from);
            Primitives.WriteU30(output, _to);
            Primitives.WriteU30(output, _target);
            Primitives.WriteU30(output, _execType);
            Primitives.WriteU30(output, _varName);
        }

        #endregion
    }
}
