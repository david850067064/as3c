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
