using UnityEngine;
using System.Collections;
using LuaInterface;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using System;

namespace Game.Manager
{
    public class LuaManager : MonoBehaviour
    {
        static private LuaState lua;
        private LuaLooper loop = null;

        public static void Init()
        {
            if (lua == null)
            {
                lua = new LuaState();
                OpenLibs();
                lua.LuaSetTop(0);

                LuaBinder.Bind(lua);
                DelegateFactory.Init(); 
            }
        }

        void Awake()
        {
            Init();
            LuaCoroutine.Register(lua, this);
        }

        void Start()
        {
            InitLuaPath();
            lua.Start();    //启动LUAVM
            this.StartMain();
            this.StartLooper();
        }

        void OnDisable()
        {
            Close();
        }

        void StartLooper()
        {
            loop = gameObject.AddComponent<LuaLooper>();
            loop.luaState = lua;
        }

        //cjson 比较特殊，只new了一个table，没有注册库，这里注册一下
        static protected void OpenCJson()
        {
            lua.LuaGetField(LuaIndexes.LUA_REGISTRYINDEX, "_LOADED");
            lua.OpenLibs(LuaDLL.luaopen_cjson);
            lua.LuaSetField(-2, "cjson");

            lua.OpenLibs(LuaDLL.luaopen_cjson_safe);
            lua.LuaSetField(-2, "cjson.safe");
        }

        void StartMain()
        {
            lua.DoFile("GameMain.lua");

            LuaFunction main = lua.GetFunction("GameMain");
            main.Call();
            main.Dispose();
            main = null;
        }

        /// <summary>
        /// 初始化加载第三方库
        /// </summary>
        static void OpenLibs()
        {
            lua.OpenLibs(LuaDLL.luaopen_pb);
            lua.OpenLibs(LuaDLL.luaopen_struct);
            lua.OpenLibs(LuaDLL.luaopen_lpeg);

#if UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
        lua.OpenLibs(LuaDLL.luaopen_bit);
#endif
            if (LuaConst.openLuaSocket)
            {
                OpenLuaSocket();
            }

            if (LuaConst.openLuaDebugger)
            {
                OpenZbsDebugger();
            }

            OpenCJson();
        }

        static public void OpenZbsDebugger(string ip = "localhost")
        {
            if (!Directory.Exists(LuaConst.zbsDir))
            {
                Debugger.LogWarning("ZeroBraneStudio not install or LuaConst.zbsDir not right");
                return;
            }

            if (!LuaConst.openLuaSocket)
            {
                OpenLuaSocket();
            }

            if (!string.IsNullOrEmpty(LuaConst.zbsDir))
            {
                lua.AddSearchPath(LuaConst.zbsDir);
            }

            lua.LuaDoString(string.Format("DebugServerIp = '{0}'", ip));
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

        static protected void OpenLuaSocket()
        {
            LuaConst.openLuaSocket = true;

            lua.BeginPreLoad();
            lua.RegFunction("socket.core", LuaOpen_Socket_Core);
            lua.RegFunction("mime.core", LuaOpen_Mime_Core);
            lua.EndPreLoad();
        }

        /// <summary>
        /// 初始化Lua代码加载路径
        /// </summary>
        void InitLuaPath()
        {
            string rootPath = Application.dataPath;
            lua.AddSearchPath(rootPath + "/Lua");
            lua.AddSearchPath(rootPath + "/ToLua/Lua");
        }

        public object[] DoFile(string filename)
        {
            return lua.DoFile(filename);
        }

        public void LuaGC()
        {
            lua.LuaGC(LuaGCOptions.LUA_GCCOLLECT);
        }

        public void Close()
        {
            loop.Destroy();
            loop = null;

            lua.Dispose();
            lua = null;
        }
    }
}