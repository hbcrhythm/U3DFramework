using UnityEditor;
using System.IO;
using UnityEngine;
using Assets.Scripts.Game.Platform;
using Assets.Scripts.Game.Utils;

public class CommandBuild
{
    public static void BuildAssets()
    {
        BuildAssetBundle.BuildAssetsWithBuildTarget(BuildAssetBundleOptions.ForceRebuildAssetBundle, GetBuildTarget());
    }

    public static void MakeBundlesInfo()
    {
        BuildAssetBundle.MakeMD5(GetBuildTarget());
    }

    public static void BuildGame()
    {
        string[] levels = GetPublishType() == "Development" ? GetDevelopmentBuildLevels() : GetProductBuildLevels();
        string[] platformList = GetExternalArg("PlatformList").Split('#');
        var buildTarget = GetBuildTarget();

        for (int i = 0, count = platformList.Length; i < count; i++)
        {
            var platformName = platformList[i];
            BuildAssetBundle.MakePlatformConfig(platformName);
            SetBundleVersion(platformName, buildTarget);

            switch (buildTarget)
            {
                case BuildTarget.Android:
                    BuildGameAndroid(levels, platformName);
                    break;
                case BuildTarget.iOS:
                    BuildGameIOS(levels);
                    return;
            }
        }
    }

    private static void SetBundleVersion(string platformName, BuildTarget buildTarget)
    {
        string config = File.ReadAllText(Application.dataPath + "/RawResources/platform/" + platformName + "/config.txt", System.Text.Encoding.UTF8);
        var version = new IniFile(config).GetValue("game", "version");

        PlayerSettings.bundleVersion = version;
        PlayerSettings.iOS.buildNumber = version;

        if (buildTarget == BuildTarget.Android)
        {
            PlayerSettings.Android.bundleVersionCode = new PlatformVersion(version).version;
        }
    }

    static void BuildGameAndroid(string[] levels, string platformName)
    {
        BuildPipeline.BuildPlayer(levels, string.Format("xfz_{0}.apk", platformName), BuildTarget.Android, BuildOptions.None);
    }

    static void BuildGameIOS(string[] levels)
    {
        BuildPipeline.BuildPlayer(levels, "ios", BuildTarget.iOS, BuildOptions.None);
    }

    private static string[] GetProductBuildLevels()
    {
        string[] levels = { "Assets/Scenes/Launch.unity", "Assets/Scenes/Scene.unity", "Assets/Scenes/Combat.unity" };
        return levels;
    }

    private static string[] GetDevelopmentBuildLevels()
    {
        string[] levels = { "Assets/Scenes/Login.unity", "Assets/Scenes/Scene.unity", "Assets/Scenes/Combat.unity" };
        return levels;
    }

    public static BuildTarget GetBuildTarget()
    {
        string runtimePlatform = GetExternalArg("RuntimePlatform");

        switch (runtimePlatform)
        {
            case "ANDROID":
                return BuildTarget.Android;
            case "IOS":
                return BuildTarget.iOS;
        }

        return BuildTarget.NoTarget;
    }

    public static string GetPublishType()
    {
        return GetExternalArg("PublishType");
    }

    public static string GetExternalArg(string name)
    {
        foreach (string arg in System.Environment.GetCommandLineArgs())
        {
            if (arg.StartsWith(name))
            {
                return arg.Split('-')[1];
            }
        }

        return "";
    }
}