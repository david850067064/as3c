using System;
using System.Collections.Generic;
using System.Text;
using SwfLibrary;
using SwfLibrary.Abc;
using As3c.Common;
using As3c.Disassembler.Utils;
using SwfLibrary.Types;
using System.Collections;
using SwfLibrary.Abc.Utils;
using SwfLibrary.Abc.Constants;

namespace As3c.Compiler
{
    public class CompilerOptimize
    {
        protected SwfFormat _swf;
        protected LabelUtil _labelUtil;

        public CompilerOptimize(SwfFormat swf)
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
                    PatchBody(abc, (MethodBodyInfo)abc.MethodBodies[j], j);
                }
            }
        }

        private void PatchBody(Abc46 abc, MethodBodyInfo body, int bodyId)
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
            bool ignoreBody = false;

            _labelUtil.Clear();

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
                        S24 offset = (S24)command.Parameters[0];
                        command.Parameters[0] = String.Format(":{0}:", _labelUtil.GetLabelAt((uint)(addr + 1 + offset.Length + offset.Value)).id);
                        instructions.Add(command);
                        break;

                    case (byte)Op.GetLex:
                        name = NameUtil.ResolveMultiname(abc, (MultinameInfo)(abc.ConstantPool.MultinameTable[(int)((U30)command.Parameters[0]).Value]));
                        if ("public::Math" == name || "Math" == name)
                            patchBody = true;
                        instructions.Add(command);
                        break;

                    case (byte)Op.LookupSwitch:
                        ignoreBody = true;
                        instructions.Add(command);
                        break;

                    default:
                        instructions.Add(command);
                        break;
                }
            }

            if (ignoreBody)
            {
#if DEBUG
                Console.WriteLine("[-] Have to ignore body because of lookupswitch ...");
#endif
            }

            if (patchBody && !ignoreBody)
            {
                //
                // We have to patch this function ...
                // Note: We do not change the initScopeDepth
                //

#if DEBUG
                Console.WriteLine("[i] Patching body (id: {0})", bodyId);
#endif
                CompilerAs3c compAs3c = new CompilerAs3c();

                compAs3c.Compile(abc, instructions, labels, true);

                // Now .. only patch if we find a correct stack!

                U30 maxStack = maxStack = ByteCodeAnalyzer.CalcMaxStack(compAs3c.Code);

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
