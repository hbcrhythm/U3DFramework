using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CommandBuild {

    public static void BuildAssets()
    {
        BuildAssetBundle.BuildAssetsWithBuildTarget(BuildAssetBundleOptions.ForceRebuildAssetBundle, GetBuildTarget());
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
            case "WIN":
                return BuildTarget.StandaloneWindows64;
        }

        return BuildTarget.NoTarget;
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