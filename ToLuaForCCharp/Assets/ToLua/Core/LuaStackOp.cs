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

using System;
using System.Runtime.InteropServices;
using System.Collections;

namespace LuaInterface
{
    public class LuaStackOp
    {
        public sbyte ToSByte(IntPtr L, int stackPos)
        {
            double ret = LuaDLL.lua_tonumber(L, stackPos);
            return Convert.ToSByte(ret);
        }

        public byte ToByte(IntPtr L, int stackPos)
        {
            double ret = LuaDLL.lua_tonumber(L, stackPos);
            return Convert.ToByte(ret);
        }

        public short ToInt16(IntPtr L, int stackPos)
        {
            double ret = LuaDLL.lua_tonumber(L, stackPos);
            return Convert.ToInt16(ret);
        }

        public ushort ToUInt16(IntPtr L, int stackPos)
        {
            double ret = LuaDLL.lua_tonumber(L, stackPos);
            return Convert.ToUInt16(ret);
        }

        public char ToChar(IntPtr L, int stackPos)
        {
            double ret = LuaDLL.lua_tonumber(L, stackPos);
            return Convert.ToChar(ret);
        }

        public int ToInt32(IntPtr L, int stackPos)
        {
            double ret = LuaDLL.lua_tonumber(L, stackPos);
            return Convert.ToInt32(ret);
        }

        public uint ToUInt32(IntPtr L, int stackPos)
        {
            double ret = LuaDLL.lua_tonumber(L, stackPos);
            return Convert.ToUInt32(ret);
        }

        public decimal ToDecimal(IntPtr L, int stackPos)
        {
            double ret = LuaDLL.lua_tonumber(L, stackPos);
            return Convert.ToDecimal(ret);
        }

        public float ToFloat(IntPtr L, int stackPos)
        {
            double ret = LuaDLL.lua_tonumber(L, stackPos);
            return Convert.ToSingle(ret);
        }

        public LuaByteBuffer ToLuaByteBuffer(IntPtr L, int stackPos)
        {
            return new LuaByteBuffer(ToLua.ToByteBuffer(L, stackPos));
        }   

        public IEnumerator ToIter(IntPtr L, int stackPos)
        {
            return (IEnumerator)ToLua.ToObject(L, stackPos);
        }

        public Type ToType(IntPtr L, int stackPos)
        {
            return (Type)ToLua.ToObject(L, stackPos);
        }

        public EventObject ToEventObject(IntPtr L, int stackPos)
        {
            return (EventObject)ToLua.ToObject(L, stackPos);
        }

        public object ToObject(IntPtr L, int stackPos)
        {
            return ToLua.ToObject(L, stackPos);
        }

        public sbyte CheckSByte(IntPtr L, int stackPos)
        {
            double ret = LuaDLL.luaL_checknumber(L, stackPos);
            return Convert.ToSByte(ret);
        }

        public byte CheckByte(IntPtr L, int stackPos)
        {
            double ret = LuaDLL.luaL_checknumber(L, stackPos);
            return Convert.ToByte(ret);
        }

        public short CheckInt16(IntPtr L, int stackPos)
        {
            double ret = LuaDLL.luaL_checknumber(L, stackPos);
            return Convert.ToInt16(ret);
        }

        public ushort CheckUInt16(IntPtr L, int stackPos)
        {
            double ret = LuaDLL.luaL_checknumber(L, stackPos);
            return Convert.ToUInt16(ret);
        }

        public char CheckChar(IntPtr L, int stackPos)
        {
            double ret = LuaDLL.luaL_checknumber(L, stackPos);
            return Convert.ToChar(ret);
        }

        public int CheckInt32(IntPtr L, int stackPos)
        {
            double ret = LuaDLL.luaL_checknumber(L, stackPos);
            return Convert.ToInt32(ret);
        }

        public uint CheckUInt32(IntPtr L, int stackPos)
        {
            double ret = LuaDLL.luaL_checknumber(L, stackPos);
            return Convert.ToUInt32(ret);
        }

        public decimal CheckDecimal(IntPtr L, int stackPos)
        {
            double ret = LuaDLL.luaL_checknumber(L, stackPos);
            return Convert.ToDecimal(ret);
        }     

        public float CheckFloat(IntPtr L, int stackPos)
        {
            double ret = LuaDLL.luaL_checknumber(L, stackPos);
            return Convert.ToSingle(ret);
        }

