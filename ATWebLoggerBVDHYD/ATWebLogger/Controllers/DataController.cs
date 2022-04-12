using ATWebLogger.Core;
using ATWebLogger.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ATWebLogger.Controllers
{
    public class DataController : Controller
    {
        public static WebLogger WebLogger { get; set; } = new WebLogger();

        // GET: Data
        public ActionResult Index()
        {
            return View();
        }
    }
}