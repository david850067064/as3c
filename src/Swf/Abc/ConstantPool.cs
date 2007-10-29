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
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

using As3c.Swf.Abc.Constants;
using As3c.Swf.Types;
using As3c.Swf.Utils;
using As3c.Swf.Exceptions;

namespace As3c.Swf.Abc
{
    public class ConstantPool : IExternalizeable
    {
        #region ConstantPool Members

        protected ArrayList _intTable;
        protected ArrayList _uintTable;
        protected ArrayList _doubleTable;
        protected ArrayList _stringTable;
        protected ArrayList _namespaceTable;
        protected ArrayList _nsTable;
        protected ArrayList _multinameTable;

        public ArrayList IntTable
        {
            get { return _intTable; }
            set { _intTable = value; }
        }

        public int ResolveInt(S32 value)
        {
            for (int i = 0; i < _intTable.Count; ++i)
            {
                if (value.Value == ((S32)_intTable[i]).Value)
                    return i;
            }

            int j = _intTable.Count;

            _intTable.Add(value);

            return j;
        }

        public ArrayList UIntTable
        {
            get { return _uintTable; }
            set { _uintTable = value; }
        }

        public int ResolveUInt(U32 value)
        {
            for (int i = 0; i < _uintTable.Count; ++i)
            {
                if (value.Value == ((U32)_uintTable[i]).Value)
                    return i;
            }

            int j = _uintTable.Count;

            _uintTable.Add(value);

            return j;
        }

        public ArrayList DoubleTable
        {
            get { return _doubleTable; }
            set { _doubleTable = value; }
        }

        public int ResolveDouble(double value)
        {
            for (int i = 0; i < _doubleTable.Count; ++i)
            {
                if (value == ((double)_doubleTable[i]))
                    return i;
            }

            int j = _doubleTable.Count;

            _doubleTable.Add(value);

            return j;
        }

        public ArrayList StringTable
        {
            get { return _stringTable; }
            set { _stringTable = value; }
        }

        public ArrayList NamespaceTable
        {
            get { return _namespaceTable; }
            set { _namespaceTable = value; }
        }

        public ArrayList NamespaceSetTable
        {
            get { return _nsTable; }
            set { _nsTable = value; }
        }

        public ArrayList MultinameTable
        {
            get { return _multinameTable; }
            set { _multinameTable = value; }
        }

        #endregion

        #region IExternalizeable Members

        public void ReadExternal(BinaryReader input)
        {
            #region integer

            uint n = Primitives.ReadU30(input).Value;
            
            _intTable = new ArrayList(Capacity.Max(n));
            _intTable.Add(new S32());
            
            for (uint i = 1; i < n; ++i)
                _intTable.Add(Primitives.ReadS32(input));

            #endregion

            #region uinteger

            n = Primitives.ReadU30(input).Value;

            _uintTable = new ArrayList(Capacity.Max(n));
            _uintTable.Add(new U32());

            for (uint i = 1; i < n; ++i)
                _uintTable.Add(Primitives.ReadU32(input));

            #endregion

            #region double

            n = Primitives.ReadU30(input).Value;

            _doubleTable = new ArrayList(Capacity.Max(n));
            _doubleTable.Add(double.NaN);

            for (uint i = 1; i < n; ++i)
                _doubleTable.Add(input.ReadDouble());

            #endregion

            #region string_info

            n = Primitives.ReadU30(input).Value;

            _stringTable = new ArrayList(Capacity.Max(n));
            _stringTable.Add(new StringInfo());

            for (uint i = 1; i < n; ++i)
            {
                StringInfo stringInfo = new StringInfo();
                stringInfo.ReadExternal(input);

                _stringTable.Add(stringInfo);
            }

            #endregion

            #region namespace_info

            n = Primitives.ReadU30(input).Value;

            _namespaceTable = new ArrayList(Capacity.Max(n));
            _namespaceTable.Add(null);

            for (uint i = 1; i < n; ++i)
            {
                NamespaceInfo namespaceInfo = new NamespaceInfo();
                namespaceInfo.ReadExternal(input);

                _namespaceTable.Add(namespaceInfo);
            }

            #endregion

            #region ns_set_info

            n = Primitives.ReadU30(input).Value;

            _nsTable = new ArrayList(Capacity.Max(n));
            _nsTable.Add(null);

            for (uint i = 1; i < n; ++i)
            {
                NamespaceSetInfo nsInfo = new NamespaceSetInfo();
                nsInfo.ReadExternal(input);

                _nsTable.Add(nsInfo);
            }

            #endregion

            #region multiname_info

            n = Primitives.ReadU30(input).Value;

            _multinameTable = new ArrayList(Capacity.Max(n));
            _multinameTable.Add(null);

            for (uint i = 1; i < n; ++i)
            {
                MultinameInfo multinameInfo = new MultinameInfo();
                multinameInfo.ReadExternal(input);

                _multinameTable.Add(multinameInfo);
            }

            #endregion
        }

        public void WriteExternal(BinaryWriter output)
        {
            #region integer

            int n = GetCount(_intTable);

            Primitives.WriteU30(output, (uint)n);

            for (int i = 1; i < n; ++i)
                Primitives.WriteS32(output, (S32)_intTable[i]);

            #endregion

            #region uinteger

            n = GetCount(_uintTable);

            Primitives.WriteU30(output, (uint)n);

            for (int i = 1; i < n; ++i)
                Primitives.WriteU32(output, (U32)_uintTable[i]);

            #endregion

            #region double

            n = GetCount(_doubleTable);

            Primitives.WriteU30(output, (uint)n);

            for (int i = 1; i < n; ++i)
                output.Write((double)_doubleTable[i]);

            #endregion

            #region string_info

            n = GetCount(_stringTable);

            Primitives.WriteU30(output, (uint)n);

            for (int i = 1; i < n; ++i)
                ((StringInfo)_stringTable[i]).WriteExternal(output);

            #endregion

            #region namespace_info

            n = GetCount(_namespaceTable);

            Primitives.WriteU30(output, (uint)n);

            for (int i = 1; i < n; ++i)
                ((NamespaceInfo)_namespaceTable[i]).WriteExternal(output);

            #endregion

            #region ns_set_info

            n = GetCount(_nsTable);

            Primitives.WriteU30(output, (uint)n);

            for (int i = 1; i < n; ++i)
                ((NamespaceSetInfo)_nsTable[i]).WriteExternal(output);

            #endregion

            #region multiname_info

            n = GetCount(_multinameTable);

            Primitives.WriteU30(output, (uint)n);

            for (int i = 1; i < n; ++i)
                ((MultinameInfo)_multinameTable[i]).WriteExternal(output);

            #endregion
        }

        #endregion

        protected int GetCount(ArrayList table)
        {
            if (1 == table.Count) return 0;
            return table.Count;
        }
    }
}
