/*
Copyright (c) 2015-2017 topameng(topameng@qq.com)

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
//打开开关没有写入导出列表的纯虚类自动跳过
//#define JUMP_NODEFINED_ABSTRACT         


using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.IO;
using System.Diagnostics;
using LuaInterface;


using Debugger = LuaInterface.Debugger;
using System.Threading;

public static class ToLuaMenu
{
    //不需要导出或者无法导出的类型
    public static List<Type> dropType = new List<Type>
    {
        typeof(ValueType),                                  //不需要
        typeof(LuaInterface.NullObject),             
        typeof(System.Array),                        
        typeof(System.Reflection.MemberInfo),    
        typeof(System.Reflection.BindingFlags),
        typeof(LuaClient),
        typeof(LuaInterface.LuaFunction),
        typeof(LuaInterface.LuaTable),
        typeof(LuaInterface.LuaThread),
        typeof(LuaInterface.LuaByteBuffer),                 //只是类型标识符
        typeof(DelegateFactory),                            //无需导出，导出类支持lua函数转换为委托。如UIEventListener.OnClick(luafunc)
    };

    //可以导出的内部支持类型
    public static List<Type> baseType = new List<Type>
    {
        typeof(System.Object),
        typeof(System.Delegate),
        typeof(System.String),
        typeof(System.Enum),
        typeof(System.Type),
        typeof(System.Collections.IEnumerator),
        typeof(LuaInterface.EventObject),
        typeof(LuaInterface.LuaMethod),
        typeof(LuaInterface.LuaProperty),
        typeof(LuaInterface.LuaField),
        typeof(LuaInterface.LuaConstructor),        
    };

    private static bool beAutoGen = false;
    private static bool beCheck = true;        
    static List<BindType> allTypes = new List<BindType>();

    public class BindType
    {
        public string name;                 //类名称
        public Type type;
        public bool IsStatic;        
        public string wrapName = "";        //产生的wrap文件名字
        public string libName = "";         //注册到lua的名字
        public Type baseType = null;
        public string nameSpace = null;     //注册到lua的table层级

        public List<Type> extendList = new List<Type>();

        public BindType(Type t)
        {
            if (typeof(System.MulticastDelegate).IsAssignableFrom(t))
            {
                throw new NotSupportedException(string.Format("\nDon't export Delegate {0} as a class, register it in customDelegateList", LuaMisc.GetTypeName(t)));
            }            

            type = t;                        
            nameSpace = ToLuaExport.GetNameSpace(t, out libName);
            name = ToLuaExport.CombineTypeStr(nameSpace, libName);            
            libName = ToLuaExport.ConvertToLibSign(libName);

            if (name == "object")
            {
                wrapName = "System_Object";
                name = "System.Object";
            }
            else if (name == "string")
            {
                wrapName = "System_String";
                name = "System.String";
            }
            else
            {
                wrapName = name.Replace('.', '_');
                wrapName = ToLuaExport.ConvertToLibSign(wrapName);
            }

            int index = CustomSettings.staticClassTypes.IndexOf(type);

            if (index >= 0 || (type.IsAbstract && type.IsSealed))
            {
                IsStatic = true;                
            }

            baseType = LuaMisc.GetExportBaseType(type);
        }

        public BindType SetBaseType(Type t)
        {
            baseType = t;
            return this;
        }

        public BindType AddExtendType(Type t)
        {
            if (!extendList.Contains(t))
            {
                extendList.Add(t);
            }

            return this;
        }

        public BindType SetWrapName(string str)
        {
            wrapName = str;
            return this;
        }

        public BindType SetLibName(string str)
        {
            libName = str;
            return this;
        }

        public BindType SetNameSpace(string space)
        {
            nameSpace = space;            
            return this;
        }

        public static bool IsObsolete(Type type)
        {
            object[] attrs = type.GetCustomAttributes(true);

            for (int j = 0; j < attrs.Length; j++)
            {
                Type t = attrs[j].GetType();

                if (t == typeof(System.ObsoleteAttribute) || t == typeof(NoToLuaAttribute) || t.Name == "MonoNotSupportedAttribute" || t.Name == "MonoTODOAttribute")
                {
                    return true;
                }
            }

            return false;
        }
    }

    static void AutoAddBaseType(BindType bt, bool beDropBaseType)
    {
        Type t = bt.baseType;

        if (t == null)
        {
            return;
        }

        if (CustomSettings.sealedList.Contains(t))
        {
            CustomSettings.sealedList.Remove(t);
            Debugger.LogError("{0} not a sealed class, it is parent of {1}", LuaMisc.GetTypeName(t), bt.name);
        }

        if (t.IsInterface)
        {
            Debugger.LogWarning("{0} has a base type {1} is Interface, use SetBaseType to jump it", bt.name, t.FullName);
            bt.baseType = t.BaseType;
        }
        else if (dropType.IndexOf(t) >= 0)
        {
            Debugger.LogWarning("{0} has a base type {1} is a drop type", bt.name, t.FullName);
            bt.baseType = t.BaseType;
        }
        else if (!beDropBaseType || baseType.IndexOf(t) < 0)
        {
            int index = allTypes.FindIndex((iter) => { return iter.type == t; });

            if (index < 0)
            {
#if JUMP_NODEFINED_ABSTRACT
                if (t.IsAbstract && !t.IsSealed)
                {
                    Debugger.LogWarning("not defined bindtype for {0}, it is abstract class, jump it, child class is {1}", LuaMisc.GetTypeName(t), bt.name);
                    bt.baseType = t.BaseType;
                }
                else
                {
                    Debugger.LogWarning("not defined bindtype for {0}, autogen it, child class is {1}", LuaMisc.GetTypeName(t), bt.name);
                    bt = new BindType(t);
                    allTypes.Add(bt);
                }
#else
                Debugger.LogWarning("not defined bindtype for {0}, autogen it, child class is {1}", LuaMisc.GetTypeName(t), bt.name);                        
                bt = new BindType(t);
                allTypes.Add(bt);
#endif
            }
            else
            {
                return;
            }
        }
        else
        {
            return;
        }

        AutoAddBaseType(bt, beDropBaseType);
    }

    static BindType[] GenBindTypes(BindType[] list, bool beDropBaseType = true)
    {
        allTypes = new List<BindType>(list);

        for (int i = 0; i < list.Length; i++)
        {
            for (int j = i + 1; j < list.Length; j++)
            {
                if (list[i].type == list[j].type)
                    throw new NotSupportedException("Repeat BindType:" + list[i].type);
            }

            if (dropType.IndexOf(list[i].type) >= 0)
            {
                Debug.Fail(list[i].type.FullName + " in dropType table, not need to export");
                allTypes.Remove(list[i]);
                continue;
            }
            else if (beDropBaseType && baseType.IndexOf(list[i].type) >= 0)
            {
                Debug.Fail(list[i].type.FullName + " is Base Type, not need to export");
                allTypes.Remove(list[i]);
                continue;
            }
            else if (list[i].type.IsEnum)
            {
                continue;
            }

            AutoAddBaseType(list[i], beDropBaseType);
        }

        return allTypes.ToArray();
    }


    public static void GenerateClassWraps()
    {

        if (!File.Exists(CustomSettings.saveDir))
        {
            Directory.CreateDirectory(CustomSettings.saveDir);
        }

        allTypes.Clear();
        BindType[] typeList = CustomSettings.customTypeList;

        BindType[] list = GenBindTypes(typeList);
        ToLuaExport.allTypes.AddRange(baseType);

        for (int i = 0; i < list.Length; i++)
        {            
            ToLuaExport.allTypes.Add(list[i].type);
        }

        for (int i = 0; i < list.Length; i++)
        {
            ToLuaExport.Clear();
            ToLuaExport.className = list[i].name;
            ToLuaExport.type = list[i].type;
            ToLuaExport.isStaticClass = list[i].IsStatic;            
            ToLuaExport.baseType = list[i].baseType;
            ToLuaExport.wrapClassName = list[i].wrapName;
            ToLuaExport.libClassName = list[i].libName;
            ToLuaExport.extendList = list[i].extendList;
            ToLuaExport.Generate(CustomSettings.saveDir);
        }

        Console.WriteLine("Generate lua binding files over");
        ToLuaExport.allTypes.Clear();
        allTypes.Clear();        
    }

    static HashSet<Type> GetCustomTypeDelegates()
    {
        BindType[] list = CustomSettings.customTypeList;
        HashSet<Type> set = new HashSet<Type>();
        BindingFlags binding = BindingFlags.Public | BindingFlags.Static | BindingFlags.IgnoreCase | BindingFlags.Instance;

        for (int i = 0; i < list.Length; i++)
        {
            Type type = list[i].type;
            FieldInfo[] fields = type.GetFields(BindingFlags.GetField | BindingFlags.SetField | binding);
            PropertyInfo[] props = type.GetProperties(BindingFlags.GetProperty | BindingFlags.SetProperty | binding);
            MethodInfo[] methods = null;

            if (type.IsInterface)
            {
                methods = type.GetMethods();
            }
            else
            {
                methods = type.GetMethods(BindingFlags.Instance | binding);
            }

            for (int j = 0; j < fields.Length; j++)
            {
                Type t = fields[j].FieldType;

                if (ToLuaExport.IsDelegateType(t))
                {
                    set.Add(t);
                }
            }

            for (int j = 0; j < props.Length; j++)
            {
                Type t = props[j].PropertyType;

                if (ToLuaExport.IsDelegateType(t))
                {
                    set.Add(t);
                }
            }

            for (int j = 0; j < methods.Length; j++)
            {
                MethodInfo m = methods[j];

                if (m.IsGenericMethod)
                {
                    continue;
                }

                ParameterInfo[] pifs = m.GetParameters();

                for (int k = 0; k < pifs.Length; k++)
                {
                    Type t = pifs[k].ParameterType;
                    if (t.IsByRef) t = t.GetElementType();

                    if (ToLuaExport.IsDelegateType(t))
                    {
                        set.Add(t);
                    }
                }
            }

        }

        return set;
    }


    static void GenLuaDelegates()
    {

        ToLuaExport.Clear();
        List<DelegateType> list = new List<DelegateType>();
        list.AddRange(CustomSettings.customDelegateList);
        HashSet<Type> set = GetCustomTypeDelegates();        

        foreach (Type t in set)
        {
            if (null == list.Find((p) => { return p.type == t; }))
            {
                list.Add(new DelegateType(t));
            }
        }

        ToLuaExport.GenDelegates(list.ToArray());
        set.Clear();
        ToLuaExport.Clear();

        Console.WriteLine("Create lua delegate over");
    }    

    static ToLuaTree<string> InitTree()
    {                        
        ToLuaTree<string> tree = new ToLuaTree<string>();
        ToLuaNode<string> root = tree.GetRoot();        
        BindType[] list = GenBindTypes(CustomSettings.customTypeList);

        for (int i = 0; i < list.Length; i++)
        {
            string space = list[i].nameSpace;
            AddSpaceNameToTree(tree, root, space);
        }

        DelegateType[] dts = CustomSettings.customDelegateList;
        string str = null;      

        for (int i = 0; i < dts.Length; i++)
        {            
            string space = ToLuaExport.GetNameSpace(dts[i].type, out str);
            AddSpaceNameToTree(tree, root, space);            
        }

        return tree;
    }

    static void AddSpaceNameToTree(ToLuaTree<string> tree, ToLuaNode<string> parent, string space)
    {
        if (space == null || space == string.Empty)
        {
            return;
        }

        string[] ns = space.Split(new char[] { '.' });

        for (int j = 0; j < ns.Length; j++)
        {
            List<ToLuaNode<string>> nodes = tree.Find((_t) => { return _t == ns[j]; }, j);

            if (nodes.Count == 0)
            {
                ToLuaNode<string> node = new ToLuaNode<string>();
                node.value = ns[j];
                parent.childs.Add(node);
                node.parent = parent;
                node.layer = j;
                parent = node;
            }
            else
            {
                bool flag = false;
                int index = 0;

                for (int i = 0; i < nodes.Count; i++)
                {
                    int count = j;
                    int size = j;
                    ToLuaNode<string> nodecopy = nodes[i];

                    while (nodecopy.parent != null)
                    {
                        nodecopy = nodecopy.parent;
                        if (nodecopy.value != null && nodecopy.value == ns[--count])
                        {
                            size--;
                        }
                    }

                    if (size == 0)
                    {
                        index = i;
                        flag = true;
                        break;
                    }
                }

                if (!flag)
                {
                    ToLuaNode<string> nnode = new ToLuaNode<string>();
                    nnode.value = ns[j];
                    nnode.layer = j;
                    nnode.parent = parent;
                    parent.childs.Add(nnode);
                    parent = nnode;
                }
                else
                {
                    parent = nodes[index];
                }
            }
        }
    }

    static string GetSpaceNameFromTree(ToLuaNode<string> node)
    {
        string name = node.value;

        while (node.parent != null && node.parent.value != null)
        {
            node = node.parent;
            name = node.value + "." + name;
        }

        return name;
    }

    static void GenLuaBinder()
    {

        allTypes.Clear();
        ToLuaTree<string> tree = InitTree();        
        StringBuilder sb = new StringBuilder();
        List<DelegateType> dtList = new List<DelegateType>();

        List<DelegateType> list = new List<DelegateType>();
        list.AddRange(CustomSettings.customDelegateList);
        HashSet<Type> set = GetCustomTypeDelegates();

        List<BindType> backupList = new List<BindType>();
        backupList.AddRange(allTypes);
        ToLuaNode<string> root = tree.GetRoot();
        string libname = null;

        foreach (Type t in set)
        {
            if (null == list.Find((p) => { return p.type == t; }))
            {
                DelegateType dt = new DelegateType(t);                                
                AddSpaceNameToTree(tree, root, ToLuaExport.GetNameSpace(t, out libname));
                list.Add(dt);
            }
        }

        sb.AppendLineEx("//this source code was auto-generated by tolua#, do not modify it");
        sb.AppendLineEx("using System;");
        //sb.AppendLineEx("using UnityEngine;");
        sb.AppendLineEx("using LuaInterface;");
        sb.AppendLineEx();
        sb.AppendLineEx("public static class LuaBinder");
        sb.AppendLineEx("{");
        sb.AppendLineEx("\tpublic static void Bind(LuaState L)");
        sb.AppendLineEx("\t{");
        //sb.AppendLineEx("\t\tfloat t = Time.realtimeSinceStartup;");
        sb.AppendLineEx("\t\tL.BeginModule(null);");

        GenRegisterInfo(null, sb, list, dtList);

        Action<ToLuaNode<string>> begin = (node) =>
        {
            if (node.value == null)
            {
                return;
            }

            sb.AppendFormat("\t\tL.BeginModule(\"{0}\");\r\n", node.value);
            string space = GetSpaceNameFromTree(node);

            GenRegisterInfo(space, sb, list, dtList);
        };

        Action<ToLuaNode<string>> end = (node) =>
        {
            if (node.value != null)
            {
                sb.AppendLineEx("\t\tL.EndModule();");
            }
        };

        tree.DepthFirstTraversal(begin, end, tree.GetRoot());        
        sb.AppendLineEx("\t\tL.EndModule();");
        
        if (CustomSettings.dynamicList.Count > 0)
        {
            sb.AppendLineEx("\t\tL.BeginPreLoad();");            

            for (int i = 0; i < CustomSettings.dynamicList.Count; i++)
            {
                Type t1 = CustomSettings.dynamicList[i];
                BindType bt = backupList.Find((p) => { return p.type == t1; });
                if (bt != null) sb.AppendFormat("\t\tL.AddPreLoad(\"{0}\", LuaOpen_{1}, typeof({0}));\r\n", bt.name, bt.wrapName);
            }

            sb.AppendLineEx("\t\tL.EndPreLoad();");
        }

        //sb.AppendLineEx("\t\tDebugger.Log(\"Register lua type cost time: {0}\", Time.realtimeSinceStartup - t);");
        sb.AppendLineEx("\t}");

        for (int i = 0; i < dtList.Count; i++)
        {
            ToLuaExport.GenEventFunction(dtList[i].type, sb);
        }

        if (CustomSettings.dynamicList.Count > 0)
        {
            
            for (int i = 0; i < CustomSettings.dynamicList.Count; i++)
            {
                Type t = CustomSettings.dynamicList[i];
                BindType bt = backupList.Find((p) => { return p.type == t; });
                if (bt != null) GenPreLoadFunction(bt, sb);
            }            
        }

        sb.AppendLineEx("}\r\n");
        allTypes.Clear();
        string file = CustomSettings.saveDir + "LuaBinder.cs";

        using (StreamWriter textWriter = new StreamWriter(file, false, Encoding.UTF8))
        {
            textWriter.Write(sb.ToString());
            textWriter.Flush();
            textWriter.Close();
        }
        Console.WriteLine("Generate LuaBinder over !");
    }

    static void GenRegisterInfo(string nameSpace, StringBuilder sb, List<DelegateType> delegateList, List<DelegateType> wrappedDelegatesCache)
    {
        for (int i = 0; i < allTypes.Count; i++)
        {
            Type dt = CustomSettings.dynamicList.Find((p) => { return allTypes[i].type == p; });

            if (dt == null && allTypes[i].nameSpace == nameSpace)
            {
                string str = "\t\t" + allTypes[i].wrapName + "Wrap.Register(L);\r\n";
                sb.Append(str);
                allTypes.RemoveAt(i--);
            }
        }

        string funcName = null;

        for (int i = 0; i < delegateList.Count; i++)
        {
            DelegateType dt = delegateList[i];
            Type type = dt.type;
            string typeSpace = ToLuaExport.GetNameSpace(type, out funcName);

            if (typeSpace == nameSpace)
            {
                funcName = ToLuaExport.ConvertToLibSign(funcName);
                string abr = dt.abr;
                abr = abr == null ? funcName : abr;
                sb.AppendFormat("\t\tL.RegFunction(\"{0}\", {1});\r\n", abr, dt.name);
                wrappedDelegatesCache.Add(dt);
            }
        }
    }

    static void GenPreLoadFunction(BindType bt, StringBuilder sb)
    {
        string funcName = "LuaOpen_" + bt.wrapName;

        sb.AppendLineEx("\r\n\t[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]");
        sb.AppendFormat("\tstatic int {0}(IntPtr L)\r\n", funcName);
        sb.AppendLineEx("\t{");
        sb.AppendLineEx("\t\ttry");
        sb.AppendLineEx("\t\t{");        
        sb.AppendLineEx("\t\t\tLuaState state = LuaState.Get(L);");
        sb.AppendFormat("\t\t\tstate.BeginPreModule(\"{0}\");\r\n", bt.nameSpace);
        sb.AppendFormat("\t\t\t{0}Wrap.Register(state);\r\n", bt.wrapName);
        sb.AppendFormat("\t\t\tint reference = state.GetMetaReference(typeof({0}));\r\n", bt.name);
        sb.AppendLineEx("\t\t\tstate.EndPreModule(L, reference);");                
        sb.AppendLineEx("\t\t\treturn 1;");
        sb.AppendLineEx("\t\t}");
        sb.AppendLineEx("\t\tcatch(Exception e)");
        sb.AppendLineEx("\t\t{");
        sb.AppendLineEx("\t\t\treturn LuaDLL.toluaL_exception(L, e);");
        sb.AppendLineEx("\t\t}");
        sb.AppendLineEx("\t}");
    }

    //生成所有的Lua
   public static void GenLuaAll()
    {
        beAutoGen = true;
        GenLuaDelegates();
        GenerateClassWraps();
        GenLuaBinder();
        beAutoGen = false;
    }
    //清除所有的Lua
    public static void ClearLuaWraps()
    {
        string[] files = Directory.GetFiles(CustomSettings.saveDir, "*.cs", SearchOption.TopDirectoryOnly);

        for (int i = 0; i < files.Length; i++)
        {
            File.Delete(files[i]);
        }

        ToLuaExport.Clear();
        List<DelegateType> list = new List<DelegateType>();
        ToLuaExport.GenDelegates(list.ToArray());
        ToLuaExport.Clear();

        StringBuilder sb = new StringBuilder();
        sb.AppendLineEx("using System;");
        sb.AppendLineEx("using LuaInterface;");
        sb.AppendLineEx();
        sb.AppendLineEx("public static class LuaBinder");
        sb.AppendLineEx("{");
        sb.AppendLineEx("\tpublic static void Bind(LuaState L)");
        sb.AppendLineEx("\t{");
        sb.AppendLineEx("\t\tthrow new LuaException(\"Please generate LuaBinder files first!\");");
        sb.AppendLineEx("\t}");
        sb.AppendLineEx("}");

        string file = CustomSettings.saveDir + "LuaBinder.cs";

        using (StreamWriter textWriter = new StreamWriter(file, false, Encoding.UTF8))
        {
            textWriter.Write(sb.ToString());
            textWriter.Flush();
            textWriter.Close();
        }

    }

    static void GetAllDirs(string dir, List<string> list)
    {
        string[] dirs = Directory.GetDirectories(dir);
        list.AddRange(dirs);

        for (int i = 0; i < dirs.Length; i++)
        {
            GetAllDirs(dirs[i], list);
        }
    }

    //static string GetFileContentMD5(string file)
    //{
    //    if (!File.Exists(file))
    //    {
    //        return string.Empty;
    //    }
    //    FileStream fs = new FileStream(file, FileMode.Open);
    //    System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
    //    byte[] retVal = md5.ComputeHash(fs);
    //    fs.Close();
    //    StringBuilder sb = StringBuilderCache.Acquire();
    //    for (int i = 0; i < retVal.Length; i++)
    //    {
    //        sb.Append(retVal[i].ToString("x2"));
    //    }
    //    return StringBuilderCache.GetStringAndRelease(sb);
    //}
}
