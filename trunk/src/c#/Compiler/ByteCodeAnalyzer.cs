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
using As3c.Swf.Types;
using As3c.Common;
using As3c.Swf.Abc;

namespace As3c.Compiler
{
    //TODO merge 3 loops into 1...
    class ByteCodeAnalyzer
    {
        public static uint InvalidStack = 0;

        private static uint _flags;

        public static uint Flags
        {
            get { return _flags; }
        }

        private static void ClearFlags()
        {
            _flags = 0;
        }

        private static void RaiseFlag(uint flag)
        {
            _flags |= flag;
        }

        public static U30 CalcLocalCount(byte[] code)
        {
            ClearFlags();

            uint i = 0;
            uint n = (uint)code.Length;

            uint local = 0;
            uint maxLocal = 0;

            while (i < n)
            {
                AVM2Command cmd = Translator.ToCommand(code[i++]);

                if (null == cmd)
                    throw new Exception();

                i += cmd.ReadParameters(code, i);

                switch (cmd.OpCode)
                {
                    case (byte)Op.Debug:
                        byte type = (byte)cmd.Parameters[0];
                        if (1 == type)
                        {
                            local = (byte)cmd.Parameters[2] + 1U;
                        }
                        break;
                    case (byte)Op.SetLocal:
                    case (byte)Op.GetLocal:
                        U30 register = (U30)cmd.Parameters[0];
                        local = register.Value;
                        break;

                    case (byte)Op.SetLocal0:
                    case (byte)Op.GetLocal0:
                        local = 1;
                        break;

                    case (byte)Op.SetLocal1:
                    case (byte)Op.GetLocal1:
                        local = 2;
                        break;

                    case (byte)Op.SetLocal2:
                    case (byte)Op.GetLocal2:
                        local = 3;
                        break;

                    case (byte)Op.SetLocal3:
                    case (byte)Op.GetLocal3:
                        local = 4;
                        break;
                }

                if (local > maxLocal)
                    maxLocal = local;
            }

            U30 result = new U30();
            result.Value = maxLocal + 1;

            return result;
        }

        public static U30 CalcScopeDepth(byte[] code)
        {
            ClearFlags();

            uint i = 0;
            uint n = (uint)code.Length;

            int scopeDepth = 0;
            int maxScopeDepth = 0;

            while (i < n)
            {
                AVM2Command cmd = Translator.ToCommand(code[i++]);

                if (null == cmd)
                    throw new Exception();

                i += cmd.ReadParameters(code, i);

                switch (cmd.OpCode)
                {
                    case (byte)Op.PushScope:
                    case (byte)Op.PushWith:
                        scopeDepth++;
                        break;

                    case (byte)Op.PopScope:
                        scopeDepth--;
                        break;
                }

                if (scopeDepth > maxScopeDepth)
                    maxScopeDepth = scopeDepth;
            }

            U30 result = new U30();
            result.Value = (uint)maxScopeDepth;

            return result;
        }

