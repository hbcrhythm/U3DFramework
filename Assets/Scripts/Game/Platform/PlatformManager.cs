using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Assets.Scripts.Game.Utils;
using System.IO;
using Game.Core;

namespace Assets.Scripts.Game.Platform
{
    public class PlatformManager : Singleton<PlatformManager>
    {
        public string mPlatformName { get; private set; }
        static string sPlatformName;
        public static string GetPlatformName()
        {
            if (string.IsNullOrEmpty(sPlatformName))
            {
                sPlatformName = GetLastPlatformName();
            }

            return sPlatformName;
        }

        public static string GetLastPlatformName()
        {

#if UNITY_IOS && !UNITY_EDITOR
            string customPlatform = IOSUtil.GetCustomPlatform();
            if (customPlatform.Length > 0)
            {
                return customPlatform;
            }
#endif

            TextAsset textAsset = Resources.Load("Platform/config") as TextAsset;
            string name = textAsset.text.Trim();
            Resources.UnloadAsset(textAsset);

            return name;
        }

        public static string GetPlatformConfigPath()
        {
            return string.Format("platform/{0}/config",GetPlatformName());
        }
    }
}