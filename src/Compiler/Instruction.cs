using System;
using System.Collections.Generic;
using System.Text;

using As3c.Common;
using As3c.Compiler.Exceptions;

namespace As3c.Compiler
{
    public class Instruction
    {
        protected DebugInformation _debugInfo;
        protected AVM2Command _cmd;
        protected List<string> _arguments;
 
        public Instruction(string command, DebugInformation debugInfo)
        {
            _debugInfo = debugInfo;
            
            char[] separators = { ' ' };
            string[] tokens = command.Split(separators,2);

            AVM2Command cmd = Translator.ToCommand(tokens[0]);

            if (cmd == null)
            {
                throw new InstructionException(InstructionException.Type.InvalidSyntax, _debugInfo);
            }

            if (tokens.Length == 1 && cmd.ParameterCount > 0)
            {
                throw new InstructionException(InstructionException.Type.NotEnoughArguments, _debugInfo);
            }
            else
            {
                if (tokens.Length == 2)
                {
                    separators[0] = ',';
                    string[] args = tokens[1].Split(separators, 0xff);

                    if (args.Length > cmd.ParameterCount)
                    {
                        throw new InstructionException(InstructionException.Type.TooManyArguments, _debugInfo);
                    }
                    else if (args.Length < cmd.ParameterCount)
                    {
                        throw new InstructionException(InstructionException.Type.NotEnoughArguments, _debugInfo);
                    }

                    _arguments = new List<string>();

                    if ( cmd.ParameterCount != 0 )
                    {
                        for (int i = 0; i < args.Length; ++i)
                        {
                            _arguments.Add(args[i]);
                        }
                    }
                }
            }

            _cmd = cmd; 
        }

        public List<string> Arguments { get { return _arguments; } }

        public AVM2Command Command { get { return _cmd; } }

        public DebugInformation DebugInfo { get { return _debugInfo; } }
    }
}
