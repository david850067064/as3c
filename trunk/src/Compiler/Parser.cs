using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace As3c.Compiler
{
    class Parser
    {
        protected List<Instruction> _instructions;

        public Parser(string inputPath)
        {
            StreamReader inputStream = new StreamReader(inputPath, Encoding.UTF8);

            int lineNumber = 0;

            _instructions = new List<Instruction>();

            while (!inputStream.EndOfStream)
            {
                lineNumber++;

                string line = inputStream.ReadLine();
                string lineOriginal = line;

                int commentIndex = line.IndexOf(';');

                // Remove comment if existing
                if (commentIndex != -1)
                {
                    line = line.Substring(0, commentIndex);
                }

                // Remove spaces at beginning and end.
                line = line.Trim();

                // Repalce \t characters with 0x32 IF they are not in between two quotes.
                #region Removing \t
                string lineBuffer = "";
                char lastChar = '\0';
                bool canReplace = true;
                
                for (int i = 0; i < line.Length; ++i)
                {
                    switch (line[i])
                    {
                        case '"':
                            if (lastChar != '\\')
                            {
                                canReplace = !canReplace;
                                lineBuffer += '"';
                            }
                            break;

                        case '\t':
                            if (canReplace)
                            {
                                lineBuffer += ' ';
                            }
                            else
                            {
                                lineBuffer += '\t';
                            }
                            break;

                        default:
                            lineBuffer += line[i];
                            break;
                    }

                    lastChar = line[i];
                }

                line = lineBuffer;
                #endregion

                if ("" == line) continue;

                _instructions.Add(new Instruction(line, new DebugInformation(inputPath, lineOriginal, lineNumber)));
            }

            inputStream.Close();
            inputStream.Dispose();
        }

        public List<Instruction> Instructions { get { return _instructions; } }
    }
}
