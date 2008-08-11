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
using System.Text.RegularExpressions;
using System.Collections;
using As3c.Compiler.Exceptions;

namespace As3c.Compiler
{
    class ParserAs3c
    {
        protected Dictionary<string, Label> _labels;
        protected ArrayList _instructions;

        protected string file;

        protected bool _hasMaxScopeDepth;
        protected bool _hasInitScopeDepth;
        protected bool _hasMaxStack;
        protected bool _hasLocalCount;

        protected uint _maxScopeDepth;
        protected uint _initScopeDepth;
        protected uint _maxStack;
        protected uint _localCount;

        public bool HasMaxScopeDepth { get { return _hasMaxScopeDepth; } }
        public bool HasInitScopeDepth { get { return _hasInitScopeDepth; } }
        public bool HasMaxStack { get { return _hasMaxStack; } }
        public bool HasLocalCount { get { return _hasLocalCount; } }

        public uint MaxScopeDepth { get { return _maxScopeDepth; } }
        public uint InitScopeDepth { get { return _initScopeDepth; } }
        public uint MaxStack { get { return _maxStack; } }
        public uint LocalCount { get { return _localCount; } }

        public ParserAs3c()
        {
            _labels = new Dictionary<string, Label>();
            file = null;
        }

        public void Parse(string inputPath)
        {
            file = inputPath;

            Parse(File.Open(inputPath, FileMode.Open, FileAccess.Read));
        }

        public void Parse(Stream stream)
        {
            if (null == file)
            {
                file = stream.ToString();
            }

            StreamReader inputStream = new StreamReader(stream, Encoding.UTF8);

            int lineNumber = 0;

            _instructions = new ArrayList();

            while (!inputStream.EndOfStream)
            {
                lineNumber++;

                string line = inputStream.ReadLine();
                string lineOriginal = line;

                int commentIndex = line.IndexOf(';');

                //
                // Remove comment if existing
                //
                if (commentIndex != -1)
                {
                    line = line.Substring(0, commentIndex);
                }

                //
                // Remove spaces at beginning and end.
                //
                line = line.Trim();

                //
                // Repalce \t characters with 0x32 IF they are not in between two quotes.
                //
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

                if (0 == line.IndexOf("#"))
                {
                    #region Directives

                    string[] tokens = line.Split(new char[] { ' ' }, 2);

                    switch (tokens[0].ToLower())
                    {
                        case "#initscopedepth":
                            _hasInitScopeDepth = true;
                            _initScopeDepth = Convert.ToUInt32(tokens[1]);
                            break;

                        case "#maxscopedepth":
                            _hasMaxScopeDepth = true;
                            _maxScopeDepth = Convert.ToUInt32(tokens[1]);
                            break;

                        case "#localcount":
                            _hasLocalCount = true;
                            _localCount = Convert.ToUInt32(tokens[1]);
                            break;

                        case "#maxstack":
                            _hasMaxStack = true;
                            _maxStack = Convert.ToUInt32(tokens[1]);
                            break;

                        default: //TODO throw proper exception
                            throw new Exception(String.Format("Unknown compiler directive \"{0}\".", line));
                    }

                    #endregion
                }
                else if (0 == line.IndexOf(".") && (line.Length - 1) == line.IndexOf(":"))
                {
                    string labelId = line.Substring(0, line.Length - 1);
                    Label label = new Label(labelId);

                    if (_labels.ContainsKey(labelId))
                    {
                        throw new InstructionException(InstructionException.Type.LabelRedefined, new ParserInformation(file, lineOriginal, lineNumber));
                    }

                    _labels.Add(labelId, label);
                    _instructions.Add(label);
                }
                else
                {
                    _instructions.Add(new Instruction(line, new ParserInformation(file, lineOriginal, lineNumber)));
                }
            }

            inputStream.Close();
            inputStream.Dispose();
        }

        public ArrayList Instructions { get { return _instructions; } }
        public Dictionary<string, Label> Labels { get { return _labels; } }
    }
}
