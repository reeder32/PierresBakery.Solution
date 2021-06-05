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
  public class FlavorsController : Controller
  {
    private readonly PierresBakeryContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public FlavorsController(UserManager<ApplicationUser> userManager, PierresBakeryContext db)
    {
      _db = db;
      _userManager = userManager;
    }

    private List<Flavor> AllFlavors() => _db.Flavors.ToList();
  private async Task<bool?> IsOwnerAsync(Flavor f) 
  {
      if (User.Identity.IsAuthenticated)
      {
        var user = await _userManager.GetUserAsync(User);
        if (user.Id == f.User.Id)
        {
          return true;
        }
        else
        {
          return false;
        }
      }
      else
      {
        return false;
      }
    }
    [AllowAnonymous]
    public ActionResult Index() => View(AllFlavors());

    public ActionResult Create() => View();
    [HttpPost]
    public async Task<ActionResult> Create(Flavor flavor)
    {
      var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var currentUser = await _userManager.FindByIdAsync(userId);
        flavor.User = currentUser;
        _db.Flavors.Add(flavor);
        _db.SaveChanges();
        return RedirectToAction("Index");
    }

    [AllowAnonymous]
    public async Task<ActionResult> Details(int id)
    {
      var thisFlavor = _db.Flavors
      .Include(flavor => flavor.FlavorTreats)
      .ThenInclude(join => join.Treat)
      .FirstOrDefault(flavor => flavor.FlavorId == id);
      Console.WriteLine(thisFlavor.Name);
      ViewBag.IsOwner =  await IsOwnerAsync(thisFlavor);
      return View(thisFlavor);
    }

    public async Task<ActionResult> Edit(int id)
    {
      var thisFlavor = _db.Flavors.FirstOrDefault(Flavor => Flavor.FlavorId == id);
      ViewBag.TreatId = new SelectList(_db.Treats, "TreatId", "Name");
      var isOwnerAsync = await IsOwnerAsync(thisFlavor);
      if ((bool)isOwnerAsync)
      {
        return RedirectToAction("Details", new { id = thisFlavor.FlavorId });
      }
      else
      {
        return View(thisFlavor);
      }
    }

    [HttpPost]
    public async Task<ActionResult> Edit(Flavor Flavor, int TreatId, string UserId)
    {
      if (TreatId != 0)
      {
        _db.FlavorTreats.Add(new FlavorTreat() { TreatId = TreatId, FlavorId = Flavor.FlavorId });
      }
      var currentUser = await _userManager.FindByIdAsync(UserId);
      Flavor.User = currentUser;
      _db.Entry(Flavor).State = EntityState.Modified;
      _db.SaveChanges();
      return RedirectToAction("Index");
    }
  
    public async Task<ActionResult> Delete(int id)
    {
      var thisFlavor = _db.Flavors.FirstOrDefault(f => f.FlavorId == id);
      if ((bool)await IsOwnerAsync(thisFlavor))
      {
        return View(thisFlavor);
      }
      else
      {
        return RedirectToAction("Details", new { id = thisFlavor.FlavorId });
      }
      
    }

    [HttpPost, ActionName("Delete")]
    public ActionResult DeleteConfirmed(int id)
    {
      var thisFlavor = _db.Flavors.FirstOrDefault(f => f.FlavorId == id);
      _db.Flavors.Remove(thisFlavor);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }
  }
}