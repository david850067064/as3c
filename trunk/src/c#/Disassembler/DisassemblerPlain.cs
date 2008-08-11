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
using SwfLibrary.Types;
using As3c.Common;
using SwfLibrary.Abc;

namespace As3c.Disassembler
{
    public class DisassemblerPlain : DisassemblerBase
    {
        public DisassemblerPlain() : base() { }

        protected override void FormatAbc(SwfLibrary.Abc.Abc46 abc)
        {
            base.FormatAbc(abc);

            MethodBodyInfo methodBody;
            AVM2Command command;

            int n = abc.MethodBodies.Count;

            for (int i = 0; i < n; ++i)
            {
                methodBody = (MethodBodyInfo)abc.MethodBodies[i];

                FormatBody(methodBody);

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

                    FormatCommand(k, command);
                }
            }
        }

        protected void FormatBody(SwfLibrary.Abc.MethodBodyInfo methodBody)
        {
            _output.Add("\r\n");   
        }

        protected void FormatCommand(uint address, AVM2Command cmd)
        {
            string output = "";

            output += String.Format("{0:X4}\t", address);
            output += cmd.StringRepresentation + "\t\t";

            if (cmd.StringRepresentation.Length < 8)
                output += "\t";

            int n = cmd.Parameters.Count;
            int m = n - 1;

            for (int i = 0; i < n; ++i)
            {
                object t = cmd.Parameters[i];

                if (t is byte) { output += String.Format("{0}", (byte)t); }
                else if (t is S24) { output += String.Format("{0}", ((S24)t).Value); }
                else if (t is U30) { output += String.Format("{0}", ((U30)t).Value); }
                else if (t is U32) { output += String.Format("{0}", ((U30)t).Value); }
                
                if (i != m)
                    output += ", ";
            }

            output += "\r\n";

            _output.Add(output);
        }
    }
}
