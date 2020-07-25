using System.IO;

namespace COVID19.Data.Utilities.TimeSeries
{
    public class FileTimeSeriesConfiguration : StreamTimeSeriesReader
    {
        private readonly string _path;
        public FileTimeSeriesConfiguration(TimeSeriesConfiguration config, string path) : base(config)
        {
            _path = path;
        }

        public override StreamReader GetStream()
        {
            return new StreamReader(_path);
        }
    }
}