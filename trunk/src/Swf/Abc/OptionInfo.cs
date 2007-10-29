using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using As3c.Swf.Utils;
using As3c.Swf.Types;
using System.Collections;

namespace As3c.Swf.Abc
{
    public class OptionInfo : IExternalizeable
    {
        protected ArrayList _optionDetail;

        public ArrayList Detail
        {
            get { return _optionDetail; }
            set { _optionDetail = value; }
        }

        #region IExternalizeable Members

        public void ReadExternal(BinaryReader input)
        {
            uint n = Primitives.ReadU30(input).Value;

            _optionDetail = new ArrayList(Capacity.Max(n));

            for (uint i = 0; i < n; ++i)
            {
                OptionDetail detail = new OptionDetail();
                detail.ReadExternal(input);

                _optionDetail.Add(detail);
            }
        }

        public void WriteExternal(BinaryWriter output)
        {
            int n = _optionDetail.Count;

            Primitives.WriteU30(output, (uint)n);

            for (int i = 0; i < n; ++i)
                ((OptionDetail)_optionDetail[i]).WriteExternal(output);
        }

        #endregion
    }
}
