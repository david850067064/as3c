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
using SwfLibrary.Abc;
using System.Collections;
using As3c.Compiler.Exceptions;
using As3c.Common;
using SwfLibrary.Types;
using SwfLibrary.Abc.Utils;
using SwfLibrary.Abc.Constants;

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

        public void Compile(Abc46 abc, ArrayList instructions, Dictionary<string, Label> labels, bool patchMath)
        {
#if DEBUG
            //Console.WriteLine("[i] Starting to compile method body ...");
#endif
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

            bool hasLocalMath = false;
            uint mathRegister = 0;
            U30 mathMultiName = new U30();
            AVM2Command mathCommand = null;
            AVM2Command lastCommand = null;

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
                else if (instructions[i] is AVM2Command)
                {
                    AVM2Command command = (AVM2Command)instructions[i];

                    output.Write((byte)command.OpCode);

                    switch (command.OpCode)
                    {
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
                            string labelId = (string)command.Parameters[0];

                            try
                            {
                                label = labels[labelId];
                            }
                            catch(Exception)
                            {
                                Console.WriteLine("[-] WARNING: Jumping to an unknown label");
                                continue;
                            }

                            if (label.HasAddress)
                            {
                                int offset = (int)(label.Address - ((uint)buffer.Position + 3));
                                Primitives.WriteS24(output, offset);
                            }
                            else
                            {
                                replaceList.Add(new ReplaceInformation((uint)buffer.Position, label, false));
                                Primitives.WriteS24(output, 0);
                            }

                            label.Referenced = true;
                            break;

                        case (byte)Op.GetLex:
                            if (patchMath)
                            {
                                string lexName = NameUtil.ResolveMultiname(abc, (MultinameInfo)(abc.ConstantPool.MultinameTable[(int)((U30)command.Parameters[0]).Value]));

                                //
                                // If getlex public::Math is called we will repalce it with 
                                // a get_local N where Math will be stored.
                                //

                                if (("public::Math" == lexName || "Math" == lexName) && (null != lastCommand) && (lastCommand.OpCode != (byte)Op.SetLocal))
                                {
#if DEBUG
                                    Console.WriteLine("[i] Found call to Math class ...");
#endif

                                    if (!hasLocalMath)
                                    {
                                        mathMultiName = (U30)command.Parameters[0];

                                        U30 currentMaxLocal = ByteCodeAnalyzer.CalcLocalCount(instructions);

#if DEBUG
                                        Console.WriteLine(String.Format("[i] Math register will be {0}", currentMaxLocal.Value));
#endif

                                        if (currentMaxLocal.Value < 4)
                                        {
                                            int val = (int)currentMaxLocal.Value;
                                            mathCommand = Translator.ToCommand((byte)(0xd0 + val));
                                        }
                                        else
                                        {
                                            mathCommand = Translator.ToCommand((byte)Op.GetLocal);
                                            mathCommand.Parameters.Add(currentMaxLocal);
                                        }

                                        mathRegister = currentMaxLocal.Value;

                                        hasLocalMath = true;
                                    }

                                    output.Seek(-1, SeekOrigin.Current);
                                    output.Write((byte)mathCommand.OpCode);

                                    if (mathCommand.OpCode == (byte)Op.GetLocal)
                                        mathCommand.WriteParameters(output);
                                }
                                else
                                {
                                    command.WriteParameters(output);
                                }
                            }
                            else
                            {
                                command.WriteParameters(output);
                            }
                            break;

                        default:
                            command.WriteParameters(output);
                            break;
                    }

                    lastCommand = command;
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
                        case (byte)Op.PushNamespace:
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
                            if (instruction.Arguments[0].StartsWith("\"") && instruction.Arguments[0].EndsWith("\""))//TODO fix ugly hack
                            {
                                Primitives.WriteU30(output, (U30)abc.ConstantPool.ResolveString(instruction.Arguments[0].Substring(1, instruction.Arguments[0].Length - 2)));
                            }
                            else
                            {
                                Primitives.WriteU30(output, (U30)abc.ConstantPool.ResolveString(instruction.Arguments[0]));
                            }
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

                            string labelId = null;

                            try
                            {
                                labelId = instruction.Arguments[0];
                            }
                            catch (Exception)
                            {
                                Console.WriteLine("[-] WARNING: Jumping to an unknown label");
                                continue;
                            }

                            //
                            // Make sure the label exsists.
                            //

                            if (!labels.ContainsKey(labelId))
                            {
#if DEBUG
                                Console.WriteLine("[-] Label \"{0}\" is missing ...", labelId);
#endif
                                throw new InstructionException(InstructionException.Type.LabelMissing, instruction.DebugInfo);
                            }

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
                                        output.Write((byte)Convert.ToByte(argument));
                                    }
                                }
                                catch (Exception)
                                {
                                    throw new InstructionException(InstructionException.Type.UnknownType, instruction.DebugInfo);
                                }
                            }
                            break;
                    }

                    lastCommand = instruction.Command;
                }
            }

            int labelOffset = 0;

            #region Add automatically generated code

            //
            // Add local variable containing Math
            //

            if (patchMath && hasLocalMath)
            {
#if DEBUG
                Console.WriteLine("[i] Body has local Math");
#endif

                int mathAddress = 2;

                int lastOpCode = -1;
                int opCode = -1;

                long lastPosition = output.BaseStream.Position;

                output.BaseStream.Seek(0, SeekOrigin.Begin);

                while(((opCode = output.BaseStream.ReadByte()) != -1))
                {
                    if (lastOpCode == (byte)Op.GetLocal0 && opCode == (byte)Op.PushScope)
                    {
                        break;
                    }

                    lastOpCode = opCode;
                }


                mathAddress = (int)output.BaseStream.Position;

                output.BaseStream.Seek(lastPosition, SeekOrigin.Begin);

#if DEBUG
                //Console.WriteLine("[i] Dump Before:");
                //DebugUtil.Dump((MemoryStream)output.BaseStream);
#endif

                InsertByte(output, mathAddress);
                InsertByte(output, mathAddress);
                
                labelOffset += 2;

                int byteCoundMulti = 0;

                while (byteCoundMulti < mathMultiName.Length)
                {
                    byteCoundMulti++;
                    labelOffset++;
                    InsertByte(output, mathAddress);
                }
                

                if (mathRegister > 3)
                {
#if DEBUG
                    Console.WriteLine("[i] Adding extra register");
#endif
                    int byteCount = 0;

                    while (byteCount < ((U30)mathRegister).Length+1)
                    {
                        ++byteCount;
                        ++labelOffset;
                        InsertByte(output, mathAddress);
                    }
                }

                
                output.Seek(mathAddress, SeekOrigin.Begin);

                AVM2Command getLexCommand = Translator.ToCommand((byte)Op.GetLex);
                getLexCommand.Parameters.Add(mathMultiName);

                output.Write((byte)getLexCommand.OpCode);
                getLexCommand.WriteParameters(output);

                if (mathRegister > 3)
                {
                    AVM2Command setLocalCommand = Translator.ToCommand((byte)Op.SetLocal);
                    setLocalCommand.Parameters.Add((U30)mathRegister);

                    output.Write((byte)setLocalCommand.OpCode);
                    setLocalCommand.WriteParameters(output);
                }
                else
                {
                    output.Write((byte)(0xd4 + mathRegister));
                }
                

#if DEBUG
                //Console.WriteLine("[i] Dump After:");
                //DebugUtil.Dump((MemoryStream)output.BaseStream);
#endif

                output.Seek(0, SeekOrigin.End);
                output.Flush();
            }

            #endregion

            //
            // Solve remainig labels
            //

            for (int i = 0, n = replaceList.Count; i < n; ++i)
            {
                ReplaceInformation replaceInfo = replaceList[i];

                label = replaceInfo.label;

                if (!label.Referenced || !label.HasAddress)
                {
                    Console.WriteLine("[-] Warning: Label {0} has never been defined or used ...", label.Identifier);
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

                    buffer.Seek(replaceInfo.address + labelOffset, SeekOrigin.Begin);
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

#if DEBUG
            //Console.WriteLine("[i] Done compiling method body ...");
#endif
        }

        private void InsertByte(BinaryWriter writer, int address)
        {
            int b0, b1;

            long oldPosition = writer.BaseStream.Position;

            writer.Seek(0, SeekOrigin.End);

            writer.Write((byte)0xff);

            while (writer.BaseStream.Position > (address+1))
            {
                writer.Seek(-1, SeekOrigin.Current);
                b0 = writer.BaseStream.ReadByte();
                writer.Seek(-2, SeekOrigin.Current);
                b1 = writer.BaseStream.ReadByte();
                writer.Seek(-1, SeekOrigin.Current);
                writer.Write((byte)b0);
                writer.Write((byte)b1);
                writer.Seek(-1, SeekOrigin.Current);
            }

            writer.BaseStream.Seek(oldPosition, SeekOrigin.Begin);
        }

        public byte[] Code
        {
            get { return _code; }
        }
    }
}
