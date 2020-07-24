using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using COVID_19.MVC.Models;
using Microsoft.VisualBasic;

namespace COVID_19.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public async Task<IActionResult> GetAllData()
        {
            Data.Models.TimeSeriesLibrary library;
            bool success = false;
            string errorMessage = "NA";
            try
            {
                library = await Data.Utilities.TimeSeriesReader.ReadData();
                success = true;
            }
            catch (Exception e)
            {
                library = null;
                success = false;
                errorMessage = e.Message;
            }

            return Json(new { success, library, errorMessage });
        }

        public async Task<IActionResult> GetStateData(string state, string country = "US")
        {
            try
            {
                if(country == null || country == "undefined")
                {
                    throw new ArgumentNullException("Country");
                }
                var confirmedCases = await Data.Utilities.TimeSeriesReader.ReadStateChanges(state, country, true);
                var deaths = await Data.Utilities.TimeSeriesReader.ReadStateChanges(state, country, false);
                var dates = new string[deaths.Data.Length];
                var date = DateTime.Parse(Data.Models.TimeSeriesData.StartDate);
                for(int i = 0; i < deaths.Data.Length; i++)
                {
                    dates[i] = date.ToShortDateString();
                    date = date.AddDays(1);
                }

                return Json(new { success = true, confirmedCases, deaths, dates});
            }
            catch (Exception e)
            {
                return Json(new { success = false, errorMessage = e.Message });
            }

        }

        public async Task<IActionResult> GetStateList(string country = "US")
        {
            try
            {
                return Json(new { success = true, states = await Data.Utilities.TimeSeriesReader.GetStateNamesArray(country) });
            }
            catch(Exception e)
            {
                return Json(new { success = false, errorMessage = e.Message });
            }
        }

        public async Task<IActionResult> GetCountryList()
        {
            try
            {
                var countries = await Data.Utilities.TimeSeriesReader.GetCountryNamesArray();
                return Json(new { success = true, countries });
            }
            catch (Exception e)
            {
                return Json(new { success = false, errorMessage = e.Message });
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
