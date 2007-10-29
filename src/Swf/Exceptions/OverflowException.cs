using System;
using System.Collections.Generic;
using System.Text;

namespace As3c.Swf.Exceptions
{
    class OverflowException : Exception
    {
        public OverflowException() { }
        public OverflowException(string message) : base(message) { }
    }
}
