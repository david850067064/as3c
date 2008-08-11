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
using System.Collections;

namespace As3c.Disassembler.Utils
{
    public class LabelUtil
    {
        public struct Label
        {
            public uint address;
            public uint id;
        };

        protected ArrayList _labels;
        protected uint _labelCount;

        public LabelUtil()
        {
            _labels = new ArrayList();
            _labelCount = 0;
        }

        public bool IsMarked(uint address)
        {
            for (int i = 0, n = _labels.Count; i < n; ++i)
            {
                if (address == ((LabelUtil.Label)_labels[i]).address)
                {
                    return true;
                }
            }

            return false;
        }

        public LabelUtil.Label GetLabelAt(uint address)
        {
            for (int i = 0, n = _labels.Count; i < n; ++i)
            {
                if (address == ((LabelUtil.Label)_labels[i]).address)
                {
                    return (LabelUtil.Label)_labels[i];
                }
            }

            LabelUtil.Label newLabel = new LabelUtil.Label();

            newLabel.address = address;
            newLabel.id = _labelCount++;

            _labels.Add(newLabel);

            return newLabel;
        }

        public void Clear()
        {
            _labels.Clear();
            _labelCount = 0;
        }
    }
}
