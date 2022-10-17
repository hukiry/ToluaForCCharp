
using System;
using System.Collections.Generic;
using System.Reflection;

namespace LuaInterface
{
    //代表一个反射函数
    public sealed class LuaMethod
    {        
        MethodInfo method = null;
        List<Type> list = new List<Type>();
        Type kclass = null;

        [NoToLuaAttribute]
        public LuaMethod(MethodInfo md, Type t, Type[] types)
        {
            method = md;
            kclass = t;            

            if (types != null)
            {
                list.AddRange(types);
            }
        }
        
        public int Call(IntPtr L)
        {            
            object[] args = null;
            object obj = null;
            int offset = 1;

            if (!method.IsStatic)
            {
                offset += 1;
                obj = ToLua.CheckObject(L, 2, kclass);
            }

            ToLua.CheckArgsCount(L, list.Count + offset);

            if (list.Count > 0)
            {
                args = new object[list.Count];
                offset += 1;

                for (int i = 0; i < list.Count; i++)
                {
                    bool isRef = list[i].IsByRef;
                    Type t0 = isRef ? list[i].GetElementType() : list[i];
                    object o = ToLua.CheckVarObject(L, i + offset, t0);                    
                    args[i] = o;
                }
            }
            
            object ret = method.Invoke(obj, args);
            int count = 0;

            if (method.ReturnType != typeof(void))
            {
                ++count;
                ToLua.Push(L, ret);
            }

            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].IsByRef)
                {
                    ++count;
                    ToLua.Push(L, args[i]);
                }
            }

            return count;
        }

        public void Destroy()
        {
            method = null;
            list.Clear();
        }
    }
}
