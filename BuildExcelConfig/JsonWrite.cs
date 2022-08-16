using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LitJson;

namespace BuildExcelConfig
{
    internal class JsonWrite : DataWrite
    {
        public JsonData jsonData;
        public JsonWrite(string fileName)
        {
            string jsonPath = Config.writeDataPath + "ConfigAssetJson/";
            Init(jsonPath, fileName + "Config.json");
            jsonData = new JsonData();
            jsonData["name"] = fileName + "Config";
        }
        public void SetValue(int index, string key, string value, string type)
        {
            if (value == null || value == "")
            {
                if (type.ToLower().IndexOf("string") > -1)
                    value = "";
                else
                    value = "0";
            }
            if (((IDictionary)jsonData).Contains("config"))
            {
                while (jsonData["config"].Count <= index)
                {
                    jsonData["config"].Add(new JsonData());
                }
                jsonData["config"][index][key] = value;
            }
            else
            {
                jsonData["config"] = new JsonData();
                JsonData item = new JsonData();
                item[key] = value;
                jsonData["config"].Add(item);
            }
        }

        public override void Save()
        {
            sw.Write(Tool.JsonFormat(jsonData));
            base.Save();
        }

    }
}
