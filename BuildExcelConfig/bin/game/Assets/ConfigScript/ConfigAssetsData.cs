using UnityEngine;
/// <summary>
/// 获取配置的单例类
/// </summary>
public class ConfigAssetsData : MonoBehaviour
{
    static private ConfigAssetsData configAssetsDate;
    static public ConfigAssetsData instance
    { get { return configAssetsDate; } }
    static public LanguageConfigAsset.LanguageType languageType = LanguageConfigAsset.LanguageType.Base;

    AssetBundle ab;
    public bool initFinish { private set; get; }
    private void Awake()
    {
        if (configAssetsDate == null) configAssetsDate = this;
        else
        {
            Debug.LogError(this.GetType().Name + "已经存在");
            Destroy(this.gameObject);
            return;
        }
        Init();
    }
    public void Init()
    {
        string streamingFilePath =
#if UNITY_ANDROID
        Application.streamingAssetsPath +"/Android/";
#elif UNITY_STANDALONE_OSX
        Application.streamingAssetsPath +"/IOS/";
#elif UNITY_IOS
        Application.streamingAssetsPath +"/IOS/";
#elif UNITY_STANDALONE_WIN
        Application.streamingAssetsPath + "/Windows/";
#endif
        ab = AssetBundle.LoadFromFile(streamingFilePath + "/config");
        _languageConfigAsset = ab.LoadAsset<LanguageConfigAsset>(typeof(LanguageConfigAsset.LanguageConfig).Name);        if(_languageConfigAsset != null)            _languageConfigAsset.ReadList();        _languageDataConfigAsset = ab.LoadAsset<LanguageDataConfigAsset>(typeof(LanguageDataConfigAsset.LanguageDataConfig).Name);        if(_languageDataConfigAsset != null)            _languageDataConfigAsset.ReadList();
        initFinish = true;
        ab.Unload(false);
    }
    public string GetLanguageText(string languageKey)
    {
        if (languageConfigAsset != null)
            return languageConfigAsset.GetLanguageText(languageKey);
        if (languageDataConfigAsset != null)
            return languageDataConfigAsset.GetLanguageText(languageKey);
        return languageKey;
    }
    private LanguageConfigAsset _languageConfigAsset;    public LanguageConfigAsset languageConfigAsset    {        get        {            if (_languageConfigAsset == null)                Debug.LogError("没有初始化Language AssetBundle");            return _languageConfigAsset;        }    }    private LanguageDataConfigAsset _languageDataConfigAsset;    public LanguageDataConfigAsset languageDataConfigAsset    {        get        {            if (_languageDataConfigAsset == null)                Debug.LogError("没有初始化LanguageData AssetBundle");            return _languageDataConfigAsset;        }    }
}