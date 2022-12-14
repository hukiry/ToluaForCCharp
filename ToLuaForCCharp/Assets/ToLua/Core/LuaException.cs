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
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace LuaInterface
{
    public class LuaException : Exception
    {
        public static Exception luaStack = null;
        public static string projectFolder = null;
        public static int InstantiateCount = 0;
        public static int SendMsgCount = 0;
        public static IntPtr L = IntPtr.Zero;

        public override string StackTrace
        {
            get
            {
                return _stack;
            }
        }

        protected string _stack = string.Empty;        

        public LuaException(string msg, Exception e = null, int skip = 1)
            : base(msg)
        {                                  
            if (e != null)
            {
                if (e is LuaException)
                {
                    _stack = e.StackTrace;
                }
                else
                {                                        
                    StackTrace trace = new StackTrace(e, true);
                    StringBuilder sb = new StringBuilder();
                    ExtractFormattedStackTrace(trace, sb);                    
                    StackTrace self = new StackTrace(skip, true);
                    ExtractFormattedStackTrace(self, sb, trace);
                    _stack = sb.ToString();                    
                }
            }
            else
            {
                StackTrace self = new StackTrace(skip, true);
                StringBuilder sb = new StringBuilder();
                ExtractFormattedStackTrace(self, sb);
                _stack = sb.ToString();
            }

            Console.WriteLine(_stack.ToString());
        }

        public static Exception GetLastError()
        {
            Exception last = luaStack;
            luaStack = null;
            return last;
        }

        public static void ExtractFormattedStackTrace(StackTrace trace, StringBuilder sb, StackTrace skip = null)
        {
            Console.WriteLine(trace.ToString()+sb.ToString());
            if (skip != null)
            {
                Console.WriteLine(skip.ToString());
            }
        }
    }
}