        public IntPtr CheckIntPtr(IntPtr L, int stackPos)
        {
            LuaTypes luaType = LuaDLL.lua_type(L, stackPos);

            switch(luaType)
            {
                case LuaTypes.LUA_TNIL:
                    return IntPtr.Zero;
                case LuaTypes.LUA_TLIGHTUSERDATA:
                    return LuaDLL.lua_touserdata(L, stackPos);
                default:
                    LuaDLL.luaL_typerror(L, stackPos, "IntPtr");
                    return IntPtr.Zero;
            }            
        }

        public UIntPtr CheckUIntPtr(IntPtr L, int stackPos)
        {
            throw new LuaException("NYI");
        }

        public LuaByteBuffer CheckLuaByteBuffer(IntPtr L, int stackPos)
        {
            return new LuaByteBuffer(ToLua.CheckByteBuffer(L, stackPos));
        }

        public EventObject CheckEventObject(IntPtr L, int stackPos)
        {
            return (EventObject)ToLua.CheckObject(L, stackPos, typeof(EventObject));
        }


        public void Push(IntPtr L, sbyte n)
        {
            LuaDLL.lua_pushnumber(L, n);
        }

        public void Push(IntPtr L, byte n)
        {
            LuaDLL.lua_pushnumber(L, n);
        }

        public void Push(IntPtr L, short n)
        {
            LuaDLL.lua_pushnumber(L, n);
        }

        public void Push(IntPtr L, ushort n)
        {
            LuaDLL.lua_pushnumber(L, n);
        }

        public void Push(IntPtr L, char n)
        {
            LuaDLL.lua_pushnumber(L, n);
        }

        public void Push(IntPtr L, int n)
        {
            LuaDLL.lua_pushnumber(L, n);
        }

        public void Push(IntPtr L, uint n)
        {
            LuaDLL.lua_pushnumber(L, n);
        }

        public void Push(IntPtr L, decimal n)
        {
            LuaDLL.lua_pushnumber(L, (double)n);
        }

        public void Push(IntPtr L, float n)
        {
            LuaDLL.lua_pushnumber(L, n);
        }

        public void Push(IntPtr L, UIntPtr p)
        {
            throw new LuaException("NYI");
        }

        public void Push(IntPtr L, Delegate ev)
        {
            ToLua.Push(L, ev);
        }

        public void Push(IntPtr L, object obj)
        {
            ToLua.Push(L, obj);
        }

     

        #region Nullable
        public Nullable<sbyte> ToNullSByte(IntPtr L, int stackPos)
        {
            if (LuaDLL.lua_type(L, stackPos) == LuaTypes.LUA_TNIL)
            {
                return null;
            }

            double ret = LuaDLL.lua_tonumber(L, stackPos);
            return Convert.ToSByte(ret);
        }

        public Nullable<byte> ToNullByte(IntPtr L, int stackPos)
        {
            if (LuaDLL.lua_type(L, stackPos) == LuaTypes.LUA_TNIL)
            {
                return null;
            }

            double ret = LuaDLL.lua_tonumber(L, stackPos);
            return Convert.ToByte(ret);
        }

        public Nullable<short> ToNullInt16(IntPtr L, int stackPos)
        {
            if (LuaDLL.lua_type(L, stackPos) == LuaTypes.LUA_TNIL)
            {
                return null;
            }

            double ret = LuaDLL.lua_tonumber(L, stackPos);
            return Convert.ToInt16(ret);
        }

        public Nullable<ushort> ToNullUInt16(IntPtr L, int stackPos)
        {
            if (LuaDLL.lua_type(L, stackPos) == LuaTypes.LUA_TNIL)
            {
                return null;
            }

            double ret = LuaDLL.lua_tonumber(L, stackPos);
            return Convert.ToUInt16(ret);
        }

        public Nullable<char> ToNullChar(IntPtr L, int stackPos)
        {
            if (LuaDLL.lua_type(L, stackPos) == LuaTypes.LUA_TNIL)
            {
                return null;
            }

            double ret = LuaDLL.lua_tonumber(L, stackPos);
            return Convert.ToChar(ret);
        }

        public Nullable<int> ToNullInt32(IntPtr L, int stackPos)
        {
            if (LuaDLL.lua_type(L, stackPos) == LuaTypes.LUA_TNIL)
            {
                return null;
            }

            double ret = LuaDLL.lua_tonumber(L, stackPos);
            return Convert.ToInt32(ret);
        }

