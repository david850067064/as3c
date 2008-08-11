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

namespace SwfLibrary.Types
{
    public class RecordHeader : IExternalizeable
    {
        protected int _type;
        protected int _length;

#if DEBUG
        protected bool _lengthRead;
#endif

        public int Type
        {
            get { return _type; }
            set { _type = value; }
        }

        public int Length
        {
            get { return _length; }
            set { _length = value; }
        }

        #region IExternalizeable Members

        public void ReadExternal(BinaryReader input)
        {
            ushort head = input.ReadUInt16();

            if ((head & 0x3f) == 0x3f)
            {
#if DEBUG
                _lengthRead = true;
#endif
                _length = input.ReadInt32();
            }
            else
            {
#if DEBUG
                _lengthRead = false;
#endif
                _length = (head & 0x3f);
            }

            _type = (head & 0xffc0) >> 6;
        }

        public void WriteExternal(BinaryWriter output)
        {
            bool writeLength = false;

#if DEBUG
            writeLength = (_length >= 0x3f || _lengthRead);
#else
            writeLength = _length >= 0x3f;
#endif

            ushort head = (ushort)((ushort)(_type << 6) & (ushort)0xffc0);
            head |= (writeLength) ? (ushort)0x3f : (ushort)_length;

            output.Write(head);

            if (writeLength)
            {
                output.Write(_length);
            }
        }

        #endregion

        override public string ToString()
        {
            switch (_type)
            {
                case 0x00: return "[End]";
                case 0x01: return "[ShowFrame]";
                case 0x02: return "[DefineShape]";
                case 0x04: return "[PlaceObject]";
                case 0x05: return "[RemoveObject]";
                case 0x06: return "[DefineBits]";
                case 0x07: return "[DefineButton]";
                case 0x08: return "[JPEGTables]";
                case 0x09: return "[SetBackgroundColor]";
                case 0x0a: return "[DefineFont]";
                case 0x0b: return "[DefineText]";
                case 0x0c: return "[DoAction]";
                case 0x0d: return "[DefineFontInfo]";
                case 0x0e: return "[DefineSound]";
                case 0x0f: return "[StartSound]";
                case 0x11: return "[DefineButtonSound]";
                case 0x12: return "[SoundStreamHead]";
                case 0x13: return "[SoundStreamBlock]";
                case 0x14: return "[DefineBitsLossless]";
                case 0x15: return "[DefineBitsJPEG2]";
                case 0x16: return "[DefineShape2]";
                case 0x17: return "[DefineButtonCxform]";
                case 0x18: return "[Protect]";
                case 0x1a: return "[PlaceObject2]";
                case 0x1c: return "[RemoveObject2]";
                case 0x20: return "[DefineShape3]";
                case 0x21: return "[DefineText2]";
                case 0x22: return "[DefineButton2]";
                case 0x23: return "[DefineBitsJPEG3]";
                case 0x24: return "[DefineBitsLossless2]";
                case 0x25: return "[DefineEditText]";
                case 0x27: return "[DefineSprite]";
                case 0x2b: return "[FrameLabel]";
                case 0x2d: return "[SoundStreamHead2]";
                case 0x2e: return "[DefineMorphShape]";
                case 0x30: return "[DefineFont2]";
                case 0x38: return "[ExportAssets]";
                case 0x39: return "[ImportAssets]";
                case 0x3a: return "[EnableDebugger]";
                case 0x3b: return "[DoInitAction]";
                case 0x3c: return "[DefineVideoStream]";
                case 0x3d: return "[VideoFrame]";
                case 0x3e: return "[DefineFontInfo2]";
                case 0x40: return "[EnableDebugger2]";
                case 0x41: return "[ScriptLimits]";
                case 0x42: return "[SetTabIndex]";
                case 0x45: return "[FileAttributes]";
                case 0x46: return "[PlaceObject3]";
                case 0x47: return "[ImportAssets2]";
                case 0x49: return "[DefineFontAlignZones]";
                case 0x4a: return "[CSMTextSettings]";
                case 0x4b: return "[DefineFont3]";
                case 0x4c: return "[SymbolClass]";
                case 0x4d: return "[Metadata]";
                case 0x4e: return "[DefineScalingGrid]";
                case 0x52: return "[DoABC]";
                case 0x53: return "[DefineShape4]";
                case 0x54: return "[DefineMorphShape2]";
                case 0x56: return "[DefineSceneAndFrameLabelData]";
                case 0x57: return "[DefineBinaryData]";
                case 0x58: return "[DefineFontName]";
                case 0x59: return "[StartSound2]";
                default: return "{UnkownTag}";
            }
        }
    }
}
