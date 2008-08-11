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

using As3c.Common;
using As3c.Compiler.Exceptions;
using SwfLibrary.Abc;
using SwfLibrary.Abc.Constants;
using SwfLibrary.Types;

namespace As3c.Compiler
{
    public class Instruction
    {
        protected ParserInformation _debugInfo;
        protected AVM2Command _cmd;
        protected List<string> _arguments;

        public Instruction(Abc46 abc, AVM2Command command, List<AVM2Command> arguments)
        {
            _cmd = command;
            _arguments = new List<string>();

            for (int i = 0; i < arguments.Count; ++i)
            {
                AVM2Command argument = arguments[i];

                switch (argument.OpCode)
                {
                    case (byte)Op.PushByte:
                        _arguments.Add(((byte)argument.Parameters[0]).ToString());
                        break;

                    case (byte)Op.PushShort:
                        _arguments.Add(((U30)argument.Parameters[0]).Value.ToString());
                        break;

                    case (byte)Op.PushDouble:
                        _arguments.Add(((double)(abc.ConstantPool.DoubleTable[(int)((U30)argument.Parameters[0])])).ToString());
                        break;

                    case (byte)Op.PushInt:
                        _arguments.Add(((S32)(abc.ConstantPool.IntTable[(int)((U30)argument.Parameters[0])])).Value.ToString());
                        Console.WriteLine("My arguments are now: {0}", _arguments);
                        break;

                    case (byte)Op.PushUInt:
                        _arguments.Add(((U32)(abc.ConstantPool.DoubleTable[(int)((U30)argument.Parameters[0])])).Value.ToString());
                        break;

                    case (byte)Op.PushString:
                        _arguments.Add(((StringInfo)(abc.ConstantPool.StringTable[(int)((U30)argument.Parameters[0])])).ToString());
                        break;

                    case (byte)Op.GetLocal0:
                        if ((byte)Op.GetLocal == _cmd.OpCode)
                            _cmd.OpCode = (byte)Op.GetLocal0;
                        else if ((byte)Op.SetLocal == _cmd.OpCode)
                            _cmd.OpCode = (byte)Op.SetLocal0;
                        else
                            _arguments.Add("0");
                        break;

                    case (byte)Op.GetLocal1:
                        if ((byte)Op.GetLocal == _cmd.OpCode)
                            _cmd.OpCode = (byte)Op.GetLocal1;
                        else if ((byte)Op.SetLocal == _cmd.OpCode)
                            _cmd.OpCode = (byte)Op.SetLocal1;
                        else
                            _arguments.Add("1");
                        break;

                    case (byte)Op.GetLocal2:
                        if ((byte)Op.GetLocal == _cmd.OpCode)
                            _cmd.OpCode = (byte)Op.GetLocal2;
                        else if ((byte)Op.SetLocal == _cmd.OpCode)
                            _cmd.OpCode = (byte)Op.SetLocal2;
                        else
                            _arguments.Add("2");
                        break;

                    case (byte)Op.GetLocal3:
                        if ((byte)Op.GetLocal == _cmd.OpCode)
                            _cmd.OpCode = (byte)Op.GetLocal3;
                        else if ((byte)Op.SetLocal == _cmd.OpCode)
                            _cmd.OpCode = (byte)Op.SetLocal3;
                        else
                            _arguments.Add("3");
                        break;

                    case (byte)Op.GetLocal:
                        _arguments.Add(((int)((U30)argument.Parameters[0])).ToString());
                        break;

                    default:
#if DEBUG
                        Console.WriteLine("[-] Parameter type {0} not handled ...", argument.StringRepresentation);
#endif
                        break;
                }
            }
        }

        public Instruction(string command, ParserInformation debugInfo)
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
                            _arguments.Add(args[i].Trim());
                        }
                    }
                }
            }

            _cmd = cmd; 
        }

        public List<string> Arguments { get { return _arguments; } }

        public AVM2Command Command { get { return _cmd; } }

        public ParserInformation DebugInfo { get { return _debugInfo; } }
    }
}
