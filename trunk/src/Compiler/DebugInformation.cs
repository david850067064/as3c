using System;
using System.Collections.Generic;
using System.Text;

namespace As3c.Compiler
{
    public class DebugInformation
    {
        private string _filePath;
        private string _line;
        private int _lineNumber;

        public DebugInformation(string filePath, string line, int lineNumber)
        {
            _filePath = filePath;
            _line = line;
            _lineNumber = lineNumber;
        }

        public string FilePath { get { return _filePath; } }
        public string Line { get { return _line; } }
        public int LineNumber { get { return _lineNumber; } }
    }
}
