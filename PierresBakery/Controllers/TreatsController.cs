using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using PierresBakery.Models;


namespace PierresBakery.Controllers
{
  [Authorize]
  public class TreatsController : Controller
  {
    private readonly PierresBakeryContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public TreatsController(UserManager<ApplicationUser> userManager, PierresBakeryContext db)
    {
      _db = db;
      _userManager = userManager;
    }

    private List<Treat> AllTreats() => _db.Treats.ToList();
    private async Task<bool> IsOwnerAsync(Treat t)
    {
      var user = await _userManager.GetUserAsync(User);
      if (user.Id == t.User.Id)
      {
        return true;
      }
      else
      {
        return false;
      }
    }
    private bool IsJoined(int treatId, int flavorId)
    {
      var joinedItem = _db.FlavorTreats.FirstOrDefault(ft => ft.FlavorId == flavorId && ft.TreatId == treatId);
      if (joinedItem != null)
      {
        return true;
      }
      else
      {
        return false;
      }
    }
    [AllowAnonymous]
    public ActionResult Index() => View(AllTreats());

    public ActionResult Create()
    {
    ViewBag.FlavorId = new SelectList(_db.Flavors, "FlavorId", "Name");
    return View();
    }

[HttpPost]
    public async Task<ActionResult> Create(Treat treat, int FlavorId)
    {
      var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var currentUser = await _userManager.FindByIdAsync(userId);
        treat.User = currentUser;
        _db.Treats.Add(treat);
        _db.SaveChanges();
      Console.WriteLine($"{treat.TreatId}", FlavorId);
      if (FlavorId != 0 && !IsJoined(treat.TreatId, FlavorId))
        {
            _db.FlavorTreats.Add(new FlavorTreat() { FlavorId = FlavorId, TreatId = treat.TreatId });
        }
        _db.SaveChanges();
        return RedirectToAction("Index");
    }
    public async Task<ActionResult> Details(int id)
    {
      var thisTreat = _db.Treats
      .Include(treat => treat.FlavorTreats)
      .ThenInclude(join => join.Flavor)
      .FirstOrDefault(treat => treat.TreatId == id);
      ViewBag.IsOwner =  await IsOwnerAsync(thisTreat);
      return View(thisTreat);
    }

    public async Task<ActionResult> Edit(int id)
    {
      var thisTreat = _db.Treats.FirstOrDefault(treat => treat.TreatId == id);
      ViewBag.FlavorId = new SelectList(_db.Flavors, "FlavorId", "Name");
      var isOwnerAsync = await IsOwnerAsync(thisTreat);
      if (isOwnerAsync)
      {
        return View(thisTreat);
      }
      else
      {
        return RedirectToAction("Details", new { id = thisTreat.TreatId });
      }
    }

    [HttpPost]
    public ActionResult Edit(Treat treat, int FlavorId)
    {
      if (FlavorId != 0 && !IsJoined(treat.TreatId, FlavorId))
      {
        _db.FlavorTreats.Add(new FlavorTreat() { TreatId = treat.TreatId, FlavorId = FlavorId });
      }
      _db.Entry(treat).State = EntityState.Modified;
      _db.SaveChanges();
      return RedirectToAction("Index");
    }
  
    public async Task<ActionResult> Delete(int id)
    {
      var thisTreat = _db.Treats.FirstOrDefault(Treat => Treat.TreatId == id);
      if (await IsOwnerAsync(thisTreat))
      {
        return View(thisTreat);
      }
      else
      {
        return RedirectToAction("Details", new { id = thisTreat.TreatId });
      }
      
    }

    [HttpPost, ActionName("Delete")]
    public ActionResult DeleteConfirmed(int id)
    {
      var thisFlavor = _db.Flavors.FirstOrDefault(Flavor => Flavor.FlavorId == id);
      _db.Flavors.Remove(thisFlavor);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }
  }
}