using System.IO;
using System.Threading.Tasks;

namespace COVID19.Data.Utilities.TimeSeries
{
    public interface ITimeSeriesReader
    {
        StreamReader GetStream();
        Task<StreamReader> GetStreamAsync();
    }
}