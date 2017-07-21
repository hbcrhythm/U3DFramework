using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Game.Manager
{
    public class RemoteLoadInfo
    {
        public bool isAssetBundle;
        public string path;
        public UnityWebRequest unityWebRequest;
        public Action<RemoteLoadInfo> callBack;
        public int retryNum = 0;
        public ulong downloadedBytes = 0;
        public bool isDone = false;
        public string errorStr = null;

        public void CreateUnityWebRequest()
        {
            if (unityWebRequest != null)
            {
                unityWebRequest.Dispose();
                unityWebRequest = null;
            }

            if (++retryNum > 2)
            {
                errorStr = "网络异常，请重新检查更新";
                return;
            }

            if (isAssetBundle)
            {
                unityWebRequest = UnityWebRequest.GetAssetBundle(path.Replace(" ", "%20"));
            }
            else
            {
                unityWebRequest = UnityWebRequest.Get(path.Replace(" ", "%20"));
            }

            unityWebRequest.Send();
        }

        public bool CheckLoad()
        {
            if (unityWebRequest != null)
            {
                if (unityWebRequest.isError)
                {
                    Debug.LogError(path + "  " + unityWebRequest.error);
                    CreateUnityWebRequest();
                }
                else if (unityWebRequest.isDone)
                {
                    isDone = true;
                }
                else
                {
                    downloadedBytes = Math.Max(unityWebRequest.downloadedBytes, downloadedBytes);
                }
            }
            else
            {
                CreateUnityWebRequest();
            }

            return isDone;
        }

        public void DoCallBack()
        {
            if (isDone)
            {
                callBack(this);
            }

            UnLoad();
        }

        public void UnLoad()
        {
            if (unityWebRequest != null)
            {
                unityWebRequest.Dispose();
                unityWebRequest = null;
            }
        }
    }
}
