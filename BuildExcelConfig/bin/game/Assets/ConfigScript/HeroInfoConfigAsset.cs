using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
/// <summary>
/// Json数据类型的类
/// </summary>
public class HeroInfoConfigAsset : ConfigAssetBase
{
    [System.Serializable]
    public class HeroInfoConfig : ConfigAssetBase.ConfigAsset
    {
		/*英雄名称*/
        [SerializeField]
        private string _heroName;        public string heroName
        {get{return ConfigAssetsData.instance.GetLanguageText(_heroName); }set{ _heroName = value;}}
		/*英雄介绍*/
        [SerializeField]
        private string _heroDesc;        public string heroDesc
        {get{return ConfigAssetsData.instance.GetLanguageText(_heroDesc); }set{ _heroDesc = value;}}
		/*英雄资源id*/
        public string heroRes;		/*英雄类型*/
        public string heroType;		/*普攻技能id*/
        public int defaultSkill;		/*大招技能id*/
        public int ultSkill;		/*被动技能id*/
        public int passiveSkill;
        public void InitJson(JsonData jd)
        {
			/*英雄名称*/
            _heroName = jd["heroName"].ToString();			/*英雄介绍*/
            _heroDesc = jd["heroDesc"].ToString();			/*英雄资源id*/
            heroRes = jd["heroRes"].ToString();			/*英雄类型*/
            heroType = jd["heroType"].ToString();			/*普攻技能id*/
            defaultSkill = int.Parse(jd["defaultSkill"].ToString());			/*大招技能id*/
            ultSkill = int.Parse(jd["ultSkill"].ToString());			/*被动技能id*/
            passiveSkill = int.Parse(jd["passiveSkill"].ToString());
        }
    }
    public List<HeroInfoConfig> configs;
    public Dictionary<int, HeroInfoConfig> configsDictionary;
    public override string GetConfigName()
    {
        return "HeroInfoConfig";
    }
    public override void ReadList()
    {
        configsDictionary = new Dictionary<int, HeroInfoConfig>();
        for (int i = 0; i < configs.Count; i++)
        {
            if (!configsDictionary.ContainsKey(configs[i].ID))
                configsDictionary.Add(configs[i].ID, configs[i]);
        }
    }

    public override void ReadFromData(object obj)
    {
        JsonData jsonData = obj as JsonData;
        configs = new List<HeroInfoConfig>();
        if (jsonData["name"].ToString() == GetConfigName())
        {
            if (((IDictionary)jsonData).Contains("config"))
            {
                JsonData config = jsonData["config"];
                for (int i = 0; i < config.Count; i++)
                {
                    HeroInfoConfig configItem = new HeroInfoConfig();
                    configItem.ID = int.Parse(config[i]["ID"].ToString());
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
