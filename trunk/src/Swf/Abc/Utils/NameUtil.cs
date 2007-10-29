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
using As3c.Swf.Abc.Constants;
using As3c.Swf.Exceptions;
using As3c.Swf.Utils;
using System.Collections;
using As3c.Swf.Abc.Traits;

namespace As3c.Swf.Abc.Utils
{
    public class NameUtil
    {
        public static string ResolveMultiname(Abc46 abc, MultinameInfo multiName)
        {
            switch (multiName.Kind)
            {
                case MultinameInfo.RTQName:
                case MultinameInfo.RTQNameA:
                    return ((StringInfo)abc.ConstantPool.StringTable[(int)multiName.Data[0].Value]).ToString();

                case MultinameInfo.QName:
                case MultinameInfo.QNameA:
                    NamespaceInfo ns = ((NamespaceInfo)abc.ConstantPool.NamespaceTable[(int)multiName.Data[0].Value]);                    
                    StringInfo name = ((StringInfo)abc.ConstantPool.StringTable[(int)multiName.Data[1].Value]);

                    string result = "";

                    switch (ns.Kind)
                    {
                        case NamespaceInfo.Namespace:
                        case NamespaceInfo.ExplicitNamespace:
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
                            throw new VerifyException("Unexpected namespace kind.");
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
                    return "";

                case MultinameInfo.Multiname_:
                case MultinameInfo.MultinameA:
                    return "";

                case MultinameInfo.MultinameL:
                case MultinameInfo.MultinameLA:
                    return "";

                default:
                    throw new VerifyException("Unknown multiname kind.");
            }
        }

        public static string ResolveClass(Abc46 abc, InstanceInfo info)
        {
            return ResolveMultiname(abc, (MultinameInfo)abc.ConstantPool.MultinameTable[(int)info.Name.Value]);
        }
    }
}
