using System;
using System.IO;
using LitJson;

namespace BuildExcelConfig
{
    internal abstract class Config
    {
        static private string _currentPath = string.Empty;
        static public string currentPath
        {
            get
            {
                if (_currentPath == string.Empty)
                {
                    _currentPath = System.AppDomain.CurrentDomain.BaseDirectory.Replace("\\\\", "/");
                    _currentPath = _currentPath.Replace("\\", "/");
                    _currentPath = _currentPath.Replace("//", "/");
                    Console.WriteLine("工具路径：" + _currentPath);
                }
                return _currentPath;
            }
        }
        private static JsonData pathFile;
        static public void Init()
        {
            //string path = currentPath + "path.json";
            string json;
            //if (!File.Exists(path))
            {
                //FileStream fs = new FileStream(path, FileMode.Create);
                //StreamWriter sw = new StreamWriter(fs);

                JsonData jd = new JsonData();
                jd["read"] = new JsonData();
                jd["read"]["excel"] = currentPath + "excel/";
                Directory.CreateDirectory(currentPath + "excel/");
                jd["read"]["template"] = currentPath + "template/";
                jd["write"] = new JsonData();
                jd["write"]["data"] = currentPath + "data/";
                Directory.CreateDirectory(currentPath + "data/");
                jd["write"]["script"] = currentPath + "ConfigScript/";
                Directory.CreateDirectory(currentPath + "ConfigScript/");
                json = Tool.JsonFormat(jd.ToJson());
                //sw.Write(json);
                //清空缓冲区
                //sw.Flush();
                //关闭流
                //sw.Close();
                //关闭文件
                //fs.Close();
            }
            //else
            //{
            //    StreamReader sr = new StreamReader(path);
            //    json = sr.ReadToEnd();
            //    json = json.Replace("\\\\", "/");
            //    sr.Close();
            //}
            pathFile = JsonMapper.ToObject(json);
        }
        //刷新文件夹:删除旧的配置文件夹和脚本文件夹
        public static void RefreshFolder()
        {
            if (Directory.Exists(currentPath + "data/"))
                Directory.Delete(currentPath + "data/", true);
            Directory.CreateDirectory(currentPath + "data/");
            if (Directory.Exists(currentPath + "ConfigScript/"))
                Directory.Delete(currentPath + "ConfigScript/", true);
            Directory.CreateDirectory(currentPath + "ConfigScript/");
        }
        public static bool pathError
        {
            get
            {

                if (!Directory.Exists(readExcelPath))
                {
                    Console.WriteLine(string.Format("读取excel配置路径错误==>[{0}]", currentPath + "excel/path.txt"));
                    return true;
                }
                else if (!Directory.Exists(readTemplatePath))
                {
                    Console.WriteLine(string.Format("读取读取模版路径错误==>[{0}]", readTemplatePath));
                    return true;
                }
                else if (!Directory.Exists(writeScriptPath))
                {
                    Console.WriteLine(string.Format("生成配置脚本路径错误==>[{0}]", writeScriptPath));
                    return true;
                }
                else if (!Directory.Exists(writeDataPath))
                {
                    Console.WriteLine(string.Format("生成配置路径错误==>[{0}]", writeDataPath));
                    return true;
                }
                return false;
            }
        }
        //excel路径
        internal static string readExcelPath
        {
            get
            {
                string file = pathFile["read"]["excel"].ToString() + "path.txt";
                if (!File.Exists(file))
                {
                    Directory.CreateDirectory(currentPath + "excel/");
                    FileStream fs = new FileStream(file, FileMode.Create);
                    StreamWriter sw = new StreamWriter(fs);
                    sw.Write("excel配置文件夹路径");
                    //清空缓冲区
                    sw.Flush();
                    //关闭流
                    sw.Close();
                    //关闭文件
                    fs.Close();
                    return "";
                }
                else
                {
                    return GetFileContent(file);
                }
            }
        }
        internal static string readTemplatePath
        {
            get
            {
                //return AppDomain.CurrentDomain.BaseDirectory + "template/";
                return pathFile["read"]["template"].ToString();
            }
        }

        //导出的配置路径
        internal static string writeScriptPath
        { get { return pathFile["write"]["script"].ToString(); } }
        internal static string writeDataPath
        { get { return pathFile["write"]["data"].ToString(); } }

        static string GetFileContent(string filePath)
        {
            StreamReader sr = new StreamReader(filePath);
            string path = sr.ReadToEnd();
            sr.Close();
            return path;
        }
    }
}
