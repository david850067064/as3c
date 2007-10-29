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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Reflection;

using As3c.Common;
using As3c.Common.Exceptions;
using As3c.Compiler;
using As3c.Compiler.Exceptions;
using As3c.Swf;
using As3c.Swf.Abc;
using As3c.Swf.Types;
using As3c.Swf.Types.Tags;
using As3c.Decompiler;
using As3c.Swf.Abc.Constants;

namespace As3c
{
    class Program
    {
        static private string _pathInput;

        static private string _pathOutput;

        static void Main(string[] args)
        {
#if DEBUG
            DateTime start = DateTime.Now;
#endif
            try
            {
                #region Initialization

                WriteInfo();
                Console.WriteLine("As3c - ActionScript3 ASM compiler");

                Translator.InitTable();

                #endregion

                #region Integrity scan for the Translator class (DEBUG only)

#if DEBUG
                if (Translator.CheckIntegrity())
                {
                    WriteSuccess();
                    Console.WriteLine("Integrity scan passed!");
                }
                else
                {
                    WriteError();
                    Console.WriteLine("Integrity scan failed...!");
                }
#endif

                #endregion

                #region Input parameters

                if (args.Length < 2)
                {
                    Usage();
                    return;
                }

                _pathInput = args[0];

                _pathOutput = args[1];

                #endregion

                FileStream reader;
                FileStream writer;

                SwfFormat swf = new SwfFormat();

                reader = new FileStream("C:\\Test.swf", FileMode.Open);

                swf.Read(reader);

                reader.Close();

                reader.Dispose();

                //((StringInfo)swf.GetAbcAt(0).ConstantPool.StringTable[9]).Data = new byte[] { (byte)'h', (byte)'e', (byte)'l', (byte)'l', (byte)'o', (byte)' ', (byte)'w', (byte)'o', (byte)'r', (byte)'l', (byte)'d', (byte)'!' };

                if (false)
                {
                    #region write

                    writer = new FileStream("C:\\TestO.swf", FileMode.Create);

                    swf.Write(writer);

                    writer.Close();

                    writer.Dispose();

                    #endregion
                }

                if (true)
                {
                    DecompilerBase decompiler = new DecompilerLookup();

                    decompiler.Parse(swf);

                    //decompiler.EmitToConsole();

                    FileStream decompiled = File.Open("C:\\decompiled.txt", FileMode.Create);
                    decompiler.EmitToStream(decompiled);
                }

                Parser parser = null;

                parser = new Parser(PathInput);

                ByteCodeWriter bw = new ByteCodeWriter(PathOutput, parser.Instructions);

                #region Program end

                WriteInfo();
                Console.WriteLine("Done.");

                #endregion
            }
            catch (IOException IoEx)
            {
                WriteError();
                Console.WriteLine("Error: {0}", IoEx.Message);
                WriteError();
                Console.WriteLine("Please make sure to input the path to a valid SWF file.");
            }
            catch (InstructionException InstrEx)
            {
                string Message;

                switch (InstrEx.ErrorType)
                {
                    case InstructionException.Type.InvalidSyntax:
                        Message = "Error while parsing";
                        break;
                    case InstructionException.Type.NotEnoughArguments:
                        Message = "Not enough arguments";
                        break;
                    case InstructionException.Type.TooManyArguments:
                        Message = "Too many arguments";
                        break;
                    default:
                        Message = "Unknown error at";
                        break;
                }

                WriteError();
                Console.WriteLine(Message + ":");
                Console.WriteLine("{0}({1}): {2}", InstrEx.DebugInfo.FilePath, InstrEx.DebugInfo.LineNumber, InstrEx.DebugInfo.Line);
            }
            catch (Exception Ex)
            {
                WriteError();
                Console.WriteLine("Unexpected error: {0}", Ex.Message);
                WriteError();
                Console.WriteLine("Stacktrace: {0}", Ex.StackTrace);
            }
#if DEBUG
            DateTime end = DateTime.Now;
            TimeSpan delta = end - start;

            WriteInfo();
            Console.WriteLine("Total: {0}sec {1}ms", delta.Seconds, delta.Milliseconds);

            Console.ReadKey();
#endif
        }

        static private void Usage()
        {
            WriteQuestion();
            Console.WriteLine("Usage: {0} (input) (output)", Path.GetFileName(Assembly.GetEntryAssembly().Location));

            Console.WriteLine("\tinput:\tActionScript input file (*.as).");
            Console.WriteLine("\toutput:\t.as3c output file.");
        }

        protected static void WriteInfo()
        {
            Console.ResetColor();
            WriteOpen();
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("i");
            WriteClose();
            Console.Write(" ");
            Console.ResetColor();
        }

        protected static void WriteSuccess()
        {
            Console.ResetColor();
            WriteOpen();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("+");
            WriteClose();
            Console.Write(" ");
            Console.ResetColor();
        }

        protected static void WriteError()
        {
            Console.ResetColor();
            WriteOpen();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("-");
            WriteClose();
            Console.Write(" ");
            Console.ResetColor();
        }

        protected static void WriteQuestion()
        {
            Console.ResetColor();
            WriteOpen();
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("?");
            WriteClose();
            Console.Write(" ");
            Console.ResetColor();
        }

        protected static void WriteOpen()
        {
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("[");
        }

        protected static void WriteClose()
        {
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("]");
            Console.ResetColor();
        }

        public static string PathInput { get { return _pathInput; } }

        public static string PathOutput { get { return _pathOutput; } }
    }
}