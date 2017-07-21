using Assets.Scripts.Game.Platform;
using Game.Manager;
using UnityEngine;
using Assets.Scripts.Game.Utils;
using System.Collections.Generic;
using Assets.Scripts.Game.GameUpdate;
using Assets.Scripts.Game.Events;
using System.IO;
using System;
using UnityEngine.Networking;
using Assets.Scripts.Game.Manager;

namespace Assets.Scripts.Game
{
    public class LaunchVersion
    {
        public const int version = 20170515;
    }

    public class Launch : MonoBehaviour
    {
        void InitBuglySDK()
        {
            //BuglyAgent.ConfigAutoQuitApplication(true);
            // TODO NOT Required. Set the crash reporter type and log to report
            // BuglyAgent.ConfigCrashReporter (1, 2);

            // TODO NOT Required. Enable debug log print, please set false for release version
            //BuglyAgent.ConfigDebugMode(false);
            // TODO NOT Required. Register log callback with 'BuglyAgent.LogCallbackDelegate' to replace the 'Application.RegisterLogCallback(Application.LogCallback)'
            // BuglyAgent.RegisterLogCallback (CallbackDelegate.Instance.OnApplicationLogCallbackHandler);

            // BuglyAgent.ConfigDefault ("Bugly", null, "ronnie", 0);

#if UNITY_IOS
        BuglyAgent.InitWithAppId ("52f47d1a79");
#elif UNITY_ANDROID
            //BuglyAgent.InitWithAppId("36c53b618c");
            BuglyAgent.EnableExceptionHandler();
#endif

            // TODO Required. If you do not need call 'InitWithAppId(string)' to initialize the sdk(may be you has initialized the sdk it associated Android or iOS project),
            // please call this method to enable c# exception handler only.

        }

        private LaunchView mLaunchView;
        private ResourceManager mResourceManager = ResourceManager.Instance;
        private GameObject mMain;
        void Awake()
        {
            //InitBuglySDK();

            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            mMain = new GameObject("Main");
            GameObject.DontDestroyOnLoad(mMain);
            mMain.AddComponent<OutputLog>();

            GameObject.DontDestroyOnLoad(gameObject);
            mLaunchView = new LaunchView(gameObject);

            EventDispatcher.Instance.AddEventListener(EventConstant.RETRY_CHECK_VERSION, CheckVersion);
        }

        void Start()
        {
            CheckVersion();
        }

        bool mPriorLoadPersistent = false;
        public static IniFile sLastConfig;
        string mLastVersion;
        string mResourceList;
        string mPlatformConfigPath;
        void CheckVersion()
        {
            mTotalFileSize = 0;
            mLoadedFileSize = 0;
            mUpdateFiles.Clear();

            EventDispatcher.Instance.Dispatch<string>(EventConstant.UPDATE_TIP, "正在检查服务器最新版本");

            IniFile persistentConfig = null;
            string persistentVersion = "";
            mPlatformConfigPath = PlatformManager.GetPlatformConfigPath();
            mResourceManager.LoadPersistentAndRaw(mPlatformConfigPath, delegate(WWW p)
            {
                persistentConfig = new IniFile(p.assetBundle.LoadAsset<TextAsset>("config").text);
                persistentVersion = persistentConfig.GetValue("game", "version");
            }, delegate(WWW r)
            {
                var rawConfig = new IniFile(r.assetBundle.LoadAsset<TextAsset>("config").text);
                var rawVersion = rawConfig.GetValue("game", "version");

                Debug.Log(string.Format("rawVersion:{0},persistentVersion:{1}", rawVersion, persistentVersion));
                if (PlatformVersion.CompareVersion(rawVersion, persistentVersion) >= 0)
                {
                    sLastConfig = rawConfig;
                    mLastVersion = rawVersion;
                    mPriorLoadPersistent = false;
                }
                else
                {
                    sLastConfig = persistentConfig;
                    mLastVersion = persistentVersion;
                    mPriorLoadPersistent = true;
                }

                if (mPriorLoadPersistent == false)
                {
                    //清空
                    string persistenDataFile = Application.persistentDataPath + "/StreamingAssets";
                    if (Directory.Exists(persistenDataFile))
                    {
                        Directory.Delete(persistenDataFile, true);
                    }
                }

                LuaManager.Init();

                mResourceList = sLastConfig.GetValue("game", "resourceList");
                mResourceManager.LoadRemote(mResourceList + "/" + mPlatformConfigPath, delegate(RemoteLoadInfo info)
                {
                    DownloadHandler www = info.unityWebRequest.downloadHandler;
                    DownloadHandlerAssetBundle downloadHandlerAssetBundle = www as DownloadHandlerAssetBundle;
                    var remoteConfig = new IniFile(downloadHandlerAssetBundle.assetBundle.LoadAsset<TextAsset>("config").text);
                    downloadHandlerAssetBundle.assetBundle.Unload(true);
                    var remoteVersion = remoteConfig.GetValue("game", "version");
                    //remoteVersion = "1.0.3";

                    if (PlatformVersion.CheckUpgradeGame(remoteVersion, mLastVersion))
                    {
                        CheckUpgradeGame();
                    }
                    else if (PlatformVersion.CompareVersion(mLastVersion, remoteVersion) >= 0)
                    {
                        EventDispatcher.Instance.Dispatch<string>(EventConstant.UPDATE_TIP, "当前为最新版本");
                        //不需要更新
                        StartGame();
                    }
                    else
                    {
                        sLastConfig = remoteConfig;
                        EventDispatcher.Instance.Dispatch<string>(EventConstant.UPDATE_TIP, "");
                        //需要更新
                        CheckUpdate();
                    }

                }, true);
            });
        }

