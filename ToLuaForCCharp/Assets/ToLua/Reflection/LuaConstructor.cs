
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace LuaInterface
{
    public sealed class LuaConstructor
    {
        ConstructorInfo method = null;
        List<Type> list = null;

        [NoToLuaAttribute]
        public LuaConstructor(ConstructorInfo func, Type[] types)
        {
            method = func;            

            if (types != null)
            {
                list = new List<Type>(types);
            }
        }

        public int Call(IntPtr L)
        {
            object[] args = null;
            ToLua.CheckArgsCount(L, list.Count + 1);

            if (list.Count > 0)
            {
                args = new object[list.Count];

                for (int i = 0; i < list.Count; i++)
                {
                    bool isRef = list[i].IsByRef;
                    Type t0 = isRef ? list[i].GetElementType() : list[i];
                    object o = ToLua.CheckVarObject(L, i + 2, t0);
                    args[i] = o;
                }
            }

            object ret = method.Invoke(args);
            int count = 1;
            ToLua.Push(L, ret);

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
