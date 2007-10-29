using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using As3c.Swf.Exceptions;
using As3c.Swf.Types;
using As3c.Swf.Utils;
using System.Collections;

namespace As3c.Swf.Abc
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
