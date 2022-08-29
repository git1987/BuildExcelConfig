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
#if !UNITY_EDITOR
        ab = AssetBundle.LoadFromFile(streamingFilePath + "/config");
#endif
        _languageConfigAsset = GetConfigAsset<LanguageConfigAsset, LanguageConfigAsset.LanguageConfig>();        _languageDataConfigAsset = GetConfigAsset<LanguageDataConfigAsset, LanguageDataConfigAsset.LanguageDataConfig>();        _skillInfoConfigAsset = GetConfigAsset<SkillInfoConfigAsset, SkillInfoConfigAsset.SkillInfoConfig>();        _skillEffectConfigAsset = GetConfigAsset<SkillEffectConfigAsset, SkillEffectConfigAsset.SkillEffectConfig>();        _projectileConfigAsset = GetConfigAsset<ProjectileConfigAsset, ProjectileConfigAsset.ProjectileConfig>();
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
        if (languageKey.IndexOf("language_") == -1) return languageKey;
        if (languageConfigAsset != null)
            return languageConfigAsset.GetLanguageText(languageKey);
        if (languageDataConfigAsset != null)
            return languageDataConfigAsset.GetLanguageText(languageKey);
        return languageKey;
    }
    private LanguageConfigAsset _languageConfigAsset;    public LanguageConfigAsset languageConfigAsset    {        get        {            if (_languageConfigAsset == null)                Debug.LogError("没有初始化Language AssetBundle");            return _languageConfigAsset;        }    }    private LanguageDataConfigAsset _languageDataConfigAsset;    public LanguageDataConfigAsset languageDataConfigAsset    {        get        {            if (_languageDataConfigAsset == null)                Debug.LogError("没有初始化LanguageData AssetBundle");            return _languageDataConfigAsset;        }    }    private SkillInfoConfigAsset _skillInfoConfigAsset;    public SkillInfoConfigAsset skillInfoConfigAsset    {        get        {            if (_skillInfoConfigAsset == null)                Debug.LogError("没有初始化SkillInfo AssetBundle");            return _skillInfoConfigAsset;        }    }    private SkillEffectConfigAsset _skillEffectConfigAsset;    public SkillEffectConfigAsset skillEffectConfigAsset    {        get        {            if (_skillEffectConfigAsset == null)                Debug.LogError("没有初始化SkillEffect AssetBundle");            return _skillEffectConfigAsset;        }    }    private ProjectileConfigAsset _projectileConfigAsset;    public ProjectileConfigAsset projectileConfigAsset    {        get        {            if (_projectileConfigAsset == null)                Debug.LogError("没有初始化Projectile AssetBundle");            return _projectileConfigAsset;        }    }
}