using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using LitJson;
using System.Collections;

namespace BuildExcelConfig
{
    public class LanguageData
    {
        static public void Save(Dictionary<string, string> languageDatas)
        {
            CsvWrite csv = new CsvWrite("Language", Config.currentPath);
            foreach (string key in languageDatas.Keys)
            {
                csv.Append(key + "," + languageDatas[key].ToString() + '\n');
            }
            csv.Save();
            Console.WriteLine("导出翻译配置==>"+Config.currentPath+"language.csv");
        }
        static public void CreateLanguageDataConfigAsset()
        {

        }
    }
}
