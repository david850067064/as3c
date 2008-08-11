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
using SwfLibrary.Types;
using SwfLibrary.Abc;
using SwfLibrary.Abc.Constants;
using SwfLibrary.Abc.Utils;
using As3c.Disassembler.Utils;
using System.Collections;
using SwfLibrary.Abc.Traits;
using SwfLibrary.Utils;

namespace As3c.Disassembler
{
    public class DisassemblerAs3c : DisassemblerBase
    {
        public DisassemblerAs3c() : base() { _labels = new LabelUtil(); }

        protected Abc46 _abc;

        protected LabelUtil _labels;

        protected bool _isFirst;

        protected override void FormatAbc(Abc46 abc46)
        {
            base.FormatAbc(abc46);

            _abc = abc46;

            _output.Add(";namespaces:\r\n");

            for (int i = 0, n = _abc.ConstantPool.NamespaceTable.Count; i < n; ++i)
            {
                NamespaceInfo ns = (NamespaceInfo)_abc.ConstantPool.NamespaceTable[i];

                string result = "";

                switch (ns.Kind)
                {
                    case NamespaceInfo.Namespace:
                    case NamespaceInfo.ExplicitNamespace:
                        //TODO implement this
                        //user defined
                        break;
                    case NamespaceInfo.PrivateNs:
                        result = "private";
                        break;
                    case NamespaceInfo.ProtectedNamespace:
                        result = "protected";
                        break;
                    case NamespaceInfo.StaticProtectedNs:
                        result = "protected$";
                        break;
                    case NamespaceInfo.PackageInternalNs:
                        result = "internal";
                        break;
                    case NamespaceInfo.PackageNamespace:
                        result = "public";
                        break;
                    default:
                        result = "*";
                        break;
                    //throw new VerifyException("Unexpected namespace kind.");
                }

                result += "::";
                result += ((StringInfo)_abc.ConstantPool.StringTable[(int)ns.Name.Value]).ToString();

                _output.Add(String.Format(";id{0}\t\t{1}\r\n", i, result));
            }

            _output.Add(";multinames:\r\n");

            for (int i = 0, n = abc46.ConstantPool.MultinameTable.Count; i < n; ++i)
            {
                _output.Add(String.Format(";id{0}\t\t{1}\r\n", i, NameUtil.ResolveMultiname(abc46, i)));
            }

            _output.Add(";strings\r\n");

            for (int i = 0, n = abc46.ConstantPool.StringTable.Count; i < n; ++i)
            {
                _output.Add(String.Format(";id{0}\t\t{1}\r\n", i, ((StringInfo)abc46.ConstantPool.StringTable[i]).ToString()));
            }

            for (int i = 0, n = _abc.Scripts.Count; i < n; ++i)
            {
                FormatScript(i);
            }
        }

        protected void FormatScript(int index)
        {
            ScriptInfo info = (ScriptInfo)_abc.Scripts[index];

            _output.Add(String.Format(";script{0}\r\n", index));

            //TODO format init method info.Init

            FormatTraits(info);

            _output.Add("\r\n");
        }

        protected void FormatMethod(U30 method)
        {
            MethodInfo methodInfo = (MethodInfo)_abc.Methods[(int)method];
            MethodBodyInfo methodBody = null;

            int bodyIndex = 0;

            for (int i = 0, n = _abc.MethodBodies.Count; i < n; ++i)
            {
                methodBody = (MethodBodyInfo)_abc.MethodBodies[i];

                if (methodBody.Method.Value == method.Value)
                {
                    bodyIndex = i;
                    break;
                }
            }

            _output.Add(String.Format(";body{0}\r\n",bodyIndex));
            FormatBody(methodBody);
        }