        void CheckUpgradeGame()
        {
            EventDispatcher.Instance.Dispatch<string>(EventConstant.UPDATE_TIP, "");
            EventDispatcher.Instance.Dispatch<string, Action>(EventConstant.SHOW_ALERT, "需要更新到最新版本才能进入游戏，点击下载！", delegate()
            {
                CheckUpgradeGame();

#if UNITY_IOS
                IOSUtil.GotoAppStore();
#elif UNITY_ANDROID
                AndroidJavaClassUtil.Call("DownLoadAPK", sLastConfig.GetValue("game", "apk"));
#endif

            });
        }

        void StartGame()
        {
            EventDispatcher.Instance.Dispatch<string>(EventConstant.UPDATE_TIP, "正在初始化游戏");

            mMain.AddComponent<Main>();
        }

        LinkedList<FileMD5Vo> mUpdateFiles = new LinkedList<FileMD5Vo>();
        public static float mTotalFileSize;
        public static float mLoadedFileSize;
        string mBundlesInfoPath = "bundles_info/game_bundles_info";
        void CheckUpdate()
        {
            Dictionary<string, FileMD5Vo> localBundleInfos = null;
            mResourceManager.PriorLoadPersistent(mPriorLoadPersistent, mBundlesInfoPath, delegate(WWW www)
            {
                var textAsset = www.assetBundle.LoadAsset<TextAsset>("game_bundles_info");
                localBundleInfos = ParseMD5Text(textAsset.text);
            });

            mResourceManager.LoadRemote(mResourceList + "/" + mBundlesInfoPath, delegate(RemoteLoadInfo info)
            {
                DownloadHandler www = info.unityWebRequest.downloadHandler;
                DownloadHandlerAssetBundle downloadHandlerAssetBundle = www as DownloadHandlerAssetBundle;
                var textAsset = downloadHandlerAssetBundle.assetBundle.LoadAsset<TextAsset>("game_bundles_info");
                var text = textAsset.text;
                CompareMD5List(mUpdateFiles, out mTotalFileSize, localBundleInfos, ParseMD5Text(textAsset.text));
                downloadHandlerAssetBundle.assetBundle.Unload(true);
                CheckNetwork();
            }, true);
        }

        void CheckNetwork()
        {
            if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
            {
                LoadUpdateBundles();
            }
            else
            {
                var tip = string.Format("本次更新补丁包为<color=#00ff00>{0}</color>,是否下载最新资源？", GetSizeStr(mTotalFileSize));
                EventDispatcher.Instance.Dispatch<string, Action>(EventConstant.SHOW_ALERT, tip, delegate()
                {
                    LoadUpdateBundles();
                });
            }
        }

        void LoadUpdateBundles()
        {
            var last = mUpdateFiles.Last;
            var first = mUpdateFiles.First;
            while (last != null)
            {
                LoadUpdateBundle(last, last == first);
                last = last.Previous;
            }
        }

