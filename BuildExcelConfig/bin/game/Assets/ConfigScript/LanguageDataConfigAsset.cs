﻿using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using LitJson;

public class LanguageDataConfigAsset : ConfigAssetBase
{
    public enum LanguageType
    {
        Base = -1,
        zh = 0,        en = 1,        jp = 2,
    }
    [System.Serializable]
    public class LanguageDataConfig : ConfigAssetBase.ConfigAsset
    {
        public string key;
		/*内容*/
        public string zh;		/*内容*/
        public string en;		/*内容*/
        public string jp;
        public LanguageDataConfig()
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
    public LanguageType languageType;
    public List<LanguageDataConfig> configs;
    public Dictionary<string, LanguageDataConfig> configsDictionary;
    
    public override string GetConfigName()
    {
        return "LanguageDataConfig";
    }
    public string GetLanguageText(string key)
    {
        if (configsDictionary.ContainsKey(key))
            return configsDictionary[key].GetLanguageText(languageType);
        else
        {
            UnityEngine.Debug.Log(key + " is null");
            return key;
        }
    }
    public void ReadList()
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