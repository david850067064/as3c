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

using SwfLibrary.Abc;
using SwfLibrary.Types;
using SwfLibrary.Types.Tags;

using zlib;

namespace SwfLibrary
{
    public class SwfFormat
    {
        // Header Part 1 (before any compression)
        protected Header _header;

        // Header Part 2 (could be compressed already)
        protected RECT _frameSize;
        protected float _frameRate;
        protected ushort _frameCount;

        protected ArrayList _tags;

        protected ArrayList _abcTags;

        public RECT FrameSize
        {
            get { return _frameSize; }
            set { _frameSize = value; }
        }

        public float FrameRate
        {
            get { return _frameRate; }
            set { _frameRate = value; }
        }

        public ArrayList Tags
        {
            get { return _tags; }
        }

        public SwfFormat()
        {
            _abcTags = new ArrayList();
        }

        internal void AddAbc(DoABC abcTag)
        {
            _abcTags.Add(abcTag);
        }

        public int AbcCount
        {
            get { return _abcTags.Count; }
        }

        public Abc46 GetAbcAt(int index)
        {
            return ((DoABC)_abcTags[index]).Abc;
        }

        public void Read(Stream stream)
        {
            BinaryReader input = new BinaryReader(stream, Encoding.UTF8);
            MemoryStream buffer = null;

            // Part 1
            _header = new Header();
            _header.ReadExternal(input);

            if (_header.IsCompressed)
            {
                int len;
                byte[] bb = new byte[1024];

                buffer = new MemoryStream();

                /* TODO can we do something about this zlib crap? */

                ZOutputStream zo = new ZOutputStream(buffer);
                
#if DEBUG
                Console.WriteLine("[i] Decompressing ...");
#endif
                while ((len = input.Read(bb, 0, 1024)) > 0)
                {
                    // Careful ... This is getting stuck in an endless loop if Z_BEST_COMPRESSION was used when
                    // compressing the output.
                    zo.Write(bb, 0, len);
                }

#if DEBUG
                Console.WriteLine("[+] Decompressed");
#endif

                zo.Flush();

                input.Close();

                buffer.Seek(0, SeekOrigin.Begin);

                input = new BinaryReader(buffer, Encoding.UTF8);
            }

            // Part 2
            _frameSize = new RECT();
            _frameSize.ReadExternal(input);

            // 8.8 fixed value ??
            // TODO verify
            byte lo = input.ReadByte();
            byte hi = input.ReadByte();
            _frameRate = hi + (lo * 0.01f);

            _frameCount = input.ReadUInt16();

            _tags = new ArrayList();

            while (true)
            {
                Tag tag = new Tag(this);

                tag.ReadExternal(input);

                _tags.Add(tag);

                if (0x00 == tag.Header.Type)
                {
                    break;
                }
            }

            input.Close();

            if (null != buffer)
            {
                buffer.Close();
                buffer.Dispose();
            }
        }

        public void Write(Stream stream)
        {
            BinaryWriter output = new BinaryWriter(stream, Encoding.UTF8);
            MemoryStream buffer = new MemoryStream();
            BinaryWriter bufferOut;

            Console.WriteLine("BAD! SETTING COMPRESSION TO FALSE ALWAYS FOR OUTPUT CURRENTLY!");
            _header.IsCompressed = false;

            // Part 1: Write header basics)
            output.Write(_header.Signature);
            output.Write(_header.Version);

            bufferOut = new BinaryWriter(buffer, Encoding.UTF8);
            
            // Part 2: Write tags into buffer
            _frameSize.WriteExternal(bufferOut);

            // 8.8 fixed ??
            // TODO verify
            float d = _frameRate - ((byte)(_frameRate));
            d *= 100;

            bufferOut.Write((byte)d);
            bufferOut.Write((byte)_frameRate);

            bufferOut.Write(_frameCount);

            foreach (Tag tag in _tags)
            {
                tag.WriteExternal(bufferOut);
            }

            // Part 3: Update header size
            output.Write((uint)((uint)buffer.Length + 8U));


            // Part 4: Write buffer (compressed) to the main stream
            buffer.Seek(0, SeekOrigin.Begin);

            byte[] bb = new byte[1024];
            int len;

            if (_header.IsCompressed)
            {
                /* TODO can we do something about this zlib crap? */

                // Careful! zlib.net can not read when itself compresses using Z_BEST_COMPRESSION
                // So since zlib.Inflate was getting stuck in and endless lopp I decided to use
                // Z_DEFAULT_COMPRESSION which has been working fine for me now ...
                
                ZOutputStream zOut = new ZOutputStream(stream, zlibConst.Z_DEFAULT_COMPRESSION );

                while ((len = buffer.Read(bb, 0, 1024)) > 0)
                {
                    zOut.Write(bb, 0, len);
                }

                zOut.Flush();
                zOut.finish();
            }
            else
            {
                while ((len = buffer.Read(bb, 0, 1024)) > 0)
                {
                    output.Write(bb, 0, len);
                }
            }

            // Part 4: Finalize
            bufferOut.Close();
            buffer.Dispose();

            output.Close();
        }
    }
}
