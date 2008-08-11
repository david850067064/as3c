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

using SwfLibrary.Types;
using SwfLibrary.Utils;

namespace SwfLibrary.Abc
{
    public class Abc46 : IExternalizeable
    {
        public const ushort VERSION_MINOR = 16;
        public const ushort VERSION_MAJOR = 46;

        protected uint _length;

        protected string _name;
        protected ushort _minor;
        protected ushort _major;
        protected ConstantPool _cpool;
        protected ArrayList _methods;
        protected ArrayList _metadata;
        protected ArrayList _instanceInfo;
        protected ArrayList _classInfo;
        protected ArrayList _scriptInfo;
        protected ArrayList _methodBodyInfo;

        public uint Length
        {
            get { return _length; }
            set { _length = value; }
        }

        public uint Version
        {
            get { return (uint)((_major << 16) | _minor); }
            set
            {
                _major = (ushort)((value & 0xffff0000) >> 16);
                _minor = (ushort)(value & 0x0000ffff);
            }
        }

        public ushort MajorVersion
        {
            get { return _major; }
            set { _major = value; }
        }

        public ushort MinorVersion
        {
            get { return _minor; }
            set { _minor = value; }
        }

        public ConstantPool ConstantPool
        {
            get { return _cpool; }
            set { _cpool = value; }
        }

        public ArrayList Methods
        {
            get { return _methods; }
            set { _methods = value; }
        }

        public ArrayList Metadata
        {
            get { return _metadata; }
            set { _metadata = value; }
        }

        public ArrayList Instances
        {
            get { return _instanceInfo; }
            set { _instanceInfo = value; }
        }

        public ArrayList Classes
        {
            get { return _classInfo; }
            set { _classInfo = value; }
        }

        public ArrayList Scripts
        {
            get { return _scriptInfo; }
            set { _scriptInfo = value; }
        }

        public ArrayList MethodBodies
        {
            get { return _methodBodyInfo; }
            set { _methodBodyInfo = value; }
        }

        #region IExternalizeable Members

        public void ReadExternal(BinaryReader input)
        {
            #region frame

            char chr;
            _name = "";
            
            while ((chr = (char)input.ReadByte()) != 0)
            {
                _name += chr;
            }

            #endregion

            #region version

            _minor = input.ReadUInt16();
            _major = input.ReadUInt16();

            if (_major > VERSION_MAJOR)
            {
                throw new Exception(String.Format("Unsupported .abc format {0}.{1}.", MajorVersion, MinorVersion));
            }

            #endregion

            #region cpool_info

            _cpool = new ConstantPool();
            _cpool.ReadExternal(input);

            #endregion

            #region method_info

            uint n = Primitives.ReadU30(input).Value;

            _methods = new ArrayList(Capacity.Max(n));
            
            for (uint i = 0; i < n; ++i)
            {
                MethodInfo method = new MethodInfo();
                method.ReadExternal(input);

                _methods.Add(method);
            }

            #endregion

            #region metadata_info

            n = Primitives.ReadU30(input).Value;

            _metadata = new ArrayList(Capacity.Max(n));

            for (uint i = 0; i < n; ++i)
            {
                MetadataInfo metadata = new MetadataInfo();
                metadata.ReadExternal(input);

                _metadata.Add(metadata);
            }

            #endregion

            #region instance_info

            n = Primitives.ReadU30(input).Value;

            _instanceInfo = new ArrayList(Capacity.Max(n));
            
            for (uint i = 0; i < n; ++i)
            {
                InstanceInfo instanceInfo = new InstanceInfo();
                instanceInfo.ReadExternal(input);

                _instanceInfo.Add(instanceInfo);
            }

            #endregion

            #region class_info

            //n is same as for instance_info

            _classInfo = new ArrayList(Capacity.Max(n));
            
            for (uint i = 0; i < n; ++i)
            {
                ClassInfo classInfo = new ClassInfo();
                classInfo.ReadExternal(input);

                _classInfo.Add(classInfo);
            }

            #endregion

            #region script_info

            n = Primitives.ReadU30(input).Value;

            _scriptInfo = new ArrayList(Capacity.Max(n));
            
            for (uint i = 0; i < n; ++i)
            {
                ScriptInfo scriptInfo = new ScriptInfo();
                scriptInfo.ReadExternal(input);

                _scriptInfo.Add(scriptInfo);
            }

            #endregion

            #region method_body_info

            n = Primitives.ReadU30(input).Value;

            _methodBodyInfo = new ArrayList(Capacity.Max(n));

            for (uint i = 0; i < n; ++i)
            {
                MethodBodyInfo methodBodyInfo = new MethodBodyInfo();
                methodBodyInfo.ReadExternal(input);

                _methodBodyInfo.Add(methodBodyInfo);
            }

            #endregion
        }

        public void WriteExternal(BinaryWriter output)
        {
            #region frame

            for (int i = 0; i < _name.Length; ++i)
                output.Write(_name[i]);

            output.Write((byte)0);

            #endregion

            #region version

            output.Write(_minor);
            output.Write(_major);

            #endregion

            #region cpool

            _cpool.WriteExternal(output);

            #endregion

            #region method_info

            int n = _methods.Count;

            Primitives.WriteU30(output, (uint)n);

            for (int i = 0; i < n; ++i)
                ((MethodInfo)_methods[i]).WriteExternal(output);

            #endregion

            #region metadata_info

            n = _metadata.Count;

            Primitives.WriteU30(output, (uint)n);

            for (int i = 0; i < n; ++i)
                ((MetadataInfo)_metadata[i]).WriteExternal(output);

            #endregion metadata_info

            #region instance_info

            n = _instanceInfo.Count;

            Primitives.WriteU30(output, (uint)n);

            for (int i = 0; i < n; ++i)
                ((InstanceInfo)_instanceInfo[i]).WriteExternal(output);

            #endregion

            #region class_info

            for (int i = 0; i < n; ++i)
                ((ClassInfo)_classInfo[i]).WriteExternal(output);

            #endregion

            #region script_info

            n = _scriptInfo.Count;

            Primitives.WriteU30(output, (uint)n);

            for (int i = 0; i < n; ++i)
                ((ScriptInfo)_scriptInfo[i]).WriteExternal(output);

            #endregion

            #region method_body_info

            n = _methodBodyInfo.Count;

            Primitives.WriteU30(output, (uint)n);

            for (int i = 0; i < n; ++i)
                ((MethodBodyInfo)_methodBodyInfo[i]).WriteExternal(output);

            #endregion
        }

        #endregion
    }
}