        void LoadUpdateBundle(LinkedListNode<FileMD5Vo> node, bool checkComplete)
        {
            var bundleName = node.Value.path;

            mResourceManager.LoadRemote(mResourceList + "/" + bundleName, delegate(RemoteLoadInfo info)
            {
                DownloadHandler www = info.unityWebRequest.downloadHandler;
                var result = SaveFile(bundleName, www.data);
                if (result > 0)
                {
                    mLoadedFileSize += node.Value.size;
                    EventDispatcher.Instance.Dispatch<float, float>(EventConstant.UPDATE_PROGRESS, mLoadedFileSize, mTotalFileSize);

                    if (checkComplete)
                    {
                        //更新完成
                        EventDispatcher.Instance.Dispatch<string>(EventConstant.UPDATE_TIP, "更新完成");
                        StartGame();
                    }
                }
                else if (result == -2)
                {
                    info.errorStr = "磁盘空间已满，请先清理磁盘空间，重新检查更新！";
                }
                else if (result == -1)
                {
                    info.errorStr = "补丁文件安装异常，请重新检查更新！";
                }
            }, false);
        }

        static public string GetSizeStr(float size)
        {
            float num = size / (1024 * 1024);
            if (num > 1f)
            {
                return string.Format("{0:0.#}M", num);
            }

            float num2 = size / 1024f;
            if (num2 > 1f)
            {
                return string.Format("{0:0.#}kb", num2);
            }
            return (size.ToString() + "b");
        }

        public int SaveFile(string path, byte[] bytes, int saveCount = 0)
        {
            path = Application.persistentDataPath + "/StreamingAssets/" + path;
            string directory = Path.GetDirectoryName(path);

            if (Directory.Exists(directory) == false)
                Directory.CreateDirectory(directory);

            try
            {
                using (FileStream stream = new FileStream(path, FileMode.Create, FileAccess.ReadWrite))
                {
                    stream.Write(bytes, 0, bytes.Length);
                    stream.Flush();
                }
            }
            catch (Exception exception)
            {
                //磁盘已满
                if (exception.Message.IndexOf("Disk full.") != -1)
                {
                    return -2;
                }

                if (saveCount >= 1)
                {
                    Debug.LogError(string.Format("Fail---Save File Error path:{0},saveCount:{1},len:{2},desc:{3}", path, saveCount, bytes.Length, exception.Message));
                    return -1;
                }

                return SaveFile(path, bytes, ++saveCount);
            }

            return 1;
        }

        Dictionary<string, FileMD5Vo> ParseMD5Text(string text)
        {
            Dictionary<string, FileMD5Vo> dic = new Dictionary<string, FileMD5Vo>();
            var lineList = text.Split('\n');
            foreach (var line in lineList)
            {
                if (line.Length <= 0)
                    continue;

                var infoList = line.Split(',');
                FileMD5Vo vo = new FileMD5Vo();
                vo.path = infoList[0];
                vo.md5 = infoList[1];
                vo.size = int.Parse(infoList[2]);

                dic[vo.path] = vo;
            }
            return dic;
        }

        void CompareMD5List(LinkedList<FileMD5Vo> updateFiles, out float fileSize, Dictionary<string, FileMD5Vo> local, Dictionary<string, FileMD5Vo> remote)
        {
            int size = 0;
            foreach (var kvp in remote)
            {
                var remoteVo = kvp.Value;
                var bundleName = remoteVo.path;
                FileMD5Vo localVo;

                if (local.TryGetValue(bundleName, out localVo) == false || remoteVo.md5 != localVo.md5)
                {
                    if (bundleName.IndexOf("platform") != 0)
                    {
                        size += remoteVo.size;
                        updateFiles.AddLast(remoteVo);
                    }
                }
            }

            FileMD5Vo bundleInfo = new FileMD5Vo();
            bundleInfo.path = mBundlesInfoPath;
            updateFiles.AddFirst(bundleInfo);

            var config = remote[mPlatformConfigPath];
            size += config.size;
            updateFiles.AddFirst(config);

            Debug.Log("mTotalFileSize:" + size / (1024 * 1024));

            fileSize = size;
        }

        void Update()
        {
            mResourceManager.Update();
        }
    }

    public class FileMD5Vo
    {
        public int size;
        public string md5;
        public string path;
    }
}
