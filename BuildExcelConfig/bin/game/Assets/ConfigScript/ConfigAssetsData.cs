using UnityEngine;
/// <summary>
/// 获取配置的单例类
/// </summary>
public class ConfigAssetsData : MonoBehaviour
{
    static private ConfigAssetsData configAssetsDate;
    static public ConfigAssetsData instance
    { get { return configAssetsDate; } }
    static public Enum_LanguageType languageType = 0;

    AssetBundle ab = null;
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
    private void OnDestroy()
    {
        ab?.Unload(true);
    }
    public void Init()
    {

#if UNITY_ANDROID
        string streamingFilePath = Application.streamingAssetsPath +"/Config/Android/config_android";
#elif UNITY_IOS
        string streamingFilePath = Application.streamingAssetsPath +"/Config/IOS/config_ios";
#elif UNITY_STANDALONE_WIN
        string streamingFilePath = Application.streamingAssetsPath +"/Config/Windows/config_windows";
#elif UNITY_WEBGL
        string streamingFilePath = Application.streamingAssetsPath +"/Config/WebGL/config_webgl";
#else
        string streamingFilePath = Application.streamingAssetsPath +"/Config/Other/config_other";
#endif
#if !UNITY_EDITOR
        ab = AssetBundle.LoadFromFile(streamingFilePath);
#endif
        _battleInfoConfigAsset = GetConfigAsset<BattleInfoConfigAsset, BattleInfoConfigAsset.BattleInfoConfig>();
        _battleGroupConfigAsset = GetConfigAsset<BattleGroupConfigAsset, BattleGroupConfigAsset.BattleGroupConfig>();
        _languageConfigAsset = GetConfigAsset<LanguageConfigAsset, LanguageConfigAsset.LanguageConfig>();
        _languageDataConfigAsset = GetConfigAsset<LanguageDataConfigAsset, LanguageDataConfigAsset.LanguageDataConfig>();

        initFinish = true;
#if !UNITY_EDITOR
        ab.Unload(false);
#endif
    }
    T GetConfigAsset<T, V>() where T : ConfigAssetBase
    {
#if UNITY_EDITOR
        T asset = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(string.Format("Assets/Res/ConfigAsset/{0}.asset", typeof(V).Name));
#else
        T asset = ab.LoadAsset<T>(typeof(V).Name);
#endif
        if (asset != null) asset.ReadList();
        return asset;
    }
    public string GetLanguageText(string languageKey)
    {
        if (languageKey.IndexOf("language_") > -1)
        {
            if (languageDataConfigAsset == null) Debug.LogError("LanguageData config is not init!");
            else return languageDataConfigAsset.GetLanguageText(languageKey);
        }
        else
        {
            if (languageConfigAsset == null) Debug.LogError("Language config is not init!");
            else return languageConfigAsset.GetLanguageText(languageKey);
        }
        Debug.LogError(languageKey + "is not in config!");
        return languageKey;
    }
    private BattleInfoConfigAsset _battleInfoConfigAsset;
    public BattleInfoConfigAsset battleInfoConfigAsset
    {
        get
        {
            if (_battleInfoConfigAsset == null)
                Debug.LogError("没有初始化BattleInfo AssetBundle");
            return _battleInfoConfigAsset;
        }
    }
    private BattleGroupConfigAsset _battleGroupConfigAsset;
    public BattleGroupConfigAsset battleGroupConfigAsset
    {
        get
        {
            if (_battleGroupConfigAsset == null)
                Debug.LogError("没有初始化BattleGroup AssetBundle");
            return _battleGroupConfigAsset;
        }
    }
    private LanguageConfigAsset _languageConfigAsset;
    public LanguageConfigAsset languageConfigAsset
    {
        get
        {
            if (_languageConfigAsset == null)
                Debug.LogError("没有初始化Language AssetBundle");
            return _languageConfigAsset;
        }
    }
    private LanguageDataConfigAsset _languageDataConfigAsset;
    public LanguageDataConfigAsset languageDataConfigAsset
    {
        get
        {
            if (_languageDataConfigAsset == null)
                Debug.LogError("没有初始化LanguageData AssetBundle");
            return _languageDataConfigAsset;
        }
    }

}