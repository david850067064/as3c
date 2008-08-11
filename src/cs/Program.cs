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
using SwfLibrary;
using SwfLibrary.Abc;
using SwfLibrary.Types;
using SwfLibrary.Types.Tags;
using As3c.Disassembler;
using SwfLibrary.Abc.Constants;
using SwfLibrary.Abc.Traits;
using SwfLibrary.Abc.Utils;

namespace As3c
{
    class Program
    {
        private enum Action
        {
            Usage,
            Replace,
            Inline,
            Dasm,
            Optimize
        };

        private enum Dasm
        {
            As3c,
            Plain
        };

        // Path variables
        private static string _pathOutput = "";
        private static string _pathSwf = "";
        private static string _pathAsm = "";

        // Output mode
        private static bool _isQuiet = false;

        // Action
        private static Action _action = Action.Usage;

        // Replace properties
        private static bool _isMethodStatic = false;
        private static bool _isConstructor = false;
        private static string _methodName = "";
        private static string _className = "";
        private static string _namespace = "";

        // Dasm properties
        private static Dasm _dasmType = Dasm.As3c;

        static void Main(string[] args)
        {
#if DEBUG
            DateTime start = DateTime.Now;
#endif
            try
            {
                #region Initialization

                try
                {
                    bool hasMethod = false;

                    if (args.Length < 1)
                    {
                        WriteInfo();
                        Console.WriteLine("[?] Use -help for some instructions.");
                        return;
                    }

                    for (int i = 0, n = args.Length; i < n; ++i)
                    {
                        switch (args[i])
                        {
                            case "-h":
                            case "-help":
                            case "-?":
                            case "/?":
                            case "/help":
                                WriteInfo();
                                Usage();
                                return;

                            // OPTIONS /////////////////////////////////////////////////////////////////////

                            case "-o":
                            case "-output":
                                _pathOutput = args[++i];
                                break;

                            case "-q":
                            case "-quiet":
                                _isQuiet = true;
                                break;

                            // REPLACE ////////////////////////////////////////////////////////////////////

                            case "-r":
                            case "-replace":
                                _action = Action.Replace;
                                break;

                            // REPLACE PROPERTIES ////////////////////////////////////////////////////////

                            case "-sm":
                            case "-static-method":
                                if (hasMethod) throw new Exception("Two methods defined.");
                                _isMethodStatic = true;
                                _methodName = args[++i];
                                break;

                            case "-m":
                            case "-method":
                                if (hasMethod) throw new Exception("Two methods defined.");
                                _isMethodStatic = false;
                                _methodName = args[++i];
                                break;

                            case "-co":
                            case "-constructor":
                                if (hasMethod) throw new Exception("Two methods defined.");
                                _isConstructor = true;
                                _isMethodStatic = false;
                                break;

                            case "-sc":
                            case "-static-constructor":
                                if (hasMethod) throw new Exception("Two methods defined.");
                                _isConstructor = true;
                                _isMethodStatic = true;
                                break;

                            case "-c":
                            case "-class":
                                _className = args[++i];
                                break;

                            case "-n":
                            case "-namespace":
                                _namespace = args[++i];
                                break;

                            // INLINE ///////////////////////////////////////////////////////////////////

                            case "-i":
                            case "-inline":
                                _action = Action.Inline;
                                _pathSwf = args[++i];
                                break;

                            // PATCH ///////////////////////////////////////////////////////////////////
                            case "-optimize":
                                _action = Action.Optimize;
                                _pathSwf = args[++i];
                                break;

                            // DASM ////////////////////////////////////////////////////////////////////

                            case "-d":
                            case "-dasm":
                                _action = Action.Dasm;
                                break;

                            // DASM PROPERTIES ////////////////////////////////////////////////////////

                            case "-a":
                            case "-as3c":
                                _dasmType = Dasm.As3c;
                                break;

                            case "-p":
                            case "-plain":
                                _dasmType = Dasm.Plain;
                                break;

                            default:
                                if (File.Exists(args[i]))
                                {
                                    try
                                    {
                                        BinaryReader fileHead = new BinaryReader(File.Open(args[i], FileMode.Open, FileAccess.Read));

                                        if (fileHead.BaseStream.Length < 3)
                                            throw new Exception("Invalid file given.");

                                        byte[] head = fileHead.ReadBytes(3);

                                        fileHead.Close();

                                        //TODO fix this ugly part...
                                        if ((head[0] == (byte)'C' || head[0] == (byte)'F') && head[1] == (byte)'W' && head[2] == (byte)'S')
                                        {
                                            if (_pathSwf != "")
                                            {
                                                throw new Exception("Two SWF files given.");
                                            }

                                            _pathSwf = args[i];
                                        }
                                        else
                                        {
                                            if ("" != _pathAsm)
                                            {
                                                throw new Exception("Two ASM files given.");
                                            }

                                            _pathAsm = args[i];
                                        }
                                    }
                                    catch (IOException io)
                                    {
                                        WriteInfo();
                                        Console.Error.WriteLine("[-] Error: Can not open file {0}", args[i]);
#if DEBUG
                                        Console.Error.WriteLine("[-] {0}", io.Message);
#endif
                                        return;
                                    }
                                    catch (Exception e)
                                    {
                                        WriteInfo();
                                        Console.Error.WriteLine("[-] Error: {0}", e.Message);
#if DEBUG
                                        Console.Error.WriteLine("[-] {0}", e.StackTrace);
#endif
                                        return;
                                    }
                                }
                                else
                                {
                                    if (args[i].IndexOf(".") != -1)
                                    {
                                        WriteInfo();
                                        Console.Error.WriteLine("[-] File {0} does not exist.", args[i]);
                                        return;
                                    }
                                    else
                                    {
                                        throw new Exception("Invalid argument given.");
                                    }
                                }
                                break;
                        }
                    }
                }
                catch (Exception e)
                {
                    WriteInfo();
                    Console.Error.WriteLine("[-] Error: Invalid arguments. Use -help for help ...");
#if DEBUG
                    Console.Error.WriteLine("[-] {0}", e.Message);
                    Console.Error.WriteLine("[-] {0}", e.StackTrace);
#endif
                    return;
                }

                if (!_isQuiet)
                {
                    Console.WriteLine("[i] As3c - ActionScript3 ASM compiler");
                }

                Translator.InitTable();

                #endregion

                #region Integrity scan for the Translator class (DEBUG only)

#if DEBUG
                if (Translator.CheckIntegrity())
                {
                    if (!_isQuiet)
                    {
                        Console.WriteLine("[+] Integrity scan passed!");
                    }
                }
                else
                {
                    Console.WriteLine("[-] Integrity scan failed...!");
                }
#endif

                #endregion

                FileStream fileInput;
                FileStream fileOutput;
                SwfFormat swf;
                
                switch (_action)
                {
                    case Action.Dasm:

                        #region Disassemble file

                        swf = new SwfFormat();
                        fileInput = File.Open(_pathSwf, FileMode.Open, FileAccess.Read);

                        swf.Read(fileInput);

                        fileInput.Close();
                        fileInput.Dispose();

                        DisassemblerBase dasm;

                        switch (_dasmType)
                        {
                            case Dasm.As3c:
                                dasm = new DisassemblerAs3c();
                                break;

                            case Dasm.Plain:
                                dasm = new DisassemblerPlain();
                                break;

                            default:
                                throw new Exception();
                        }

                        dasm.Parse(swf);

                        if ("" == _pathOutput)
                        {
                            dasm.EmitToConsole();
                        }
                        else
                        {
                            fileOutput = File.Open(_pathOutput, FileMode.OpenOrCreate, FileAccess.Write);
                            dasm.EmitToStream(fileOutput);

                            fileOutput.Close();
                            fileOutput.Dispose();
                        }

                        #endregion

                        break;

                    case Action.Optimize:

                        #region Optimize SWF

                        swf = new SwfFormat();
                        fileInput = File.Open(_pathSwf, FileMode.Open, FileAccess.Read);

                        swf.Read(fileInput);

                        fileInput.Close();
                        fileInput.Dispose();

                        CompilerOptimize compOptimize = new CompilerOptimize(swf);

                        compOptimize.Compile();

                        fileOutput = File.Open((_pathOutput != "") ? _pathOutput : _pathSwf, FileMode.OpenOrCreate, FileAccess.Write);

                        swf.Write(fileOutput);

                        fileOutput.Close();
                        fileOutput.Dispose();

                        #endregion

                        break;

                    case Action.Inline:

                        #region Compile inline instructions

                        swf = new SwfFormat();
                        fileInput = File.Open(_pathSwf, FileMode.Open, FileAccess.Read);

                        swf.Read(fileInput);

                        fileInput.Close();
                        fileInput.Dispose();

                        CompilerInline compInline = new CompilerInline(swf);
                        compInline.Compile();

                        fileOutput = File.Open((_pathOutput != "") ? _pathOutput : _pathSwf, FileMode.OpenOrCreate, FileAccess.Write);

                        swf.Write(fileOutput);

                        fileOutput.Close();
                        fileOutput.Dispose();

                        #endregion

                        break;

                    case Action.Replace:

                        #region Replace method body

                        #region Simple pre-check

                        if ("" == _pathAsm || !File.Exists(_pathAsm))
                        {
                            Console.Error.WriteLine("[-] No valid ASM input given.");
                            return;
                        }

                        if ("" == _pathSwf || !File.Exists(_pathSwf))
                        {
                            Console.Error.WriteLine("[-] No valid SWF target given.");
                            return;
                        }

                        if ("" == _className)
                        {
                            Console.Error.WriteLine("[-] No class given.");
                            return;
                        }

                        if (!_isConstructor)
                        {
                            if ("" == _methodName)
                            {
                                Console.Error.WriteLine("[-] Need method name or constructor to replace");
                                return;
                            }
                        }

                        #endregion

                        swf = new SwfFormat();
                        fileInput = File.Open(_pathSwf, FileMode.Open, FileAccess.Read);

                        swf.Read(fileInput);

                        fileInput.Close();
                        fileInput.Dispose();

                        #region Body lookup

                        Abc46 abc = null;
                        SwfLibrary.Abc.MethodInfo method = null;
                        MethodBodyInfo body = null;
                        bool instanceFound = false;
                        Abc46 currentAbc = null;

                        string classFormat;

                        if (_className.IndexOf(".") != -1)
                        {
                            classFormat = "public::" + _className.Substring(0, _className.LastIndexOf(".")) + "::" + _className.Substring(_className.LastIndexOf(".") + 1, _className.Length - _className.LastIndexOf(".") - 1);
                        }
                        else
                        {
                            classFormat = "public::" + _className;
                        }

                        string methodFormat = _namespace + "::" + _methodName;

                        // Parse for all possibilities
                        for (int i = 0, n = swf.AbcCount; i < n; ++i)
                        {
                            currentAbc = swf.GetAbcAt(i);

                            for (int j = 0, m = currentAbc.Scripts.Count; j < m; ++j)
                            {
                                ScriptInfo script = (ScriptInfo)currentAbc.Scripts[j];

                                for (int k = 0, o = script.Traits.Count; k < o; ++k)
                                {
                                    TraitInfo scriptTrait = (TraitInfo)script.Traits[k];

                                    if (!(scriptTrait.Body is TraitClass))
                                    {
                                        continue;
                                    }

                                    TraitClass classBody = (TraitClass)scriptTrait.Body;
                                    ClassInfo classInfo = (ClassInfo)currentAbc.Classes[(int)classBody.ClassI];
                                    InstanceInfo instanceInfo = (InstanceInfo)currentAbc.Instances[(int)classBody.ClassI];

                                    string instanceName = NameUtil.ResolveMultiname(currentAbc, instanceInfo.Name);

                                    if (classFormat == instanceName)
                                    {
                                        instanceFound = true;

                                        if (!_isQuiet)
                                            Console.WriteLine("[+] Found class {0}", instanceName);

                                        if (_isMethodStatic)
                                        {
                                            if (_isConstructor)
                                            {
                                                if (null != body)
                                                {
                                                    Console.Error.WriteLine("[-] Can not explicitly determine method body.");
                                                    return;
                                                }

                                                method = (SwfLibrary.Abc.MethodInfo)currentAbc.Methods[(int)classInfo.CInit];

                                                body = FindBody(currentAbc, classInfo.CInit);

                                                abc = currentAbc;

                                                if (null != body)
                                                {
                                                    if (!_isQuiet)
                                                        Console.WriteLine("[+] Found static class initializer.");
                                                }
                                            }
                                            else
                                            {
                                                Console.Error.WriteLine("[-] Sorry, static methods do not work yet ...");
                                                return;
                                                //TODO support static methods...
                                            }
                                        }
                                        else
                                        {
                                            if (_isConstructor)
                                            {
                                                if (null != body)
                                                {
                                                    Console.Error.WriteLine("[-] Can not explicitly determine method body.");
                                                    return;
                                                }

                                                method = (SwfLibrary.Abc.MethodInfo)currentAbc.Methods[(int)instanceInfo.IInit];

                                                body = FindBody(currentAbc, instanceInfo.IInit);

                                                abc = currentAbc;

                                                if (null != body)
                                                {
                                                    if (!_isQuiet)
                                                        Console.WriteLine("[+] Found class initializer.");
                                                }
                                            }
                                            else
                                            {
                                                // here begins the ugly part ...
                                                for (int l = 0, p = instanceInfo.Traits.Count; l < p; ++l)
                                                {
                                                    TraitInfo instanceTrait = (TraitInfo)instanceInfo.Traits[l];

                                                    if (!(instanceTrait.Body is TraitMethod))
                                                    {
                                                        continue;
                                                    }

                                                    string methodName = NameUtil.ResolveMultiname(currentAbc, instanceTrait.Name);

                                                    if ("" == _namespace)
                                                    {
                                                        if ("public::" + _methodName != methodName &&
                                                            "private::" + _methodName != methodName &&
                                                            "protected::" + _methodName != methodName &&
                                                            "protected$::" + _methodName != methodName &&
                                                            "internal::" + _methodName != methodName)
                                                        {
                                                            continue;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (methodName != methodFormat)
                                                        {
                                                            continue;
                                                        }
                                                    }

                                                    if (null != body)
                                                    {
                                                        Console.Error.WriteLine("[-] Can not explicitly determine method body.");
                                                        return;
                                                    }

                                                    TraitMethod methodBody = (TraitMethod)instanceTrait.Body;

                                                    method = (SwfLibrary.Abc.MethodInfo)currentAbc.Methods[(int)methodBody.Method];

                                                    body = FindBody(currentAbc, methodBody.Method);

                                                    abc = currentAbc;

                                                    if (null != body)
                                                    {
                                                        if (!_isQuiet)
                                                            Console.WriteLine("[+] Found method {0}", methodName);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if (null == body)
                        {
                            Console.Error.WriteLine("[-] Could not find {0}.", (instanceFound) ? "method" : "class");
                            return;
                        }

                        #endregion

                        //
                        // We have valid body to replace. Start the parser.
                        //

                        ParserAs3c parser = new ParserAs3c();
                        parser.Parse(_pathAsm);

                        //
                        // Convert the parser instructions to actual bytecode for the AVM2.
                        //

                        CompilerAs3c compAs3c = new CompilerAs3c();
                        compAs3c.Compile(currentAbc, parser.Instructions, parser.Labels, false);

                        //
                        // Get body information (maxstack, localcount, maxscopedepth, initscopedepth)
                        // We keep compiler directives in respect here ...
                        //

                        #region MethodBody information

                        U30 maxStack;
                        U30 maxScopeDepth;
                        U30 localCount;
                        U30 initScopeDepth = new U30();

                        if (parser.HasMaxStack)
                        {
                            maxStack = new U30();
                            maxStack.Value = parser.MaxStack;
                        }
                        else
                        {
                            maxStack = ByteCodeAnalyzer.CalcMaxStack(compAs3c.Code);

                            if (maxStack.Value < method.ParameterCount.Value)
                            {
                                maxStack.Value = method.ParameterCount.Value;
                            }
                        }

                        if (parser.HasLocalCount)
                        {
                            localCount = new U30();
                            localCount.Value = parser.LocalCount;
                        }
                        else
                        {
                            localCount = ByteCodeAnalyzer.CalcLocalCount(compAs3c.Code);
                        }

                        if (parser.HasInitScopeDepth)
                        {
                            initScopeDepth.Value = parser.InitScopeDepth;
                        }
                        else
                        {
                            initScopeDepth.Value = 1;
                        }

                        if (parser.HasMaxScopeDepth)
                        {
                            maxScopeDepth = new U30();
                            maxScopeDepth.Value = parser.MaxScopeDepth;
                        }
                        else
                        {
                            maxScopeDepth = ByteCodeAnalyzer.CalcScopeDepth(compAs3c.Code);
                            maxScopeDepth.Value += initScopeDepth.Value;
                        }

                        if (!_isQuiet)
                        {
                            Console.WriteLine("[i] InitScopeDepth: {0}", (int)initScopeDepth);
                            Console.WriteLine("[i] MaxScopeDepth: {0}", (int)maxScopeDepth);
                            Console.WriteLine("[i] MaxStack: {0}", (int)maxStack);
                            Console.WriteLine("[i] LocalCount: {0}", (int)localCount);
                        }

                        #endregion

                        //
                        // Replace the code of the body.
                        //

                        body.Code = compAs3c.Code;

                        //
                        // Update body information
                        //

                        body.MaxStack = maxStack;
                        body.MaxScopeDepth = maxScopeDepth;
                        body.InitScopeDepth = initScopeDepth;
                        body.LocalCount = localCount;

                        #region Write output

                        if ("" == _pathOutput)
                        {
                            fileOutput = File.Open(_pathSwf, FileMode.OpenOrCreate, FileAccess.Write);
                        }
                        else
                        {
                            fileOutput = File.Open(_pathOutput, FileMode.OpenOrCreate, FileAccess.Write);
                        }

                        swf.Write(fileOutput);

                        fileOutput.Close();
                        fileOutput.Dispose();

                        #endregion

                        #endregion

                        break;

                    case Action.Usage:
                        Usage();
                        break;
                }

                #region Program end

                if (!_isQuiet)
                {
                    Console.WriteLine("[i] Done.");
                }
                #endregion
            }
            catch (IOException ioex)
            {
                Console.Error.WriteLine("[-] Error: {0}", ioex.Message);
            }
            catch (InstructionException iex)
            {
                #region Parser errors

                string message;

                switch (iex.ErrorType)
                {
                    case InstructionException.Type.InvalidSyntax:
                        message = "Error while parsing";
                        break;
                    case InstructionException.Type.NotEnoughArguments:
                        message = "Not enough arguments";
                        break;
                    case InstructionException.Type.TooManyArguments:
                        message = "Too many arguments";
                        break;
                    case InstructionException.Type.UnknownType:
                        message = "Unknown type";
                        break;
                    case InstructionException.Type.LabelRedefined:
                        message = "Label has already been defined";
                        break;
                    case InstructionException.Type.LabelMissing:
                        message = "Label has never been defined";
                        break;
                    default:
                        message = "Unknown error at";
                        break;
                }

                Console.Error.WriteLine("[-] " + message + ":");

                if (null != iex.ParserInfo)
                {
                    Console.Error.WriteLine("{0}({1}): {2}", iex.ParserInfo.FilePath, iex.ParserInfo.LineNumber, iex.ParserInfo.Line);
                }

                #endregion
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("[-] Unexpected error: {0}", ex.Message);
#if DEBUG
                Console.Error.WriteLine("[-] Stacktrace: {0}", ex.StackTrace);
#endif
            }

#if DEBUG
            DateTime end = DateTime.Now;
            TimeSpan delta = end - start;

            Console.WriteLine("[i] Total: {0}sec {1}ms", delta.Seconds, delta.Milliseconds);
#endif
        }

        static private MethodBodyInfo FindBody(Abc46 abc, U30 methodIndex)
        {
            SwfLibrary.Abc.MethodInfo methodInfo = (SwfLibrary.Abc.MethodInfo)abc.Methods[(int)methodIndex];
            MethodBodyInfo methodBody = null;

            for (int i = 0, n = abc.MethodBodies.Count; i < n; ++i)
            {
                methodBody = (MethodBodyInfo)abc.MethodBodies[i];

                if (methodBody.Method.Value == methodIndex.Value)
                {
                    break;
                }
            }

            return methodBody;
        }

        static private void WriteInfo()
        {
            Console.WriteLine("[i] As3c - ActionScript3 ASM compiler");
        }

        static private void Usage()
        {
            string exec = Path.GetFileName(Assembly.GetEntryAssembly().Location);

            Console.WriteLine("[?] Help");
            Console.WriteLine("General syntax:");
            Console.WriteLine("");
            Console.WriteLine(" {0} [options] [action [properties]] [files]", exec);
            Console.WriteLine("");
            Console.WriteLine("  Options:");
            Console.WriteLine("");
            Console.WriteLine("   -quiet");
            Console.WriteLine("   As3c will not generate any output besides error messages while running.");
            Console.WriteLine("");
            Console.WriteLine("   -output [file]");
            Console.WriteLine("   Specify output file.");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("  Actions:");
            Console.WriteLine("");
            Console.WriteLine("   -replace [properties] [swf] [asm]");
            Console.WriteLine("");
            Console.WriteLine("   Replaces an existing method body. In order to replace a method body with");
            Console.WriteLine("   your compiled bytecode you have to tell As3c which method to replace.");
            Console.WriteLine("");
            Console.WriteLine("   You will have to specify the SWF file to work with and an ASM file to");
            Console.WriteLine("   replace the currently existing method.");
            Console.WriteLine("");
            Console.WriteLine("    Properties:");
            Console.WriteLine("");
            Console.WriteLine("     -namespace [uri]");
            Console.WriteLine("");
            Console.WriteLine("     The namespace of the method. You only have to pass a namespace");
            Console.WriteLine("     if the method lies inside on. Otherwise you do not have to pass");
            Console.WriteLine("     this parameter.");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("     -class [name]");
            Console.WriteLine("");
            Console.WriteLine("     The name of the class that contains the method to replace. The");
            Console.WriteLine("     name should also include the package path.");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("     -method [method]");
            Console.WriteLine("");
            Console.WriteLine("     The name of the method to replace. This method can have any visibility.");
            Console.WriteLine("     It does not matter if it is private, protected, internal or public.");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("     -constructor");
            Console.WriteLine("");
            Console.WriteLine("     The constructor of the class. This is the so called instance constructor");
            Console.WriteLine("     and what you basically know from actionscript when writing a simple");
            Console.WriteLine("     constructor.");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("     -static-method [method]");
            Console.WriteLine("");
            Console.WriteLine("     Same as the -method but this one will work with static functions.");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("     -static-constructor");
            Console.WriteLine("");
            Console.WriteLine("     This one is the class constructor which is executed on class initialization");
            Console.WriteLine("     level. This constructor is not executed per instance.");
            Console.WriteLine("");
            Console.WriteLine("    Example:");
            Console.WriteLine("");
            Console.WriteLine("     {0} -o optimized.swf -class foo.Bar", exec);
            Console.WriteLine("      -method toString test.swf optimized.asm");
            Console.WriteLine("");
            Console.WriteLine("     This would replace the toString() function of the Bar class which");
            Console.WriteLine("     is located in the foo package.");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("   -inline [swf]");
            Console.WriteLine("");
            Console.WriteLine("   Compile inline ASM instructions that have to be written using the As3c");
            Console.WriteLine("   ActionScript 3 framework.");
            Console.WriteLine("");
            Console.WriteLine("    Example:");
            Console.WriteLine("");
            Console.WriteLine("     {0} -o output.swf -inline input.swf", exec);
            Console.WriteLine("");
            Console.WriteLine("     This would replace all inline ASM instructions in the input.swf with");
            Console.WriteLine("     their proper bytecode and write to output.swf.");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("     {0} -inline input.swf", exec);
            Console.WriteLine("");
            Console.WriteLine("     This is the simple command to just replace all inline ASM instructions");
            Console.WriteLine("     in one file and save it. Probably what you have been looking for ;)");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("   -dasm [properties] [swf]");
            Console.WriteLine("");
            Console.WriteLine("   Disassemble bytecode of given SWF. If you do not specify any output As3c");
            Console.WriteLine("   will write it to the console.");
            Console.WriteLine("");
            Console.WriteLine("    Properties:");
            Console.WriteLine("");
            Console.WriteLine("     -as3c");
            Console.WriteLine("");
            Console.WriteLine("     Disassemble to As3c syntax. The output will include some comments");
            Console.WriteLine("     to make it easier for you to navigate through and find the function");
            Console.WriteLine("     you search for.");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("     -plain");
            Console.WriteLine("");
            Console.WriteLine("     Disassemble keeping plain syntax without resolving any constant");
            Console.WriteLine("     or namespace.");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("    Example:");
            Console.WriteLine("");
            Console.WriteLine("     {0} -o dasm.txt -as3c test.swf",exec);
            Console.WriteLine("");
            Console.WriteLine("     This would disassemble the test.swf file using As3c syntax and write");
            Console.WriteLine("     the output into dasm.txt.");
        }
    }
}