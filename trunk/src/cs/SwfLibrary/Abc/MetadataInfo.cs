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

using SwfLibrary.Exceptions;
using SwfLibrary.Types;
using SwfLibrary.Utils;
using System.Collections;

namespace SwfLibrary.Abc
{
    public class MetadataInfo : IExternalizeable
    {
        public struct ItemInfo
        {
            public U30 key;
            public U30 value;
        }

        protected U30 _name;
        protected ArrayList _items;

        public U30 Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public ArrayList Items
        {
            get { return _items; }
            set { _items = value; }
        }

        #region IExternalizeable Members

        public void ReadExternal(BinaryReader input)
        {
            _name = Primitives.ReadU30(input);

            if (0 == _name.Value)
                throw new VerifyException("Name must not be 0.");

            uint n = Primitives.ReadU30(input).Value;

            _items = new ArrayList(Capacity.Max(n));

            for (uint i = 0; i < n; ++i)
            {
                ItemInfo itemInfo = new ItemInfo();

                itemInfo.key = Primitives.ReadU30(input);
                itemInfo.value = Primitives.ReadU30(input);

                _items.Add(itemInfo);
            }
        }

        public void WriteExternal(BinaryWriter output)
        {
            Primitives.WriteU30(output, _name);

            int n = _items.Count;

            Primitives.WriteU30(output, (uint)n);

            for (int i = 0; i < n; ++i)
            {
                ItemInfo itemInfo = (ItemInfo)_items[i];

                Primitives.WriteU30(output, itemInfo.key);
                Primitives.WriteU30(output, itemInfo.value);
            }
        }

        #endregion
    }
}