        protected void FormatBody(MethodBodyInfo body)
        {
            byte[] code = body.Code;

            AVM2Command command;
            uint i = 0;
            int n = code.Length;
            uint k;

            _output.Add("\r\n#maxStack " + body.MaxStack.Value + "\r\n");
            _output.Add("#localCount " + body.LocalCount.Value + "\r\n");
            _output.Add("#maxScopeDepth " + body.MaxScopeDepth.Value + "\r\n");
            _output.Add("#initScopeDepth " + body.InitScopeDepth.Value + "\r\n\r\n");

            while (i < n)
            {
                k = i;

                try
                {
                    command = Translator.ToCommand(code[i++]);
                }
                catch (Exception)
                {
                    throw new Exception(String.Format("Command {0} is not understood.", code[i - 1]));
                }

                if (null == command)
                {
                    throw new Exception("Unknown opcode detected.");
                }

                i += command.ReadParameters(code, i);

                FormatCommand(k, command);
            }

            _output.Add("\r\n\r\n");
        }

        protected void FormatTraits(IHasTraits traitsObject)
        {
            ArrayList traitList = traitsObject.Traits;
            TraitInfo trait;

            for (int i = 0, n = traitList.Count; i < n; ++i)
            {
                trait = (TraitInfo)traitList[i];

                if (trait.Body is TraitClass)
                {
                    TraitClass classBody = (TraitClass)trait.Body;

                    #region Class information

                    ClassInfo classInfo = (ClassInfo)_abc.Classes[(int)classBody.ClassI];
                    InstanceInfo instanceInfo = (InstanceInfo)_abc.Instances[(int)classBody.ClassI];

                    string classDefinition = "";

                    if (0 != (instanceInfo.Flags & InstanceInfo.ClassInterface))
                    {
                        classDefinition += "interface ";
                    }
                    else
                    {
                        if (0 != (instanceInfo.Flags & InstanceInfo.ClassFinal))
                        {
                            classDefinition += "final ";
                        }

                        if (0 == (instanceInfo.Flags & InstanceInfo.ClassSealed))
                        {
                            classDefinition += "dynamic ";
                        }

                        classDefinition += "class ";
                    }

                    //TODO add protected namespace lookup

                    classDefinition += NameUtil.ResolveMultiname(_abc, (int)instanceInfo.Name) + " extends " + NameUtil.ResolveMultiname(_abc, (int)instanceInfo.SuperName) + " ";

                    if (instanceInfo.Interfaces.Count > 0)
                    {
                        classDefinition += "implements ";

                        for (int j = 0, m = instanceInfo.Interfaces.Count; j < m; ++j)
                        {
                            classDefinition += NameUtil.ResolveMultiname(_abc, (U30)instanceInfo.Interfaces[j]);

                            if (j != m - 1)
                            {
                                classDefinition += ", ";
                            }
                        }
                    }

                    _output.Add(";" + classDefinition + "\r\n");

                    _output.Add(";cinit (static class initializer)\r\n");

                    FormatMethod(classInfo.CInit);

                    _output.Add(";class members\r\n");

                    FormatTraits(classInfo);

                    _output.Add(";iinit (instance initializer aka constructor)\r\n");

                    FormatMethod(instanceInfo.IInit);

                    _output.Add(";instance members\r\n");

                    FormatTraits(instanceInfo);

                    #endregion
                }
                else if (trait.Body is TraitConst)
                {
                }
                else if (trait.Body is TraitFunction)
                {
                }
                else if ((trait.Body is TraitMethod)||(trait.Body is TraitGetter)||(trait.Body is TraitSetter))
                {
                    TraitMethod methodBody = (TraitMethod)trait.Body;

                    _output.Add(";" + NameUtil.ResolveMultiname(_abc, trait.Name) + "\r\n");

                    FormatMethod(methodBody.Method);
                }
                else
                {
                }
            }
        }

        protected void FormatCommand(uint address, AVM2Command cmd)
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
                
                case (byte)Op.GetSuper:
                case (byte)Op.InitProperty:
                case (byte)Op.IsType:
                case (byte)Op.SetProperty:
                    output += NameUtil.ResolveMultiname(_abc, GetMultiname(cmd, 0));
                    break;

                case (byte)Op.GetProperty:
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
                    output += GetNamespace(cmd, 0);
                    break;

                // Param 1: U30 -> DoubleTable
                case (byte)Op.PushDouble:
                    //TODO fix this and do not use replace...
                    output += GetDouble(cmd, 0).ToString().Replace(',', '.');
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
                    output += ((U30)cmd.Parameters[0]).Value + " ;call to anonymous method " + ((U30)cmd.Parameters[0]).Value;
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