        public static U30 CalcMaxStack(byte[] code)
        {
            ClearFlags();

            uint i = 0;
            uint n = (uint)code.Length;

            int stack = 0;
            int maxStack = 0;

            while (i < n)
            {
                AVM2Command cmd = Translator.ToCommand(code[i++]);

                if (null == cmd)
                    throw new Exception();

                i += cmd.ReadParameters(code, i);

                // There are a couple of opcodes marked "incorrect" with a comment.
                // The explanation is: If the index in the multiname array is a RTQName
                // there could be a namspace and/or namespace set on the stack as well
                // that would be popped. 
                //
                // We do not take that in account here - in the worst case that a namespace
                // and namespace set is present it could add +2 to the max sack if the
                // stack is greater than the one we already have.
                //
                // In the calculation of the possible max stack we will therefore only remove
                // the number of arguments from the current value. If there are no arguments
                // the opcode will be listed here as incorrect without any following calculation.
                //
                // Although this is not a problem for the Flash Player. It is just not very
                // nice...
                switch (cmd.OpCode)
                {
                    case (byte)Op.PushByte:
                    case (byte)Op.PushDouble:
                    case (byte)Op.PushFalse:
                    case (byte)Op.PushInt:
                    case (byte)Op.PushNamespace:
                    case (byte)Op.PushNaN:
                    case (byte)Op.PushNull:
                    case (byte)Op.PushShort:
                    case (byte)Op.PushString:
                    case (byte)Op.PushTrue:
                    case (byte)Op.PushUInt:
                    case (byte)Op.PushUndefined:
                    case (byte)Op.Dup:
                    case (byte)Op.FindProperty://incorrect
                    case (byte)Op.FindPropertyStrict://incorrect
                    case (byte)Op.GetGlobalScope:
                    case (byte)Op.GetGlobalSlot:
                    case (byte)Op.GetLex:
                    case (byte)Op.GetLocal:
                    case (byte)Op.GetLocal0:
                    case (byte)Op.GetLocal1:
                    case (byte)Op.GetLocal2:
                    case (byte)Op.GetLocal3:
                    case (byte)Op.GetScopeObject:
                    case (byte)Op.HasNext2:
                    case (byte)Op.NewActivation:
                    case (byte)Op.NewCatch:
                    case (byte)Op.NewFunction:
                        stack++;
                        break;

                    case (byte)Op.Add:
                    case (byte)Op.AddInt:
                    case (byte)Op.AsTypeLate:
                    case (byte)Op.BitAnd:
                    case (byte)Op.BitOr:
                    case (byte)Op.BitXor:
                    case (byte)Op.Divide:
                    case (byte)Op.DefaultXmlNamespaceLate:
                    case (byte)Op.Equals:
                    case (byte)Op.GreaterEquals:
                    case (byte)Op.GreaterThan:
                    case (byte)Op.HasNext:
                    case (byte)Op.IfFalse:
                    case (byte)Op.IfTrue:
                    case (byte)Op.In:
                    case (byte)Op.InstanceOf:
                    case (byte)Op.IsTypeLate:
                    case (byte)Op.LessEquals:
                    case (byte)Op.LessThan:
                    case (byte)Op.LookupSwitch:
                    case (byte)Op.LeftShift:
                    case (byte)Op.Modulo:
                    case (byte)Op.Multiply:
                    case (byte)Op.MultiplyInt:
                    case (byte)Op.NextName:
                    case (byte)Op.NextValue:
                    case (byte)Op.Pop:
                    case (byte)Op.PushScope://pop from stack, push to scope stack
                    case (byte)Op.PushWith://pop from stack, push to scope stack
                    case (byte)Op.ReturnValue:
                    case (byte)Op.RightShift:
                    case (byte)Op.SetLocal:
                    case (byte)Op.SetLocal0:
                    case (byte)Op.SetLocal1:
                    case (byte)Op.SetLocal2:
                    case (byte)Op.SetLocal3:
                    case (byte)Op.SetGlobalSlot:
                    case (byte)Op.StrictEquals:
                    case (byte)Op.Subtract:
                    case (byte)Op.SubtractInt:
                    case (byte)Op.Throw:
                    case (byte)Op.UnsignedRightShift:
                        stack--;
                        break;

                    case (byte)Op.IfEqual:
                    case (byte)Op.IfGreaterEqual:
                    case (byte)Op.IfGreaterThan:
                    case (byte)Op.IfLessEqual:
                    case (byte)Op.IfLowerThan:
                    case (byte)Op.IfNotEqual:
                    case (byte)Op.IfNotGreaterEqual:
                    case (byte)Op.IfNotGreaterThan:
                    case (byte)Op.IfNotLowerEqual:
                    case (byte)Op.IfNotLowerThan:
                    case (byte)Op.IfStrictEqual:
                    case (byte)Op.IfStrictNotEqual:
                    case (byte)Op.InitProperty:
                    case (byte)Op.SetProperty://incorrect
                    case (byte)Op.SetSlot:
                    case (byte)Op.SetSuper://incorrect
                        stack -= 2;
                        break;

                    case (byte)Op.Call:
                    case (byte)Op.ConstructSuper:
                        stack -= 1 + (int)((U30)cmd.Parameters[0]);
                        break;

                    case (byte)Op.Construct:
                    case (byte)Op.NewArray:
                        stack -= (int)((U30)cmd.Parameters[0]);
                        break;

                    case (byte)Op.CallMethod:
                    case (byte)Op.CallProperty://incorrect
                    case (byte)Op.CallPropertyLex://incorrect
                    case (byte)Op.CallPropertyVoid://incorrect
                    case (byte)Op.CallStatic:
                    case (byte)Op.CallSuper://incorrect
                    case (byte)Op.CallSuperVoid://incorrect
                    case (byte)Op.ConstructProperty:
                        stack -= (int)((U30)cmd.Parameters[1]);
                        break;

                    case (byte)Op.NewObject:
                        stack -= ((int)((U30)cmd.Parameters[0])) << 1;
                        break;

                    //case (byte)Op.DeleteProperty://incorrect
                    //case (byte)Op.GetDescendants://incorrect
                    //case (byte)Op.GetProperty://incorrect
                    //case (byte)Op.GetSuper://incorrect
                    //    break;
                }

                if (stack < 0)
                {
                    RaiseFlag(InvalidStack);
                    Console.WriteLine("[-] Warning: Stack underflow error ...");
                }

                if (stack > maxStack)
                    maxStack = stack;
            }

            U30 result = new U30();
            result.Value = (uint)maxStack;

            return result;
        }
    }
}
