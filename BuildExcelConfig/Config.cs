using System;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Text;
using LitJson;

namespace BuildExcelConfig
{
    internal sealed class Config
    {
        static private string _appPath = string.Empty;
        static public string appPath
        {
            get
            {
                if (_appPath == string.Empty)
                {
                    _appPath = Tool.ResetPathSlash(System.AppDomain.CurrentDomain.BaseDirectory);
                    Console.WriteLine("工具路径：" + _appPath);
                }
                return _appPath;
            }
        }
        static public void Init()
        {
            ///路径配置文件
            string filePath = appPath + "BuildExcelConfig.json";
            Console.WriteLine("配置文件：" + filePath);
            JsonData jd;
            if (File.Exists(filePath))
            {
                StreamReader sr = new StreamReader(filePath);
                StringBuilder sb = new StringBuilder(sr.ReadToEnd());
                sb.Replace("\\", "/");
                Console.WriteLine(sb.ToString());
                jd = JsonMapper.ToObject(sb.ToString());
                sr.Close();
            }
            else
            {
                DirectoryInfo parentFolder = new DirectoryInfo(appPath).Parent;
                foreach (DirectoryInfo di in parentFolder.GetDirectories("*", SearchOption.AllDirectories))
                {
                    if (di.Name == ("Assets"))
                    {
                        unityPath = di.FullName;
                        break;
                    }
                }
                Console.WriteLine("Unity工程：" + unityPath);
                jd = new JsonData();
                if (Directory.Exists(appPath + "excel"))
                    jd["excel"] = appPath + "excel";
                else
                    jd["excel"] = "excel配置路径";
                if (parentFolder != null)
                {
                    jd["unitypath"] = Tool.ResetPathSlash(unityPath);
                    jd["outputDataPath"] = Tool.ResetPathSlash(unityPath) + "/ConfigAsset";
                }
                else
                {
                    jd["unitypath"] = $"unity工程路径";
                    jd["outputDataPath"] = "配置输出路径";
                }
                FileStream fs = new FileStream(filePath, FileMode.Create);
                StreamWriter sw = new StreamWriter(fs);
                sw.Write(Tool.JsonFormat(jd.ToJson()));
                //清空缓冲区
                sw.Flush();
                //关闭流
                sw.Close();
                //关闭文件
                fs.Close();
                Console.WriteLine("第一次创建BuildExcelConfig.json配置文件，修改对应路径");
            }
            readExcelPath = jd["excel"].ToString();
            unityPath = jd["unitypath"].ToString();
            if (unityPath.IndexOf("Assets") > -1)
            {
                if (unityPath[unityPath.Length - 1] != '/')
                    unityPath += '/';
            }
            else
            {
                Console.WriteLine("请选择Unity工程目录");
                unityPath = string.Empty;
            }
            readTemplatePath = $"{appPath}template/";
            writeScriptPath = $"{appPath}ConfigScript/";
            writeDataPath = $"{appPath}data/";
            outputDataPath = jd["outputDataPath"].ToString();
        }
        //刷新文件夹:删除旧的配置文件夹和脚本文件夹
        public static void RefreshFolder()
        {
            if (Directory.Exists(appPath + "data/"))
                Directory.Delete(appPath + "data/", true);
            Directory.CreateDirectory(appPath + "data/");
            if (Directory.Exists(appPath + "ConfigScript/"))
                Directory.Delete(appPath + "ConfigScript/", true);
            Directory.CreateDirectory(appPath + "ConfigScript/");
        }
        public static bool pathError
        {
            get
            {
                if (!Directory.Exists(readExcelPath))
                {
                    Console.WriteLine("设置excel路径");
                    return true;
                }
                else if (!Directory.Exists(unityPath))
                {
                    Console.WriteLine("unity工程路径错误");
                    return true;
                }
                return false;
            }
        }
        /// <summary>
        /// excel路径
        /// </summary>
        internal static string readExcelPath { private set; get; }
        /// <summary>
        /// unity工程目录
        /// </summary>
        internal static string unityPath { private set; get; }
        /// <summary>
        /// 模板文件路径
        /// </summary>
        internal static string readTemplatePath { private set; get; }
        /// <summary>
        /// 导出的配置脚本路径
        /// </summary>
        internal static string writeScriptPath { private set; get; }
        /// <summary>
        /// 导出的配置文件路径
        /// </summary>
        internal static string writeDataPath { private set; get; }
        /// <summary>
        /// 导出到工程中配置文件的路径
        /// </summary>
        internal static string outputDataPath { private set; get; }
    }
}
