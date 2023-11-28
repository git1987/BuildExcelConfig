using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using LitJson;

/// <summary>
/// 编辑器内使用build unity asset的editor 类
/// </summary>
public class CreateConfig
{
#if UNITY_ANDROID
        static string abName = "config_android";
#elif UNITY_IOS
        static string abName = "config_ios";
#elif UNITY_STANDALONE_WIN
        static string abName = "config_windows";
#elif UNITY_WEBGL
        static string abName = "config_webgl";
#else
        static string abName = "config_other";
#endif
    static string assetPath = Application.dataPath + "/Res/ConfigAsset/";
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
        CreateConfigAsset<BattleInfoConfigAsset>();
        CreateConfigAsset<BattleGroupConfigAsset>();
        CreateConfigAsset<LanguageConfigAsset>();
        CreateConfigAsset<LanguageDataConfigAsset>();

        Debug.Log("创建asset配置完毕！");
        AssetDatabase.Refresh();
    }

    static void CreateConfigAsset<T>() where T : ScriptableObject
    {
        ConfigAssetBase asset = ScriptableObject.CreateInstance<T>() as ConfigAssetBase;
        string configName = asset.GetConfigName();
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
        if (IsFolderExists(Application.streamingAssetsPath + "/Config"))
        {
            Directory.Delete(Application.streamingAssetsPath + "/Config", true);
        }
        //打包资源的路径 

#if UNITY_ANDROID
        string targetPath = Application.streamingAssetsPath + "/Config/Android/";
#elif UNITY_IOS
        string targetPath = Application.streamingAssetsPath + "/Config/IOS/";
#elif UNITY_STANDALONE_WIN
        string targetPath = Application.streamingAssetsPath + "/Config/Windows/";
#elif UNITY_WEBGL
        string targetPath = Application.streamingAssetsPath + "/Config/WebGL/";
#else
        string targetPath = Application.streamingAssetsPath + "/Config/Other/";
#endif
        //创建目录
        if (!IsFolderExists(targetPath)) Directory.CreateDirectory(targetPath);
        //打包资源 
        AssetBundleManifest manifest;
        DirectoryInfo configAssetFolder = new DirectoryInfo(Application.dataPath + "/Res/ConfigAsset");
        FileInfo[] files = configAssetFolder.GetFiles("*.asset");
        AssetBundleBuild[] abb = new AssetBundleBuild[1];
        abb[0] = new AssetBundleBuild()
        {
            assetBundleName = abName,
            assetNames = new string[files.Length],
        };
        int index = 0;
        foreach (FileInfo file in files)
        {
            string unityAssetPath = "Assets/Res/ConfigAsset/" + file.Name;
            Object obj = AssetDatabase.LoadAssetAtPath<Object>(unityAssetPath);
            Debug.Log(obj);
            abb[0].assetNames[index] = unityAssetPath;
            index++;
        }
#if UNITY_ANDROID
        //移动端压缩ab
        manifest = BuildPipeline.BuildAssetBundles(targetPath, abb, BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.Android);
#elif UNITY_IOS
        //移动端压缩ab
        manifest = BuildPipeline.BuildAssetBundles(targetPath, abb, BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.iOS);
#elif UNITY_STANDALONE_WIN
        //PC端不压缩ab
        manifest = BuildPipeline.BuildAssetBundles(targetPath, abb, BuildAssetBundleOptions.UncompressedAssetBundle, BuildTarget.StandaloneWindows);
#elif UNITY_WEBGL
        //WebGL端压缩ab
        manifest = BuildPipeline.BuildAssetBundles(targetPath, abb, BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.WebGL);
#endif
        Debug.Log("配置打包配置ab完成");
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
        asset.assetBundleName = abName;
        EditorUtility.FocusProjectWindow();
    }
}