        public Nullable<uint> ToNullUInt32(IntPtr L, int stackPos)
        {
            if (LuaDLL.lua_type(L, stackPos) == LuaTypes.LUA_TNIL)
            {
                return null;
            }

            double ret = LuaDLL.lua_tonumber(L, stackPos);
            return Convert.ToUInt32(ret);
        }

        public Nullable<decimal> ToNullDecimal(IntPtr L, int stackPos)
        {
            if (LuaDLL.lua_type(L, stackPos) == LuaTypes.LUA_TNIL)
            {
                return null;
            }

            double ret = LuaDLL.lua_tonumber(L, stackPos);
            return Convert.ToDecimal(ret);
        }

        public Nullable<float> ToNullFloat(IntPtr L, int stackPos)
        {
            if (LuaDLL.lua_type(L, stackPos) == LuaTypes.LUA_TNIL)
            {
                return null;
            }

            double ret = LuaDLL.lua_tonumber(L, stackPos);
            return Convert.ToSingle(ret);
        }

        public Nullable<double> ToNullNumber(IntPtr L, int stackPos)
        {
            if (LuaDLL.lua_type(L, stackPos) == LuaTypes.LUA_TNIL)
            {
                return null;
            }

            return LuaDLL.lua_tonumber(L, stackPos);
        }

        public Nullable<bool> ToNullBool(IntPtr L, int stackPos)
        {
            if (LuaDLL.lua_type(L, stackPos) == LuaTypes.LUA_TNIL)
            {
                return null;
            }

            return LuaDLL.lua_toboolean(L, stackPos);
        }

        public Nullable<long> ToNullInt64(IntPtr L, int stackPos)
        {
            if (LuaDLL.lua_type(L, stackPos) == LuaTypes.LUA_TNIL)
            {
                return null;
            }

            return LuaDLL.tolua_toint64(L, stackPos);
        }

        public Nullable<ulong> ToNullUInt64(IntPtr L, int stackPos)
        {
            if (LuaDLL.lua_type(L, stackPos) == LuaTypes.LUA_TNIL)
            {
                return null;
            }

            return LuaDLL.tolua_touint64(L, stackPos);
        }

        public sbyte[] ToSByteArray(IntPtr L, int stackPos)
        {
            return ToLua.ToNumberArray<sbyte>(L, stackPos);
        }

        public short[] ToInt16Array(IntPtr L, int stackPos)
        {
            return ToLua.ToNumberArray<short>(L, stackPos);
        }

        public ushort[] ToUInt16Array(IntPtr L, int stackPos)
        {
            return ToLua.ToNumberArray<ushort>(L, stackPos);
        }

        public decimal[] ToDecimalArray(IntPtr L, int stackPos)
        {
            return ToLua.ToNumberArray<decimal>(L, stackPos);
        }

        public float[] ToFloatArray(IntPtr L, int stackPos)
        {
            return ToLua.ToNumberArray<float>(L, stackPos);
        }

        public double[] ToDoubleArray(IntPtr L, int stackPos)
        {
            return ToLua.ToNumberArray<double>(L, stackPos);
        }

        public int[] ToInt32Array(IntPtr L, int stackPos)
        {
            return ToLua.ToNumberArray<int>(L, stackPos);
        }

        public uint[] ToUInt32Array(IntPtr L, int stackPos)
        {
            return ToLua.ToNumberArray<uint>(L, stackPos);
        }

        public long[] ToInt64Array(IntPtr L, int stackPos)
        {
            return ToLua.ToStructArray<long>(L, stackPos);
        }

        public ulong[] ToUInt64Array(IntPtr L, int stackPos)
        {
            return ToLua.ToStructArray<ulong>(L, stackPos);
        }

      

        public Type[] ToTypeArray(IntPtr L, int stackPos)
        {
            return ToLua.ToObjectArray<Type>(L, stackPos);
        }

        public Nullable<sbyte> CheckNullSByte(IntPtr L, int stackPos)
        {
            if (LuaDLL.lua_type(L, stackPos) == LuaTypes.LUA_TNIL)
            {
                return null;
            }

            double ret = LuaDLL.luaL_checknumber(L, stackPos);
            return Convert.ToSByte(ret);
        }

