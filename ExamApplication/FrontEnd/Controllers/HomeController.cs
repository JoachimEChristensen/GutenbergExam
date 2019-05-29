using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FrontEnd.Models;
using DataClasses;
using MoreLinq;

namespace FrontEnd.Controllers
{
    public class HomeController : Controller
    {
        private readonly QueriesSql _queriesSql = new QueriesSql();
        private readonly QueriesMongoDb _queriesMongoDb = new QueriesMongoDb();

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult CitySql()
        {
            return View(new List<Book>());
        }

        public IActionResult CityMongoDb()
        {
            return View(new List<Book>());
        }

        public IActionResult BookTitleSql()
        {
            return View(new List<City>());
        }

        public IActionResult BookTitleMongoDb()
        {
            return View(new List<City>());
        }

        public IActionResult BookAuthorSql()
        {
            return View(new List<Book>());
        }

        public IActionResult BookAuthorMongoDb()
        {
            return View(new List<Book>());
        }

        public IActionResult CityGeoSql()
        {
            return View(new List<Book>());
        }

        public IActionResult CityGeoMongoDb()
        {
            return View(new List<Book>());
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public IActionResult CitySql(string cityName)
        {
            return View(_queriesSql.City(cityName));
        }

        [HttpPost]
        public IActionResult CityMongoDb(string cityName)
        {
            return View(_queriesMongoDb.City(cityName));
        }

        [HttpPost]
        public IActionResult BookTitleSql(string bookTitle)
        {
            return View(_queriesSql.BookTitle(bookTitle));
        }

        [HttpPost]
        public IActionResult BookTitleMongoDb(string bookTitle)
        {
            return View(_queriesMongoDb.BookTitle(bookTitle));
        }

        [HttpPost]
        public IActionResult BookAuthorSql(string bookAuthor)
        {
            (List<City>, List<Book>) temp = _queriesSql.BookAuthor(bookAuthor);
            ViewBag.cities = temp.Item1;
            return View(temp.Item2.DistinctBy(b => b.Title));
        }

        [HttpPost]
        public IActionResult BookAuthorMongoDb(string bookAuthor)
        {
            (List<City>, List<Book>) temp = _queriesMongoDb.BookAuthor(bookAuthor);
            ViewBag.cities = temp.Item1;
            return View(temp.Item2.DistinctBy(b => b.Title));
        }

        [HttpPost]
        public IActionResult CityGeoSql(string cityLatitude, string cityLongitude)
        {
            decimal.TryParse(cityLatitude, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal latitude);
            decimal.TryParse(cityLongitude, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal longitude);
            return View(_queriesSql.CityGeo(latitude, longitude));
        }

        [HttpPost]
        public IActionResult CityGeoMongoDb(string cityLatitude, string cityLongitude)
        {
            decimal.TryParse(cityLatitude, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal latitude);
            decimal.TryParse(cityLongitude, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal longitude);
            return View(_queriesMongoDb.CityGeo(latitude, longitude));
        }
    }
}
