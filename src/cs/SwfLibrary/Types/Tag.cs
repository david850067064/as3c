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

using SwfLibrary.Utils;
using SwfLibrary.Types.Tags;

namespace SwfLibrary.Types
{
    public class Tag : IExternalizeable
    {
        protected RecordHeader _header;
        protected TagBody _body;
        protected SwfFormat _parent;

        public RecordHeader Header
        {
            get { return _header; }
            set { _header = value; }
        }

        public TagBody Body
        {
            get { return _body; }
            set { _body = value; }
        }

        public Tag(SwfFormat parent)
        {
            _parent = parent;
        }

        #region IExternalizeable Members

        public void ReadExternal(BinaryReader input)
        {
            _header = new RecordHeader();
            _header.ReadExternal(input);

            switch ( _header.Type )
            {
                case 0x52:
                    _body = new DoABC(this);
                    _parent.AddAbc((DoABC)_body);
                    break;

                case 0x45://FileAttributes
                default:
                    _body = new DefaultBody(this);
                    break;
            }

            
            _body.ReadExternal(input);
        }

        public void WriteExternal(BinaryWriter output)
        {
            if (0x52 == _header.Type)//DoABC
            {
                DoABC abcBody = (DoABC)_body;
                
                MemoryStream buffer = new MemoryStream();
                BinaryWriter bufferOut = new BinaryWriter(buffer, Encoding.UTF8);

                // Part 1: Write .abc to buffer
                abcBody.WriteExternal(bufferOut);

                // Part 2: Update and write header
                _header.Length = (int)buffer.Length;
                _header.WriteExternal(output);

                // Part 3: Copy .abc buffer into output
                buffer.Seek(0, SeekOrigin.Begin);

                byte[] bb = new byte[1024];
                int len;

                while ((len = buffer.Read(bb, 0, 1024)) > 0)
                    output.Write(bb, 0, len);
            }
            else
            {
                // Other tags can be written very simple
                _header.WriteExternal(output);
                _body.WriteExternal(output);
            }
        }

        #endregion
    }
}
