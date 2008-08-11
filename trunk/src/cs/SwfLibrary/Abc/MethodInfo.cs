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
    public class MethodInfo : IExternalizeable
    {
        public const byte NeedArguments = 0x01;
        public const byte NeedActivation = 0x02;
        public const byte NeedRest = 0x04;
        public const byte HasOptional = 0x08;
        public const byte SetDxns = 0x40;
        public const byte HasParamNames = 0x80;

        protected U30 _paramCount;
        protected U30 _returnType;
        protected ArrayList _paramType;
        protected U30 _name;
        protected byte _flags;
        protected OptionInfo _optionInfo;
        protected ArrayList _paramName;

        public U30 ParameterCount
        {
            get { return _paramCount; }
            set { _paramCount = value; }
        }

        public U30 ReturnType
        {
            get { return _returnType; }
            set { _returnType = value; }
        }

        public ArrayList ParameterType
        {
            get { return _paramType; }
            set { _paramType = value; }
        }

        public U30 Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public byte Flags
        {
            get { return _flags; }
            set { _flags = value; }
        }

        public OptionInfo OptionalParametersInfo
        {
            get { return _optionInfo; }
            set { _optionInfo = value; }
        }

        public ArrayList ParameterNames
        {
            get { return _paramName; }
            set { _paramName = value; }
        }

        #region IExternalizeable Members

        public void ReadExternal(BinaryReader input)
        {
            _paramCount = Primitives.ReadU30(input);
            _returnType = Primitives.ReadU30(input);

            _paramType = new ArrayList(Capacity.Max(_paramCount));

            for (uint i = 0; i < _paramCount._value; ++i)
                _paramType.Add(Primitives.ReadU30(input));

            _name = Primitives.ReadU30(input);
            _flags = input.ReadByte();

            if (HasFlag(HasOptional))
            {
                _optionInfo = new OptionInfo();
                _optionInfo.ReadExternal(input);
            }

            if (HasFlag(HasParamNames))
            {
                //param_info { u30 param_name[param_count] }
                _paramName = new ArrayList(Capacity.Max(_paramCount));

                for (uint i = 0; i < _paramCount._value; ++i)
                    _paramName.Add(Primitives.ReadU30(input));
            }
        }

        public void WriteExternal(BinaryWriter output)
        {
            Primitives.WriteU30(output, _paramCount);
            Primitives.WriteU30(output, _returnType);

            int n = (int)_paramCount.Value;

            for (int i = 0; i < n; ++i)
                Primitives.WriteU30(output, (U30)_paramType[i]);

            Primitives.WriteU30(output, _name);
            output.Write(_flags);

            if (HasFlag(HasOptional))
                _optionInfo.WriteExternal(output);

            if (HasFlag(HasParamNames))
            {
                //param_info { u30 param_name[param_count] }
                for (int i = 0; i < n; ++i)
                    Primitives.WriteU30(output, (U30)_paramName[i]);
            }
        }

        #endregion

        public bool HasFlag(byte flag)
        {
            return (0 != (_flags & flag));
        }
    }
}
