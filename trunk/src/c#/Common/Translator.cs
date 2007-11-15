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

#define USE_ACCELTABLES

using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.IO;

namespace As3c.Common
{
    public enum Op
    {
        Unused_00, Unused_01,
        Nop,
        Throw,
        GetSuper,
        SetSuper,
        DefaultXmlNamespace,
        DefaultXmlNamespaceLate,
        Kill,
        Label,
        Unused_0A, Unused_0B,
        IfNotLowerThan,
        IfNotLowerEqual,
        IfNotGreaterThan,
        IfNotGreaterEqual,
        Jump,
        IfTrue,
        IfFalse,
        IfEqual,
        IfNotEqual,
        IfLowerThan,
        IfLessEqual,
        IfGreaterThan,
        IfGreaterEqual,
        IfStrictEqual,
        IfStrictNotEqual,
        LookupSwitch,
        PushWith,
        PopScope,
        NextName,
        HasNext,
        PushNull,
        PushUndefined,
        Unused_22,
        NextValue,
        PushByte,
        PushShort,
        PushTrue,
        PushFalse,
        PushNaN,
        Pop,
        Dup,
        Swap,
        PushString,
        PushInt,
        PushUInt,
        PushDouble,
        PushScope,
        PushNamespace,
        HasNext2,
        Unused_33, Unused_34, Unused_35, Unused_36,
        Unused_37, Unused_38, Unused_39, Unused_3A,
        Unused_3B, Unused_3C, Unused_3D, Unused_3E,
        Unused_3F,
        NewFunction,
        Call,
        Construct,
        CallMethod,
        CallStatic,
        CallSuper,
        CallProperty,
        ReturnVoid,
        ReturnValue,
        ConstructSuper,
        ConstructProperty,
        Unused_4B,
        CallPropertyLex,
        Unused_4D,
        CallSuperVoid,
        CallPropertyVoid,
        Unused_50, Unused_51, Unused_52, Unused_53,
        Unused_54,
        NewObject,
        NewArray,
        NewActivation,
        NewClass,
        GetDescendants,
        NewCatch,
        Unused_5B, Unused_5C,
        FindPropertyStrict,
        FindProperty,
        Unused_5F,
        GetLex,
        SetProperty,
        GetLocal,
        SetLocal,
        GetGlobalScope,
        GetScopeObject,
        GetProperty,
        Unused_67,
        InitProperty,
        Unused_69,
        DeleteProperty,
        Unused_6B,
        GetSlot,
        SetSlot,
        GetGlobalSlot,
        SetGlobalSlot,
        ConvertToString,
        EsccapeElement,
        EsccapeXmlAttribute,
        ConvertToInt,
        ConvertToUInt,
        ConvertToDouble,
        ConvertToBoolean,
        ConvertToObject,
        Checkfilter,
        Unused_79, Unused_7A, Unused_7B, Unused_7C,
        Unused_7D, Unused_7E, Unused_7F,
        Coerce,
        Unused_81,
        CoerceAny,
        Unused_83, Unused_84,
        CoerceString,
        AsType,
        AsTypeLate,
        Unused_88, Unused_89, Unused_8A, Unused_8B,
        Unused_8C, Unused_8D, Unused_8E, Unused_8F,
        Negate,
        Increment,
        IncrementLocal,
        Decrement,
        Unused_94,
        TypeOf,
        Not,
        BitNot,
        Unused_98, Unused_99, Unused_9A, Unused_9B,
        Unused_9C, Unused_9D, Unused_9E, Unused_9F,
        Add,
        Subtract,
        Multiply,
        Divide,
        Modulo,
        LeftShift,
        RightShift,
        UnsignedRightShift,
        BitAnd,
        BitOr,
        BitXor,
        Equals,
        StrictEquals,
        LessThan,
        LessEquals,
        GreaterThan,
        GreaterEquals,
        InstanceOf,
        IsType,
        IsTypeLate,
        In,
        Unused_B5, Unused_B6, Unused_B7, Unused_B8,
        Unused_B9, Unused_BA, Unused_BB, Unused_BC,
        Unused_BD, Unused_BE, Unused_BF,
        IncrementInt,
        DecrementInt,
        IncrementLocalInt,
        DecrementLocalInt,
        NegateInt,
        AddInt,
        SubtractInt,
        MultiplyInt,
        Unused_C8, Unused_C9, Unused_CA, Unused_CB,
        Unused_CC, Unused_CD, Unused_CE, Unused_CF,
        GetLocal0,
        GetLocal1,
        GetLocal2,
        GetLocal3,
        SetLocal0,
        SetLocal1,
        SetLocal2,
        SetLocal3,
        Unused_D8, Unused_D9, Unused_DA, Unused_DB,
        Unused_DC, Unused_DD, Unused_DE, Unused_DF,
        Unused_E0, Unused_E1, Unused_E2, Unused_E3,
        Unused_E4, Unused_E5, Unused_E6, Unused_E7,
        Unused_E8, Unused_E9, Unused_EA, Unused_EB,
        Unused_EC, Unused_ED, Unused_EE,
        Debug,
        DebugLine,
        DebugFile,
        Unused_F2, Unused_F3, Unused_F4, Unused_F5,
        Unused_F6, Unused_F7, Unused_F8, Unused_F9,
        Unused_FA, Unused_FB, Unused_FC, Unused_FD,
        Unused_FE, Unused_FF
    }

