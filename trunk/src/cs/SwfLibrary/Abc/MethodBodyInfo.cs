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

namespace SwfLibrary.Abc
{
    public class MethodBodyInfo : IExternalizeable, IHasTraits
    {
        protected U30 _method;
        protected U30 _maxStack;
        protected U30 _localCount;
        protected U30 _initScopeDepth;
        protected U30 _maxScopeDepth;
        protected byte[] _code;
        protected ArrayList _exceptionInfo;
        protected ArrayList _traits;
        protected bool _isInline;
        protected bool _inlineScan;

        public byte[] Code
        {
            get { return _code; }
            set { _code = value; }
        }

        public U30 Method
        {
            get { return _method; }
            set { _method = value; }
        }

        public U30 MaxStack
        {
            get { return _maxStack; }
            set { _maxStack = value; }
        }

        public U30 LocalCount
        {
            get { return _localCount; }
            set { _localCount = value; }
        }

        public U30 InitScopeDepth
        {
            get { return _initScopeDepth; }
            set { _initScopeDepth = value; }
        }

        public U30 MaxScopeDepth
        {
            get { return _maxScopeDepth; }
            set { _maxScopeDepth = value; }
        }

        public ArrayList Exceptions
        {
            get { return _exceptionInfo; }
            set { _exceptionInfo = value; }
        }
        
        public ArrayList Traits
        {
            get { return _traits; }
            set { _traits = value; }
        }

        public bool IsInline
        {
            get { return _isInline; }

            set
            {
                _isInline = value;
                _inlineScan = true;
            }
        }

        public bool IsInlineKnown
        {
            get { return _inlineScan; }
        }

        #region IExternalizeable Members

        public void ReadExternal(BinaryReader input)
        {
            uint n;

            _method = Primitives.ReadU30(input);
            _maxStack = Primitives.ReadU30(input);
            _localCount = Primitives.ReadU30(input);
            _initScopeDepth = Primitives.ReadU30(input);
            _maxScopeDepth = Primitives.ReadU30(input);

            n = Primitives.ReadU30(input).Value;
            _code = new byte[n];//the holy grail...finally...
            for (uint i = 0; i < n; ++i)
                _code[i] = input.ReadByte();


            n = Primitives.ReadU30(input).Value;
            _exceptionInfo = new ArrayList(Capacity.Max(n));
            for (uint i = 0; i < n; ++i)
            {
                ExceptionInfo ei = new ExceptionInfo();
                ei.ReadExternal(input);

                _exceptionInfo.Add(ei);
            }

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
            Primitives.WriteU30(output, _method);
            Primitives.WriteU30(output, _maxStack);
            Primitives.WriteU30(output, _localCount);
            Primitives.WriteU30(output, _initScopeDepth);
            Primitives.WriteU30(output, _maxScopeDepth);

            int n = _code.Length;

            Primitives.WriteU30(output, (uint)n);
            output.Write(_code);

            n = _exceptionInfo.Count;
            
            Primitives.WriteU30(output, (uint)n);

            for (int i = 0; i < n; ++i)
            {
                ((ExceptionInfo)_exceptionInfo[i]).WriteExternal(output);
            }

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
