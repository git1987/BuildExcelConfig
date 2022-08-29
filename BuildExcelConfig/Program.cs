using System;
using System.IO;
using System.Collections.Generic;

using Excel;

using LitJson;

namespace BuildExcelConfig
{
    class Program
    {
        static void test()
        {

        }
        static void Main(string[] args)
        {
            //test();
            //return;
            Config.Init();
            while (Config.pathError)
            {
                Console.WriteLine("修改文件夹下的path文件路径");
                Console.ReadKey();
            }
            while (true)
            {
                Console.WriteLine("是否使用翻译配置(yes/no)=====>配置路径：" + Config.readExcelPath);
                string input = Console.ReadLine();
                Console.WriteLine("开始导出配置：" + input.ToLower() == "yes" || input.ToLower() == "y" ? "导出翻译" : "");
                Config.RefreshFolder();
                AllExcel(input.ToLower() == "yes" || input.ToLower() == "y");
                Console.WriteLine("导出配置、创建scprite脚本文件==>完成");
                ScriptWrite.CopyCSharpTemFile();
                Console.WriteLine(".cstem==>完成");
                ScriptWrite.CopyCSharpFile();
                Console.WriteLine("copy.cs==>完成");
                //复制到Unity工程中，父级目录和Unity工程目录放在一起
                CopyToUnity();
                Console.WriteLine("按Esc键退出");
                ConsoleKeyInfo info = Console.ReadKey();
                if (info.Key == ConsoleKey.Escape)
                {
                    break;
                }
            }
        }
        static void AllExcel(bool useLanguage)
        {
            List<List<string>> classNameList = new List<List<string>>();
            DirectoryInfo dir = new DirectoryInfo(Config.readExcelPath);
            Dictionary<string, string> languageDatas = null;
            if (useLanguage)
                languageDatas = new Dictionary<string, string>();
            foreach (FileInfo file in dir.GetFiles("*.xlsx"))
            {
                Console.WriteLine(file.Name);
                if (file.Name.ToLower().IndexOf("enum") > -1)
                {
                    ReadEnumExcel(file.FullName);
                }
                else
                {
                    classNameList.Add(ReadExcel(file.FullName, languageDatas));
                }
            }
            if (useLanguage)
                LanguageData.Save(languageDatas);
        }
        //读取枚举的excel
        static void ReadEnumExcel(string excelPath)
        {
            FileStream fileStream = File.Open(excelPath, FileMode.Open, FileAccess.Read);
            IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(fileStream);
            string currentName = string.Empty;
            ConfigEnum ce = new ConfigEnum();
            do
            {
                if (excelReader.Name != "enum")
                    continue;
                //excelReader.Name：sheet名称
                int index = 0;
                while (excelReader.Read())
                {
                    //数据起点
                    int dataIndex = 2 - 1;
                    //读取每行的内容
                    if (index < dataIndex)
                    {
                    }
                    else
                    {
                        //枚举名称
                        string enumName = excelReader.GetString(0);
                        //枚举注释
                        string enumNameSummary;
                        if (enumName != null && enumName != string.Empty && currentName != enumName)
                        {
                            currentName = excelReader.GetString(0);
                            enumNameSummary = excelReader.GetString(1);
                            ce.AddEnumSummary(currentName, enumNameSummary);
                        }
                        //枚举值名称
                        string enumValueName = excelReader.GetString(2);
                        //枚举值注释
                        string enumValueNameSummary = excelReader.GetString(3);
                        int? enumValue = excelReader.GetString(4).ToIntOrNull();
                        if (enumValueName == null)
                        {
                            continue;
                        }
                        ce.AddEnum(currentName, enumValueName, enumValue, enumValueNameSummary);
                    }
                    index++;
                }
            } while (excelReader.NextResult());
            ce.Save(Config.currentPath + "excel/Config_Enum.cs");
        }

