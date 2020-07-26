using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COVID19.Data.Utilities.TimeSeries
{
    public class DummyTimeSeriesReader : StreamTimeSeriesReader
    {
        private readonly Stream _stream;
        public DummyTimeSeriesReader(TimeSeriesConfiguration config, Stream stream) : base(config)
        {
            _stream = stream;
        }
        public override StreamReader GetStream()
        {
            return new StreamReader(_stream);
        }

        public override Task<StreamReader> GetStreamAsync()
        {
            return Task.Run(() => GetStream());
        }
    }
}
