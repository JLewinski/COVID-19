using System.Collections.Generic;

namespace COVID19.Data.Models
{
    public class Country
    {
        public Country(string name)
        {
            Name = name;
            States = new Dictionary<string, State>();
        }
        public string Name { get; set; }
        public Dictionary<string, State> States { get; set; }
    }
}