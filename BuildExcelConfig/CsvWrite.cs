using System.IO;
using System.Text;

namespace BuildExcelConfig
{
    internal class CsvWrite : DataWrite
    {
        StringBuilder config;
        public CsvWrite(string configName)
        {
            string csvPath = Config.writeDataPath + "ConfigAssetCsv/";
            Init(csvPath, configName + "Config.csv");
            config = new StringBuilder();
        }
        public CsvWrite(string configName, string path)
        {
            string csvPath = path;
            Init(csvPath, configName + ".csv");
            config = new StringBuilder();
        }

        public void Append(string content)
        {
            config.Append(content);
        }
        public override void Save()
        {
            sw.Write(config.ToString());
            base.Save();
        }
    }
}
