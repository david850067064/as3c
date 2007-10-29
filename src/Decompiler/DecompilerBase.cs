using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using As3c.Common;
using As3c.Swf;
using As3c.Swf.Abc;
using As3c.Swf.Types;
using As3c.Swf.Types.Tags;

namespace As3c.Decompiler
{
    public class DecompilerBase
    {
        /// <summary>
        /// ArrayList of string's that contain the output and can be written
        /// to the Console or a stream for instance.
        /// </summary>
        protected ArrayList _output;

        public DecompilerBase()
        {
            _output = new ArrayList();
        }

        public void Parse(SwfFormat swf)
        {
            foreach (Tag tag in swf.Tags)
            {
                if (tag.Header.Type != 0x52)
                {
                    continue;
                }

                MethodBodyInfo methodBody;
                AVM2Command command;
                DoABC abcTag = (DoABC)tag.Body;

                int n = abcTag.Abc.MethodBodies.Count;

                OnAbc(abcTag.Abc);

                for (int i = 0; i < n; ++i)
                {
                    methodBody = (MethodBodyInfo)abcTag.Abc.MethodBodies[i];

                    OnBody(methodBody);

                    byte[] code = methodBody.Code;

                    uint j = 0;
                    int m = code.Length;
                    uint k;

                    while (j < m)
                    {
                        k = j;
                        command = Translator.ToCommand(code[j++]);

                        if (null == command)
                        {
                            throw new Exception("Unknown opcode detected.");
                        }

                        j += command.ReadParameters(code, j);

                        OnCommand(k, command);
                    }
                }
            }
        }

        protected virtual void OnAbc(Abc46 abc46) { }

        protected virtual void OnBody(MethodBodyInfo methodBody) { }

        protected virtual void OnCommand(uint address, AVM2Command cmd) { }

        public void EmitToConsole()
        {
            foreach (string line in _output)
            {
                Console.Write(line);
            }
        }

        public void EmitToStream(Stream output)
        {
            StreamWriter writer = new StreamWriter(output, Encoding.UTF8);

            foreach (string line in _output)
            {
                writer.Write(line);
            }

            writer.Flush();
            writer.Close();
        }
    }
}
