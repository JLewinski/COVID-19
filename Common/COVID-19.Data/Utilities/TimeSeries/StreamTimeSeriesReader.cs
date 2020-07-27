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

        public async Task<Dictionary<string, State>> GetCountriesAsync()
        {
            var countryDictionary = new Dictionary<string, State>();
            using (var streamReader = await GetStreamAsync())
            {
                var line = await streamReader.ReadLineAsync();
                while ((line = await streamReader.ReadLineAsync()) != null)
                {
                    var data = line.Split('"');
                    for (int i = 0; i < data.Length; i++)
                    {
                        if (i % 2 == 1)
                        {
                            data[i] = $"{data[i].Replace(',', ';')}";
                        }
                    }
                    line = data.Aggregate((x, y) => x + y);
                    data = line.Split(',');

                    var countryName = data[_config.CountryIndex].Replace(';', ',');
                    if (!countryDictionary.TryGetValue(countryName, out var country))
                    {
                        country = new State { Name = countryName };
                        countryDictionary.Add(countryName, country);
                    }

                    string stateName = data[_config.StateIndex];
                    var exists = country.Children.TryGetValue(stateName, out var state);
                    var tempState = GetState(data, state ?? new State { Name = stateName, Key = $"{stateName}, {countryName}" });

                    if (tempState.Name == string.Empty)
                    {
                        if (country.IsSet)
                        {
                            throw new System.Exception("The country was already manually set but it is trying to be set again");
                        }

                        tempState.Name = countryName;
                        tempState.Children = country.Children;
                        countryDictionary[countryName] = tempState;
                    }
                    else if (!exists)
                    {
                        country.Children.Add(tempState.Name, tempState);
                    }
                    else
                    {
                        country.Children[tempState.Name] = tempState;
                    }
                }
            }
            return countryDictionary;
        }

        public State GetState(string[] data, State state)
        {
            if (_config.CityIndex < 0)
            {
                if (state.IsSet == true)
                {
                    throw new System.Exception("State was already manually set and is being set again");
                }
                else
                {
                    var temp = GetData(data, _config.StateIndex);
                    temp.Children = state.Children;
                    return temp;
                }
            }

            var city = GetData(data, _config.CityIndex);
            if (city.Name == string.Empty) //The city isn't really a city (it's a state)
            {
                city.Name = state.Name;
                city.Children = state.Children;
                return city;
            }
            else if (state.Children.ContainsKey(city.Name))
            {
                throw new System.Exception("State already contains the city");
            }
            else
            {
                state.Children.Add(city.Name, city);
                return state;
            }
        }

        public State GetData(string[] data, int nameIndex)
        {
            return new State
            {
                Key = _config.KeyIndex >= 0
                    ? data[_config.KeyIndex].Replace(';', ',')
                    : _config.CityIndex >= 0
                        ? $"{data[_config.CityIndex]}, {data[_config.StateIndex]}, {data[_config.CountryIndex]}"
                        : $"{data[_config.StateIndex]}, {data[_config.CountryIndex]}",
                Latitude = double.Parse(data[_config.LatitudeIndex]),
                Longitude = double.Parse(data[_config.LongitudeIndex]),
                Name = data[nameIndex].Replace(';', ','),
                Population = _config.PopulationIndex >= 0 ? int.Parse(data[_config.PopulationIndex]) : 0,
                TotalData = data.Skip(_config.DataStartIndex).Select(x => int.Parse(x)).ToArray()
            };
        }

        public abstract StreamReader GetStream();
        public abstract Task<StreamReader> GetStreamAsync();
    }
}