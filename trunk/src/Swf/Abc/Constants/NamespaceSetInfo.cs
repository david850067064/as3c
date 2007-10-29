using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using As3c.Swf.Utils;
using As3c.Swf.Types;
using System.Collections;
using As3c.Swf.Exceptions;

namespace As3c.Swf.Abc.Constants
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
