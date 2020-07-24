using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using COVID_19.MVC.Models;

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

        public async Task<IActionResult> GetStateData(string state)
        {
            try
            {
                var confirmedCases = await Data.Utilities.TimeSeriesReader.ReadStateChanges(state, true);
                var deaths = await Data.Utilities.TimeSeriesReader.ReadStateChanges(state, false);
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
