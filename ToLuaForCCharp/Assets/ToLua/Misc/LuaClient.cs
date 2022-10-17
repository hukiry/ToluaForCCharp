
using System.Collections.Generic;
using LuaInterface;
using System.Collections;
using System.IO;
using System;

public class LuaClient 
{
    public static LuaClient Instance
    {
        get;
        protected set;
    } = new LuaClient();

    protected LuaState luaState = null;
    protected LuaLooper loop = null;
    protected LuaFunction levelLoaded = null;

    protected bool openLuaSocket = false;
    protected bool beZbStart = false;

    protected LuaFileUtils InitLoader()=>LuaFileUtils.Instance;       

    protected void OpenLibs()
    {
        luaState.OpenLibs(LuaDLL.luaopen_pb);
        luaState.OpenLibs(LuaDLL.luaopen_struct);
        luaState.OpenLibs(LuaDLL.luaopen_lpeg);
        this.OpenCJson();
        if (LuaConst.openLuaSocket)
        {
            OpenLuaSocket();            
        }        

    }


    [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
    static int LuaOpen_Socket_Core(IntPtr L)
    {        
        return LuaDLL.luaopen_socket_core(L);
    }

    [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
    static int LuaOpen_Mime_Core(IntPtr L)
    {
        return LuaDLL.luaopen_mime_core(L);
    }

    protected void OpenLuaSocket()
    {
        LuaConst.openLuaSocket = true;

        luaState.BeginPreLoad();
        luaState.RegFunction("socket.core", LuaOpen_Socket_Core);
        luaState.RegFunction("mime.core", LuaOpen_Mime_Core);                
        luaState.EndPreLoad();                     
    }

    //cjson 比较特殊，只new了一个table，没有注册库，这里注册一下
    protected void OpenCJson()
    {
        luaState.LuaGetField(LuaIndexes.LUA_REGISTRYINDEX, "_LOADED");
        luaState.OpenLibs(LuaDLL.luaopen_cjson);
        luaState.LuaSetField(-2, "cjson");

        luaState.OpenLibs(LuaDLL.luaopen_cjson_safe);
        luaState.LuaSetField(-2, "cjson.safe");                               
    }

    public void Init()
    {
        Instance = this;

        InitLoader();
        luaState = new LuaState();
        OpenLibs();
        luaState.LuaSetTop(0);
        LuaBinder.Bind(luaState);
        DelegateFactory.Init();

        luaState.Start();
        luaState.DoFile("Main.lua");


        LuaFunction main = luaState.GetFunction("Main");
        main.Call();
        main.Dispose();
        main = null;
    }

    public void Destroy()
    {
        if (luaState != null)
        { 
            luaState.Call("OnApplicationQuit", false);
            LuaState state = luaState;
            luaState = null;

            state.Dispose();
            Instance = null;
        }
    }

    public static LuaState GetMainState()=> Instance.luaState;

}
