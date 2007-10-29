using System;
using System.Collections.Generic;
using System.Text;

namespace As3c.Compiler.Exceptions
{
    public class InstructionException : Exception
    {
        // Invalid syntax
        public enum Type
        {
            InvalidSyntax,
            NotEnoughArguments,
            TooManyArguments
        };

        protected DebugInformation _debugInfo;
        protected Type _errorType;

        public InstructionException( Type errorType, DebugInformation debugInfo )
        {
            _errorType = errorType;
            _debugInfo = debugInfo;
        }

        public DebugInformation DebugInfo { get { return _debugInfo; } }
        public Type ErrorType { get { return _errorType; } }
    }
}
