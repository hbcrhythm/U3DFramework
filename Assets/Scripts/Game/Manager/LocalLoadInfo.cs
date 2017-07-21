using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Game.Manager
{
    class LocalLoadInfo
    {
        public string path;
        public Action<WWW> callBack;
        public WWW www;

        public bool CheckLoad()
        {
            if (www != null)
            {
                if (www.isDone)
                {
                    if (string.IsNullOrEmpty(www.error) == false)
                    {
                        Debug.LogError(path + "  " + www.error);
                    }

                    return true;
                }
            }
            else
            {
                www = new WWW(path);
            }

            return false;
        }

        public void Unload()
        {
            var bundle = www.assetBundle;
            callBack(www);
            bundle.Unload(true);
            www.Dispose();
        }
    }
}
