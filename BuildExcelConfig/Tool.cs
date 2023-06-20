using LitJson;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace BuildExcelConfig
{
    internal abstract class Tool
    {
        /// <summary>
        /// 删除_，并且将_后面的字符变成大写
        /// </summary>
        /// <param name="lower"></param>
        /// <returns></returns>
        public static string LowerToUpper(string lower, bool containFirst = false)
        {
            if (lower == string.Empty) return string.Empty;
            StringBuilder content = new StringBuilder();
            string[] lowers = lower.Split('_');
            if (!containFirst)
                content.Append(lowers[0]);
            for (int i = containFirst ? 0 : 1; i < lowers.Length; i++)
            {
                if (lowers[i].ToCharArray()[0] >= 'a' && lowers[i].ToCharArray()[0] <= 'z')
                {
                    content.Append(lowers[i].Substring(0, 1).ToUpper() + lowers[i].Substring(1, lowers[i].Length - 1));
                }
                else
                    content.Append(lowers[i]);
            }
            return content.ToString();
        }

        public static string FirstUpper(string text)
        {
            string str = text.Substring(0, 1).ToUpper() + text.Substring(1, text.Length - 1);
            return str;
        }

        public static string FirstLower(string text)
        {
            string str = text.Substring(0, 1).ToLower() + text.Substring(1, text.Length - 1);
            return str;
        }
        static public bool IsChinese(string text)
        {
            if (Regex.IsMatch(text, @"[\u4e00-\u9fbb]"))
            {
                return true;
            }
            return false;
        }

        //public static string Normal(string json) {
        //JsonSerializer serializer = new JsonSerializer();
        //TextReader tr = new StringReader(str);
        //JsonTextReader jtr = new JsonTextReader(tr);
        //object obj = serializer.Deserialize(jtr);
        //if (obj != null)
        //{
        //    StringWriter textWriter = new StringWriter();
        //    JsonTextWriter jsonWriter = new JsonTextWriter(textWriter)
        //    {
        //        Formatting = Formatting.Indented,
        //        Indentation = 4,
        //        IndentChar = ' '
        //    };
        //    serializer.Serialize(jsonWriter, obj);
        //    return textWriter.ToString();
        //}
        //else
        //{
        //    return str;
        //}
        //}

        /// <summary>
        /// 格式化json字符串
        /// </summary>
        /// <returns></returns>
        public static string JsonFormat(string json)
        {
            try
            {
                //JObject.ToString()方法会内部调用格式化，所以直接使用即可
                //判读是数组还是对象
                if (json.StartsWith("["))
                {
                    JArray jobj = JArray.Parse(json.Trim());
                    return jobj.ToString();
                }
                else if (json.StartsWith("{"))
                {
                    JObject jobj = JObject.Parse(json.Trim());
                    return jobj.ToString();
                }

            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            return json;
        }
        public static string JsonFormat(JsonData jsonData)
        {
            return JsonFormat(jsonData.ToJson());
        }

        /// <summary>
        /// 修改路径中的\，使用/
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string ResetPathSlash(string path)
        {
            string _path = path.Replace("\\\\", "/");
            _path = _path.Replace("\\", "/");
            _path = _path.Replace("//", "/");
            return _path;
        }
    }
}
