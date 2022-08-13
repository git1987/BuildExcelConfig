using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace BuildExcelConfig
{
    abstract class LanguageData
    {
        static StringBuilder log = new StringBuilder();
        class Language
        {
            //key
            public string baseLanguage;
            public Dictionary<string, string> _languages;
            public Language(string _baseLanguage)
            {
                this.baseLanguage = _baseLanguage;
                _languages = new Dictionary<string, string>();
            }
            public void SetLanguage(string type, string text)
            {
                if (_languages.ContainsKey(type))
                {
                    log.AppendLine(baseLanguage + "有相同的翻译类型:" + type);
                }
                else
                {
                    _languages.Add(type, text);
                }
            }
            public override string ToString()
            {
                StringBuilder sb = new StringBuilder(string.Format("{0}", baseLanguage));
                foreach (string type in _languages.Keys)
                {
                    if (_languages[type].IndexOf("|") >= 0)
                    {
                        _languages[type].Replace("|", "^");
                        Console.WriteLine(_languages[type]);
                    }
                    sb.Append("|" + _languages[type]);
                }
                sb.Append("\n");
                return sb.ToString();
            }
        }
        static Dictionary<string, int> languageType = new Dictionary<string, int>();
        static Dictionary<string, Language> languages = new Dictionary<string, Language>();
        static public void SetKey(string key)
        {
            if (key == null) return;
            if (key == string.Empty) return;
            if (!languages.ContainsKey(key))
            {
                languages.Add(key, new Language(key));
            }
            else
                log.AppendLine(key);
        }
        static public void Write(string key, string type, string text)
        {
            if (key == null) return;

            if (languageType.ContainsKey(type)) languageType[type]++;
            else languageType.Add(type, 1);
            if (!languages.ContainsKey(key))
            {
                Console.WriteLine("没有这个key====>" + key);
                return;
            }
            languages[key].SetLanguage(type, text);
        }
        static private void BuildLanguageConfigAsset()
        {
            StreamReader sr = new StreamReader(Config.readTemplatePath + "/LanguageData.tem");
            StringBuilder template = new StringBuilder(sr.ReadToEnd());
            List<int> list = new List<int>();
            int temp = 0;
            foreach (string str in template.ToString().Split('\n'))
            {
                int indexIf = str.IndexOf("#if");
                int indexEndif = str.IndexOf("#endif");
                if (indexIf >= 0)
                    list.Add(indexIf + temp);
                if (indexEndif >= 0)
                    list.Add(indexEndif + temp);
                temp += str.Length + 1;
            }
            if (list.Count % 2 != 0) throw new Exception("#if和#endif不相等");
            for (int i = list.Count - 1; i > 0; i -= 2)
            {
                string oldContent = template.ToString().Substring(list[i - 1], list[i] - list[i - 1] + 6);
                if (oldContent.IndexOf("languageType") >= 0)
                {
                    StringBuilder newContent = new StringBuilder();
                    foreach (string type in languageType.Keys)
                    {
                        newContent.AppendLine("        " + type + ",");
                    }
                    template.Replace(oldContent, newContent.ToString());
                }
            }
            //保存
            if (!Directory.Exists(Config.writeScriptPath))
            {
                Directory.CreateDirectory(Config.writeScriptPath);
            }
            FileStream fs = new FileStream(Config.writeScriptPath + "LanguageData.cs", FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            sw.Write(template.ToString());
            sw.Flush();
            sw.Close();
            fs.Close();
            Console.WriteLine("LanguageData.cs创建完成");
        }
        static public void Save()
        {
            BuildLanguageConfigAsset();
            CsvWrite csv = new CsvWrite("LanguageData");
            foreach (string key in languages.Keys)
            {
                csv.Append(languages[key].ToString());
            }
            csv.SaveCsv();
            if (log.ToString() != null)
            {
                //Console.WriteLine("重复翻译：\n" + log.ToString());
            }
        }
    }
}
