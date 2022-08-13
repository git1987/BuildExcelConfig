using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace BuildExcelConfig
{
    internal class ScriptWrite
    {
        static string scriptTemplate;
        static List<string> classNameList = new List<string>();
        static ScriptWrite()
        {
            StreamReader sr = new StreamReader(Config.readTemplatePath + "/ConfigAsset.tem");
            scriptTemplate = sr.ReadToEnd();
        }
        public enum StringType
        {
            variableName = 1,
            variableType,
            summary,
            language
        }
        FileStream fs;
        StreamWriter sw;
        StringBuilder config;

        string className;
        //变量名称
        public List<string> variableNameList = new List<string>();
        //备注
        public List<string> summarylist = new List<string>();
        //变量类型
        public List<string> variableTypeList = new List<string>();
        //是否是多语言
        public List<bool> languageList = new List<bool>();

        static public string scriptPath
        {
            get { return Config.writeScriptPath + "ConfigAssetScript/"; }
        }
        public ScriptWrite(string fileName)
        {
            classNameList.Add(fileName);
            if (!Directory.Exists(scriptPath))
                Directory.CreateDirectory(scriptPath);
            fs = new FileStream(scriptPath + fileName + "ConfigAsset.cs", FileMode.Create);
            className = fileName + "ConfigAsset";
            sw = new StreamWriter(fs);
            config = new StringBuilder(ScriptWrite.scriptTemplate);
            config.Replace("#{ClassName}", fileName);
        }
        public void Append(string content, int type)
        {
            switch ((StringType)type)
            {
                case StringType.variableName:
                    variableNameList.Add(content);
                    break;
                case StringType.summary:
                    summarylist.Add(content);
                    break;
                case StringType.variableType:
                    variableTypeList.Add(content);
                    break;
                case StringType.language:
                    languageList.Add(!(content == "0" || content == ""));
                    break;
            }
        }
        void SetVariable()
        {
            if (variableNameList.Count != summarylist.Count ||
                variableNameList.Count != variableTypeList.Count)
            {
                throw new Exception("变量名、变量类型、备注数量不相等：" + className);
            }
            //变量个数
            int forCount = variableNameList.Count;
            List<int> list = new List<int>();
            int temp = 0;
            foreach (string str in scriptTemplate.Split('\n'))
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
                string oldContent = scriptTemplate.Substring(list[i - 1], list[i] - list[i - 1] + 6);
                StringBuilder newContent = new StringBuilder();
                StringBuilder newContents = new StringBuilder();
                if (oldContent.IndexOf("List") >= 0)
                {
                    string[] oldContents = oldContent.Split('\n');
                    for (int j = 1; j < oldContents.Length - 1; j++)
                    {
                        newContent.Append(oldContents[j]);
                    }
                    if (newContent.ToString() == string.Empty)
                    {
                        throw new Exception("#if中没有内容");
                    }
                    int languageIndex;
                    for (int k = 1; k < forCount; k++)
                    {
                        //languageIndex = k;
                        newContents.AppendLine("        /*" + summarylist[k] + "*/");
                        StringBuilder changeContent = new StringBuilder(newContent.ToString());
                        if (changeContent.ToString().IndexOf("#{variableName}") >= 0)
                        {
                            if (languageList[k])
                                changeContent.Replace("#{variableName}", "_" + Tool.LowerToUpper(variableNameList[k]));
                            else
                                changeContent.Replace("#{variableName}", Tool.LowerToUpper(variableNameList[k]));
                        }
                        if (changeContent.ToString().IndexOf("#{assignment}") >= 0)
                        {
                            StringBuilder str = new StringBuilder(string.Format("list[{0}]", k));
                            if (variableTypeList[k] == "string")
                            {
                                str = new StringBuilder(string.Format("{0} as {1}", str, variableTypeList[k]));
                            }
                            else if (variableTypeList[k].IndexOf("List") >= 0)
                            {
                                str.Append(string.Format(".To{0}()", variableTypeList[k]));
                            }
                            else
                            {
                                str = new StringBuilder(string.Format("{0}.Parse({1} as string)", variableTypeList[k], str));
                            }
                            changeContent.Replace("#{assignment}", str.ToString());
                        }
                        if (changeContent.ToString().IndexOf("#{typeName}") >= 0)
                        {
                            //类
                            if (className.IndexOf("ConfigAsset") > -1)
                            {
                                if (languageList[k])
                                {
                                    changeContent.Replace("#{typeName}", "[SerializeField]\n        private " + variableTypeList[k]);
                                    changeContent.AppendLine("        public " + variableTypeList[k] + " " + Tool.LowerToUpper(variableNameList[k]) +
                                        "\n        {get{return ConfigAssetsData.instance.GetLanguageText(_" + Tool.LowerToUpper(variableNameList[k]) + "); }set{ _" + Tool.LowerToUpper(variableNameList[k]) + " = value;}}");
                                    for (languageIndex = k; languageIndex < forCount; languageIndex++)
                                    {
                                        if (variableNameList[languageIndex].IndexOf(variableNameList[k]) < 0)
                                        {
                                            //k = languageIndex;
                                            break;
                                        }
                                    }
                                }
                                else
                                    changeContent.Replace("#{typeName}", "public " + variableTypeList[k]);
                            }
                            else
                                changeContent.Replace("#{typeName}", variableTypeList[k]);
                        }
                        newContents.Append(changeContent);
                    }
                }
                config.Replace(oldContent, newContents.ToString());
            }
        }

        public void SaveScript()
        {
            SetVariable();
            sw.Write(config.ToString());
            sw.Flush();
            sw.Close();
            fs.Close();
        }
        static public void BuildCreateConfig()
        {
            //StreamReader sr = new StreamReader(Config.readTemplatePath + "/CreateConfig.tem");
            //StringBuilder template = new StringBuilder(sr.ReadToEnd());
            //List<int> list = new List<int>();
            //int temp = 0;
            //foreach (string str in template.ToString().Split('\n'))
            //{
            //    int indexIf = str.IndexOf("#if");
            //    int indexEndif = str.IndexOf("#endif");
            //    if (indexIf >= 0)
            //        list.Add(indexIf + temp);
            //    if (indexEndif >= 0)
            //        list.Add(indexEndif + temp);
            //    temp += str.Length + 1;
            //}
            //if (list.Count % 2 != 0) throw new Exception("#if和#endif不相等");
            //for (int i = list.Count - 1; i > 0; i -= 2)
            //{
            //    string oldContent = template.ToString().Substring(list[i - 1], list[i] - list[i - 1] + 6);
            //    StringBuilder newContent = new StringBuilder();
            //    StringBuilder newContents = new StringBuilder();
            //    if (oldContent.IndexOf("List") >= 0)
            //    {
            //        string[] oldContents = oldContent.Split('\n');
            //        for (int j = 1; j < oldContents.Length - 1; j++)
            //        {
            //            newContent.AppendLine(oldContents[j]);
            //        }
            //        if (newContent.ToString() == string.Empty)
            //        {
            //            throw new Exception("#if中没有内容");
            //        }
            //        for (int j = 0; j < classNameList.Count; j++)
            //        {
            //            StringBuilder changeContent = new StringBuilder(newContent.ToString());
            //            changeContent.Replace("#{ClassName}", classNameList[j]);
            //            newContents.Append(changeContent.ToString());
            //        }
            //        template.Replace(oldContent, newContents.ToString());
            //    }
            //}
            ////保存
            //if (!Directory.Exists(scriptPath + "Editor/"))
            //{
            //    Directory.CreateDirectory(scriptPath + "Editor/");
            //}
            //FileStream fs = new FileStream(scriptPath + "Editor/" + "CreateConfig.cs", FileMode.Create);
            //StreamWriter sw = new StreamWriter(fs);
            //sw.Write(template.ToString());
            //sw.Flush();
            //sw.Close();
            //fs.Close();
            //Console.WriteLine("CreateConfig.cs创建完成");
            WriteCSFile("CreateConfig.tem", scriptPath + "Editor/" + "CreateConfig.cs");
        }
        static public void BuildConfigAssetsData()
        {
            //StreamReader sr = new StreamReader(Config.readTemplatePath + "/ConfigAssetsData.tem");
            //StringBuilder template = new StringBuilder(sr.ReadToEnd());
            //List<int> list = new List<int>();
            //int temp = 0;
            //foreach (string str in template.ToString().Split('\n'))
            //{
            //    int indexIf = str.IndexOf("#if");
            //    int indexEndif = str.IndexOf("#endif");
            //    if (indexIf >= 0)
            //        list.Add(indexIf + temp);
            //    if (indexEndif >= 0)
            //        list.Add(indexEndif + temp);
            //    temp += str.Length + 1;
            //}
            //if (list.Count % 2 != 0) throw new Exception("#if和#endif不相等");
            //for (int i = list.Count - 1; i > 0; i -= 2)
            //{
            //    string oldContent = template.ToString().Substring(list[i - 1], list[i] - list[i - 1] + 6);
            //    StringBuilder newContent = new StringBuilder();
            //    StringBuilder newContents = new StringBuilder();
            //    if (oldContent.IndexOf("List") >= 0)
            //    {
            //        string[] oldContents = oldContent.Split('\n');
            //        for (int j = 1; j < oldContents.Length - 1; j++)
            //        {
            //            newContent.Append(oldContents[j]);
            //        }
            //        if (newContent.ToString() == string.Empty)
            //        {
            //            throw new Exception("#if中没有内容");
            //        }
            //        for (int j = 0; j < classNameList.Count; j++)
            //        {
            //            StringBuilder changeContent = new StringBuilder(newContent.ToString());
            //            changeContent.Replace("#{ClassName}", Tool.FirstUpper(classNameList[j]));
            //            changeContent.Replace("#{className}", Tool.FirstLower(classNameList[j]));
            //            newContents.Append(changeContent.ToString());
            //        }
            //        template.Replace(oldContent, newContents.ToString());
            //    }
            //}
            ////保存
            //if (!Directory.Exists(Config.writeScriptPath))
            //{
            //    Directory.CreateDirectory(Config.writeScriptPath);
            //}
            //FileStream fs = new FileStream(Config.writeScriptPath + "ConfigAssetsData.cs", FileMode.Create);
            //StreamWriter sw = new StreamWriter(fs);
            //sw.Write(template.ToString());
            //sw.Flush();
            //sw.Close();
            //fs.Close();
            //Console.WriteLine("ConfigAssetsData.cs创建完成");
            WriteCSFile("ConfigAssetsData.tem", Config.writeScriptPath + "ConfigAssetsData.cs");
        }
        //CsvReader脚本
        static public void BuildCsvReader()
        {
            //StreamReader sr = new StreamReader(Config.readTemplatePath + "/CsvReader.tem");
            //StringBuilder template = new StringBuilder(sr.ReadToEnd());
            //FileStream fs = new FileStream(Config.writeScriptPath + "CsvReader.cs", FileMode.Create);
            //StreamWriter sw = new StreamWriter(fs);
            //sw.Write(template.ToString());
            //sw.Flush();
            //sw.Close();
            //fs.Close();
            //Console.WriteLine("CsvReader.cs创建完成");
            WriteCSFile("ConfigAssetBase.tem", Config.writeScriptPath + "CsvReader.cs");
        }
        static public void BuildConfigAssetBase()
        {
            //StreamReader sr = new StreamReader(Config.readTemplatePath + "/ConfigAssetBase.tem");
            //StringBuilder template = new StringBuilder(sr.ReadToEnd());
            //FileStream fs = new FileStream(Config.writeScriptPath + "ConfigAssetBase.cs", FileMode.Create);
            //StreamWriter sw = new StreamWriter(fs);
            //sw.Write(template.ToString());
            //sw.Flush();
            //sw.Close();
            //fs.Close();
            //Console.WriteLine("ConfigAssetBase.cs创建完成");
            WriteCSFile("CsvReader.tem", Config.writeScriptPath + "ConfigAssetBase.cs");
        }
        public static void CopyListExtend()
        {
            if (File.Exists(Config.readTemplatePath + "ListExtend.cs"))
                File.Copy(Config.readTemplatePath + "ListExtend.cs", Config.writeScriptPath + "ListExtend.cs", true);
            //WriteCSFile("ListExtend.cs", Config.writeScriptPath + "ListExtend.cs");
        }
        //根据tem模版文件生成cs脚本
        static private void WriteCSFile(string temFileName, string csFilePath)
        {
            if (!File.Exists(Config.readTemplatePath + temFileName))
            {
                Console.WriteLine("模版文件不存在====>" + temFileName);
            }
            StreamReader sr = new StreamReader(Config.readTemplatePath + temFileName);
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
                StringBuilder newContent = new StringBuilder();
                StringBuilder newContents = new StringBuilder();
                if (oldContent.IndexOf("List") >= 0)
                {
                    string[] oldContents = oldContent.Split('\n');
                    for (int j = 1; j < oldContents.Length - 1; j++)
                    {
                        newContent.Append(oldContents[j]);
                    }
                    if (newContent.ToString() == string.Empty)
                    {
                        throw new Exception("#if中没有内容");
                    }
                    for (int j = 0; j < classNameList.Count; j++)
                    {
                        StringBuilder changeContent = new StringBuilder(newContent.ToString());
                        changeContent.Replace("#{ClassName}", Tool.FirstUpper(classNameList[j]));
                        changeContent.Replace("#{className}", Tool.FirstLower(classNameList[j]));
                        newContents.Append(changeContent.ToString());
                    }
                    template.Replace(oldContent, newContents.ToString());
                }
            }
            //保存
            if (!Directory.Exists(scriptPath + "Editor/"))
            {
                Directory.CreateDirectory(scriptPath + "Editor/");
            }
            FileStream fs = new FileStream(csFilePath.IndexOf(".cs") >= 0 ? csFilePath : csFilePath + ".cs", FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            sw.Write(template.ToString());
            sw.Flush();
            sw.Close();
            fs.Close();
            Console.WriteLine("CS脚本文件" + csFilePath + "创建完成");
        }
    }
}