    public enum ParameterType
    {
        U8,
        S24,
        U30,
        UInt,
        Dynamic
    }

    public class Translator
    {
        protected static ArrayList _dictionary = new ArrayList();

        protected static byte _dictionaryLength;

#if USE_ACCELTABLES

        protected static AVM2Command[] _accelTableOp = new AVM2Command[byte.MaxValue];

        protected static Dictionary<string, AVM2Command> _accelTableString = new Dictionary<string, AVM2Command>(byte.MaxValue);

        protected static Dictionary<string, AVM2Command> _accelTableInline = new Dictionary<string, AVM2Command>(byte.MaxValue);
#endif

        public static ArrayList Dictionary
        {
            get
            {
                return _dictionary;
            }
        }

        public static void write(StreamWriter tw)
        {
            foreach (AVM2Command cmd in _dictionary)
            {
                tw.WriteLine(cmd.StringRepresentation);
            }
        }

        public static void InitTable()
        {
            _dictionary.Add(new AVM2Command("add", 0xa0, 0));
            _dictionary.Add(new AVM2Command("add_i", "addInt", 0xc5, 0));
            _dictionary.Add(new AVM2Command("astype", "asType", 0x86, 1, new ParameterType[] { ParameterType.U30 }));
            _dictionary.Add(new AVM2Command("astypelate", "asTypeLate", 0x87, 0));
            _dictionary.Add(new AVM2Command("bitand", "and", 0xa8, 0));
            _dictionary.Add(new AVM2Command("bitnot", "not", 0x97, 0));
            _dictionary.Add(new AVM2Command("bitor", "or", 0xa9, 0));
            _dictionary.Add(new AVM2Command("bitxor", "xor", 0xaa, 0));
            _dictionary.Add(new AVM2Command("call", 0x41, 1, new ParameterType[] { ParameterType.U30 }));
            _dictionary.Add(new AVM2Command("callmethod", "callMethod", 0x43, 2, new ParameterType[] { ParameterType.U30, ParameterType.U30 }));
            _dictionary.Add(new AVM2Command("callproperty", "callProperty", 0x46, 2, new ParameterType[] { ParameterType.U30, ParameterType.U30 }));
            _dictionary.Add(new AVM2Command("callproplex", "callPropertyLex", 0x4c, 2, new ParameterType[] { ParameterType.U30, ParameterType.U30 }));
            _dictionary.Add(new AVM2Command("callpropvoid", "callPropertyVoid", 0x4f, 2, new ParameterType[] { ParameterType.U30, ParameterType.U30 }));
            _dictionary.Add(new AVM2Command("callstatic", "callStatic", 0x44, 2, new ParameterType[] { ParameterType.U30, ParameterType.U30 }));
            _dictionary.Add(new AVM2Command("callsuper", "callSuper", 0x45, 2, new ParameterType[] { ParameterType.U30, ParameterType.U30 }));
            _dictionary.Add(new AVM2Command("callsupervoid", "callSuperVoid", 0x4e, 2, new ParameterType[] { ParameterType.U30, ParameterType.U30 }));
            _dictionary.Add(new AVM2Command("checkfilter", "checkFilter", 0x78, 0));
            _dictionary.Add(new AVM2Command("coerce", 0x80, 1, new ParameterType[] { ParameterType.U30 }));
            _dictionary.Add(new AVM2Command("coerce_a", "coerceAny", 0x82, 0));
            _dictionary.Add(new AVM2Command("coerce_s", "coerceString", 0x85, 0));
            _dictionary.Add(new AVM2Command("construct", 0x42, 1, new ParameterType[] { ParameterType.U30 }));
            _dictionary.Add(new AVM2Command("constructprop", "constructProperty", 0x4a, 2, new ParameterType[] { ParameterType.U30, ParameterType.U30 }));
            _dictionary.Add(new AVM2Command("constructsuper", "constructSuper", 0x49, 1, new ParameterType[] { ParameterType.U30 }));
            _dictionary.Add(new AVM2Command("convert_b", "toBool", 0x76, 0));
            _dictionary.Add(new AVM2Command("convert_i", "toInt", 0x73, 0));
            _dictionary.Add(new AVM2Command("convert_d", "toDouble", 0x75, 0));
            _dictionary.Add(new AVM2Command("convert_o", "toObject", 0x77, 0));
            _dictionary.Add(new AVM2Command("convert_u", "toUInt", 0x74, 0));
            _dictionary.Add(new AVM2Command("convert_s", "toString", 0x70, 0));
            _dictionary.Add(new AVM2Command("debug", 0xef, 4, new ParameterType[] { ParameterType.U8, ParameterType.U30, ParameterType.U8, ParameterType.U30 }));
            _dictionary.Add(new AVM2Command("debugfile", 0xf1, 1, new ParameterType[] { ParameterType.U30 }));
            _dictionary.Add(new AVM2Command("debugline", 0xf0, 1, new ParameterType[] { ParameterType.U30 }));
            _dictionary.Add(new AVM2Command("declocal_i", "decLocalInt", 0xc3, 1, new ParameterType[] { ParameterType.U30 }));
            _dictionary.Add(new AVM2Command("decrement", 0x93, 0));
            _dictionary.Add(new AVM2Command("decrement_i", "decrementInt", 0xc1, 0));
            _dictionary.Add(new AVM2Command("deleteproperty", "deleteProperty", 0x6a, 1, new ParameterType[] { ParameterType.U30 }));
            _dictionary.Add(new AVM2Command("divide", 0xa3, 0));
            _dictionary.Add(new AVM2Command("dup", "duplicate", 0x2a, 0));
            _dictionary.Add(new AVM2Command("dxns", "defaultXMLNamespace", 0x06, 1, new ParameterType[] { ParameterType.U30 }));
            _dictionary.Add(new AVM2Command("dxnslate", "defaultXMLNamespaceLate", 0x07, 0));
            _dictionary.Add(new AVM2Command("equals", 0xab, 0));
            _dictionary.Add(new AVM2Command("esc_xattr", "escapeXMLAttribute", 0x72, 0));
            _dictionary.Add(new AVM2Command("esc_elem", "escapeXMLElement", 0x71, 0));
            _dictionary.Add(new AVM2Command("findproperty", "findProperty", 0x5e, 1, new ParameterType[] { ParameterType.U30 }));
            _dictionary.Add(new AVM2Command("findpropstrict", "findPropertyStrict", 0x5d, 1, new ParameterType[] { ParameterType.U30 }));
            _dictionary.Add(new AVM2Command("getdescendants", "getDescendants", 0x59, 1, new ParameterType[] { ParameterType.U30 }));
            _dictionary.Add(new AVM2Command("getglobalscope", "getGlobalScope", 0x64, 0));
            _dictionary.Add(new AVM2Command("getglobalslot", "getGlobalSlot", 0x6e, 1, new ParameterType[] { ParameterType.U30 }));
            _dictionary.Add(new AVM2Command("getlex", "getLex", 0x60, 1, new ParameterType[] { ParameterType.U30 }));
            _dictionary.Add(new AVM2Command("getlocal", "getLocal", 0x62, 1, new ParameterType[] { ParameterType.U30 }));
            for (int i = 0; i < 4; ++i) _dictionary.Add(new AVM2Command("get_local" + i.ToString(), "getLocal"+i.ToString(), (byte)(0xd0 + i), 0));
            _dictionary.Add(new AVM2Command("getproperty", "getProperty", 0x66, 1, new ParameterType[] { ParameterType.U30 }));
            _dictionary.Add(new AVM2Command("getscopeobject", "getScopeObject", 0x65, 1, new ParameterType[] { ParameterType.U8 }));
            _dictionary.Add(new AVM2Command("getslot", "getSlot", 0x6c, 1, new ParameterType[] { ParameterType.U30 }));
            _dictionary.Add(new AVM2Command("getsuper", "getSuper", 0x04, 1, new ParameterType[] { ParameterType.U30 }));
            _dictionary.Add(new AVM2Command("greaterequals", "greaterEquals", 0xb0, 0));
            _dictionary.Add(new AVM2Command("greaterthan", "greaterThan", 0xaf, 0));
            _dictionary.Add(new AVM2Command("hasnext", "hasNext", 0x1f, 0));
            _dictionary.Add(new AVM2Command("hasnext2", "hasNext2", 0x32, 2, new ParameterType[] { ParameterType.UInt, ParameterType.UInt }));
            { // s24 offset (3b) - conditional jumps
                _dictionary.Add(new AVM2Command("ifeq", "ifEqual", 0x13, 1, new ParameterType[] { ParameterType.S24 }));
                _dictionary.Add(new AVM2Command("iffalse", "ifFalse", 0x12, 1, new ParameterType[] { ParameterType.S24 }));
                _dictionary.Add(new AVM2Command("ifge", "ifGreaterEqual", 0x18, 1, new ParameterType[] { ParameterType.S24 }));
                _dictionary.Add(new AVM2Command("ifgt", "ifGreaterThan", 0x17, 1, new ParameterType[] { ParameterType.S24 }));
                _dictionary.Add(new AVM2Command("ifle", "ifLessEqual", 0x16, 1, new ParameterType[] { ParameterType.S24 }));
                _dictionary.Add(new AVM2Command("iflt", "ifLessThan", 0x15, 1, new ParameterType[] { ParameterType.S24 }));
                _dictionary.Add(new AVM2Command("ifnge", "ifNotGreaterEqual", 0x0f, 1, new ParameterType[] { ParameterType.S24 }));
                _dictionary.Add(new AVM2Command("ifngt", "ifNotGreaterThan", 0x0e, 1, new ParameterType[] { ParameterType.S24 }));
                _dictionary.Add(new AVM2Command("ifnle", "ifNotLessEqual", 0x0d, 1, new ParameterType[] { ParameterType.S24 }));
                _dictionary.Add(new AVM2Command("ifnlt", "ifNotLessThan", 0x0c, 1, new ParameterType[] { ParameterType.S24 }));
                _dictionary.Add(new AVM2Command("ifne", "ifNotEqual", 0x14, 1, new ParameterType[] { ParameterType.S24 }));
                _dictionary.Add(new AVM2Command("ifstricteq", "ifStrictEqual", 0x19, 1, new ParameterType[] { ParameterType.S24 }));
                _dictionary.Add(new AVM2Command("ifstrictne", "ifStrictNotEqual", 0x1a, 1, new ParameterType[] { ParameterType.S24 }));
                _dictionary.Add(new AVM2Command("iftrue", "ifTrue", 0x11, 1, new ParameterType[] { ParameterType.S24 }));
            }
            _dictionary.Add(new AVM2Command("in", "in_", 0xb4, 0));
            _dictionary.Add(new AVM2Command("inclocal", "incLocal", 0x92, 1, new ParameterType[] { ParameterType.U30 }));
            _dictionary.Add(new AVM2Command("inclocal_i", "incLocalInt", 0xc2, 1, new ParameterType[] { ParameterType.U30 }));
            _dictionary.Add(new AVM2Command("increment", 0x91, 0));
            _dictionary.Add(new AVM2Command("increment_i", "incrementInt", 0xc0, 0));
            _dictionary.Add(new AVM2Command("initproperty", "initProperty", 0x68, 1, new ParameterType[] { ParameterType.U30 }));
            _dictionary.Add(new AVM2Command("instanceof", "instanceOf", 0xb1, 0));
            _dictionary.Add(new AVM2Command("istype", "isType", 0xb2, 1, new ParameterType[] { ParameterType.U30 }));
            _dictionary.Add(new AVM2Command("istypelate", "isTypeLate", 0xb3, 0));
            { // s24 offset (3b) - unconditional jump
                _dictionary.Add(new AVM2Command("jump", 0x10, 1, new ParameterType[] { ParameterType.S24 }));
            }
            _dictionary.Add(new AVM2Command("kill", 0x08, 1, new ParameterType[] { ParameterType.U30 }));
            _dictionary.Add(new AVM2Command("label", 0x09, 0));
            _dictionary.Add(new AVM2Command("lessequals", "lessEquals", 0xae, 0));
            _dictionary.Add(new AVM2Command("lessthan", "lessThan", 0xad, 0));
            { // dynamic parameter length based on case_count. conditional jump
                _dictionary.Add(new AVM2Command("lookupswitch", "lookUpSwitch", 0x1b, 1, new ParameterType[] { ParameterType.Dynamic }));
            }
            _dictionary.Add(new AVM2Command("lshift", "shiftLeft", 0xa5, 0));
            _dictionary.Add(new AVM2Command("modulo", 0xa4, 0));
            _dictionary.Add(new AVM2Command("multiply", 0xa2, 0));
            _dictionary.Add(new AVM2Command("multiply_i", "multiplyInt", 0xc7, 0));
            _dictionary.Add(new AVM2Command("negate", 0x90, 0));
            _dictionary.Add(new AVM2Command("negate_i", "negateInt", 0xc4, 0));
            _dictionary.Add(new AVM2Command("newactivation", "newActivation", 0x57, 0));
            _dictionary.Add(new AVM2Command("newarray", "newArray", 0x56, 1, new ParameterType[] { ParameterType.U30 }));
            _dictionary.Add(new AVM2Command("newcatch", "newCatch", 0x5a, 1, new ParameterType[] { ParameterType.U30 }));
            _dictionary.Add(new AVM2Command("newclass", "newClass", 0x58, 1, new ParameterType[] { ParameterType.U30 }));
            _dictionary.Add(new AVM2Command("newfunction", "newFunction", 0x40, 1, new ParameterType[] { ParameterType.U30 }));
            _dictionary.Add(new AVM2Command("newobject", "newObject", 0x55, 1, new ParameterType[] { ParameterType.U30 }));
            _dictionary.Add(new AVM2Command("nextname", "nextName", 0x1e, 0));
            _dictionary.Add(new AVM2Command("nextvalue", "nextValue", 0x23, 0));
            _dictionary.Add(new AVM2Command("nop", 0x02, 0));
            _dictionary.Add(new AVM2Command("not", "notBool", 0x96, 0));
            _dictionary.Add(new AVM2Command("pop", 0x29, 0));
            _dictionary.Add(new AVM2Command("popscope", "popScope", 0x1d, 0));
            _dictionary.Add(new AVM2Command("pushbyte", "pushByte", 0x24, 1, new ParameterType[] { ParameterType.U8 }));
            _dictionary.Add(new AVM2Command("pushdouble", "pushDouble", 0x2f, 1, new ParameterType[] { ParameterType.U30 }));
            _dictionary.Add(new AVM2Command("pushfalse", "pushFalse", 0x27, 0));
            _dictionary.Add(new AVM2Command("pushint", "pushInt", 0x2d, 1, new ParameterType[] { ParameterType.U30 }));
            _dictionary.Add(new AVM2Command("pushnamespace", "pushNamespace", 0x31, 1, new ParameterType[] { ParameterType.U30 }));
            _dictionary.Add(new AVM2Command("pushnan", "pushNaN", 0x28, 0));
            _dictionary.Add(new AVM2Command("pushnull", "pushNull", 0x20, 0));
            _dictionary.Add(new AVM2Command("pushscope", "pushScope", 0x30, 0));
            _dictionary.Add(new AVM2Command("pushshort", "pushShort", 0x25, 1, new ParameterType[] { ParameterType.U30 }));
            _dictionary.Add(new AVM2Command("pushstring", "pushString", 0x2c, 1, new ParameterType[] { ParameterType.U30 }));
            _dictionary.Add(new AVM2Command("pushtrue", "pushTrue", 0x26, 0));
            _dictionary.Add(new AVM2Command("pushuint", "pushUInt", 0x2e, 1, new ParameterType[] { ParameterType.U30 }));
            _dictionary.Add(new AVM2Command("pushundefined", "pushUndefined", 0x21, 0));
            _dictionary.Add(new AVM2Command("pushwith", "pushWith", 0x1c, 0));
            _dictionary.Add(new AVM2Command("returnvalue", "returnValue", 0x48, 0));
            _dictionary.Add(new AVM2Command("returnvoid", "returnVoid", 0x47, 0));
            _dictionary.Add(new AVM2Command("rshift", "shiftRight", 0xa6, 0));
            _dictionary.Add(new AVM2Command("setlocal", "setLocal", 0x63, 1, new ParameterType[] { ParameterType.U30 }));
            for (int i = 0; i < 4; ++i) _dictionary.Add(new AVM2Command("set_local" + i.ToString(), "setLocal" + i.ToString(), (byte)(0xd4 + i), 0));
            _dictionary.Add(new AVM2Command("setglobalslot", "setGlobalSlot", 0x6f, 1, new ParameterType[] { ParameterType.U30 }));
            _dictionary.Add(new AVM2Command("setproperty", "setProperty", 0x61, 1, new ParameterType[] { ParameterType.U30 }));
            _dictionary.Add(new AVM2Command("setslot", "setSlot", 0x6d, 1, new ParameterType[] { ParameterType.U30 }));
            _dictionary.Add(new AVM2Command("setsuper", "setSuper", 0x05, 1, new ParameterType[] { ParameterType.U30 }));
            _dictionary.Add(new AVM2Command("strictequals", "strictEquals", 0xac, 0));
            _dictionary.Add(new AVM2Command("subtract", 0xa1, 0));
            _dictionary.Add(new AVM2Command("subtract_i", "subtractInt", 0xc6, 0));
            _dictionary.Add(new AVM2Command("swap", 0x2b, 0));
            _dictionary.Add(new AVM2Command("throw", "throw_", 0x03, 0));
            _dictionary.Add(new AVM2Command("typeof", "typeOf", 0x95, 0));
            _dictionary.Add(new AVM2Command("urshift", "shiftRightUInt", 0xa7, 0));

            _dictionaryLength = (byte)_dictionary.Count;

            /*StreamWriter outs = new StreamWriter(File.Open("c:\\codes2.txt", FileMode.OpenOrCreate));

            for (int i = 0; i < _dictionary.Count; ++i)
            {
                AVM2Command c = (AVM2Command)_dictionary[i];
                string args = "";
                for (int j = 0; j < c.ParameterCount; ++j)
                {
                    args += "a" + j + ": *";

                    if (j != c.ParameterCount - 1)
                    {
                        args += ", ";
                    }
                }

                string cmd = String.Format("\tpublic static function {0}({1}):void{{}}//0x{2:x2}", c.InlineName, args, c.OpCode);
                
                outs.WriteLine(cmd);
            }

            outs.Close();
            outs.Dispose();*/

#if USE_ACCELTABLES

            /**
             * Build accelerator tables.
             */
            foreach (AVM2Command cmd in _dictionary)
            {
                _accelTableOp[cmd.OpCode] = cmd;
                _accelTableString[cmd.StringRepresentation] = cmd;
                _accelTableInline[cmd.InlineName] = cmd;
            }

#endif
        }

