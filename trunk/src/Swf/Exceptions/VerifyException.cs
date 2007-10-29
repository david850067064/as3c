using System;
using System.Collections.Generic;
using System.Text;

namespace As3c.Swf.Exceptions
{
    class VerifyException : Exception
    {
        public VerifyException(string message) : base(message) { }
    }
}