        public Nullable<byte> CheckNullByte(IntPtr L, int stackPos)
        {
            if (LuaDLL.lua_type(L, stackPos) == LuaTypes.LUA_TNIL)
            {
                return null;
            }

            double ret = LuaDLL.luaL_checknumber(L, stackPos);
            return Convert.ToByte(ret);
        }

        public Nullable<short> CheckNullInt16(IntPtr L, int stackPos)
        {
            if (LuaDLL.lua_type(L, stackPos) == LuaTypes.LUA_TNIL)
            {
                return null;
            }

            double ret = LuaDLL.luaL_checknumber(L, stackPos);
            return Convert.ToInt16(ret);
        }

        public Nullable<ushort> CheckNullUInt16(IntPtr L, int stackPos)
        {
            if (LuaDLL.lua_type(L, stackPos) == LuaTypes.LUA_TNIL)
            {
                return null;
            }

            double ret = LuaDLL.luaL_checknumber(L, stackPos);
            return Convert.ToUInt16(ret);
        }

        public Nullable<char> CheckNullChar(IntPtr L, int stackPos)
        {
            if (LuaDLL.lua_type(L, stackPos) == LuaTypes.LUA_TNIL)
            {
                return null;
            }

            double ret = LuaDLL.luaL_checknumber(L, stackPos);
            return Convert.ToChar(ret);
        }

        public Nullable<int> CheckNullInt32(IntPtr L, int stackPos)
        {
            if (LuaDLL.lua_type(L, stackPos) == LuaTypes.LUA_TNIL)
            {
                return null;
            }

            double ret = LuaDLL.luaL_checknumber(L, stackPos);
            return Convert.ToInt32(ret);
        }

        public Nullable<uint> CheckNullUInt32(IntPtr L, int stackPos)
        {
            if (LuaDLL.lua_type(L, stackPos) == LuaTypes.LUA_TNIL)
            {
                return null;
            }

            double ret = LuaDLL.luaL_checknumber(L, stackPos);
            return Convert.ToUInt32(ret);
        }

        public Nullable<decimal> CheckNullDecimal(IntPtr L, int stackPos)
        {
            if (LuaDLL.lua_type(L, stackPos) == LuaTypes.LUA_TNIL)
            {
                return null;
            }

            double ret = LuaDLL.luaL_checknumber(L, stackPos);
            return Convert.ToDecimal(ret);
        }

        public Nullable<float> CheckNullFloat(IntPtr L, int stackPos)
        {
            if (LuaDLL.lua_type(L, stackPos) == LuaTypes.LUA_TNIL)
            {
                return null;
            }

            double ret = LuaDLL.luaL_checknumber(L, stackPos);
            return Convert.ToSingle(ret);
        }

        public Nullable<double> CheckNullNumber(IntPtr L, int stackPos)
        {
            if (LuaDLL.lua_type(L, stackPos) == LuaTypes.LUA_TNIL)
            {
                return null;
            }

            return LuaDLL.luaL_checknumber(L, stackPos);
        }

        public Nullable<bool> CheckNullBool(IntPtr L, int stackPos)
        {
            if (LuaDLL.lua_type(L, stackPos) == LuaTypes.LUA_TNIL)
            {
                return null;
            }

            return LuaDLL.luaL_checkboolean(L, stackPos);
        }

        public Nullable<long> CheckNullInt64(IntPtr L, int stackPos)
        {
            if (LuaDLL.lua_type(L, stackPos) == LuaTypes.LUA_TNIL)
            {
                return null;
            }

            return LuaDLL.tolua_checkint64(L, stackPos);
        }

        public Nullable<ulong> CheckNullUInt64(IntPtr L, int stackPos)
        {
            if (LuaDLL.lua_type(L, stackPos) == LuaTypes.LUA_TNIL)
            {
                return null;
            }

            return LuaDLL.tolua_checkuint64(L, stackPos);
        }

        public sbyte[] CheckSByteArray(IntPtr L, int stackPos)
        {
            return ToLua.CheckNumberArray<sbyte>(L, stackPos);
        }

        public short[] CheckInt16Array(IntPtr L, int stackPos)
        {
            return ToLua.CheckNumberArray<short>(L, stackPos);
        }

        public ushort[] CheckUInt16Array(IntPtr L, int stackPos)
        {
            return ToLua.CheckNumberArray<ushort>(L, stackPos);
        }

        public decimal[] CheckDecimalArray(IntPtr L, int stackPos)
        {
            return ToLua.CheckNumberArray<decimal>(L, stackPos);
        }

