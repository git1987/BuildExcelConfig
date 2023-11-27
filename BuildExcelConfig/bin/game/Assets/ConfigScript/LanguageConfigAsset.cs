using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

public class LanguageConfigAsset : ConfigAssetBase
{
    [System.Serializable]
    public class LanguageConfig : ConfigAssetBase.ConfigAsset
    {
        public string key;
		/*内容*/
        public string zh;		/*内容*/
        public string en;		/*内容*/
        public string jp;
        public LanguageConfig()
        {
        }
        public void InitJson(JsonData jd)
        {
            this.key = jd["key"].ToString();
			/*内容*/
            zh = jd["zh"].ToString();			/*内容*/
            en = jd["en"].ToString();			/*内容*/
            jp = jd["jp"].ToString();
        }
        public string GetLanguageText(LanguageType type)
        {
            switch (type)
            {
                case LanguageType.Base:
                case LanguageType.zh:                    return zh;                case LanguageType.en:                    return en;                case LanguageType.jp:                    return jp;
                default:
                    UnityEngine.Debug.LogError(type.ToString() + " is null===>" + key);
                    return key;
            }
        }
    }
    public List<LanguageConfig> configs;
    public Dictionary<string, LanguageConfig> configsDictionary;
    
    public override string GetConfigName()
    {
        return "LanguageConfig";
    }
    public string GetLanguageText(string key)
    {
        if (configsDictionary.TryGetValue(key, out LanguageConfig config))
            return config.GetLanguageText(ConfigAssetData.languageType);
        else
        {
            return key;
        }
    }
    public override void ReadList()
    {
        configsDictionary = new Dictionary<string, LanguageConfig>();
        for (int i = 0; i < configs.Count; i++)
        {
            if (!configsDictionary.ContainsKey(configs[i].key))
                configsDictionary.Add(configs[i].key, configs[i]);
        }
    }
    public override void ReadFromData(object obj)
    {
        JsonData jsonData = obj as JsonData;
        configs = new List<LanguageConfig>();
        if (jsonData["name"].ToString() == GetConfigName())
        {
            if (((IDictionary)jsonData).Contains("config"))
            {
                JsonData config = jsonData["config"];
                for (int i = 0; i < config.Count; i++)
                {
                    LanguageConfig configItem = new LanguageConfig();
                    configItem.InitJson(config[i]);
                    configs.Add(configItem);
                }
            }
        }
        else
        {
            throw new System.Exception("配置名称不对==>" + jsonData["name"].ToString());
        }
    }
}