        public static bool CheckIntegrity()
        {
            /**
             * Simple integrity scan of the dictionary.
             * 
             * 1) Test if number of parameters matches parameter types
             * 2) Test if one OpCode is used twice.
             */
            bool[] opTable = new bool[0xff];

            foreach (AVM2Command cmd in _dictionary)
            {
                if (0 == cmd.ParameterCount && cmd.Types != null)
                {
                    Console.WriteLine("[i] Failure at {0} - Invalid parameters (more types)", cmd.OpCode);
                }
                else if (cmd.Types != null && cmd.Types.Length != cmd.ParameterCount)
                {
                    Console.WriteLine("[i] Failure at {0} - Invalid parameters", cmd.OpCode);
                }

                if (opTable[cmd.OpCode])
                {
                    Console.WriteLine("[i] Failure at {0} - Redefine", cmd.OpCode);
                    return false;
                }

                opTable[cmd.OpCode] = true;
            }

            return true;
        }

        public static AVM2Command ToCommand(byte opCode)
        {
#if USE_ACCELTABLES

            return _accelTableOp[opCode].Clone();

#else

            foreach (AVM2Command cmd in _dictionary)
            {
                if (cmd.OpCode == opCode)
                {
                    return cmd;
                }
            }

            return null;

#endif
        }

        public static AVM2Command ToCommand(string command)
        {
            return ToCommand(command, false);
        }

        public static AVM2Command ToCommand(string command, bool useInlineTable)
        {
#if USE_ACCELTABLES
            Dictionary<string, AVM2Command> table = (useInlineTable) ? _accelTableInline : _accelTableString;

            if (!table.ContainsKey(command))
                return null;

            return table[command].Clone();

#else

            byte i = 0;

            AVM2Command cmd;

            for (;i<_dictionaryLength;++i)
            {
                cmd = (AVM2Command)_dictionary[i];

                if (((useInlineTable) ? cmd.InlineName : cmd.StringRepresentation) == command)
                {
                    return cmd;
                }
            }

            return null;

#endif
        }
    }
}
