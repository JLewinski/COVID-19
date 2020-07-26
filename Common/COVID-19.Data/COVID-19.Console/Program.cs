using System;
using System.Threading.Tasks;

namespace COVID_19.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Console.WriteLine("Getting Countries!");
            var countries = COVID19.Data.Utilities.TimeSeries.WebTimeSeriesReader.GetConfirmedCases().Result;
            System.Console.WriteLine("Got Countries!");
            foreach (var country in countries.Values)
            {
                System.Console.WriteLine(country.Name + ":");
                foreach(var state in country.States.Values)
                {
                    System.Console.WriteLine($"     {state.Name}");
                }
            }
            System.Console.ReadLine();
        }
    }
}
