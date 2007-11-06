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
using As3c.Swf.Abc;
using System.Collections;
using As3c.Compiler.Exceptions;
using As3c.Common;
using As3c.Swf.Types;
using As3c.Swf.Abc.Utils;

namespace As3c.Compiler
{
    public class CompilerAs3c
    {
        private struct ReplaceInformation
        {
            public uint address;
            public Label label;
            public bool lookUpSwitch;

            public ReplaceInformation(uint address, Label label, bool lookUpSwitch)
            {
                this.address = address;
                this.label = label;
                this.lookUpSwitch = lookUpSwitch;
            }
        }

        protected byte[] _code;

        public CompilerAs3c() { }

        public void Compile(Abc46 abc, ArrayList instructions, Dictionary<string, Label> labels)
        {
            //
            // Create buffer
            //

            MemoryStream buffer = new MemoryStream();
            BinaryWriter output = new BinaryWriter(buffer);

            //
            // Convert compiler instructions to IL.
            //
            Instruction instruction;
            Label label;
            List<ReplaceInformation> replaceList = new List<ReplaceInformation>();

            for (int i = 0, n = instructions.Count; i < n; ++i)
            {
                if (instructions[i] is Label)
                {
                    label = (Label)instructions[i];

                    label.Address = (uint)buffer.Position;

                    if (!label.Referenced)
                    {
                        output.Write((byte)Op.Label);
                    }
                }
                else if (instructions[i] is Instruction)
                {
                    instruction = (Instruction)instructions[i];

                    output.Write(instruction.Command.OpCode);

                    switch (instruction.Command.OpCode)
                    {
                        case (byte)Op.PushShort:
                            Primitives.WriteU30(output, (U30)Convert.ToInt32(instruction.Arguments[0]));
                            break;

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
                            Primitives.WriteU30(output, NameUtil.GetMultiname(abc, instruction.Arguments[0]));
                            break;

                        case (byte)Op.CallProperty:
                        case (byte)Op.CallPropertyLex:
                        case (byte)Op.CallPropertyVoid:
                        case (byte)Op.CallSuper:
                        case (byte)Op.CallSuperVoid:
                        case (byte)Op.ConstructProperty:
                            Primitives.WriteU30(output, NameUtil.GetMultiname(abc, instruction.Arguments[0]));
                            Primitives.WriteU30(output, Convert.ToUInt32(instruction.Arguments[1]));
                            break;

                        case (byte)Op.NewClass:
                            Primitives.WriteU30(output, NameUtil.GetClass(abc, instruction.Arguments[0]));
                            break;

                        case (byte)Op.PushDouble:
                            Primitives.WriteU30(output, (U30)abc.ConstantPool.ResolveDouble(Convert.ToDouble(instruction.Arguments[0].Replace('.', ','))));
                            break;

                        case (byte)Op.PushInt:
                            Primitives.WriteU30(output, (U30)abc.ConstantPool.ResolveInt((S32)Convert.ToInt32(instruction.Arguments[0])));
                            break;

                        case (byte)Op.PushUInt:
                            Primitives.WriteU30(output, (U30)abc.ConstantPool.ResolveUInt((U32)Convert.ToUInt32(instruction.Arguments[0])));
                            break;

                        case (byte)Op.DebugFile:
                        case (byte)Op.PushString:
                            Primitives.WriteU30(output, (U30)abc.ConstantPool.ResolveString(instruction.Arguments[0].Substring(1,instruction.Arguments[0].Length - 2)));
                            break;

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

                            //
                            // Solve label offset
                            //

                            string labelId = instruction.Arguments[0];

                            //
                            // Make sure the label exsists.
                            //

                            if (!labels.ContainsKey(labelId))
                                throw new InstructionException(InstructionException.Type.LabelMissing, instruction.DebugInfo);

                            label = labels[labelId];

                            if (label.HasAddress)
                            {
                                //
                                // We already have the label address. This is a negative jump offset.
                                //

                                int offset = (int)(label.Address - ((uint)buffer.Position + 3));
                                Primitives.WriteS24(output, offset);
                            }
                            else
                            {
                                //
                                // We do not know the label address. This is a positive jump offset.
                                // Mark label to be solved afterwards and write a placeholder ...
                                //

                                replaceList.Add(new ReplaceInformation((uint)buffer.Position, label, false));
                                Primitives.WriteS24(output, 0);
                            }

                            label.Referenced = true;
                            break;

                        default:
                            if (0 < instruction.Command.ParameterCount)
                            {
                                try
                                {
                                    foreach (string argument in instruction.Arguments)
                                    {
                                        output.Write(Convert.ToByte(argument));
                                    }
                                }
                                catch (Exception)
                                {
                                    throw new InstructionException(InstructionException.Type.UnknownType, instruction.DebugInfo);
                                }
                            }
                            break;
                    }
                }
            }

            //
            // Solve remainig labels
            //

            for (int i = 0, n = replaceList.Count; i < n; ++i)
            {
                ReplaceInformation replaceInfo = replaceList[i];

                label = replaceInfo.label;

                if (!label.Referenced || !label.HasAddress)
                {
                    Console.WriteLine("[-] Warning: Label {0} never defined ...", label.Identifier);
                    continue;
                }

                if (replaceInfo.lookUpSwitch)
                {
                    //
                    // LookUpSwitch with special offset calculation
                    //

                    throw new Exception("IMPLEMENT ME!");
                }
                else
                {
                    //
                    // Simple jump
                    //

                    int offset = (int)(label.Address - ((uint)replaceInfo.address + 3));

                    buffer.Seek(replaceInfo.address, SeekOrigin.Begin);
                    Primitives.WriteS24(output, offset);
                }
            }

            //
            // Convert stream to byte[]
            //

            buffer.Seek(0, SeekOrigin.Begin);

            BinaryReader reader = new BinaryReader(buffer);
            
            _code = reader.ReadBytes((int)buffer.Length);


            //
            // Clean up
            //

            reader.Close();
            buffer.Dispose();
        }

        public byte[] Code
        {
            get { return _code; }
        }
    }
}
