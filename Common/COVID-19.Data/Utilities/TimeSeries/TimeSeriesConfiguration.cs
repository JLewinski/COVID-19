namespace COVID19.Data.Utilities.TimeSeries
{
    public class TimeSeriesConfiguration
    {
        public int CountryIndex { get; set; }
        public int StateIndex { get; set; }
        public int DataStartIndex { get; set; }
        public int PopulationIndex { get; set; } = -1;
        public int KeyIndex { get; set; } = -1;
        public int LongitudeIndex { get; set; }
        public int LatitudeIndex { get; set; }
    }
}