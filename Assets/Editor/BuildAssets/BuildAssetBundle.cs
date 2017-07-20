using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System;

class Depend
{
    public string path;
    public string owner;
}

public class BuildAssetBundle : Editor {

    [MenuItem("Tools/RebuildAssets")]
    static void ToolsRebuildAssets()
    {
        BuildAssetsWithBuildTarget(BuildAssetBundleOptions.ForceRebuildAssetBundle, EditorUserBuildSettings.activeBuildTarget);
    }

    public static void BuildAssetsWithBuildTarget(BuildAssetBundleOptions options, BuildTarget target)
    {

        BuildLua();
        BuildAssets(options, target);
        Debug.Log("BuildAssets finished");
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

    static void GetLuaBytes(string sourceDir, Dictionary<string, byte[]> luaScripts, string searchPattern = "*.lua", SearchOption option = SearchOption.AllDirectories)
    {
        string[] files = Directory.GetFiles(sourceDir, searchPattern, option);

        foreach (string fileName in files)
        {
            string path = fileName.Replace(@"\", @"/").Replace(sourceDir + @"/", "");
            string name = path.Replace(".lua", "");

            if (luaScripts.ContainsKey(name))
            {
                Debug.LogError("GetLuaBytes Error: " + fileName);
                return;
            }

            luaScripts[name] = System.IO.File.ReadAllBytes(fileName);
        }
    }

    static void SaveAllBytes(Dictionary<string, byte[]> dic, string fileName)
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

    static void RemoveAssetBundleNames() {
        string[] allAssetBundleNames = AssetDatabase.GetAllAssetBundleNames();
        for (int i = 0; i < allAssetBundleNames.Length; i++) {
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

        for (int i = 0; i < allFiles.Length, i++) {
            AssetImporter assetImporter = AssetImporter.GetAtPath(allFiles[i]);
            if (assetImporter) {
                string rawResource = allFiles[i].Replace(@"\", "/");
                string extension = System.IO.Path.GetExtension(rawResource);
                rawResource.Add(rawResource);

                // build asset not include .unity file
                if (IsSceneDepeed(rawResource)) continue;

                assetImporter.assetBundleName = rawResource.Replace(extension, "").Replace(rawResource + "/", "");
         
            }
        }

        for (int i = 0; i < rawResources.Count; i++)
        {
            GetDependencies(rawResources[i], references, rawResources);
        }

        foreach (Depend depend in references.Values)
        {
            AssetImporter assetImporter = AssetImporter.GetAtPath(depend.path);
            if (assetImporter)
            {
                if (depend.path.EndsWith(".cs") continue;
                if (IsSceneDepeed(depend.path)) continue;
                if (rawResources.Contains(depend.path)) continue;
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
            allBundles.Add("Assets/StreamingAssets/StreamingAssets");
            allBundles.Add("Assets/StreamingAssets/StreamingAssets.meta");
            allBundles.Add("Assets/StreamingAssets/StreamingAssets.manifest.meta");
            allBundles.Add("Assets/StreamingAssets/StreamingAssets.manifest");

            string path = "Assets/StreamingAssets";

            string[] allfiles = System.IO.Directory.GetFiles(path, "*", System.IO.SearchOption.AllDirectories);

            for (int i = 0; i < allfiles.Length; i++)
            {
                string file = allfiles[i].Replace(@"\","/");
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

    }

    static bool IsSceneDepeed(string file)
    {
        string directory = System.IO.Path.GetDirectoryName(file);
        return System.IO.File.Exists(directory + ".unity");
    }

    static void GetDependencies(string file, Dictionary<string, Depend> references, List<string> rawResource)
    {
        string[] dependencies = AssetDatabase.GetDependencies(file);
        for (int i = 0; i < dependencies.Length; i++)
        {
            string dependPath = dependencies[i];
            if (references.ContainsKey(dependPath))
            {
                references[dependPath].owner = "common" + System.IO.Path.GetFileName(references[dependPath].path);
            }
            else
            {
                Depend depend = new Depend();
                depend.owner = null;
                depend.path = dependPath;
                references.Add(dependPath, depend);
            }

            if (rawResource.Contains(dependPath) && file != dependPath)
            {
                Debug.LogError(file + "错误引用了 " + dependPath);
            }
        }

    }
}