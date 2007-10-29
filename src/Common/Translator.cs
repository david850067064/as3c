#define USE_ACCELTABLES

using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

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
        None,
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

#endif

        public static ArrayList Dictionary
        {
            get
            {
                return _dictionary;
            }
        }

        public static void InitTable()
        {
            /**
             * The set of instructions for the AVM2.
             * Taken straight from http://www.adobe.com/devnet/actionscript/articles/avm2overview.pdf
             */

            _dictionary.Add(new AVM2Command("add", 0xa0, 0));
            _dictionary.Add(new AVM2Command("add_i", 0xc5, 0));
            _dictionary.Add(new AVM2Command("astype", 0x86, 1, new ParameterType[] { ParameterType.U30 }));
            _dictionary.Add(new AVM2Command("astypelate", 0x87, 0));
            _dictionary.Add(new AVM2Command("bitand", 0xa8, 0));
            _dictionary.Add(new AVM2Command("bitnot", 0x97, 0));
            _dictionary.Add(new AVM2Command("bitor", 0xa9, 0));
            _dictionary.Add(new AVM2Command("bitxor", 0xaa, 0));
            _dictionary.Add(new AVM2Command("call", 0x41, 1, new ParameterType[] { ParameterType.U30 }));
            _dictionary.Add(new AVM2Command("callmethod", 0x43, 2, new ParameterType[] { ParameterType.U30, ParameterType.U30 }));
            _dictionary.Add(new AVM2Command("callproperty", 0x46, 2, new ParameterType[] { ParameterType.U30, ParameterType.U30 }));
            _dictionary.Add(new AVM2Command("callproplex", 0x4c, 2, new ParameterType[] { ParameterType.U30, ParameterType.U30 }));
            _dictionary.Add(new AVM2Command("callpropvoid", 0x4f, 2, new ParameterType[] { ParameterType.U30, ParameterType.U30 }));
            _dictionary.Add(new AVM2Command("callstatic", 0x44, 2, new ParameterType[] { ParameterType.U30, ParameterType.U30 }));
            _dictionary.Add(new AVM2Command("callsuper", 0x45, 2, new ParameterType[] { ParameterType.U30, ParameterType.U30 }));
            _dictionary.Add(new AVM2Command("callsupervoid", 0x4e, 2, new ParameterType[] { ParameterType.U30, ParameterType.U30 }));
            _dictionary.Add(new AVM2Command("checkfilter", 0x78, 0));
            _dictionary.Add(new AVM2Command("coerce", 0x80, 1, new ParameterType[] { ParameterType.U30 }));
            _dictionary.Add(new AVM2Command("coerce_a", 0x82, 0));
            _dictionary.Add(new AVM2Command("coerce_s", 0x85, 0));
            _dictionary.Add(new AVM2Command("construct", 0x42, 1, new ParameterType[] { ParameterType.U30 }));
            _dictionary.Add(new AVM2Command("constructprop", 0x4a, 2, new ParameterType[] { ParameterType.U30, ParameterType.U30 }));
            _dictionary.Add(new AVM2Command("constructsuper", 0x49, 1, new ParameterType[] { ParameterType.U30 }));
            _dictionary.Add(new AVM2Command("convert_b", 0x76, 0));
            _dictionary.Add(new AVM2Command("convert_i", 0x73, 0));
            _dictionary.Add(new AVM2Command("convert_d", 0x75, 0));
            _dictionary.Add(new AVM2Command("convert_o", 0x77, 0));
            _dictionary.Add(new AVM2Command("convert_u", 0x74, 0));
            _dictionary.Add(new AVM2Command("convert_s", 0x70, 0));
            _dictionary.Add(new AVM2Command("debug", 0xef, 4, new ParameterType[] { ParameterType.U8, ParameterType.U30, ParameterType.U8, ParameterType.U30 }));
            _dictionary.Add(new AVM2Command("debugfile", 0xf1, 1, new ParameterType[] { ParameterType.U30 }));
            _dictionary.Add(new AVM2Command("debugline", 0xf0, 1, new ParameterType[] { ParameterType.U30 }));
            _dictionary.Add(new AVM2Command("declocal_i", 0xc3, 1, new ParameterType[] { ParameterType.U30 }));
            _dictionary.Add(new AVM2Command("decrement", 0x93, 0));
            _dictionary.Add(new AVM2Command("decrement_i", 0xc1, 0));
            _dictionary.Add(new AVM2Command("deleteproperty", 0x6a, 1, new ParameterType[] { ParameterType.U30 }));
            _dictionary.Add(new AVM2Command("divide", 0xa3, 0));
            _dictionary.Add(new AVM2Command("dup", 0x2a, 0));
            _dictionary.Add(new AVM2Command("dxns", 0x06, 1, new ParameterType[] { ParameterType.U30 }));
            _dictionary.Add(new AVM2Command("dxnslate", 0x07, 0));
            _dictionary.Add(new AVM2Command("equals", 0xab, 0));
            _dictionary.Add(new AVM2Command("esc_xattr", 0x72, 0));
            _dictionary.Add(new AVM2Command("esc_elem", 0x71, 0));
            _dictionary.Add(new AVM2Command("findproperty", 0x5e, 1, new ParameterType[] { ParameterType.U30 }));
            _dictionary.Add(new AVM2Command("findpropstrict", 0x5d, 1, new ParameterType[] { ParameterType.U30 }));
            _dictionary.Add(new AVM2Command("getdescendants", 0x59, 1, new ParameterType[] { ParameterType.U30 }));
            _dictionary.Add(new AVM2Command("getglobalscope", 0x64, 0));
            _dictionary.Add(new AVM2Command("getglobalslot", 0x6e, 1, new ParameterType[] { ParameterType.U30 }));
            _dictionary.Add(new AVM2Command("getlex", 0x60, 1, new ParameterType[] { ParameterType.U30 }));
            _dictionary.Add(new AVM2Command("getlocal", 0x62, 1, new ParameterType[] { ParameterType.U30 }));
            for (int i = 0; i < 4; ++i) _dictionary.Add(new AVM2Command("get_local" + i.ToString(), (byte)(0xd0 + i), 0));
            _dictionary.Add(new AVM2Command("getproperty", 0x66, 1, new ParameterType[] { ParameterType.U30 }));
            _dictionary.Add(new AVM2Command("getscopeobject", 0x65, 1, new ParameterType[] { ParameterType.U8 }));
            _dictionary.Add(new AVM2Command("getslot", 0x6c, 1, new ParameterType[] { ParameterType.U30 }));
            _dictionary.Add(new AVM2Command("getsuper", 0x04, 1, new ParameterType[] { ParameterType.U30 }));
            _dictionary.Add(new AVM2Command("greaterequals", 0xb0, 0));
            _dictionary.Add(new AVM2Command("greaterthan", 0xaf, 0));
            _dictionary.Add(new AVM2Command("hasnext", 0x1f, 0));
            _dictionary.Add(new AVM2Command("hasnext2", 0x32, 2, new ParameterType[] { ParameterType.UInt, ParameterType.UInt }));
            { // s24 offset (3b) - conditional jumps
                _dictionary.Add(new AVM2Command("ifeq", 0x13, 1, new ParameterType[] { ParameterType.S24 }));
                _dictionary.Add(new AVM2Command("iffalse", 0x12, 1, new ParameterType[] { ParameterType.S24 }));
                _dictionary.Add(new AVM2Command("ifge", 0x18, 1, new ParameterType[] { ParameterType.S24 }));
                _dictionary.Add(new AVM2Command("ifgt", 0x17, 1, new ParameterType[] { ParameterType.S24 }));
                _dictionary.Add(new AVM2Command("ifle", 0x16, 1, new ParameterType[] { ParameterType.S24 }));
                _dictionary.Add(new AVM2Command("iflt", 0x15, 1, new ParameterType[] { ParameterType.S24 }));
                _dictionary.Add(new AVM2Command("ifnge", 0x0f, 1, new ParameterType[] { ParameterType.S24 }));
                _dictionary.Add(new AVM2Command("ifngt", 0x0e, 1, new ParameterType[] { ParameterType.S24 }));
                _dictionary.Add(new AVM2Command("ifnle", 0x0d, 1, new ParameterType[] { ParameterType.S24 }));
                _dictionary.Add(new AVM2Command("ifnlt", 0x0c, 1, new ParameterType[] { ParameterType.S24 }));
                _dictionary.Add(new AVM2Command("ifne", 0x14, 1, new ParameterType[] { ParameterType.S24 }));
                _dictionary.Add(new AVM2Command("ifstricteq", 0x19, 1, new ParameterType[] { ParameterType.S24 }));
                _dictionary.Add(new AVM2Command("ifstrictne", 0x1a, 1, new ParameterType[] { ParameterType.S24 }));
                _dictionary.Add(new AVM2Command("iftrue", 0x11, 1, new ParameterType[] { ParameterType.S24 }));
            }
            _dictionary.Add(new AVM2Command("in", 0xb4, 0));
            _dictionary.Add(new AVM2Command("inclocal", 0x92, 1, new ParameterType[] { ParameterType.U30 }));
            _dictionary.Add(new AVM2Command("inclocal_i", 0xc2, 1, new ParameterType[] { ParameterType.U30 }));
            _dictionary.Add(new AVM2Command("increment", 0x91, 0));
            _dictionary.Add(new AVM2Command("increment_i", 0xc0, 0));
            _dictionary.Add(new AVM2Command("initproperty", 0x68, 1, new ParameterType[] { ParameterType.U30 }));
            _dictionary.Add(new AVM2Command("instanceof", 0xb1, 0));
            _dictionary.Add(new AVM2Command("istype", 0xb2, 1, new ParameterType[] { ParameterType.U30 }));
            _dictionary.Add(new AVM2Command("istypelate", 0xb3, 0));
            { // s24 offset (3b) - unconditional jump
                _dictionary.Add(new AVM2Command("jump", 0x10, 1, new ParameterType[] { ParameterType.S24 }));
            }
            _dictionary.Add(new AVM2Command("kill", 0x08, 1, new ParameterType[] { ParameterType.U30 }));
            _dictionary.Add(new AVM2Command("label", 0x09, 0));
            _dictionary.Add(new AVM2Command("lessequals", 0xae, 0));
            _dictionary.Add(new AVM2Command("lessthan", 0xad, 0));
            { // dynamic parameter length based on case_count. conditional jump
                _dictionary.Add(new AVM2Command("lookupswitch", 0x1b, 1, new ParameterType[] { ParameterType.Dynamic }));
            }
            _dictionary.Add(new AVM2Command("lshift", 0xa5, 0));
            _dictionary.Add(new AVM2Command("modulo", 0xa4, 0));
            _dictionary.Add(new AVM2Command("multiply", 0xa2, 0));
            _dictionary.Add(new AVM2Command("multiply_i", 0xc7, 0));
            _dictionary.Add(new AVM2Command("negate", 0x90, 0));
            _dictionary.Add(new AVM2Command("negate_i", 0xc4, 0));
            _dictionary.Add(new AVM2Command("newactivation", 0x57, 0));
            _dictionary.Add(new AVM2Command("newarray", 0x56, 1, new ParameterType[] { ParameterType.U30 }));
            _dictionary.Add(new AVM2Command("newcatch", 0x5a, 1, new ParameterType[] { ParameterType.U30 }));
            _dictionary.Add(new AVM2Command("newclass", 0x58, 1, new ParameterType[] { ParameterType.U30 }));
            _dictionary.Add(new AVM2Command("newfunction", 0x40, 1, new ParameterType[] { ParameterType.U30 }));
            _dictionary.Add(new AVM2Command("newobject", 0x55, 1, new ParameterType[] { ParameterType.U30 }));
            _dictionary.Add(new AVM2Command("nextname", 0x1e, 0));
            _dictionary.Add(new AVM2Command("nextvalue", 0x23, 0));
            _dictionary.Add(new AVM2Command("nop", 0x02, 0));
            _dictionary.Add(new AVM2Command("not", 0x96, 0));
            _dictionary.Add(new AVM2Command("pop", 0x29, 0));
            _dictionary.Add(new AVM2Command("popscope", 0x1d, 0));
            _dictionary.Add(new AVM2Command("pushbyte", 0x24, 1, new ParameterType[] { ParameterType.U8 }));
            _dictionary.Add(new AVM2Command("pushdouble", 0x2f, 1, new ParameterType[] { ParameterType.U30 }));
            _dictionary.Add(new AVM2Command("pushfalse", 0x27, 0));
            _dictionary.Add(new AVM2Command("pushint", 0x2d, 1, new ParameterType[] { ParameterType.U30 }));
            _dictionary.Add(new AVM2Command("pushnamespace", 0x31, 1, new ParameterType[] { ParameterType.U30 }));
            _dictionary.Add(new AVM2Command("pushnan", 0x28, 0));
            _dictionary.Add(new AVM2Command("pushnull", 0x20, 0));
            _dictionary.Add(new AVM2Command("pushscope", 0x30, 0));
            _dictionary.Add(new AVM2Command("pushshort", 0x25, 1, new ParameterType[] { ParameterType.U30 }));
            _dictionary.Add(new AVM2Command("pushstring", 0x2c, 1, new ParameterType[] { ParameterType.U30 }));
            _dictionary.Add(new AVM2Command("pushtrue", 0x26, 0));
            _dictionary.Add(new AVM2Command("pushuint", 0x2e, 1, new ParameterType[] { ParameterType.U30 }));
            _dictionary.Add(new AVM2Command("pushundefined", 0x21, 0));
            _dictionary.Add(new AVM2Command("pushwith", 0x1c, 0));
            _dictionary.Add(new AVM2Command("returnvalue", 0x48, 0));
            _dictionary.Add(new AVM2Command("returnvoid", 0x47, 0));
            _dictionary.Add(new AVM2Command("rshift", 0xa6, 0));
            _dictionary.Add(new AVM2Command("setlocal", 0x63, 1, new ParameterType[] { ParameterType.U30 }));
            for (int i = 0; i < 4; ++i) _dictionary.Add(new AVM2Command("set_local" + i.ToString(), (byte)(0xd4 + i), 0));
            _dictionary.Add(new AVM2Command("setglobalslot", 0x6f, 1, new ParameterType[] { ParameterType.U30 }));
            _dictionary.Add(new AVM2Command("setproperty", 0x61, 1, new ParameterType[] { ParameterType.U30 }));
            _dictionary.Add(new AVM2Command("setslot", 0x6d, 1, new ParameterType[] { ParameterType.U30 }));
            _dictionary.Add(new AVM2Command("setsuper", 0x05, 1, new ParameterType[] { ParameterType.U30 }));
            _dictionary.Add(new AVM2Command("strictequals", 0xac, 0));
            _dictionary.Add(new AVM2Command("subtract", 0xa1, 0));
            _dictionary.Add(new AVM2Command("subtract_i", 0xc6, 0));
            _dictionary.Add(new AVM2Command("swap", 0x2b, 0));
            _dictionary.Add(new AVM2Command("throw", 0x03, 0));
            _dictionary.Add(new AVM2Command("typeof", 0x95, 0));
            _dictionary.Add(new AVM2Command("urshift", 0xa7, 0));

            _dictionaryLength = (byte)_dictionary.Count;

#if USE_ACCELTABLES

            /**
             * Build accelerator tables.
             */
            foreach (AVM2Command cmd in _dictionary)
            {
                _accelTableOp[cmd.OpCode] = cmd;
                _accelTableString[cmd.StringRepresentation] = cmd;
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
#if USE_ACCELTABLES

            return _accelTableString[command].Clone();

#else

            byte i = 0;

            AVM2Command cmd;

            for (;i<_dictionaryLength;++i)
            {
                cmd = (AVM2Command)_dictionary[i];

                if (cmd.StringRepresentation == command)
                {
                    return cmd;
                }
            }

            return null;

#endif
        }
    }
}
