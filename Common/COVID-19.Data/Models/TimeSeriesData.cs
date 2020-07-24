using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COVID_19.Data.Models
{
    public class TimeSeriesData
    {
        public const string StartDate = "01/22/2020";

        public string Key { get; set; }
        public int Population { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int[] Data { get; set; }
        
        //possible reference to TimeSeriesLibrary?
    }
}
