using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;
using System.Text;
class Depend
{
    public string path;
    public string owner;
}
public class BuildAssetBundle : Editor
{
    static void RemoveAssetBundleNames()
    {
        string[] allAssetBundleNames = AssetDatabase.GetAllAssetBundleNames();
        for (int i = 0; i < allAssetBundleNames.Length; i++)
        {
            AssetDatabase.RemoveAssetBundleName(allAssetBundleNames[i], true);
        }
    }
    static void BuildAssets(BuildAssetBundleOptions options, BuildTarget target)
    {
        RemoveAssetBundleNames();
        string path = "Assets/StreamingAssets";
        string rawResourcesPath = "Assets/RawResources";
        string[] allFiles = System.IO.Directory.GetFiles(rawResourcesPath, "*", System.IO.SearchOption.AllDirectories);

        List<string> rawResources = new List<string>();

        Dictionary<string, Depend> references = new Dictionary<string, Depend>();

        for (int i = 0; i < allFiles.Length; i++)
        {
            AssetImporter assetImporter = AssetImporter.GetAtPath(allFiles[i]);
            if (assetImporter)
            {
                string rawResource = allFiles[i].Replace(@"\", "/");
                string extension = System.IO.Path.GetExtension(rawResource);
                rawResources.Add(rawResource);
                if (IsSceneDepend(rawResource)) continue;
                assetImporter.assetBundleName = rawResource.Replace(extension, "").Replace(rawResourcesPath + "/", "");
                //assetImporter.assetBundleVariant = extension.Replace(".", "");
            }
        }

        for (int i = 0; i < rawResources.Count; i++)
        {
            GetDependencies(rawResources[i], references, rawResources);
        }

        AddCustomDependencies(references, "Assets/AssetsLibrary/ui/common2", "common2");

        foreach (Depend depend in references.Values)
        {
            AssetImporter assetImporter = AssetImporter.GetAtPath(depend.path);
            if (assetImporter)
            {

                if (depend.path.EndsWith(".cs")) continue;
                if (IsSceneDepend(depend.path)) continue;
                if (rawResources.Contains(depend.path))
                {
                    continue;
                }
                if (assetImporter is TextureImporter)
                {
                    var textureImporter = assetImporter as TextureImporter;
                    if (textureImporter.textureType == TextureImporterType.Sprite)
                    {
                        if (string.IsNullOrEmpty(textureImporter.spritePackingTag) == false)
                        {
                            assetImporter.assetBundleName = "common/" + textureImporter.spritePackingTag;
                        }
                        else
                        {
                            assetImporter.assetBundleName = "common/" + System.IO.Path.GetFileNameWithoutExtension(depend.path);
                        }
                        continue;
                    }
                }

                if (depend.owner == null) continue;

                string extension = System.IO.Path.GetExtension(depend.owner);
                assetImporter.assetBundleName = depend.owner.Replace(extension, "").Replace(rawResourcesPath + "/", "");
                //assetImporter.assetBundleVariant = extension.Replace(".","");
            }
        }

        if (System.IO.Directory.Exists(path) == false)
        {
            System.IO.Directory.CreateDirectory(path);
        }

        AssetBundleManifest manifest = BuildPipeline.BuildAssetBundles(path, options | BuildAssetBundleOptions.DisableWriteTypeTree, target);
        Clear(manifest);
        RemoveAssetBundleNames();
        AssetDatabase.Refresh();
    }

    static void Clear(AssetBundleManifest manifest)
    {
        string[] temp = manifest.GetAllAssetBundles();
        List<string> allBundles = new List<string>();
        for (int i = 0; i < temp.Length; i++)
        {
            string file = "Assets/StreamingAssets/" + temp[i];
            allBundles.Add(file);
            allBundles.Add(file + ".meta");
            allBundles.Add(file + ".manifest");
            allBundles.Add(file + ".manifest.meta");
        }
        allBundles.Add("Assets/StreamingAssets/StreamingAssets");
        allBundles.Add("Assets/StreamingAssets/StreamingAssets.meta");
        allBundles.Add("Assets/StreamingAssets/StreamingAssets.manifest.meta");
        allBundles.Add("Assets/StreamingAssets/StreamingAssets.manifest");

        string path = "Assets/StreamingAssets";
        string[] allFiles = System.IO.Directory.GetFiles(path, "*", System.IO.SearchOption.AllDirectories);
        for (int i = 0; i < allFiles.Length; i++)
        {
            string file = allFiles[i].Replace(@"\", "/");
            if (!allBundles.Contains(file) && file.IndexOf("external") == -1)
            {
                if (!System.IO.Directory.Exists(file.Replace(".meta", "")))
                {
                    Debug.LogWarning("删除多余文件->" + file);
                    FileUtil.DeleteFileOrDirectory(file);
                }
            }
        }
    }

    static bool IsSceneDepend(string file)
    {
        string directory = System.IO.Path.GetDirectoryName(file);
        return System.IO.File.Exists(directory + ".unity");
    }

    static void AddCustomDependencies(Dictionary<string, Depend> references, string path, string spritePackingTag)
    {
        string[] allFiles = System.IO.Directory.GetFiles(path, "*", System.IO.SearchOption.AllDirectories);

        foreach (var dependPath in allFiles)
        {
            AssetImporter assetImporter = AssetImporter.GetAtPath(dependPath);
            if (assetImporter && assetImporter is TextureImporter)
            {
                var textureImporter = assetImporter as TextureImporter;
                if (textureImporter.textureType == TextureImporterType.Sprite)
                {
                    if (textureImporter.spritePackingTag == spritePackingTag)
                    {
                        if (references.ContainsKey(dependPath) == false)
                        {
                            Depend depend = new Depend();
                            depend.owner = "common/" + System.IO.Path.GetFileName(dependPath);
                            depend.path = dependPath;
                            references.Add(dependPath, depend);
                        }
                    }
                }
            }
        }
    }

    static void GetDependencies(string file, Dictionary<string, Depend> references, List<string> rawResources)
    {
        string[] dependencies = AssetDatabase.GetDependencies(file);
        for (int i = 0; i < dependencies.Length; i++)
        {
            string dependPath = dependencies[i];
            if (references.ContainsKey(dependPath))
            {
                //公用资源
                references[dependPath].owner = "common/" + System.IO.Path.GetFileName(references[dependPath].path);
            }
            else
            {
                Depend depend = new Depend();
                depend.owner = null;
                depend.path = dependPath;
                references.Add(dependPath, depend);
            }

            if (rawResources.Contains(dependPath) && file != dependPath)
            {
                Debug.LogError(file + "  错误引用了 " + dependPath);
            }
        }
    }

    static void GetLuaBytes(string sourceDir, Dictionary<string, byte[]> luaScripts, string searchPattern = "*.lua", SearchOption option = SearchOption.AllDirectories)
    {
        string[] files = Directory.GetFiles(sourceDir, searchPattern, option);

        foreach (string fileName in files)
        {
            string path = fileName.Replace(@"\", @"/").Replace(sourceDir + @"/", "");
            string name = path.Replace(".lua", "");

            if (luaScripts.ContainsKey(name))
            {
                Debug.LogError("GetLuaBytes error:" + fileName);
                return;
            }

            luaScripts[name] = System.IO.File.ReadAllBytes(fileName);
        }
    }

    public static void BuildLua()
    {
        Dictionary<string, byte[]> luaScripts = new Dictionary<string, byte[]>();

        GetLuaBytes(LuaConst.luaDir, luaScripts);
        GetLuaBytes(LuaConst.toluaDir, luaScripts);

        luaScripts["Utils/GameBuildVersion"] = Encoding.UTF8.GetBytes(string.Format("return {0}", string.Format("{0:yyyyMMddHHmm}", System.DateTime.Now)));

        string luaPath = "Assets/RawResources/lua/lua.bytes";
        SaveAllBytes(luaScripts, luaPath);

        AssetDatabase.Refresh();
    }

    public static void SaveAllBytes(Dictionary<string, byte[]> dic, string fileName)
    {
        using (MemoryStream ms = new MemoryStream())
        {
            foreach (KeyValuePair<string, byte[]> kvp in dic)
            {
                string unikey = kvp.Key;
                byte[] unikeyBytes = System.Text.Encoding.UTF8.GetBytes(unikey);
                byte[] unikeyBytesLen = BitConverter.GetBytes(unikeyBytes.Length);

                ms.Write(unikeyBytesLen, 0, unikeyBytesLen.Length);
                ms.Write(unikeyBytes, 0, unikeyBytes.Length);

                using (MemoryStream item = new MemoryStream(kvp.Value))
                {
                    item.Position = 0;
                    byte[] itemMSBytesLen = BitConverter.GetBytes((int)item.Length);
                    ms.Write(itemMSBytesLen, 0, itemMSBytesLen.Length);

                    item.WriteTo(ms);
                }
            }

            byte[] fileBytes = ms.ToArray();
            using (FileStream fileStream = new FileStream(fileName, FileMode.Create))
            {
                fileStream.Write(fileBytes, 0, fileBytes.Length);
            }
        }
    }

    [MenuItem("Tools/RebuildAssets")]
    static void ToolsRebuildAssets()
    {
        BuildAssetsWithBuildTarget(BuildAssetBundleOptions.ForceRebuildAssetBundle, EditorUserBuildSettings.activeBuildTarget);
    }

    [MenuItem("Tools/BuildAssets")]
    static void ToolsBuildAssets()
    {
        BuildAssetsWithBuildTarget(BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
    }

    public static void BuildAssetsWithBuildTarget(BuildAssetBundleOptions options, BuildTarget target)
    {
        BuildLua();
        BuildAssets(options, target);

        Debug.Log("BuildAssets Finish");
    }

    [MenuItem("Tools/RebuildAll")]
    static void RebuildAll()
    {
        RebuildAll(BuildAssetBundleOptions.ForceRebuildAssetBundle, EditorUserBuildSettings.activeBuildTarget);
    }

    static public void RebuildAll(BuildAssetBundleOptions options, BuildTarget target)
    {
        BuildAssetsWithBuildTarget(options, target);
        MakeMD5(target);
    }

    [MenuItem("Tools/MakeMD5")]
    static void ToolsMakeMD5()
    {
        MakeMD5(EditorUserBuildSettings.activeBuildTarget);
    }

    public static void MakeMD5(BuildTarget target)
    {
        Debug.Log("ToolsMakeMD5 Start");
        AssetDatabase.Refresh();

        string assetBundleName = "game_bundles_info";
        string streamingAssetsBundlesInfo = "Assets/StreamingAssets/bundles_info/" + assetBundleName;
        if (File.Exists(streamingAssetsBundlesInfo))
        {
            File.Delete(streamingAssetsBundlesInfo);
        }

        List<string> availableBundles = new List<string>();

        string path = "Assets/StreamingAssets";
        string[] allFiles = System.IO.Directory.GetFiles(path, "*", System.IO.SearchOption.AllDirectories);
        for (int i = 0; i < allFiles.Length; i++)
        {
            string file = allFiles[i].Replace(@"\", "/");
            if (Path.GetFileName(file) == Path.GetFileNameWithoutExtension(file) && file.IndexOf(".") == -1)
            {
                availableBundles.Add(file);

                if (file.IndexOf(" ") != -1)
                {
                    Debug.LogError("名字有空格：" + file);
                }
            }
        }

        string bundles_info = "Assets/AssetsLibrary/bundles_info/game_bundles_info.txt";
        using (StreamWriter writer = new StreamWriter(bundles_info, false, Encoding.UTF8))
        {
            foreach (var bundleName in availableBundles)
            {
                if (File.Exists(bundleName))
                {
                    var fileBytes = File.ReadAllBytes(bundleName);
                    var md5 = GetMD5Str(fileBytes);
                    writer.WriteLine(string.Format("{0},{1},{2}", bundleName.Replace(path + "/", ""), md5, fileBytes.Length));
                }
            }
        }

        AssetDatabase.Refresh();
        AssetImporter assetImporter = AssetImporter.GetAtPath(bundles_info);
        assetImporter.assetBundleName = assetBundleName;

        string manifestPath = "Assets/AssetsLibrary/bundles_info/manifest";
        AssetBundleManifest manifest = BuildPipeline.BuildAssetBundles(manifestPath, BuildAssetBundleOptions.ForceRebuildAssetBundle | BuildAssetBundleOptions.DisableWriteTypeTree, target);
        RemoveAssetBundleNames();
        AssetDatabase.Refresh();
        File.Move(manifestPath + "/" + assetBundleName, streamingAssetsBundlesInfo);
        AssetDatabase.Refresh();
        Debug.Log("ToolsMakeMD5 Finish");
    }

    //获取加密服务  
    static System.Security.Cryptography.MD5CryptoServiceProvider md5CSP = new System.Security.Cryptography.MD5CryptoServiceProvider();
    static private string GetMD5Str(byte[] fileBytes)
    {
        byte[] resultEncrypt = md5CSP.ComputeHash(fileBytes);

        string result = "";
        for (int i = 0; i < resultEncrypt.Length; ++i)
        {
            result += resultEncrypt[i].ToString("X");
        }

        return result;
    }

    static public void MakePlatformConfig(string platform)
    {
        string platformPath = "Assets/Resources/Platform/config.txt";
        using (StreamWriter writer = new StreamWriter(platformPath, false, Encoding.UTF8))
        {
            writer.Write(platform);
        }

        AssetDatabase.Refresh();
    }
}
