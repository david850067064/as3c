using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace As3c.Decompiler.Utils
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
