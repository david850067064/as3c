using System;
using System.Collections.Generic;
using System.Text;
using As3c.Swf.Types;
using As3c.Common;

namespace As3c.Decompiler
{
    public class DecompilerPlain : DecompilerBase
    {
        public DecompilerPlain() : base() { }

        protected override void OnBody(As3c.Swf.Abc.MethodBodyInfo methodBody)
        {
            _output.Add("\r\n");   
        }

        protected override void OnCommand(uint address, AVM2Command cmd)
        {
            string output = "";

            output += String.Format("{0:X4}\t", address);
            output += cmd.StringRepresentation + "\t\t";

            if (cmd.StringRepresentation.Length < 8)
                output += "\t";

            int n = cmd.Parameters.Count;
            int m = n - 1;

            for (int i = 0; i < n; ++i)
            {
                object t = cmd.Parameters[i];

                if (t is byte) { output += String.Format("{0}", (byte)t); }
                else if (t is S24) { output += String.Format("{0}", ((S24)t).Value); }
                else if (t is U30) { output += String.Format("{0}", ((U30)t).Value); }
                else if (t is U32) { output += String.Format("{0}", ((U30)t).Value); }
                
                if (i != m)
                    output += ", ";
            }

            output += "\r\n";

            _output.Add(output);
        }
    }
}
