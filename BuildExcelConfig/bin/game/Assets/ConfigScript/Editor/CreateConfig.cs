using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using LitJson;

/// <summary>
/// 编辑器内使用build unity asset的editor 类
/// </summary>
public class CreateConfig
{
    [MenuItem("CreateAsset/CreateConfigAssets")]
    public static void CreateConfigAsset()
    {
        CreateConfigAssets();
        System.GC.Collect();
    }
    private static void CreateConfigAssets()
    {
        if (!Directory.Exists(Application.dataPath + "/Res/ConfigAsset/"))
            Directory.CreateDirectory(Application.dataPath + "/Res/ConfigAsset/");
        CreateConfigAsset<LanguageConfigAsset>();        CreateConfigAsset<LanguageDataConfigAsset>();        CreateConfigAsset<SkillInfoConfigAsset>();        CreateConfigAsset<SkillEffectConfigAsset>();        CreateConfigAsset<ProjectileConfigAsset>();
        AssetDatabase.Refresh();
    }

    static void CreateConfigAsset<T>() where T : ScriptableObject
    {
        ConfigAssetBase asset = ScriptableObject.CreateInstance<T>() as ConfigAssetBase;
        string configName = asset.GetConfigName();

        string assetPath = Application.dataPath + "/Res/ConfigAsset/";
        if (!Directory.Exists(assetPath))
            Directory.CreateDirectory(assetPath);
        string assetPathAndName = "Assets/Res/ConfigAsset/" + configName + ".asset";
        if (File.Exists(assetPathAndName))
            File.Delete(assetPathAndName);
        string filePath = Application.dataPath + "/ConfigAsset/" + configName + ".json";
        StreamReader sr = new StreamReader(filePath);
        JsonData jsonData = JsonMapper.ToObject(sr.ReadToEnd());
        sr.Close();

        if (!((IDictionary)jsonData).Contains("config"))
        {
            Debug.LogError("CreateConfigAsset " + configName + " [\"config\"] is null");
            return;
        }
        asset.ReadFromData(jsonData);
        CreateAssetBundle(asset, assetPathAndName);
    }
    [MenuItem("CreateAsset/AssetBundle")]
    static void BuildAssetBundle()
    {
        if (!IsFolderExists(Application.streamingAssetsPath)) Directory.CreateDirectory(Application.streamingAssetsPath);
        string folderAndroidPath = Application.streamingAssetsPath + "/Android";
        if (IsFolderExists(folderAndroidPath))
        {
            Directory.Delete(folderAndroidPath, true);
        }
        string folderIOSPath = Application.streamingAssetsPath + "/IOS";
        if (IsFolderExists(folderIOSPath))
        {
            Directory.Delete(folderIOSPath, true);
        }
        string folderWindowPath = Application.streamingAssetsPath + "/Windows";
        if (IsFolderExists(folderWindowPath))
        {
            Directory.Delete(folderWindowPath, true);
        }
        //打包资源的路径 
        string targetPath = Application.streamingAssetsPath
#if UNITY_ANDROID
       + "/Android/";
#elif UNITY_IOS
       + "/IOS/";
#elif UNITY_STANDALONE_WIN
       + "/Windows/";
#endif
        //创建目录
        if (!IsFolderExists(targetPath)) Directory.CreateDirectory(targetPath);
        //打包资源 
        AssetBundleManifest manifest;
#if UNITY_ANDROID
        manifest = BuildPipeline.BuildAssetBundles(targetPath, BuildAssetBundleOptions.CollectDependencies, BuildTarget.Android);
#elif UNITY_IOS
        manifest = BuildPipeline.BuildAssetBundles(targetPath, BuildAssetBundleOptions.CollectDependencies, BuildTarget.iOS);
#elif UNITY_STANDALONE_WIN
        manifest = BuildPipeline.BuildAssetBundles(targetPath, BuildAssetBundleOptions.CollectDependencies, BuildTarget.StandaloneWindows);
#endif
        string log = "AssetBundle";
        for (int i = 0; i < manifest.GetAllAssetBundles().Length; i++)
        {
            log += "\nname:" + manifest.GetAllAssetBundles()[i];
        }
        //刷新编辑器 
        AssetDatabase.Refresh();
    }
    //判断文件夹是否存在
    private static bool IsFolderExists(string folderPath)
    {
        if (folderPath.Equals(string.Empty))
        {
            return false;
        }
        return Directory.Exists(folderPath);
    }
    static void CreateAssetBundle(ScriptableObject so, string path)
    {
        AssetDatabase.CreateAsset(so, path);
        AssetDatabase.SaveAssets();
        AssetImporter asset = AssetImporter.GetAtPath(path);
        asset.assetBundleName = "config";
        EditorUtility.FocusProjectWindow();
    }
}