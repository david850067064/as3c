using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace As3c.Swf.Utils
{
    public interface IExternalizeable
    {
        void ReadExternal(BinaryReader input);
        void WriteExternal(BinaryWriter output);
    }
}
