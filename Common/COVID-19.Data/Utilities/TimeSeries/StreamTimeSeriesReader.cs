using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using COVID19.Data.Models;

namespace COVID19.Data.Utilities.TimeSeries
{
    public abstract class StreamTimeSeriesReader : ITimeSeriesReader
    {
        private readonly TimeSeriesConfiguration _config;
        public StreamTimeSeriesReader(TimeSeriesConfiguration config)
        {
            _config = config;
        }

        public async Task<Dictionary<string, Country>> GetCountriesAsync()
        {
            var countryDictionary = new Dictionary<string, Country>();
            using (var streamReader = GetStream())
            {
                var line = await streamReader.ReadLineAsync();
                while ((line = await streamReader.ReadLineAsync()) != null)
                {
                    var data = line.Split('"');
                    for (int i = 0; i < data.Length; i++)
                    {
                        if (i % 2 == 1)
                        {
                            data[i] = data[i].Replace(',', ';');
                        }
                    }
                    line = data.Aggregate((x, y) => x + y);
                    data = line.Split(',');

                    var countryName = data[_config.CountryIndex].Replace(';', ',');
                    if (!countryDictionary.TryGetValue(countryName, out var country))
                    {
                        country = new Country(countryName);
                        countryDictionary.Add(countryName, country);
                    }

                    var stateName = data[_config.StateIndex];
                    if (!country.States.TryGetValue(stateName, out var state))
                    {
                        state = GetState(data);
                    }
                    else
                    {
                        var tempState = GetState(data);
                        state.Latitude = (state.Latitude + tempState.Latitude) / 2;
                        state.Longitude = (state.Longitude + tempState.Longitude) / 2;
                        state.Population += tempState.Population;
                        state.Key = $"{stateName}, {countryName}";
                        for (int i = 0; i < tempState.TotalData.Length; i++)
                        {
                            state.TotalData[i] += tempState.TotalData[i];
                        }
                    }
                }
            }
            return countryDictionary;
        }

        public State GetState(string[] data) => new State
        {
            Key = _config.KeyIndex >= 0 ? data[_config.KeyIndex].Replace(';', ',') : $"{data[_config.StateIndex]}, {data[_config.CountryIndex]}",
            Latitude = double.Parse(data[_config.LatitudeIndex]),
            Longitude = double.Parse(data[_config.LongitudeIndex]),
            Name = data[_config.StateIndex].Replace(';', ','),
            Population = _config.PopulationIndex >= 0 ? int.Parse(data[_config.PopulationIndex]) : 0,
            TotalData = data.Skip(_config.DataStartIndex).Select(x => int.Parse(x)).ToArray()
        };

        public abstract StreamReader GetStream();
    }
}