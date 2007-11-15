using System;
using System.Collections.Generic;
using System.Text;
using As3c.Swf;
using As3c.Swf.Abc;
using As3c.Common;
using System.Collections;
using As3c.Disassembler.Utils;
using As3c.Swf.Types;
using As3c.Swf.Abc.Constants;
using As3c.Swf.Abc.Utils;

namespace As3c.Compiler
{
    public class CompilerInline
    {
        protected const string inlineWrapper = "public::de.popforge.asm::__asm";
        protected const string inlineKeyword = "public::de.popforge.asm::Op";

        protected SwfFormat _swf;
        protected LabelUtil _labelUtil;

        public CompilerInline(SwfFormat swf)
        {
            _swf = swf;
            _labelUtil = new LabelUtil();
        }
        
        public void Compile()
        {
            
            for (int i = 0, n = _swf.AbcCount; i < n; ++i)
            {
                Abc46 abc = _swf.GetAbcAt(i);

                for (int j = 0, m = abc.MethodBodies.Count; j < m; ++j)
                {
                    PatchBody(abc, (MethodBodyInfo)abc.MethodBodies[j]);
                }
            }
        }

        protected void PatchBody(Abc46 abc, MethodBodyInfo body)
        {
            AVM2Command command;
            AVM2Command inlineCommand;
            Dictionary<string, Label> labels = new Dictionary<string, Label>();
            ArrayList instructions = new ArrayList();
            Label label;
            string labelId;

            byte[] il = body.Code;

            uint i = 0;
            uint n = (uint)il.Length;
            
            uint addr;

            string name;

            bool patchBody = false;
            //bool parseInline = false;

            _labelUtil.Clear();


            //
            // We will convert the bytecode into a format that the As3c compiler can understand.
            // When converting labels we will use a ':' in front of them instead of a '.' char for
            // the ones "defined" by the ASC so that we have unique names (label syntax is .label)
            //

            while (i < n)
            {
                addr = i;

                if (_labelUtil.IsMarked(addr))
                {
                    labelId = String.Format(":{0}:", _labelUtil.GetLabelAt(addr).id);
                    label = new Label(labelId);

                    labels.Add(labelId, label);
                    instructions.Add(label);
                }

                command = Translator.ToCommand(il[i++]);

                if (null == command)
                {
                    throw new Exception("Unknown opcode detected.");
                }

                i += command.ReadParameters(il, i);

                switch (command.OpCode)
                {
                    case (byte)Op.Label:
                        
                        labelId = String.Format(":{0}:", _labelUtil.GetLabelAt(addr).id);
                        label = new Label(labelId);

                        labels.Add(labelId, label);
                        instructions.Add(label);
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
                        // Convert label offset to label reference ...
                        S24 offset = (S24)command.Parameters[0];
                        command.Parameters[0] = String.Format(":{0}:",_labelUtil.GetLabelAt((uint)(addr + 1 + offset.Length + offset.Value)).id);
                        instructions.Add(command);
                        break;

                    case (byte)Op.FindPropertyStrict:
                        name = NameUtil.ResolveMultiname(abc, (MultinameInfo)(abc.ConstantPool.MultinameTable[(int)((U30)command.Parameters[0]).Value]));
                        if (inlineWrapper == name)
                        {
                            bool parse = true;
                            patchBody = true;

#if DEBUG
                            Console.WriteLine("[i] Parsing inline block in method {0}", body.Method.Value);
#endif
                            while (parse && i < n)
                            {
                                inlineCommand = Translator.ToCommand(il[i++]);

                                if (null == command)
                                {
                                    throw new Exception("Unknown opcode detected.");
                                }

                                i += inlineCommand.ReadParameters(il, i);

                                switch (inlineCommand.OpCode)
                                {
                                    //
                                    // Debug instructions are kept even if they are in an inline block
                                    //

                                    case (byte)Op.Debug:
                                    case (byte)Op.DebugFile:
                                    case (byte)Op.DebugLine:
                                        instructions.Add(inlineCommand);
                                        break;

                                    //
                                    // Strings are treated as labels -- but make sure it is actually one!
                                    //

                                    case (byte)Op.PushString:

                                        labelId = ((StringInfo)abc.ConstantPool.StringTable[(int)((U30)inlineCommand.Parameters[0])]).ToString();

                                        if (labelId.IndexOf('.') != 0 || labelId.IndexOf(':') != (labelId.Length - 1))
                                        {
                                            throw new Exception("Invalid string {0} in an inline block. Labels have the format \".labelName:\"");
                                        }

                                        labelId = labelId.Substring(0, labelId.Length - 1);
                                        label = new Label(labelId);

                                        labels.Add(labelId, label);
                                        instructions.Add(label);
                                        break;

                                    //
                                    // GetLex is the one that will bring public::de.popforge.asm::Op on the stack
                                    //

                                    case (byte)Op.GetLex:
                                        name = NameUtil.ResolveMultiname(abc, (MultinameInfo)(abc.ConstantPool.MultinameTable[(int)((U30)inlineCommand.Parameters[0]).Value]));

                                        if (inlineKeyword != name)
                                        {
                                            throw new Exception("Malformed inline block. GetLex call with invalid parameters");
                                        }

                                        List<AVM2Command> args = new List<AVM2Command>();
                                        AVM2Command userCommand = null;
                                        uint argc;

                                        while (i < n)
                                        {
                                            AVM2Command arg = Translator.ToCommand(il[i++]);

                                            if (null == arg)
                                            {
                                                throw new Exception("Unknown opcode detected.");
                                            }

                                            i += arg.ReadParameters(il, i);

                                            if ((byte)Op.CallProperty == arg.OpCode)
                                            {
                                                name = NameUtil.ResolveMultiname(abc, (MultinameInfo)(abc.ConstantPool.MultinameTable[(int)((U30)arg.Parameters[0]).Value]));
                                                userCommand = Translator.ToCommand(name, true);
                                                argc = ((U30)arg.Parameters[1]).Value;

                                                if (null == userCommand)
                                                {
                                                    throw new Exception(String.Format("Unknown command {0}.", name));
                                                }

                                                break;
                                            }

                                            args.Add(arg);
                                        }

                                        if (null == userCommand)
                                        {
                                            throw new Exception("Malformed inline block.");
                                        }

                                        instructions.Add(new Instruction(abc, userCommand, args));

                                        break;

                                    case (byte)Op.CallPropertyVoid:
                                        name = NameUtil.ResolveMultiname(abc, (MultinameInfo)(abc.ConstantPool.MultinameTable[(int)((U30)inlineCommand.Parameters[0]).Value]));
                                        if (inlineWrapper == name)
                                        {
                                            parse = false;
                                        }
                                        else
                                        {
                                            throw new Exception("Malformed inline block. Method calls are not accepted ...");
                                        }
                                        break;
                                }
                            }

#if DEBUG
                            Console.WriteLine("[+] Inline block parsed");
#endif
                        }
                        else
                        {
                            instructions.Add(command);
                        }
                        break;

                    default:
                        instructions.Add(command);
                        break;
                }
            }

            if (patchBody)
            {
                //
                // We have to patch this function ...
                // Note: We do not change the initScopeDepth
                //

#if DEBUG
                Console.WriteLine("[i] Patching body");
#endif
                CompilerAs3c compAs3c = new CompilerAs3c();
                compAs3c.Compile(abc, instructions, labels);

                // Now .. only patch if we find a correct stack!

                U30 maxStack = ByteCodeAnalyzer.CalcMaxStack(compAs3c.Code);

                if (0 == (ByteCodeAnalyzer.Flags & ByteCodeAnalyzer.InvalidStack))
                {
                    MethodInfo method = (MethodInfo)abc.Methods[(int)body.Method.Value];

                    body.MaxStack = maxStack;
                    body.MaxScopeDepth = body.InitScopeDepth + ByteCodeAnalyzer.CalcScopeDepth(compAs3c.Code);

                    U30 minLocalCount = method.ParameterCount;
                    U30 maxLocalCount = ByteCodeAnalyzer.CalcLocalCount(compAs3c.Code);

                    if (maxLocalCount.Value > minLocalCount.Value)
                    {
                        body.LocalCount = ByteCodeAnalyzer.CalcLocalCount(compAs3c.Code);
                    }
                    //else <- we would have unused parameters in a function...

                    body.Code = compAs3c.Code;
                }
                else
                {
                    //
                    // What else? We will display warnings automatically but what about
                    // telling the guy in which function he has an invalid stack?
                    //
                }

#if DEBUG
                Console.WriteLine("[+] Body patched");
#endif
            }
        }
    }
}
