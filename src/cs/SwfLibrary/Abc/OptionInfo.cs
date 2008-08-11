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
using SwfLibrary.Types;
using System.Collections;

namespace SwfLibrary.Abc
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
