using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using As3c.Swf.Utils;
using As3c.Swf.Types;
using As3c.Swf.Exceptions;
using System.Collections;

namespace As3c.Swf.Abc
{
    public class InstanceInfo : IExternalizeable, IHasTraits
    {
        public const int ClassSealed = 0x01;
        public const int ClassFinal = 0x02;
        public const int ClassInterface = 0x04;
        public const int ClassProtectedNs = 0x08;

        protected U30 _name;
        protected U30 _superName;
        protected byte _flags;
        protected U30 _protectedNs;
        protected ArrayList _interface;
        protected U30 _iinit;
        protected ArrayList _traits;

        public U30 Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public U30 SuperName
        {
            get { return _superName; }
            set { _superName = value; }
        }

        public byte Flags
        {
            get { return _flags; }
            set { _flags = value; }
        }

        public U30 ProtectedNamespace
        {
            get { return _protectedNs; }
            set { _protectedNs = value; }
        }

        public ArrayList Interfaces
        {
            get { return _interface; }
            set { _interface = value; }
        }

        public U30 IInit
        {
            get { return _iinit; }
            set { _iinit = value; }
        }

        public ArrayList Traits
        {
            get { return _traits; }
            set { _traits = value; }
        }

        #region IExternalizeable Members

        public void ReadExternal(BinaryReader input)
        {
            _name = Primitives.ReadU30(input);

            _superName = Primitives.ReadU30(input);

            _flags = input.ReadByte();

            if (ClassProtectedNs == (_flags & ClassProtectedNs))
            {
                _protectedNs = Primitives.ReadU30(input);
            }

            uint n = Primitives.ReadU30(input).Value;

            _interface = new ArrayList(Capacity.Max(n));

            for (uint i = 0; i < n; ++i)
            {
                U30 iid = Primitives.ReadU30(input);

                if (0 == iid.Value)
                {
                    throw new VerifyException("Interface must not be 0.");
                }

                _interface.Add(iid);
            }

            _iinit = Primitives.ReadU30(input);

            n = Primitives.ReadU30(input).Value;

            _traits = new ArrayList(Capacity.Max(n));

            for (uint i = 0; i < n; ++i)
            {
                TraitInfo ti = new TraitInfo();
                ti.ReadExternal(input);

                _traits.Add(ti);
                
            }
        }

        public void WriteExternal(BinaryWriter output)
        {
            Primitives.WriteU30(output, _name);
            Primitives.WriteU30(output, _superName);

            output.Write(_flags);

            if (ClassProtectedNs == (_flags & ClassProtectedNs))
            {
                Primitives.WriteU30(output, _protectedNs);
            }

            int n = _interface.Count;

            Primitives.WriteU30(output, (uint)n);

            for (int i = 0; i < n; ++i)
            {
                Primitives.WriteU30(output, (U30)_interface[i]);
            }

            Primitives.WriteU30(output, _iinit);

            n = _traits.Count;

            Primitives.WriteU30(output, (uint)n);

            for (int i = 0; i < n; ++i)
            {
                ((TraitInfo)_traits[i]).WriteExternal(output);
            }
        }

        #endregion
    }
}
