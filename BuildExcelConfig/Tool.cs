using System.IO;
using System.Text;

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
    }
}
