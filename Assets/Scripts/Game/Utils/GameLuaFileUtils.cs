using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Game.Manager;
using LuaInterface;
using UnityEngine;

namespace Assets.Scripts.Game.Utils
{
    class LuaBytesInfo
    {
        public string key;
        public string keyLua;
        public byte[] bytes;
    }

    class GameLuaFileUtils : LuaFileUtils
    {
        Dictionary<string, LuaBytesInfo> luaBytesInfoList;
        HashSet<string> luaFilesSet;

        public void Init(Action callBack)
        {
            Instance = this;

            if (Application.isEditor && GameDebugConfig.loadLuaBytes == false)
            {
                luaFilesSet = new HashSet<string>();
                string[] allFiles = System.IO.Directory.GetFiles("Assets/Lua", "*.lua", System.IO.SearchOption.AllDirectories);
                foreach (var files in allFiles)
                {
                    luaFilesSet.Add(System.IO.Path.GetFileNameWithoutExtension(files));
                }

                allFiles = System.IO.Directory.GetFiles("Assets/ToLua/Lua", "*.lua", System.IO.SearchOption.AllDirectories);
                foreach (var files in allFiles)
                {
                    luaFilesSet.Add(System.IO.Path.GetFileNameWithoutExtension(files));
                }

                beZip = false;
                callBack();
            }
            else
            {
                beZip = true;

                ResourceManager.Instance.PriorLoadPersistent(true, "lua/lua", delegate(WWW www)
                {
                    TextAsset textAsset = www.assetBundle.LoadAsset<TextAsset>("lua.bytes");
                    luaBytesInfoList = Deserialize(textAsset.bytes);
                    callBack();
                });
            }
        }

        Dictionary<string, LuaBytesInfo> Deserialize(byte[] bytes)
        {
            var dic = new Dictionary<string, LuaBytesInfo>();
            int offset = 0;
            string lua = ".lua";

            while (offset < bytes.Length)
            {
                int keyLen = BitConverter.ToInt32(bytes, offset);
                offset += 4;

                string key = System.Text.Encoding.UTF8.GetString(bytes, offset, keyLen);
                offset += keyLen;

                int dataLen = BitConverter.ToInt32(bytes, offset);
                offset += 4;

                byte[] luaBytes = new byte[dataLen];
                Array.Copy(bytes, offset, luaBytes, 0, dataLen);

                LuaBytesInfo info = new LuaBytesInfo();
                info.key = key;
                info.keyLua = key + lua;
                info.bytes = luaBytes;

                dic[key] = info;
                dic[key + lua] = info;

                offset += dataLen;
            }

            return dic;
        }

        public override byte[] ReadFile(string fileName)
        {
            if (!beZip)
            {
                if (luaFilesSet.Contains(Path.GetFileNameWithoutExtension(fileName)) == false)
                {
                    throw new LuaException("lua文件名大小写错误：" + fileName);
                }

                string path = FindFile(fileName);
                byte[] str = null;

                if (!string.IsNullOrEmpty(path) && File.Exists(path))
                {
#if !UNITY_WEBPLAYER
                    str = File.ReadAllBytes(path);
#else
                    throw new LuaException("can't run in web platform, please switch to other platform");
#endif
                }

                return str;
            }
            else
            {
                LuaBytesInfo info = null;

                if (luaBytesInfoList.TryGetValue(fileName, out info) == false)
                {
                    Debug.LogError(string.Format("ReadFile error:{0}", fileName));
                }

                luaBytesInfoList.Remove(info.key);
                luaBytesInfoList.Remove(info.keyLua);
                return info.bytes;
            }
        }
    }
}
