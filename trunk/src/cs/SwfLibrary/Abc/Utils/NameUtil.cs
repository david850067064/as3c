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
using SwfLibrary.Abc.Constants;
using SwfLibrary.Exceptions;
using SwfLibrary.Utils;
using System.Collections;
using SwfLibrary.Abc.Traits;
using SwfLibrary.Types;

namespace SwfLibrary.Abc.Utils
{
    /// <summary>
    /// NameUtil is a utility class that helps creating and/or resolving Multiname
    /// structures.
    /// 
    /// TODO: The resolving of Multinames is completly wrong. This means their string
    /// representation is wrong, therefore also the whole syntax gets wrong.
    /// 
    /// Some proper error checking would be nice too.
    /// </summary>
    public class NameUtil
    {
        public static string ResolveMultiname(Abc46 abc, U30 index)
        {
            return ResolveMultiname(abc, (MultinameInfo)abc.ConstantPool.MultinameTable[(int)index.Value]);
        }

        public static string ResolveMultiname(Abc46 abc, int index)
        {
            return ResolveMultiname(abc, (MultinameInfo)abc.ConstantPool.MultinameTable[index]);
        }

        /**
         * HERE STARTS THE SPAGHETTI CODE
         */
        public static string ResolveMultiname(Abc46 abc, MultinameInfo multiName)
        {
            NamespaceInfo ns;
            NamespaceSetInfo nss;
            StringInfo name;

            string result;

            switch (multiName.Kind)
            {
                case MultinameInfo.RTQName:
                case MultinameInfo.RTQNameA:
                    //Console.WriteLine("[-] RTQName/RTQNameA is currently not supported.");
                    return ((StringInfo)abc.ConstantPool.StringTable[(int)multiName.Data[0].Value]).ToString();

                case MultinameInfo.QName:
                case MultinameInfo.QNameA:
                    ns = ((NamespaceInfo)abc.ConstantPool.NamespaceTable[(int)multiName.Data[0].Value]);
                    name = ((StringInfo)abc.ConstantPool.StringTable[(int)multiName.Data[1].Value]);

                    result = "";

                    switch (ns.Kind)
                    {
                        case NamespaceInfo.Namespace:
                        case NamespaceInfo.ExplicitNamespace:
                            //TODO implement this
                            //user defined
                            break;
                        case NamespaceInfo.PrivateNs:
                            result = "private";
                            break;
                        case NamespaceInfo.ProtectedNamespace:
                            result = "protected";
                            break;
                        case NamespaceInfo.StaticProtectedNs:
                            result = "protected$";
                            break;
                        case NamespaceInfo.PackageInternalNs:
                            result = "internal";
                            break;
                        case NamespaceInfo.PackageNamespace:
                            result = "public";
                            break;
                        default:
                            result = "*";
                            break;
                            //throw new VerifyException("Unexpected namespace kind.");
                    }

                    if (0 != ns.Name.Value)
                    {
                        string namespaceName = ((StringInfo)abc.ConstantPool.StringTable[(int)ns.Name.Value]).ToString();
                        if ("" != namespaceName && result != "")
                            result += "::";
                        result += namespaceName;
                    }

                    result += "::" + name.ToString();

                    return result;

                case MultinameInfo.RTQNameL:
                case MultinameInfo.RTQNameLA:
                    Console.WriteLine("[-] RTQNameL/RTQameLA is currently not supported.");
                    return "";

                case MultinameInfo.Multiname_:
                case MultinameInfo.MultinameA:
                    //TODO fix this -- what about the namespace set here?
                    //Console.WriteLine("[-] Multiname/MultinameA is currently not supported.");

                    name = ((StringInfo)abc.ConstantPool.StringTable[(int)multiName.Data[0].Value]);
                    nss = ((NamespaceSetInfo)abc.ConstantPool.NamespaceSetTable[(int)multiName.Data[1].Value]);

                    return name.ToString(); ;

                case MultinameInfo.MultinameL:
                case MultinameInfo.MultinameLA:
                    //Console.WriteLine("[-] MultinameL/MultinameLA is currently not supported.")
                    nss = ((NamespaceSetInfo)abc.ConstantPool.NamespaceSetTable[(int)multiName.Data[0].Value]);
                    string set = "[";
                    for (int i = 0, n = nss.NamespaceSet.Count; i < n; ++i)
                    {
                        U30 nssNs = (U30)nss.NamespaceSet[i];
                        ns = ((NamespaceInfo)abc.ConstantPool.NamespaceTable[(int)nssNs.Value]);

                        string r2 = "";

                        switch (ns.Kind)
                        {
                            case NamespaceInfo.Namespace:
                            case NamespaceInfo.ExplicitNamespace:
                                //TODO implement this
                                //user defined
                                break;
                            case NamespaceInfo.PrivateNs:
                                r2 = "private";
                                break;
                            case NamespaceInfo.ProtectedNamespace:
                                r2 = "protected";
                                break;
                            case NamespaceInfo.StaticProtectedNs:
                                r2 = "protected$";
                                break;
                            case NamespaceInfo.PackageInternalNs:
                                r2 = "internal";
                                break;
                            case NamespaceInfo.PackageNamespace:
                                r2 = "public";
                                break;
                            default:
                                r2 = "*";
                                break;
                        }

                        set += String.Format("{0}::{1}", r2, ((StringInfo)abc.ConstantPool.StringTable[(int)ns.Name.Value]).ToString());

                        if (i != n - 1)
                            set += ", ";
                    }

                    set += "]";

                    return set;

                default:
                    return "*";
                    //throw new VerifyException("Unknown multiname kind.");
            }
        }

