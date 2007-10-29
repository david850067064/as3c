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

using As3c.Common;
using As3c.Swf.Types;
using As3c.Swf.Abc;
using As3c.Swf.Abc.Constants;
using As3c.Swf.Abc.Utils;
using As3c.Decompiler.Utils;

namespace As3c.Decompiler
{
    public class DecompilerLookup : DecompilerPlain
    {
        public DecompilerLookup() : base() { _labels = new LabelUtil(); }

        protected Abc46 _abc;

        protected LabelUtil _labels;

        protected override void OnAbc(Abc46 abc46)
        {
            base.OnAbc(abc46);
            _abc = abc46;
        }

        protected override void OnBody(MethodBodyInfo methodBody)
        {
            base.OnBody(methodBody);

            _output.Add("\r\n\r\n");

            _labels.Clear();
        }

        protected override void OnCommand(uint address, AVM2Command cmd)
        {
            string output = "";

            //output += String.Format("{0:X4}\t", address);

            //output += String.Format("{0}\t", address);

            if (((byte)Op.Label == cmd.OpCode) || _labels.IsMarked(address))
            {
                output += String.Format("\r\n.label{0}:\r\n", _labels.GetLabelAt(address).id);
            }

            output += "\t" + cmd.StringRepresentation + "\t\t";

            if (cmd.StringRepresentation.Length < 8)
                output += "\t";

            int n = cmd.Parameters.Count;
            int m = n - 1;

            switch (cmd.OpCode)
            {
                // Param 1: U30 -> Multiname
                case (byte)Op.AsType:
                case (byte)Op.Coerce:
                case (byte)Op.DeleteProperty:
                case (byte)Op.FindProperty:
                case (byte)Op.FindPropertyStrict:
                case (byte)Op.GetDescendants:
                case (byte)Op.GetLex:
                case (byte)Op.GetProperty:
                case (byte)Op.GetSuper:
                case (byte)Op.InitProperty:
                case (byte)Op.IsType:
                case (byte)Op.SetProperty:
                    output += NameUtil.ResolveMultiname(_abc, GetMultiname(cmd, 0));
                    break;

                // Param 1: U30 -> Multiname
                // Param 2: Param count
                case (byte)Op.CallProperty:
                case (byte)Op.CallPropertyLex:
                case (byte)Op.CallPropertyVoid:
                case (byte)Op.CallSuper:
                case (byte)Op.CallSuperVoid:
                case (byte)Op.ConstructProperty:
                    output += NameUtil.ResolveMultiname(_abc, GetMultiname(cmd, 0));
                    output += ", " + ((U30)cmd.Parameters[1]).Value.ToString();
                    break;

                // Param 1: U30 -> MethodInfo
                // Param 2: Param count
                case (byte)Op.CallStatic:
                    output += ((StringInfo)(_abc.ConstantPool.StringTable[(int)GetMethod(cmd, 0).Name.Value])).ToString();
                    output += ", " + ((U30)cmd.Parameters[1]).Value.ToString();
                    break;

                // Param 1: U30 -> InstanceInfo
                case (byte)Op.NewClass:
                    InstanceInfo ii = (InstanceInfo)_abc.Instances[(int)((U30)cmd.Parameters[0]).Value];
                    output += NameUtil.ResolveClass(_abc, ii);
                    break;

                // Param 1: ?
                case (byte)Op.PushNamespace:
                    output += "?";
                    break;

                // Param 1: U30 -> DoubleTable
                case (byte)Op.PushDouble:
                    output += GetDouble(cmd, 0);
                    break;

                // Param 1: U30 -> IntTable
                case (byte)Op.PushInt:
                    output += GetInt(cmd, 0);
                    break;

                // Param 1: U30 -> UIntTable
                case (byte)Op.PushUInt:
                    output += GetUInt(cmd, 0);
                    break;

                // Param 1: U30 -> StringTable
                case (byte)Op.DebugFile:
                case (byte)Op.PushString:
                    output += '"' + GetString(cmd, 0).ToString() + '"';
                    break;

                // Param 1: ?
                case (byte)Op.NewFunction:
                    output += "function {}";
                    break;

                // Param 1: S24 -> Jump Offset
                case (byte)Op.IfEqual:
                case (byte)Op.IfFalse:
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
                case (byte)Op.IfTrue:
                case (byte)Op.Jump:
                    S24 offset = (S24)cmd.Parameters[0];
                    // addr + (1byte opcode) + (offset byte length) + (offset value)
                    output += String.Format(".label{0}", _labels.GetLabelAt((uint)(address + 1 + offset.Length + offset.Value)).id) + "\r\n";
                    break;
                case (byte)Op.LookupSwitch:
                    S24 defaultLabel = (S24)cmd.Parameters[0];
                    U30 count = (U30)cmd.Parameters[1];
                    
                    output += String.Format(".label{0}", _labels.GetLabelAt((uint)(address + defaultLabel.Value)).id) + ", ";

                    for (int i = 0, o = (int)count.Value+1; i < o; ++i)
                    {
                        S24 offsetLabel = (S24)cmd.Parameters[2 + i];

                        output += String.Format(".label{0}", _labels.GetLabelAt((uint)(address + offsetLabel.Value)).id);

                        if (i != o - 1)
                        {
                            output += ", ";
                        }
                    }

                    output += "\r\n";

                    break;

                default:
                    for (int i = 0; i < n; ++i)
                    {
                        object t = cmd.Parameters[i];

                        if (t is byte) { output += String.Format("{0}", (byte)t); }
                        else if (t is S24) { output += String.Format("{0}", ((S24)t).Value); }
                        else if (t is U30) { output += String.Format("{0}", ((U30)t).Value); }
                        else if (t is U32) { output += String.Format("{0}", ((U32)t).Value); }
                        
                        if (i != m)
                            output += ", ";
                    }
                    break;
            }

            output += "\r\n";

            _output.Add(output);
        }

        protected int GetInt(AVM2Command cmd, int paramIndex)
        {
            return ((S32)_abc.ConstantPool.IntTable[(int)((U30)cmd.Parameters[paramIndex]).Value]).Value;
        }

        protected uint GetUInt(AVM2Command cmd, int paramIndex)
        {
            return ((U32)_abc.ConstantPool.UIntTable[(int)((U30)cmd.Parameters[paramIndex]).Value]).Value;
        }

        protected double GetDouble(AVM2Command cmd, int paramIndex)
        {
            return (double)_abc.ConstantPool.DoubleTable[(int)((U30)cmd.Parameters[paramIndex]).Value];
        }

        protected MultinameInfo GetMultiname(AVM2Command cmd, int paramIndex)
        {
            return (MultinameInfo)(_abc.ConstantPool.MultinameTable[(int)((U30)cmd.Parameters[paramIndex]).Value]);
        }

        protected NamespaceInfo GetNamespace(AVM2Command cmd, int paramIndex)
        {
            return (NamespaceInfo)(_abc.ConstantPool.NamespaceTable[(int)((U30)cmd.Parameters[paramIndex]).Value]);
        }

        protected StringInfo GetString(AVM2Command cmd, int paramIndex)
        {
            return (StringInfo)(_abc.ConstantPool.StringTable[(int)((U30)cmd.Parameters[paramIndex]).Value]);
        }

        protected MethodInfo GetMethod(AVM2Command cmd, int paramIndex)
        {
            return (MethodInfo)_abc.Methods[(int)((U30)cmd.Parameters[paramIndex]).Value];
        }
    }
}