        public float[] CheckFloatArray(IntPtr L, int stackPos)
        {
            return ToLua.CheckNumberArray<float>(L, stackPos);
        }

        public double[] CheckDoubleArray(IntPtr L, int stackPos)
        {
            return ToLua.CheckNumberArray<double>(L, stackPos);
        }

        public int[] CheckInt32Array(IntPtr L, int stackPos)
        {
            return ToLua.CheckNumberArray<int>(L, stackPos);
        }

        public uint[] CheckUInt32Array(IntPtr L, int stackPos)
        {
            return ToLua.CheckNumberArray<uint>(L, stackPos);
        }

        public long[] CheckInt64Array(IntPtr L, int stackPos)
        {
            return ToLua.CheckStructArray<long>(L, stackPos);
        }

        public ulong[] CheckUInt64Array(IntPtr L, int stackPos)
        {
            return ToLua.CheckStructArray<ulong>(L, stackPos);
        }

      

        public Type[] CheckTypeArray(IntPtr L, int stackPos)
        {
            return ToLua.CheckObjectArray<Type>(L, stackPos);
        }

        public void Push(IntPtr L, Nullable<sbyte> n)
        {
            if (n == null)
            {
                LuaDLL.lua_pushnil(L);
            }
            else
            {
                LuaDLL.lua_pushnumber(L, n.Value);
            }
        }

        public void Push(IntPtr L, Nullable<byte> n)
        {
            if (n == null)
            {
                LuaDLL.lua_pushnil(L);
            }
            else
            {
                LuaDLL.lua_pushnumber(L, n.Value);
            }
        }

        public void Push(IntPtr L, Nullable<short> n)
        {
            if (n == null)
            {
                LuaDLL.lua_pushnil(L);
            }
            else
            {
                LuaDLL.lua_pushnumber(L, n.Value);
            }
        }

        public void Push(IntPtr L, Nullable<ushort> n)
        {
            if (n == null)
            {
                LuaDLL.lua_pushnil(L);
            }
            else
            {
                LuaDLL.lua_pushnumber(L, n.Value);
            }
        }

        public void Push(IntPtr L, Nullable<char> n)
        {
            if (n == null)
            {
                LuaDLL.lua_pushnil(L);
            }
            else
            {
                LuaDLL.lua_pushnumber(L, n.Value);
            }
        }

        public void Push(IntPtr L, Nullable<int> n)
        {
            if (n == null)
            {
                LuaDLL.lua_pushnil(L);
            }
            else
            {
                LuaDLL.lua_pushnumber(L, n.Value);
            }
        }

        public void Push(IntPtr L, Nullable<uint> n)
        {
            if (n == null)
            {
                LuaDLL.lua_pushnil(L);
            }
            else
            {
                LuaDLL.lua_pushnumber(L, n.Value);
            }
        }

        public void Push(IntPtr L, Nullable<decimal> n)
        {
            if (n == null)
            {
                LuaDLL.lua_pushnil(L);
            }
            else
            {
                LuaDLL.lua_pushnumber(L, Convert.ToDouble(n.Value));
            }
        }

        public void Push(IntPtr L, Nullable<float> n)
        {
            if (n == null)
            {
                LuaDLL.lua_pushnil(L);
            }
            else
            {
                LuaDLL.lua_pushnumber(L, n.Value);
            }
        }

        public void Push(IntPtr L, Nullable<double> n)
        {
            if (n == null)
            {
                LuaDLL.lua_pushnil(L);
            }
            else
            {
                LuaDLL.lua_pushnumber(L, n.Value);
            }
        }

        public void Push(IntPtr L, Nullable<bool> n)
        {
            if (n == null)
            {
                LuaDLL.lua_pushnil(L);
            }
            else
            {
                LuaDLL.lua_pushboolean(L, n.Value);
            }
        }

        public void Push(IntPtr L, Nullable<long> n)
        {
            if (n == null)
            {
                LuaDLL.lua_pushnil(L);
            }
            else
            {
                LuaDLL.tolua_pushint64(L, n.Value);
            }
        }

        public void Push(IntPtr L, Nullable<ulong> n)
        {
            if (n == null)
            {
                LuaDLL.lua_pushnil(L);
            }
            else
            {
                LuaDLL.tolua_pushuint64(L, n.Value);
            }
        }

     
        #endregion
    }
}
