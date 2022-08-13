using System.IO;
using System.Text;

namespace BuildExcelConfig
{
    internal class CsvWrite
    {
        FileStream fs;
        StreamWriter sw;
        StringBuilder config;
        public CsvWrite(string fileName)
        {
            string csvPath = Config.writeCsvPath + "ConfigAssetCsv/";
            if (!Directory.Exists(csvPath))
                Directory.CreateDirectory(csvPath);
            fs = new FileStream(csvPath + fileName + "Config.csv", FileMode.Create);
            sw = new StreamWriter(fs);
            config = new StringBuilder();
        }

        public void Append(string content)
        {
            config.Append(content);
        }
        public void SaveCsv()
        {
            sw.Write(config.ToString());
            sw.Flush();
            sw.Close();
            fs.Close();
        }
    }
}
