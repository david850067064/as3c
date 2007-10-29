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

namespace As3c.Compiler
{
    public class ByteCodeWriter
    {
        public ByteCodeWriter(string outputPath, List<Instruction> instructions)
        {
            FileStream writer = new FileStream(outputPath, FileMode.Create);
            BinaryWriter output = new BinaryWriter(writer, Encoding.UTF8);

            foreach (Instruction instruction in instructions)
            {
                output.BaseStream.WriteByte(instruction.Command.OpCode);

                if (0 < instruction.Command.ParameterCount)
                {
                    foreach (string argument in instruction.Arguments)
                    {
                        output.BaseStream.WriteByte((byte)argument[0]);
                    }
                }
            }

            output.Close();
            
            writer.Close();
            writer.Dispose();
        }
    }
}