        static List<string> ReadExcel(string excelPath, Dictionary<string, string> languageDatas)
        {
            FileStream fileStream = File.Open(excelPath, FileMode.Open, FileAccess.Read);
            IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(fileStream);
            List<string> nameList = new List<string>();
            do
            {
                if (excelReader.Name.ToLower().IndexOf("log") > -1
                    || excelReader.Name.ToLower().IndexOf("Sheet") > -1
                    || Tool.IsChinese(excelReader.Name))
                    continue;
                //excelReader.Name：sheet名称
                string sheetName = Tool.LowerToUpper(excelReader.Name, true);
                int index = 0;
                //CsvWrite csv = new CsvWrite(fileName);
                JsonWrite jsonConfig = new JsonWrite(sheetName);
                ScriptWrite script = new ScriptWrite(sheetName);
                nameList.Add(sheetName);
                while (excelReader.Read())
                {
                    //数据起点
                    int dataIndex = 5 - 1;
                    //读取每行的内容
                    if (index < dataIndex)
                    {
                        //第一行至第三行生成配置类 第四行是多语言
                        for (int i = 0; i < excelReader.FieldCount; i++)
                        {
                            if (i == 0 || i == 1)
                            {
                                if (excelReader.GetString(i) == null || excelReader.GetString(i) == string.Empty)
                                {

                                    Console.WriteLine("变量名称或者变量类型为空！！！");
                                    break;
                                }
                            }
                            script.Append(index, excelReader.GetString(i));
                        }
                    }
                    else
                    {
                        //从第四行开始储存配置：根据变量名长度
                        for (int i = 0; i < script.variableNameList.Count; i++)
                        {
                            //从第一列开始遍历内容
                            if (excelReader.GetString(i) == null || excelReader.GetString(i) == string.Empty)
                            {
                                //ID值为空
                                if (i == 0) break;
                            }
                            //内容
                            string content = excelReader.GetString(i);
                            if (languageDatas != null && script.languageList[i])
                            {
                                //判断是否在翻译配置
                                //language_sheetName_变量名_id
                                string key = string.Format("language_{0}_{1}_{2}", sheetName, script.variableNameList[i], excelReader.GetString(0));
                                if (languageDatas.ContainsKey(key))
                                    Console.WriteLine("====>翻译有相同的Key：" + key);
                                else
                                    languageDatas.Add(key, content);
                                content = key;
                            }
                            else
                            {
                                //csv.Append("\"" + content + "\"");
                                //if (i < script.variableNameList.Count - 1) csv.Append(",");
                                //else csv.Append("\n");
                            }
                            ///Json数据
                            if (i == 0)
                            {
                                if (sheetName.ToLower().IndexOf("language") > -1)
                                {
                                    //翻译配置
                                    jsonConfig.SetValue(index - dataIndex, "key", content, script.variableTypeList[i]);
                                }
                                else
                                    jsonConfig.SetValue(index - dataIndex, "ID", content, script.variableTypeList[i]);
                            }
                            jsonConfig.SetValue(index - dataIndex, script.variableNameList[i], content, script.variableTypeList[i]);
                            ///Csv数据
                        }
                    }
                    index++;
                }
                script.SaveScript(InitDataType.Json, sheetName);
                jsonConfig.Save();
                //csv.Save();
                Console.WriteLine("导出：" + excelReader.Name + "完成");
            } while (excelReader.NextResult());
            return nameList;
        }
        static void CopyToUnity()
        {
            DirectoryInfo folder = new DirectoryInfo(Config.currentPath);
            DirectoryInfo unityFolder = null;
            foreach (DirectoryInfo di in folder.Parent.GetDirectories("*", SearchOption.AllDirectories))
            {
                if (di.Name == "Assets")
                {
                    unityFolder = di;
                    break;
                }
            }
            if (unityFolder != null)
            {
                //脚本
                if (Directory.Exists(unityFolder.FullName + "/ConfigScript"))
                {
                    Directory.Delete(unityFolder.FullName + "/ConfigScript", true);
                }
                Directory.CreateDirectory(unityFolder.FullName + "/ConfigScript/Editor");
                DirectoryInfo script = new DirectoryInfo(Config.writeScriptPath);
                foreach (FileInfo file in script.GetFiles("*", SearchOption.AllDirectories))
                {
                    if (file.FullName.ToLower().IndexOf("editor") > -1)
                    {
                        file.CopyTo(unityFolder.FullName + "/ConfigScript/Editor/" + file.Name);
                    }
                    else
                        file.CopyTo(unityFolder.FullName + "/ConfigScript/" + file.Name);
                }
                Directory.Delete(Config.writeScriptPath, true);
                //配置
                if (Directory.Exists(unityFolder.FullName + "/ConfigAsset"))
                {
                    Directory.Delete(unityFolder.FullName + "/ConfigAsset", true);
                }
                Directory.CreateDirectory(unityFolder.FullName + "/ConfigAsset");
                DirectoryInfo data = new DirectoryInfo(Config.writeDataPath);
                foreach (FileInfo file in data.GetFiles("*", SearchOption.AllDirectories))
                {
                    file.CopyTo(unityFolder.FullName + "/ConfigAsset/" + file.Name);
                }
                Directory.Delete(Config.writeDataPath, true);
                Console.WriteLine("文件复制到工程内完毕");
            }
            else
            {
                Console.WriteLine("没有找到Unity工程目录");
            }
        }
    }
}
