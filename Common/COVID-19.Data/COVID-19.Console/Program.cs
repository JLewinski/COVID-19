using System;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace COVID_19.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Console.WriteLine("Getting Countries!");
            var countries = COVID19.Data.Utilities.TimeSeries.WebTimeSeriesReader.GetDeathCases().Result;
            System.Console.WriteLine("Got Countries!");
            foreach (var country in countries.Values)
            {
                WriteState(country);
            }
            System.Console.ReadLine();
        }

        public static void WriteState(COVID19.Data.Models.State state, string offset = "")
        {
            System.Console.WriteLine($"{offset}{state.Key} ({state.Population})");
            if (state.Children.Any())
            {
                foreach (var child in state.Children.Values)
                {
                    WriteState(child, offset + "    ");
                }
            }
        }
    }
}
