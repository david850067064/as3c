using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using As3c.Swf.Utils;
using As3c.Swf.Types;

namespace As3c.Swf.Abc
{
    public class ExceptionInfo : IExternalizeable
    {
        protected U30 _from;
        protected U30 _to;
        protected U30 _target;
        protected U30 _execType;
        protected U30 _varName;

        public U30 From
        {
            get { return _from; }
            set { _from = value; }
        }

        public U30 To
        {
            get { return _to; }
            set { _to = value; }
        }

        public U30 Target
        {
            get { return _target; }
            set { _target = value; }
        }

        public U30 ExecutionType
        {
            get { return _execType; }
            set { _execType = value; }
        }

        public U30 VariableName
        {
            get { return _varName; }
            set { _varName = value; }
        }

        #region IExternalizeable Members

        public void ReadExternal(BinaryReader input)
        {
            _from = Primitives.ReadU30(input);
            _to = Primitives.ReadU30(input);
            _target = Primitives.ReadU30(input);
            _execType = Primitives.ReadU30(input);
            _varName = Primitives.ReadU30(input);
        }

        public void WriteExternal(BinaryWriter output)
        {
            Primitives.WriteU30(output, _from);
            Primitives.WriteU30(output, _to);
            Primitives.WriteU30(output, _target);
            Primitives.WriteU30(output, _execType);
            Primitives.WriteU30(output, _varName);
        }

        #endregion
    }
}
