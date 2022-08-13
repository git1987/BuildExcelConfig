using System;
using System.IO;
using System.Collections.Generic;

using Excel;

using LitJson;
using LitJson.SQ;

namespace BuildExcelConfig
{
    class Program
    {
        static void Main(string[] args)
        {
            //LitJson.SQ.JsonSQ jsonSQ = new LitJson.SQ.JsonSQ("sq");
            //jsonSQ.data = new Data("key", "value");
            //jsonSQ.Save("G:\\BuildExcelConfig", "save");
            //Console.ReadKey(); return;
            if (!Directory.Exists(Config.readExcelPath))
            {
                Console.WriteLine("读取excel" + Config.readExcelPath + "路径不存在");
            }
            else if (!Directory.Exists(Config.readTemplatePath))
            {
                Console.WriteLine("读取模版" + Config.readTemplatePath + "路径不存在");
            }
            else if (!Directory.Exists(Config.writeScriptPath))
            {
                Console.WriteLine("生成配置脚本" + Config.writeScriptPath + "路径不存在");
            }
            else if (!Directory.Exists(Config.writeCsvPath))
            {
                Console.WriteLine("生成配置" + Config.writeCsvPath + "路径不存在");
            }
            //else
            {
                AllExcels();
            }
            Console.ReadKey();
        }
        static void AllExcels()
        {
            List<List<string>> classNameList = new List<List<string>>();
            DirectoryInfo dir = Directory.CreateDirectory(Config.readExcelPath);
            foreach (FileInfo file in dir.GetFiles())
            {
                Console.WriteLine(file.Name);
                if (Path.GetExtension(file.FullName) == ".xlsx")
                {
                    Console.WriteLine("创建Script====>文件路径：" + file.FullName);
                    classNameList.Add(ReadExcel(file.FullName));
                }
            }
            ScriptWrite.BuildCreateConfig();
            ScriptWrite.BuildConfigAssetsData();
            ScriptWrite.BuildCsvReader();
            ScriptWrite.BuildConfigAssetBase();
            ScriptWrite.CopyListExtend();
            LanguageData.Save();
            Console.WriteLine("==============>完成");
        }

        static List<string> ReadExcel(string excelPath)
        {
            FileStream fileStream = File.Open(excelPath, FileMode.Open, FileAccess.Read);
            IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(fileStream);
            List<string> nameList = new List<string>();
            do
            {
                if (excelReader.Name.IndexOf("log") > -1
                    || excelReader.Name.IndexOf("Sheet") > -1)
                    continue;
                //excelReader.Name：sheet名称
                string fileName = Tool.LowerToUpper(excelReader.Name, true);
                int index = 0;
                CsvWrite csv = new CsvWrite(fileName);
                ScriptWrite script = new ScriptWrite(fileName);
                nameList.Add(fileName);
                while (excelReader.Read())
                {
                    index++;
                    //读取每行的内容
                    if (index <= 4)
                    {
                        //第一行至第三行生成配置类 第四行是多语言
                        for (int i = 0; i < excelReader.FieldCount; i++)
                        {
                            if (excelReader.GetString(i) == null || excelReader.GetString(i) == string.Empty) break;
                            script.Append(excelReader.GetString(i), index);
                        }
                    }
                    else if (index == 4)
                    {
                        //多语言
                        //script.Append(excelReader.GetString(i), index);
                    }
                    else
                    {
                        //从第四行开始储存配置
                        for (int i = 0; i < excelReader.FieldCount; i++)
                        {
                            //从第一列开始遍历内容
                            if (excelReader.GetString(i) == null || excelReader.GetString(i) == string.Empty)
                            {
                                //ID值为空
                                if (i == 0) break;
                            }
                            csv.Append("\"" + excelReader.GetString(i) + "\"");
                            if (i < excelReader.FieldCount - 1) csv.Append(",");
                            else csv.Append("\n");
                            //翻译
                            if (script.languageList[i])
                            {
                                string baseLanguage = excelReader.GetString(i);
                                LanguageData.SetKey(baseLanguage);
                                for (int keyIndex = i + 1; keyIndex < excelReader.FieldCount; keyIndex++)
                                {
                                    if (script.variableNameList[keyIndex].IndexOf(script.variableNameList[i]) > -1)
                                    {
                                        LanguageData.Write(baseLanguage,
                                            script.variableNameList[keyIndex].Split('_')[script.variableNameList[keyIndex].Split('_').Length - 1],
                                            excelReader.GetString(keyIndex));
                                    }
                                    else
                                    {
                                        //i = keyIndex;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                script.SaveScript();
                csv.SaveCsv();
                Console.WriteLine("导出：" + excelReader.Name + "完成");
            } while (excelReader.NextResult());
            return nameList;
        }


    }
}
