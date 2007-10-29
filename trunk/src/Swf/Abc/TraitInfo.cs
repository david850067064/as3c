using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using As3c.Swf.Utils;
using As3c.Swf.Types;
using As3c.Swf.Exceptions;
using System.Collections;
using As3c.Swf.Abc.Traits;

namespace As3c.Swf.Abc
{
    public class TraitInfo : IExternalizeable
    {
        public const byte TraitSlot = 0x00;
        public const byte TraitMethod = 0x01;
        public const byte TraitGetter = 0x02;
        public const byte TraitSetter = 0x03;
        public const byte TraitClass = 0x04;
        public const byte TraitFunction = 0x05;
        public const byte TraitConst = 0x06;

        public const byte AttributeFinal = 0x01;
        public const byte AttributeOverride = 0x02;
        public const byte AttributeMetadata = 0x04;

        protected U30 _name;
        protected byte _kind;
        protected TraitBody _body;
        protected ArrayList _metadata;

        protected byte _type;
        protected byte _attr;

        public U30 Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public byte Kind
        {
            get { return _kind; }
            set { _kind = value; }
        }

        public TraitBody Body
        {
            get { return _body; }
            set { _body = value;/*TODO set type*/ }
        }

        public ArrayList Metadata
        {
            get { return _metadata; }
            set { _metadata = value; }
        }

        public byte Type
        {
            get { return _type; }
            set { _type = value;/*TODO update kind*/ }
        }

        public byte Attributes
        {
            get { return _attr; }
            set { _attr = value;/*TODO update attributes*/ }
        }

        #region IExternalizeable Members

        public void ReadExternal(BinaryReader input)
        {
            _name = Primitives.ReadU30(input);

            if (0 == (uint)_name)
            {
                throw new VerifyException("Name must not be 0.");
            }

            _kind = input.ReadByte();

            _type = (byte)(_kind & 0xf);
            _attr = (byte)((_kind >> 4) & 0xf);

            switch (_type)
            {
                case TraitSlot:
                    _body = new TraitSlot(this);
                    break;
                case TraitConst:
                    _body = new TraitConst(this);
                    break;
                case TraitClass:
                    _body = new TraitClass(this);
                    break;
                case TraitFunction:
                    _body = new TraitFunction(this);
                    break;
                case TraitMethod:
                    _body = new TraitMethod(this);
                    break;
                case TraitGetter:
                    _body = new TraitGetter(this);
                    break;
                case TraitSetter:
                    _body = new TraitSetter(this);
                    break;
                default:
                    throw new VerifyException("Unexpected trait body.");
            }

            _body.ReadExternal(input);

            if (AttributeMetadata == (_attr & AttributeMetadata))
            {
                uint n = Primitives.ReadU30(input).Value;

                _metadata = new ArrayList(Capacity.Max(n));

                for (uint i = 0; i < n; ++i)
                    _metadata.Add(Primitives.ReadU30(input));
            }
        }

        public void WriteExternal(BinaryWriter output)
        {
            Primitives.WriteU30(output, _name);

            // assume kind has been updated correctly
            output.Write(_kind);

            _body.WriteExternal(output);

            if (AttributeMetadata == (_attr & AttributeMetadata))
            {
                int n = _metadata.Count;

                Primitives.WriteU30(output, (uint)n);

                for (int i = 0; i < n; ++i)
                    Primitives.WriteU30(output, (U30)_metadata[i]);
            }
        }

        #endregion
    }
}
