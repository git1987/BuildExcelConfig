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
        /// enum配置sheet
        static ConfigEnum configEnum;
        static ScriptWrite sw;
        static void Main(string[] args)
        {
            //test();return;
            Config.Init();
            while (Config.pathError)
            {
                Console.WriteLine("按任意键继续(按Esc退出)");
                ConsoleKeyInfo info = Console.ReadKey();
                if (info.Key == ConsoleKey.Escape)
                    return;
            }
            bool language = false;
            System.Action build = () =>
            {
                Console.WriteLine("开始导出配置：" + (language ? "导出翻译" : ""));
                Config.RefreshFolder();
                configEnum = new ConfigEnum();
                AllExcel(language);
                Console.WriteLine("导出配置、创建scprite脚本文件==>完成");
                ScriptWrite.CopyCSharpTemFile();
                Console.WriteLine(".cstem==>完成");
                ScriptWrite.CopyCSharpFile();
                Console.WriteLine("copy.cs==>完成");
                //复制到Unity工程中，父级目录和Unity工程目录放在一起
                CopyToUnity();
            };
            FileSystemWatcher watcher = new FileSystemWatcher(Config.readExcelPath, "*.xlsx");
            watcher.NotifyFilter = NotifyFilters.LastWrite |
                NotifyFilters.FileName |
                NotifyFilters.Size |
                NotifyFilters.LastAccess;
            watcher.Changed += new FileSystemEventHandler((s, e) =>
            {
                Console.WriteLine("文件发生变化！自动导出配置");
                build.Invoke();
            });
            watcher.EnableRaisingEvents = true;
            while (true)
            {
                ScriptWrite.classNameList = new List<string>();
                Console.WriteLine($"=====>配置路径：{Config.readExcelPath}");
                Console.WriteLine("是否使用翻译配置（输入yes或者y：不区分大小写,按Esc键退出）");
                ConsoleKeyInfo info = Console.ReadKey();
                if (info.Key == ConsoleKey.Escape)
                {
                    break;
                }
                else if (info.Key == ConsoleKey.Enter)
                {
                    //不翻译，直接导出配置
                }
                else
                {
                    string input = info.KeyChar + Console.ReadLine();
                    language = input.ToLower() == "yes" || input.ToLower() == "y";
                }
                build.Invoke();
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
                //排除excel缓存文件
                if (file.Name.IndexOf("$") == -1)
                {
                    Console.WriteLine(file.Name);
                    classNameList.Add(ReadExcel(file.FullName, languageDatas));
                    //if (file.Name.ToLower().IndexOf("enum") > -1)
                    //{
                    //    ReadEnumExcel(file.FullName);
                    //}
                    //else
                    //{
                    //}
                }
            }
            //导出每个表中的enum  sheet
            configEnum.Save(Config.appPath + "excel/Config_Enum.cs");
            if (useLanguage)
                LanguageData.Save(languageDatas);
        }
        //读取枚举的excel
        //static void ReadEnumExcel(string excelPath)
        static void ReadEnumExcel(IExcelDataReader excelReader)
        {
            //using (FileStream fileStream = File.Open(excelPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                //excelReader = ExcelReaderFactory.CreateOpenXmlReader(fileStream);
                string currentName = string.Empty;
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
                                configEnum.AddEnumSummary(currentName, enumNameSummary);
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
                            configEnum.AddEnum(currentName, enumValueName, enumValue, enumValueNameSummary);
                        }
                        index++;
                    }
                } while (excelReader.NextResult());
                //configEnum.Save(Config.currentPath + "excel/Config_Enum.cs");
            }
        }

        static List<string> ReadExcel(string excelPath, Dictionary<string, string> languageDatas)
        {
            List<string> nameList = new List<string>();
            using (FileStream fileStream = File.Open(excelPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(fileStream);
                do
                {
                    if (excelReader.Name.ToLower() == "log"
                        || excelReader.Name.ToLower().IndexOf("sheet") > -1
                        || Tool.IsChinese(excelReader.Name))
                        continue;
                    if (excelReader.Name.ToLower() == "enum")
                    {
                        ReadEnumExcel(excelReader);
                        continue;
                    }
                    //excelReader.Name：sheet名称
                    string sheetName = Tool.LowerToUpper(excelReader.Name, true);
                    int index = 0;
                    //CsvWrite csv = new CsvWrite(fileName);
                    JsonWrite jsonConfig = new JsonWrite(sheetName);
                    ScriptWrite script = new ScriptWrite(sheetName, languageDatas != null);
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
                                if (index == 0)
                                {
                                    if (excelReader.GetString(i) == null || excelReader.GetString(i) == string.Empty)
                                    {
                                        if (i == 0)
                                            Console.WriteLine(sheetName + "====>第一个变量名称为空！！");
                                        else
                                            Console.WriteLine(sheetName + "====>有空的变量名称！！前一个变量名成为：" + excelReader.GetString(i - 1));
                                        break;
                                    }
                                }
                                else if (index == 1)
                                {
                                    if (excelReader.GetString(i) == null || excelReader.GetString(i) == string.Empty)
                                    {
                                        Console.WriteLine(sheetName + "====>有空的变量类型！！！！！！");
                                        break;
                                    }
                                }
                                //数据列数超过名称、类型后的数据忽略
                                else if (i >= script.variableNameList.Count || i >= script.variableTypeList.Count)
                                {
                                    break;
                                }
                                script.Append(index, excelReader.GetString(i));
                            }
                        }
                        else
                        {
                            //从第四行开始储存配置：根据变量名长度
                            for (int i = 0; i < script.variableNameList.Count && i < script.variableTypeList.Count; i++)
                            {
                                //从第一列开始遍历内容
                                if (excelReader.GetString(i) == null || excelReader.GetString(i) == string.Empty)
                                {
                                    //ID值为空
                                    if (i == 0) break;
                                }
                                //内容
                                string content = excelReader.GetString(i);
                                if (content == null) content = String.Empty;
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
            }
            return nameList;
        }
        static void CopyToUnity()
        {
            //DirectoryInfo folder = new DirectoryInfo(Config.appPath);
            //DirectoryInfo unityFolder = null;
            //foreach (DirectoryInfo di in folder.Parent.GetDirectories("*", SearchOption.AllDirectories))
            //{
            //    if (di.Name == "Assets")
            //    {
            //        unityFolder = di;
            //        break;
            //    }
            //}
            //if (unityFolder != null)
            if (Directory.Exists(Config.unityPath))
            {
                //脚本
                if (Directory.Exists(Config.unityPath + "ConfigScript"))
                {
                    Directory.Delete(Config.unityPath + "ConfigScript", true);
                }
                Directory.CreateDirectory(Config.unityPath + "ConfigScript/Editor");
                DirectoryInfo script = new DirectoryInfo(Config.writeScriptPath);
                foreach (FileInfo file in script.GetFiles("*", SearchOption.AllDirectories))
                {
                    if (file.FullName.ToLower().IndexOf("editor") > -1)
                    {
                        file.CopyTo(Config.unityPath + "ConfigScript/Editor/" + file.Name);
                    }
                    else
                        file.CopyTo(Config.unityPath + "ConfigScript/" + file.Name);
                }
                Directory.Delete(Config.writeScriptPath, true);
                //配置
                if (Directory.Exists(Config.outputDataPath))
                {
                    Directory.Delete(Config.outputDataPath, true);
                }
                Directory.CreateDirectory(Config.outputDataPath);
                DirectoryInfo data = new DirectoryInfo(Config.writeDataPath);
                foreach (FileInfo file in data.GetFiles("*", SearchOption.AllDirectories))
                {
                    file.CopyTo(Config.outputDataPath + "/" + file.Name);
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
