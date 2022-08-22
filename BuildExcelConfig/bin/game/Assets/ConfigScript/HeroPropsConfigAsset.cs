using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
/// <summary>
/// Json数据类型的类
/// </summary>
public class HeroPropsConfigAsset : ConfigAssetBase
{
    [System.Serializable]
    public class HeroPropsConfig : ConfigAssetBase.ConfigAsset
    {
		/*英雄id*/
        public int heroId;		/*攻击力*/
        public int attack;		/*攻击范围*/
        public int range;		/*暴击率*/
        public int criRate;		/*暴击效果*/
        public int criEffect;		/*攻击速度*/
        public int attackSpeed;		/*移动速度*/
        public int moveSpeed;		/*枚举测试*/
        public Enum_Test testName;
        public void InitJson(JsonData jd)
        {
			/*英雄id*/
            heroId = int.Parse(jd["heroId"].ToString());			/*攻击力*/
            attack = int.Parse(jd["attack"].ToString());			/*攻击范围*/
            range = int.Parse(jd["range"].ToString());			/*暴击率*/
            criRate = int.Parse(jd["criRate"].ToString());			/*暴击效果*/
            criEffect = int.Parse(jd["criEffect"].ToString());			/*攻击速度*/
            attackSpeed = int.Parse(jd["attackSpeed"].ToString());			/*移动速度*/
            moveSpeed = int.Parse(jd["moveSpeed"].ToString());			/*枚举测试*/
            testName = (Enum_Test)System.Enum.Parse(typeof(Enum_Test), jd["test_name"].ToString() == "" ? "None" : jd["test_name"].ToString());
        }
    }
    public List<HeroPropsConfig> configs;
    public Dictionary<int, HeroPropsConfig> configsDictionary;
    public override string GetConfigName()
    {
        return "HeroPropsConfig";
    }
    public override void ReadList()
    {
        configsDictionary = new Dictionary<int, HeroPropsConfig>();
        for (int i = 0; i < configs.Count; i++)
        {
            if (!configsDictionary.ContainsKey(configs[i].ID))
                configsDictionary.Add(configs[i].ID, configs[i]);
        }
    }

    public override void ReadFromData(object obj)
    {
        JsonData jsonData = obj as JsonData;
        configs = new List<HeroPropsConfig>();
        if (jsonData["name"].ToString() == GetConfigName())
        {
            if (((IDictionary)jsonData).Contains("config"))
            {
                JsonData config = jsonData["config"];
                for (int i = 0; i < config.Count; i++)
                {
                    HeroPropsConfig configItem = new HeroPropsConfig();
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
