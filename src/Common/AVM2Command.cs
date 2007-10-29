using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using As3c.Swf.Types;

namespace As3c.Common
{
    public class AVM2Command
    {
        /// <summary>
        /// The string representation of the opcode.
        /// </summary>
        protected string _stringRepresentation;

        /// <summary>
        /// The opcode. Can be any value between 0x00 and 0xff.
        /// </summary>
        protected byte _opCode;

        /// <summary>
        /// The number of parameters. This number is incorrect for lookupswitch
        /// which has a dynamic number of parameters.
        /// </summary>
        protected byte _parameterCount;

        /// <summary>
        /// The parameter types. In case of lookupswitch this is ParameterType.Dynamic.
        /// </summary>
        protected ParameterType[] _types;

        /// <summary>
        /// The value of the parameters.
        /// </summary>
        protected ArrayList _parameters;

        public AVM2Command(string stringRepresentation, byte opCode, byte argumentCount)
        {
            Init(stringRepresentation, opCode, argumentCount, null);
        }

        public AVM2Command(string stringRepresentation, byte opCode, byte parameterCount, ParameterType[] types)
        {
            Init(stringRepresentation, opCode, parameterCount, types);
        }

        protected void Init(string stringRepresentation, byte opCode, byte parameterCount, ParameterType[] types)
        {
            _parameters = new ArrayList();

            _stringRepresentation = stringRepresentation;

            _opCode = opCode;

            _parameterCount = parameterCount;

            _types = types;
        }

        public string StringRepresentation { get { return _stringRepresentation; } }

        public byte OpCode { get { return _opCode; } }

        public byte ParameterCount { get { return _parameterCount; } }

        public ParameterType[] Types { get { return _types; } }

        public ParameterType GetTypeAt(uint index) { return _types[index]; }

        public ArrayList Parameters { get { return _parameters; } }

        public AVM2Command Clone()
        {
            AVM2Command clone = new AVM2Command(_stringRepresentation, _opCode, _parameterCount, _types);
            clone._parameters = (ArrayList)_parameters.Clone();

            return clone;
        }

        public uint ReadParameters(byte[] code, uint index)
        {
            _parameters.Clear();

            uint p = index;

            for (int i = 0; i < _parameterCount; ++i)
            {
                switch (_types[i])
                {
                    case ParameterType.U30:
                        U30 _u30 = Primitives.ReadU30(code, p);

                        p += _u30.Length;

                        _parameters.Add(_u30);
                        break;

                    case ParameterType.U8:
                        _parameters.Add(code[p++]);
                        break;

                    case ParameterType.S24:
                        S24 _s24 = Primitives.ReadS24(code, p);

                        p += _s24.Length;

                        _parameters.Add(_s24);
                        break;

                    case ParameterType.UInt:
                        U32 _u32 = Primitives.ReadU32(code, p);

                        p += _u32.Length;

                        _parameters.Add(_u32);
                        break;

                    case ParameterType.Dynamic:
                        S24 d_s24 = Primitives.ReadS24(code, p);

                        p += d_s24.Length;

                        _parameters.Add(d_s24);

                        U30 c_u30 = Primitives.ReadU30(code, p);

                        p += c_u30.Length;

                        _parameters.Add(c_u30);

                        for (int j = 0; j <= c_u30.Value; ++j)
                        {
                            S24 j_s24 = Primitives.ReadS24(code, p);

                            p += j_s24.Length;

                            _parameters.Add(j_s24);
                        }

                        break;
                }
            }

            return p - index;
        }
    }
}
