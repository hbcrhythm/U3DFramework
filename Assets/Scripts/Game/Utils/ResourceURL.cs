using System.IO;
using UnityEngine;

namespace Assets.Scripts.Game.Utils
{
    public static class ResourceURL
    {
        public static string RAW_STREAMING_ASSETS { get; private set; }

        public static string PERSISTENT_STREAMING_ASSETS { get; private set; }

        public const string FILE_FLAG = "file:///";

        const string cStreamingAssetsPath = @"{0}/StreamingAssets/{1}";

        static ResourceURL()
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                RAW_STREAMING_ASSETS = Application.streamingAssetsPath;
                PERSISTENT_STREAMING_ASSETS = FILE_FLAG + Application.persistentDataPath;
            }
            else
            {
                RAW_STREAMING_ASSETS = FILE_FLAG + Application.streamingAssetsPath;
                PERSISTENT_STREAMING_ASSETS = FILE_FLAG + Application.persistentDataPath;
            }
        }

        public static string GetRawFilePath(string path)
        {
            return Application.streamingAssetsPath + "/" + path;
        }

        public static string GetRawPath(string path)
        {
            return RAW_STREAMING_ASSETS + "/" + path;
        }

        public static string GetPersistentPath(string path)
        {
            return string.Format(cStreamingAssetsPath, PERSISTENT_STREAMING_ASSETS, path);
        }

        public static string GetPersistentFilePath(string path)
        {
            return string.Format(cStreamingAssetsPath, Application.persistentDataPath, path);
        }

        public static bool CheckPersistentFileExists(string path)
        {
            return File.Exists(GetPersistentFilePath(path));
        }

        public static bool CheckRelease()
        {
            return false;
        }

        public static string GetAssetPath(string path)
        {
            if (CheckRelease())
            {
                return GetPersistentPath(path);
            }

            return GetRawPath(path);
        }

        public static string GetAssetFilePath(string path)
        {
            if (CheckRelease())
            {
                return GetPersistentFilePath(path);
            }

            return GetRawFilePath(path);
        }

        public static string GetWWWAssetPath(string path)
        {
            if (CheckRelease())
            {
                return GetPersistentPath(path);
            }

            return GetRawPath(path);
        }

    }
}
