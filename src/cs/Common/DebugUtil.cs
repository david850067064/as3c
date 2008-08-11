using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace As3c.Common
{
    public class DebugUtil
    {
        public static void Dump(byte[] bytes)
        {
            int i = 0;
            int n = bytes.Length;

            while (i < n)
            {
                Console.WriteLine("{0:D4}: {1}", i, bytes[i]);
                ++i;
            }
        }

        public static void Dump(MemoryStream stream)
        {
            long oldPosition = stream.Position;

            int i = 0;

            stream.Seek(0, SeekOrigin.Begin);

            int data;

            while (-1 != (data = stream.ReadByte()))
            {
                Console.WriteLine("{1:D4}: {0}", data, i++);
            }

            stream.Seek(oldPosition, SeekOrigin.Begin);
        }

        public static void DumpOpUntilError(byte[] code)
        {
            uint i = 0;
            uint n = (uint)code.Length;
            uint j = 0;

            while (i < n)
            {
                AVM2Command command = null;

                uint index = i;

                try
                {
                    command = Translator.ToCommand(code[i++]);
                }
                catch (Exception)
                {
                    Console.WriteLine("[-] Error occured at index {0}", index);
                    break;
                }

                i += command.ReadParameters(code, i);

                Console.WriteLine("[i] [0x{1:x4},{1:D4},#{2:D4}]: {0}", command.StringRepresentation, index, j++);
            }
        }
    }
}
