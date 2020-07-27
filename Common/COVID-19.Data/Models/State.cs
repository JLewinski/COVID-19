using System.Collections.Generic;
using System.Linq;

namespace COVID19.Data.Models
{
    public class State
    {
        public bool IsSet { get; private set; }
        public const string StartDate = "1/22/20";
        public string Name { get; set; }
        private int? _population = null;
        public int Population
        {
            get
            {
                if (_population == null && Children.Any())
                {
                    _population = Children.Sum(x => x.Value.Population);
                }
                return _population.Value;
            }
            set
            {
                _population = value;
                IsSet = true;
            }
        }
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
                    if (_newData == null)
                    {
                        if (Children.Any())
                        {
                            _totalData = new int[Children.Values.Min(x => x.TotalData.Length)];
                            for(int i = 0; i < _totalData.Length; i++)
                            {
                                _totalData[i] = Children.Values.Sum(x => x.TotalData[i]);
                            }
                        }
                    }
                    else
                    {
                        _totalData = new int[NewData.Length];
                        for (int i = 1; i < NewData.Length; i++)
                        {
                            _totalData[i] = NewData[i] + NewData[i - 1];
                        }
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
                    if(_totalData == null)
                    {
                        if (Children.Any())
                        {
                            _newData = new int[Children.Values.Min(x => x.NewData.Length)];
                            for (int i = 0; i < _totalData.Length; i++)
                            {
                                _newData[i] = Children.Values.Sum(x => x.NewData[i]);
                            }
                        }
                    }
                    else
                    {
                        _newData = new int[TotalData.Length];
                        for (int i = TotalData.Length - 1; i > 0; i--)
                        {
                            _newData[i] = TotalData[i] - TotalData[i - 1];
                            if (_newData[i] < 0) _newData[i] = 0;
                        }
                    }
                }
                return _newData;
            }
            set
            {
                _newData = value;
            }
        }

        public Dictionary<string, State> Children { get; set; } = new Dictionary<string, State>();
    }
}