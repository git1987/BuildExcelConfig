using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

public class LanguageDataConfigAsset : ConfigAssetBase
{
    [System.Serializable]
    public class LanguageDataConfig : ConfigAssetBase.ConfigAsset
    {
        public string key;

        public LanguageDataConfig()
        {
        }
        public void InitJson(JsonData jd)
        {
            this.key = jd["key"].ToString();

        }
        public string GetLanguageText(Enum_LanguageType type)
        {
            switch (type)
            {
                case Enum_LanguageType.Base:

                default:
                    UnityEngine.Debug.LogError(type.ToString() + " is null===>" + key);
                    return key;
            }
        }
    }
    public List<LanguageDataConfig> configs;
    public Dictionary<string, LanguageDataConfig> configsDictionary;
    
    public override string GetConfigName()
    {
        return "LanguageDataConfig";
    }
    public string GetLanguageText(string key)
    {
        if (configsDictionary.TryGetValue(key, out LanguageDataConfig config))
            return config.GetLanguageText(ConfigAssetData.languageType);
        else
        {
            return key;
        }
    }
    public override void ReadList()
    {
        configsDictionary = new Dictionary<string, LanguageDataConfig>();
        for (int i = 0; i < configs.Count; i++)
        {
            if (!configsDictionary.ContainsKey(configs[i].key))
                configsDictionary.Add(configs[i].key, configs[i]);
        }
    }
    public override void ReadFromData(object obj)
    {
        JsonData jsonData = obj as JsonData;
        configs = new List<LanguageDataConfig>();
        if (jsonData["name"].ToString() == GetConfigName())
        {
            if (((IDictionary)jsonData).Contains("config"))
            {
                JsonData config = jsonData["config"];
                for (int i = 0; i < config.Count; i++)
                {
                    LanguageDataConfig configItem = new LanguageDataConfig();
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