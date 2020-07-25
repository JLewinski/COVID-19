using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using COVID19.Data.Models;

namespace COVID19.Data.Utilities.TimeSeries
{
    public class WebTimeSeriesReader : StreamTimeSeriesReader
    {
        public static readonly ReadOnlyCollection<(Uri url, TimeSeriesConfiguration config)> Configurations = new ReadOnlyCollection<(Uri url, TimeSeriesConfiguration config)>(new[]{
            (new Uri(ConfirmedUS), new TimeSeriesConfiguration
            {
                CountryIndex = 7,
                StateIndex = 6,
                DataStartIndex = 11,
                KeyIndex = 10,
                LatitudeIndex = 8,
                LongitudeIndex = 9
            }),
            (new Uri(ConfirmedGlobal), new TimeSeriesConfiguration
            {
                StateIndex = 0,
                CountryIndex = 1,
                LatitudeIndex = 2,
                LongitudeIndex = 3,
                DataStartIndex = 4
            }),
            (new Uri(DeathsUS), new TimeSeriesConfiguration
            {
                CountryIndex = 7,
                StateIndex = 6,
                DataStartIndex = 12,
                LatitudeIndex = 8,
                LongitudeIndex = 9,
                KeyIndex = 10,
                PopulationIndex = 11
            }),
            (new Uri(DeathsGlobal), new TimeSeriesConfiguration
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

        private readonly Uri _path;
        public WebTimeSeriesReader(TimeSeriesConfiguration config, Uri path) : base(config)
        {
            _path = path;
        }

        public WebTimeSeriesReader(TimeSeriesConfiguration config, string path) : this(config, new Uri(path))
        {
        }

        public static async Task<Dictionary<string, Country>> GetConfirmedCases()
        {
            return await Configurations.Take(2)
                .Select(x => new WebTimeSeriesReader(x.config, x.url).GetCountriesAsync())
                .Aggregate(async (a, b) => DataExtensions.CombineCountries(await a, await b));
        }

        public static async Task<Dictionary<string, Country>> GetDeathCases()
        {
            return await Configurations.Skip(2)
                .Select(x => new WebTimeSeriesReader(x.config, x.url).GetCountriesAsync())
                .Aggregate(async (a, b) => DataExtensions.CombineCountries(await a, await b));
        }

        public override StreamReader GetStream()
        {
            byte[] data;
            using (var client = new WebClient())
            {
                data = client.DownloadData(_path);
            }
            return new StreamReader(new MemoryStream(data));
        }
    }
}