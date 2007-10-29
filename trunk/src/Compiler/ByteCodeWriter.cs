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
