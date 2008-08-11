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
using System.Collections;
using SwfLibrary.Exceptions;

namespace SwfLibrary.Abc.Constants
{
    public class NamespaceSetInfo : IExternalizeable
    {
        protected ArrayList _ns;

        public ArrayList NamespaceSet
        {
            get { return _ns; }
            set { _ns = value; }
        }

        #region IExternalizeable Members

        public void ReadExternal(BinaryReader input)
        {
            uint count = Primitives.ReadU30(input).Value;

            _ns = new ArrayList(Capacity.Max(count));

            for (uint i = 0; i < count; ++i)
            {
                U30 ns = Primitives.ReadU30(input);
                _ns.Add(ns);

                if (0 == ns.Value)
                {
                    throw new VerifyException("Namespace must not be 0.");
                }
            }
        }

        public void WriteExternal(BinaryWriter output)
        {
            int n = _ns.Count;
            Primitives.WriteU30(output, (uint)n);

            for (int i = 0; i < n; ++i)
                Primitives.WriteU30(output, (U30)_ns[i]);
        }

        #endregion
    }
}
