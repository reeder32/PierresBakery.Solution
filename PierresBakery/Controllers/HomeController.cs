using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PierresBakery.Models;

namespace PierresBakery.Controllers
{
    public class HomeController : Controller
    {
        private readonly PierresBakeryContext _db;
        public HomeController(PierresBakeryContext db)
        {
            _db = db;
        }

    public ActionResult Index()
{
      var model = new Dictionary<string, object>();
      List<Flavor> flavors = _db.Flavors.ToList();
      List<Treat> treats = _db.Treats.ToList();
      model.Add("treats", treats);
      model.Add("flavors", flavors);
      return View(model);
}
            
    }
}
