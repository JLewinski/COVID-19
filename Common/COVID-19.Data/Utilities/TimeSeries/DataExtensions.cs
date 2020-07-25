using System.Collections.Generic;
using System.Linq;
using COVID19.Data.Models;

namespace COVID19.Data.Utilities
{
    public static class DataExtensions
    {
        public static State CombineStates(this State stateA, State stateB)
        {
            if (stateA.TotalData.Length != stateB.TotalData.Length)
            {
                return stateA;
            }

            stateA.Latitude = (stateA.Latitude + stateB.Latitude) / 2;
            stateA.Longitude = (stateA.Longitude + stateB.Longitude) / 2;
            stateA.Population = (stateA.Population + stateB.Population) / 2;
            for (int i = 0; i < stateA.TotalData.Length; i++)
            {
                stateA.TotalData[i] = (stateA.TotalData[i] + stateB.TotalData[i]) / 2;
            }
            stateA.NewData = null;

            return stateA;
        }

        public static Dictionary<string, Country> CombineCountries(this Dictionary<string, Country> a, Dictionary<string, Country> b)
        {
            var countryDictionary = a;
            foreach (var countryB in b.Values)
            {
                if (countryDictionary.TryGetValue(countryB.Name, out var countryA))
                {
                    foreach (var stateB in countryB.States)
                    {
                        if (countryA.States.TryGetValue(stateB.Key, out var stateA))
                        {
                            countryA.States[stateB.Key] = stateA.CombineStates(stateB.Value);
                        }
                        else
                        {
                            countryA.States.Add(stateB.Key, stateB.Value);
                        }
                    }
                    countryDictionary[countryA.Name] = countryA;
                }
                else
                {
                    countryDictionary.Add(countryB.Name, countryB);
                }
            }
            return countryDictionary;
        }
    }
}