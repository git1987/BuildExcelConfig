
using System.IO;

namespace BuildExcelConfig
{
    internal class DataWrite
    {
        protected FileStream fs;
        protected StreamWriter sw;
        protected void Init(string filePath, string fileName)
        {
            if (!Directory.Exists(filePath))
                Directory.CreateDirectory(filePath);
            fs = new FileStream(filePath + fileName, FileMode.Create);
            sw = new StreamWriter(fs, System.Text.Encoding.UTF8);
        }
        public virtual void Save()
        {
            sw.Flush();
            sw.Close();
            fs.Close();
        }

    }
}
