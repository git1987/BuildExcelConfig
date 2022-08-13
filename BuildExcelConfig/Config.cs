using System;
using System.IO;
using LitJson;

namespace BuildExcelConfig
{
    internal abstract class Config
    {
        static public void Init()
        {
            string path = System.AppDomain.CurrentDomain.BaseDirectory + "path.json";
            string json;
            if (!File.Exists(path))
            {
                FileStream fs = new FileStream(path, FileMode.Create);
                StreamWriter sw = new StreamWriter(fs);

                JsonData jd = new JsonData();
                jd["read"] = new JsonData();
                jd["read"]["excel"] = "read excel path";
                jd["read"]["template"] = "read template path";
                jd["write"] = new JsonData();
                jd["write"]["csv"] = "export scv file path";
                jd["write"]["script"] = "export config script path";
                json = jd.ToJson();
                sw.Write(json);
                //清空缓冲区
                sw.Flush();
                //关闭流
                sw.Close();
                //关闭文件
                fs.Close();
            }
            else
            {
                StreamReader sr = new StreamReader(path);
                json = sr.ReadToEnd();
                json = json.Replace("\\", "\\\\");
            }
            _pathFile = JsonMapper.ToObject(json);
        }
        private static JsonData _pathFile; public static JsonData pathFile
        {
            set { _pathFile = value; }
            get
            {
                if (_pathFile == null) Init();
                return _pathFile;
            }

        }
        //excel路径
        internal static string readExcelPath
        { get { return pathFile["read"]["excel"].ToString(); } }
        internal static string readTemplatePath
        {
            get
            {
                return AppDomain.CurrentDomain.BaseDirectory + "template/";
                //return pathFile["read"]["template"].ToString();
            }
        }

        //导出的配置路径
        internal static string writeScriptPath
        { get { return pathFile["write"]["script"].ToString(); } }
        internal static string writeCsvPath
        { get { return pathFile["write"]["csv"].ToString(); } }
    }
}