        public static string ResolveClass(Abc46 abc, InstanceInfo info)
        {
            return ResolveMultiname(abc, (MultinameInfo)abc.ConstantPool.MultinameTable[(int)info.Name.Value]);
        }

        public static U30 GetMultiname(Abc46 abc, string argument)
        {
            if (argument.StartsWith("#"))
            {
                int index = int.Parse(argument.Substring(1,argument.Length-1));

                if (index < abc.ConstantPool.MultinameTable.Count && index > 0)
                {
                    return (U30)index;
                }

                throw new Exception(String.Format("Invalid multiname {0}", index));
            }

            NamespaceInfo ns;
            NamespaceSetInfo nss;
            StringInfo name;

            bool skipQname = argument.IndexOf("[") == 0;

            if (skipQname)
            {
                //BAD quick dirty hack
                argument = argument.Replace(" ", "");
            }

            string tempName;
            U30 result = new U30();

            for (int i = 1, n = abc.ConstantPool.MultinameTable.Count; i < n; ++i)
            {
                MultinameInfo multiName = (MultinameInfo)abc.ConstantPool.MultinameTable[i];

                switch (multiName.Kind)
                {
                    #region QName, QNameA

                    case MultinameInfo.QName:
                    case MultinameInfo.QNameA:

                        if (skipQname)
                            continue;

                        ns = ((NamespaceInfo)abc.ConstantPool.NamespaceTable[(int)multiName.Data[0].Value]);
                        name = ((StringInfo)abc.ConstantPool.StringTable[(int)multiName.Data[1].Value]);

                        tempName = "";

                        switch (ns.Kind)
                        {
                            case NamespaceInfo.Namespace:
                            case NamespaceInfo.ExplicitNamespace:
                                //TODO implement this
                                //user defined
                                break;
                            case NamespaceInfo.PrivateNs:
                                tempName = "private";
                                break;
                            case NamespaceInfo.ProtectedNamespace:
                                tempName = "protected";
                                break;
                            case NamespaceInfo.StaticProtectedNs:
                                tempName = "protected$";
                                break;
                            case NamespaceInfo.PackageInternalNs:
                                tempName = "internal";
                                break;
                            case NamespaceInfo.PackageNamespace:
                                tempName = "public";
                                break;
                            default:
                                tempName = "*";
                                break;
                        }

                        if (0 != ns.Name.Value)
                        {
                            string namespaceName = ((StringInfo)abc.ConstantPool.StringTable[(int)ns.Name.Value]).ToString();
                            if ("" != namespaceName && tempName != "")
                                tempName += "::";
                            tempName += namespaceName;
                        }

                        tempName += "::" + name.ToString();

                        if (tempName == argument)
                        {
                            result.Value = (uint)i;
                            return result;
                        }
                        break;

                    #endregion

                    #region MultinameL, MultinameLA

                    case MultinameInfo.MultinameL:
                    case MultinameInfo.MultinameLA:

                        if (!skipQname)
                            continue;

                        tempName = "[";

                        nss = (NamespaceSetInfo)abc.ConstantPool.NamespaceSetTable[(int)multiName.Data[0].Value];

                        for (int j = 0, m = nss.NamespaceSet.Count; j < m; ++j)
                        {
                            U30 nssNs = (U30)nss.NamespaceSet[j];
                            ns = ((NamespaceInfo)abc.ConstantPool.NamespaceTable[(int)nssNs.Value]);

                            string r2 = "";

                            switch (ns.Kind)
                            {
                                case NamespaceInfo.Namespace:
                                case NamespaceInfo.ExplicitNamespace:
                                    //TODO implement this
                                    //user defined
                                    break;
                                case NamespaceInfo.PrivateNs:
                                    r2 = "private";
                                    break;
                                case NamespaceInfo.ProtectedNamespace:
                                    r2 = "protected";
                                    break;
                                case NamespaceInfo.StaticProtectedNs:
                                    r2 = "protected$";
                                    break;
                                case NamespaceInfo.PackageInternalNs:
                                    r2 = "internal";
                                    break;
                                case NamespaceInfo.PackageNamespace:
                                    r2 = "public";
                                    break;
                                default:
                                    r2 = "*";
                                    break;
                            }

                            tempName += r2 + "::" + ((StringInfo)abc.ConstantPool.StringTable[(int)ns.Name.Value]).ToString();

                            if (j != (m-1))
                                tempName += ",";
                        }

                        tempName += "]";

                        if (argument == tempName)
                        {
                            result.Value = (uint)i;
                            return result;
                        }
                        break;

                    #endregion

                    default:
                        continue;
                }
            }

            if (skipQname)
            {
                #region Create MultinameL
                //
                // Create a MultinameL
                //

                // Remove [] from argument
                argument = argument.Substring(1, argument.Length - 2);

                
                // Get new NamespaceSet index
                U30 setIndex = new U30();
                setIndex.Value = (uint)abc.ConstantPool.NamespaceSetTable.Count;
                

                // Create MultinameInfo
                MultinameInfo newName = new MultinameInfo();
                newName.Data = new U30[1] { setIndex };
                newName.Kind = MultinameInfo.MultinameL;

                
                // Create NamespaceSet
                NamespaceSetInfo newSet = new NamespaceSetInfo();
                newSet.NamespaceSet = new ArrayList();

                abc.ConstantPool.NamespaceSetTable.Add(newSet);

                for (int i = 0, n = abc.ConstantPool.NamespaceTable.Count; i < n; ++i)
                {
                    ns = (NamespaceInfo)abc.ConstantPool.NamespaceTable[i];

                    string r2 = "";

                    switch (ns.Kind)
                    {
                        case NamespaceInfo.Namespace:
                        case NamespaceInfo.ExplicitNamespace:
                            //TODO implement this
                            //user defined
                            break;
                        case NamespaceInfo.PrivateNs:
                            r2 = "private";
                            break;
                        case NamespaceInfo.ProtectedNamespace:
                            r2 = "protected";
                            break;
                        case NamespaceInfo.StaticProtectedNs:
                            r2 = "protected$";
                            break;
                        case NamespaceInfo.PackageInternalNs:
                            r2 = "internal";
                            break;
                        case NamespaceInfo.PackageNamespace:
                            r2 = "public";
                            break;
                        default:
                            r2 = "*";
                            break;
                    }

                    r2 += "::" + ((StringInfo)abc.ConstantPool.StringTable[(int)ns.Name.Value]).ToString();

                    if (argument.IndexOf(r2) != -1)
                    {
                        U30 nsIndex = new U30();
                        nsIndex.Value = (uint)i;

                        newSet.NamespaceSet.Add(nsIndex);
                    }
                }

                result.Value = (uint)abc.ConstantPool.MultinameTable.Count;
                abc.ConstantPool.MultinameTable.Add(newName);

                #endregion
            }
            else
            {
                #region Create QName

                // Create a QName

                U30 nsIndex = new U30();

                if (argument.IndexOf("::") == argument.LastIndexOf("::"))
                {
                    nsIndex.Value = GetNamespace(abc, argument.Substring(0, argument.LastIndexOf("::") + 2)).Value;
                }
                else
                    nsIndex.Value = GetNamespace(abc, argument.Substring(0, argument.LastIndexOf("::"))).Value;
                    
                MultinameInfo newQName = new MultinameInfo();
                newQName.Data = new U30[2] { nsIndex, (U30)abc.ConstantPool.ResolveString(argument.Substring(argument.LastIndexOf("::") + 2)) };
                newQName.Kind = MultinameInfo.QName;

                result.Value = (uint)abc.ConstantPool.MultinameTable.Count;

                abc.ConstantPool.MultinameTable.Add(newQName);

                #endregion
            }

            return result;
        }

