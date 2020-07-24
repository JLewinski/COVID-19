using System;
using System.Collections.Generic;
using System.Text;

namespace COVID_19.Data.Models
{
    public class TimeSeriesLibrary
    {
        public DateTime[] Dates { get; set; }
        public Dictionary<string, TimeSeriesData[]> Data { get; set; }
    }
}
