using COVID19.Data.Models;
using COVID19.Data.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace COVID19.Data.Utilities.TimeSeries
{
    public class HttpTimeSeriesReader : StreamTimeSeriesReader
    {
        public static readonly ReadOnlyCollection<(string url, TimeSeriesConfiguration config)> Configurations = new ReadOnlyCollection<(string url, TimeSeriesConfiguration config)>(new[]{
            (ConfirmedUS, new TimeSeriesConfiguration
            {
                CountryIndex = 7,
                StateIndex = 6,
                CityIndex = 5,
                DataStartIndex = 11,
                KeyIndex = 10,
                LatitudeIndex = 8,
                LongitudeIndex = 9
            }),
            (ConfirmedGlobal, new TimeSeriesConfiguration
            {
                StateIndex = 0,
                CountryIndex = 1,
                LatitudeIndex = 2,
                LongitudeIndex = 3,
                DataStartIndex = 4
            }),
            (DeathsUS, new TimeSeriesConfiguration
            {
                CountryIndex = 7,
                StateIndex = 6,
                CityIndex = 5,
                DataStartIndex = 12,
                LatitudeIndex = 8,
                LongitudeIndex = 9,
                KeyIndex = 10,
                PopulationIndex = 11
            }),
            (DeathsGlobal, new TimeSeriesConfiguration
            {
                StateIndex = 0,
                CountryIndex = 1,
                LatitudeIndex = 2,
                LongitudeIndex = 3,
                DataStartIndex = 4
            })
        });

        public const string ConfirmedUS = "https://rawcdn.githack.com/CSSEGISandData/COVID-19/083598d406dbb5ce49684a5748d22046896bd62f/csse_covid_19_data/csse_covid_19_time_series/time_series_covid19_confirmed_US.csv";
        public const string ConfirmedGlobal = "https://rawcdn.githack.com/CSSEGISandData/COVID-19/083598d406dbb5ce49684a5748d22046896bd62f/csse_covid_19_data/csse_covid_19_time_series/time_series_covid19_confirmed_global.csv";
        public const string DeathsUS = "https://rawcdn.githack.com/CSSEGISandData/COVID-19/083598d406dbb5ce49684a5748d22046896bd62f/csse_covid_19_data/csse_covid_19_time_series/time_series_covid19_deaths_US.csv";
        public const string DeathsGlobal = "https://rawcdn.githack.com/CSSEGISandData/COVID-19/083598d406dbb5ce49684a5748d22046896bd62f/csse_covid_19_data/csse_covid_19_time_series/time_series_covid19_deaths_global.csv";

        private readonly string _path;
        private readonly HttpClient _client;

        public HttpTimeSeriesReader(TimeSeriesConfiguration config, string path, HttpClient client) : base(config)
        {
            _path = path;
            _client = client;
        }

        public static async Task<Dictionary<string, State>> GetConfirmedCases(HttpClient client)
        {
            return await Configurations.Take(2)
                .Select(x => new HttpTimeSeriesReader(x.config, x.url, client).GetCountriesAsync())
                .Aggregate(async (a, b) => DataExtensions.CombineCountries(await a, await b));
        }

        public static async Task<Dictionary<string, State>> GetDeathCases(HttpClient client)
        {
            return await Configurations.Skip(2)
                .Select(x => new HttpTimeSeriesReader(x.config, x.url, client).GetCountriesAsync())
                .Aggregate(async (a, b) => DataExtensions.CombineCountries(await a, await b));
        }

        public override StreamReader GetStream()
        {
            return GetStreamAsync().Result;
        }

        public override async Task<StreamReader> GetStreamAsync()
        {
            return new StreamReader(await _client.GetStreamAsync(_path));
        }
    }
}
