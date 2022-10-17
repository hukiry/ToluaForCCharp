
using System;
using System.Collections.Generic;
using LuaInterface;


using BindType = ToLuaMenu.BindType;
using System.Reflection;

public static class CustomSettings
{
    public static string saveDir = "../../../Assets/Source/Generate/";    
    public static string toluaBaseType = "../../../Assets/ToLua/BaseType/";
    public static string baseLuaDir = "../../../Assets/ToLua/Lua/";
    public static string injectionFilesPath = "../../../Assets/ToLua/Injection/";

    //导出时强制做为静态类的类型(注意customTypeList 还要添加这个类型才能导出)
    //unity 有些类作为sealed class, 其实完全等价于静态类
    public static List<Type> staticClassTypes = new List<Type>{};

    //附加导出委托类型(在导出委托时, customTypeList 中牵扯的委托类型都会导出， 无需写在这里)
    public static DelegateType[] customDelegateList = 
    {        
        _DT(typeof(Action)),                
        _DT(typeof(System.Predicate<int>)),
        _DT(typeof(System.Action<int>)),
        _DT(typeof(System.Comparison<int>)),
        _DT(typeof(System.Func<int, int>)),
    };

    //在这里添加你要导出注册到lua的类型列表
    public static BindType[] customTypeList =
    {                
        _GT(typeof(LuaInjectionStation)),
        _GT(typeof(InjectType)),
        _GT(typeof(LuaProfiler)),
        _GT(typeof(List<int>)),
    };

    public static List<Type> dynamicList = new List<Type>(){};
    //重载函数，相同参数个数，相同位置out参数匹配出问题时, 需要强制匹配解决
    //使用方法参见例子14
    public static List<Type> outList = new List<Type>(){}; 
    //ngui优化，下面的类没有派生类，可以作为sealed class
    public static List<Type> sealedList = new List<Type>(){};
    public static BindType _GT(Type t)=>new BindType(t);
    public static DelegateType _DT(Type t)=> new DelegateType(t);

}
