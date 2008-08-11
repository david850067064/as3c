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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using As3c.Common;
using SwfLibrary;
using SwfLibrary.Abc;
using SwfLibrary.Types;
using SwfLibrary.Types.Tags;

namespace As3c.Disassembler
{
    public class DisassemblerBase
    {
        /// <summary>
        /// ArrayList of string's that contain the output and can be written
        /// to the Console or a stream for instance.
        /// </summary>
        protected ArrayList _output;

        public DisassemblerBase()
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

                DoABC abcTag = (DoABC)tag.Body;

                FormatAbc(abcTag.Abc);
            }
        }

        protected virtual void FormatAbc(Abc46 abc46) { }

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
