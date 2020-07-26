namespace COVID19.Data.Models
{
    public class State
    {
        public const string StartDate = "1/22/20";
        public string Name { get; set; }
        public int Population { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Key { get; set; }

        private int[] _totalData;
        public int[] TotalData
        {
            get
            {
                if (_totalData == null)
                {
                    _totalData = new int[NewData.Length];
                    for (int i = 1; i < NewData.Length; i++)
                    {
                        _totalData[i] = NewData[i] + NewData[i - 1];
                    }
                }
                return _totalData;
            }
            set
            {
                _totalData = value;
            }
        }

        private int[] _newData;
        public int[] NewData
        {
            get
            {
                if (_newData == null)
                {
                    _newData = new int[TotalData.Length];
                    for (int i = TotalData.Length - 1; i > 0; i--)
                    {
                        _newData[i] = TotalData[i] - TotalData[i - 1];
                        if (_newData[i] < 0) _newData[i] = 0;
                    }
                }
                return _newData;
            }
            set
            {
                _newData = value;
            }
        }
    }
}