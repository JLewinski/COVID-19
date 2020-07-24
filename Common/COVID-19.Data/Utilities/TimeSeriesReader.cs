using COVID_19.Data.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace COVID_19.Data.Utilities
{
    public static class TimeSeriesReader
    {
        public static async Task<TimeSeriesData[]> ReadData(StreamReader stream, int startState, int startDate, int key, int population)
        {
            string line;
            var allData = new List<TimeSeriesData>();
            while ((line = await stream.ReadLineAsync()) != null)
            {
                var tempArray = line.Split('"');
                if (tempArray.Length > 1)
                {
                    string tempLine = string.Empty;
                    for (int i = 0; i < tempArray.Length; i++)
                    {
                        if (i % 2 == 0)
                        {
                            tempLine += tempArray[i];
                        }
                        else
                        {
                            tempLine += tempArray[i].Replace(',', ';');
                        }
                    }
                    line = tempLine;
                }

                var dataArray = line.Split(',');
                try
                {
                    var data = new TimeSeriesData
                    {
                        Key = key >= 0 ? dataArray[key] : "",
                        Population = population >= 0 ? int.Parse(dataArray[population]) : 0,
                        State = dataArray[startState],
                        Country = dataArray[startState + 1],
                        Latitude = double.Parse(dataArray[startState + 2]),
                        Longitude = double.Parse(dataArray[startState + 3]),
                        Data = dataArray.Skip(startDate).Select(x => int.Parse(x)).ToArray()
                    };
                    allData.Add(data);
                }
                catch
                {
                    //IGNORE
                }
            }
            return allData.ToArray();
        }

        public static async Task<TimeSeriesData[]> ReadData(string path, int startState, int startDate, int key = -1, int population = -1)
        {
            TimeSeriesData[] allData;
            using (var stream = new StreamReader(path))
            {
                //header
                var line = await stream.ReadLineAsync();
                allData = await ReadData(stream, startState, startDate, key, population);
            }

            return allData;
        }

        public static async Task<TimeSeriesLibrary> ReadData(string path, string label, int startState, int startDate, int key = -1, int population = -1)
        {
            TimeSeriesData[] allData;
            DateTime[] dates;
            using (var stream = new StreamReader(path))
            {
                //header
                var line = await stream.ReadLineAsync();
                var headerArray = line.Split(',');
                dates = headerArray.Skip(startDate).Select(x => DateTime.Parse(x)).ToArray();

                allData = await ReadData(stream, startState, startDate, key, population);
            }

            return new TimeSeriesLibrary
            {
                Data = new Dictionary<string, TimeSeriesData[]> { { label, allData } },
                Dates = dates
            };
        }

        private const string GlobalConfirmedPath = @"..\..\csse_covid_19_data\csse_covid_19_time_series\time_series_covid19_confirmed_global.csv";
        private const string USConfirmedPath = @"..\..\csse_covid_19_data\csse_covid_19_time_series\time_series_covid19_confirmed_US.csv";
        private const string GlobalDeathsPath = @"..\..\csse_covid_19_data\csse_covid_19_time_series\time_series_covid19_deaths_global.csv";
        private const string USDeathsPath = @"..\..\csse_covid_19_data\csse_covid_19_time_series\time_series_covid19_deaths_US.csv";

        public static async Task<TimeSeriesLibrary> ReadData()
        {
            var lib = await ReadData(GlobalConfirmedPath, "Confirmed Global", 0, 4);
            lib.Data.Add("Confirmed US", await ReadData(USConfirmedPath, 6, 11, 10));
            lib.Data.Add("Deaths Global", await ReadData(GlobalDeathsPath, 0, 4));
            lib.Data.Add("Deaths US", await ReadData(USDeathsPath, 6, 12, 10, 11));
            return lib;
        }

        public static Task<TimeSeriesData[]> ReadData(bool IsGlobal, bool IsConfirmedCases)
        {
            return IsGlobal ? ReadData(IsConfirmedCases ? GlobalConfirmedPath : GlobalDeathsPath, 0, 4)
                : IsConfirmedCases ? ReadData(USConfirmedPath, 6, 11, 10) : ReadData(USDeathsPath, 6, 12, 10, 11);
        }

        public static async Task<TimeSeriesData> ReadState(string state, bool IsConfirmedCases)
        {
            var allUS = await ReadData(false, IsConfirmedCases);

            return allUS.Where(x => x.State == state).Aggregate((x, y) =>
            {
                var data = new int[x.Data.Length];
                for (int i = 0; i < data.Length; i++)
                {
                    data[i] = x.Data[i] + y.Data[i];
                }
                return new TimeSeriesData
                {
                    Country = x.Country,
                    State = x.State,
                    Data = data,
                    Key = $"{x.State}, {x.Country}",
                    Latitude = (x.Latitude + y.Latitude) / 2,
                    Longitude = (x.Longitude + y.Longitude) / 2,
                    Population = x.Population + y.Population
                };
            });
        }

        public static async Task<TimeSeriesData> ReadStateChanges(string state, bool IsConfirmedCases)
        {
            var stateData = await ReadState(state, IsConfirmedCases);
            for(int i = stateData.Data.Length - 1; i > 0; i--)
            {
                stateData.Data[i] -= stateData.Data[i - 1];
                if(stateData.Data[i] < 0)
                {
                    stateData.Data[i] = 0;
                }
            }
            return stateData;
        }
    }
}
