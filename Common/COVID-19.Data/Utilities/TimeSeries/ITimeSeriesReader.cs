using System.IO;

namespace COVID19.Data.Utilities.TimeSeries
{
    public interface ITimeSeriesReader
    {
        StreamReader GetStream();
    }
}