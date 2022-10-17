using System;

namespace ToLuaForCCharp
{
    class Program
    {
        static void Main(string[] args)
        {
            //ToLuaMenu.GenLuaAll();//导出绑定代码
            LuaClient.Instance.Init();
            Console.ReadKey ();
        }

        public static long GetTimeSecond()
        {
            TimeSpan ts = new TimeSpan(DateTime.Now.ToUniversalTime().Ticks - new DateTime(1970, 1, 1).Ticks);
            long t = ts.Ticks / TimeSpan.TicksPerSecond;
            return t;
        }

        public static long GetTimeMilliseconds()
        {
            TimeSpan ts = new TimeSpan(DateTime.Now.ToUniversalTime().Ticks - new DateTime(1970, 1, 1).Ticks);
            return (long)ts.TotalMilliseconds;
        }

    }
}