        public static U30 GetNamespace(Abc46 abc, string argument)
        {
            NamespaceInfo ns;

            for (int i = 0, n = abc.ConstantPool.NamespaceTable.Count; i < n; ++i)
            {
                ns = (NamespaceInfo)abc.ConstantPool.NamespaceTable[i];

                string namespaceString = "";

                switch (ns.Kind)
                {
                    case NamespaceInfo.Namespace:
                    case NamespaceInfo.ExplicitNamespace:
                        //TODO implement this
                        //user defined
                        break;
                    case NamespaceInfo.PrivateNs:
                        namespaceString = "private";
                        break;
                    case NamespaceInfo.ProtectedNamespace:
                        namespaceString = "protected";
                        break;
                    case NamespaceInfo.StaticProtectedNs:
                        namespaceString = "protected$";
                        break;
                    case NamespaceInfo.PackageInternalNs:
                        namespaceString = "internal";
                        break;
                    case NamespaceInfo.PackageNamespace:
                        namespaceString = "public";
                        break;
                    default:
                        namespaceString = "*";
                        break;
                }

                namespaceString += "::" + ((StringInfo)abc.ConstantPool.StringTable[(int)ns.Name.Value]).ToString();

                if (argument.IndexOf(namespaceString) != -1)
                {
                    U30 nsIndex = new U30();
                    nsIndex.Value = (uint)i;

                    return nsIndex;
                }
            }

            NamespaceInfo newNs = new NamespaceInfo();

            string name = "";
            byte kind = 0;

            if(0 == argument.IndexOf("private::"))
            {
                kind = NamespaceInfo.PrivateNs;
                name = argument.Substring(8);
            }
            else if (0 == argument.IndexOf("public::"))
            {
                kind = NamespaceInfo.PackageNamespace;
                name = argument.Substring(7);
            }

            newNs.Kind = kind;

            if ("" == name)
                newNs.Name = (U30)0;
            else
                newNs.Name = (U30)abc.ConstantPool.ResolveString(name);

            U30 result = new U30();

            result.Value = (uint)abc.ConstantPool.NamespaceTable.Count;

            abc.ConstantPool.NamespaceTable.Add(newNs);

            return result;
        }

        public static U30 GetClass(Abc46 abc, string argument)
        {
            U30 multinameIndex = GetMultiname(abc, argument);
            U30 result = new U30();

            for (int i = 0, n = abc.Instances.Count; i < n; ++i)
            {
                InstanceInfo ii = (InstanceInfo)abc.Instances[i];

                if (ii.Name.Value == multinameIndex.Value)
                {
                    result.Value = (uint)i;
                    break;
                }
            }

            return result;
        }
    }
}
