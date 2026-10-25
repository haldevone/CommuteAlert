using CommuteAlert.Web.Data;
using CommuteAlert.Web.Models;
using CommuteAlert.Web.Sl;
using CommuteAlert.Web.Sl.Models;
using CommuteAlert.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CommuteAlert.Web.Controllers;

[Authorize]
public class WatchesController : Controller
{

    private const int MaxWatchesPerUser = 6;

    private readonly ApplicationDbContext _db;
    private readonly SlSiteService _sites;
    private readonly ISlApiService _sl;

    public WatchesController(ApplicationDbContext db, SlSiteService sites, ISlApiService sl)
    {
        _db = db;
        _sites = sites;
        _sl = sl;
    }

    private string GetUserId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier)!;
    }

    public async Task<IActionResult> Index()
    {
        var watches = await _db.Watches
            .Where(w => w.UserId == GetUserId())
            .OrderBy(w => w.SiteName)
            .ToListAsync();

        return View(watches);
    }

    public async Task<IActionResult> Search(string? query)
    {
        var model = new WatchSearchViewModel { Query = query };
        if (!string.IsNullOrWhiteSpace(query))
        {
            try
            {
                model.Results = await _sites.SearchSites(query);
            }catch (HttpRequestException)
            {
                model.Error = "Kunde inte nå SL. Försök igen senare.";
            }
        }
        return View(model);
    }

    public async Task<IActionResult> Create(int siteId)
    {
        try
        {
            var site = await _sites.GetSiteById(siteId);

            if (site == null) return RedirectToAction(nameof(Search));

            var model = new WatchCreateViewModel
            {
                SiteId = site.Id,
                SiteName = site.Name,
                SelectedDays = new List<WatchDays>
                {
                    WatchDays.Monday, WatchDays.Tuesday, WatchDays.Wednesday,
                    WatchDays.Thursday, WatchDays.Friday
                },

                TargetOptions = await LoadTargetOptions(siteId)
            };

            return View(model);
        }
        catch (HttpRequestException)
        {
            TempData["Message"] = "Kunde inte nå SL. Försök igen senare.";
            return RedirectToAction(nameof(Search));
        }
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(WatchCreateViewModel model)
    {
        SlSite? site;

        try
        {
            site = await _sites.GetSiteById(model.SiteId);
        }
        catch (HttpRequestException)
        {
            TempData["Message"] = "Kunde inte nå SL. Försök igen senare.";
            return RedirectToAction(nameof(Search));
        }

        if (site == null) return RedirectToAction(nameof(Search));
        
        model.SiteName = site.Name;

        if (await _db.Watches.CountAsync(w => w.UserId == GetUserId()) >= MaxWatchesPerUser)
            ModelState.AddModelError("", $"Du kan ha som mest {MaxWatchesPerUser} bevakningar.");

        if (!TryParseTarget(model.Target, out var mode, out var lineId, out var directionCode, out var label))
            ModelState.AddModelError(nameof(model.Target), "Välj vad du vill bevakas.");

        if (model.SelectedDays.Count == 0)
            ModelState.AddModelError(nameof(model.SelectedDays), "Välj minst en dag.");

        if (model.EndTime <= model.StartTime)
            ModelState.AddModelError(nameof(model.EndTime),
                "Sluttiden måste vara efter starttiden. Tidsfönster över midnatt stöds inte.");


        if (!ModelState.IsValid)
        {
            try
            {
                model.TargetOptions = await LoadTargetOptions(model.SiteId);
            }
            catch (HttpRequestException)
            {
                TempData["Message"] = "Kunde inte nå SL. Försök igen senare.";
                return RedirectToAction(nameof(Search));
            }
            return View(model);
        }

        _db.Watches.Add(new Watch
        {
            UserId = GetUserId(),
            SiteId = site.Id,
            SiteName = site.Name,
            TargetName = label,
            TransportMode = mode,
            LineId = lineId,
            DirectionCode = directionCode,
            Days = model.SelectedDays.Aggregate(WatchDays.None, (all, day) => all | day),
            StartTime = model.StartTime,
            EndTime = model.EndTime,
            DelayThresholdMinutes = model.DelayThresholdMinutes
        });
        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var watch = await _db.Watches.SingleOrDefaultAsync(w => w.Id == id && w.UserId == GetUserId());
        if (watch == null) return NotFound();

        _db.Watches.Remove(watch);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Toggle(int id)
    {
        var watch = await _db.Watches.SingleOrDefaultAsync(w => w.Id == id && w.UserId == GetUserId());
        if (watch == null) return NotFound();

        watch.IsActive = !watch.IsActive;
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }


    //Skapar en valbara alternativ för en station
    private async Task<List<SelectListItem>> LoadTargetOptions(int siteId)
    {
        var departures = await _sl.GetDeparturesAsync(siteId);
        var options = new List<SelectListItem> { new("Allt vid denna station", "all") };

        foreach (var mode in departures.Select(d => d.Line.TransportMode).Distinct().Order())
            options.Add(new($"Alla {SlModeNames.Plural(mode)}", $"mode|{mode}"));

        foreach (var group in departures.GroupBy(d => (d.Line.Id, d.DirectionCode)))
        {
            var first = group.First();
            var label = $"{SlModeNames.Singular(first.Line.TransportMode)} {first.Line.Designation} till {first.Destination}";
            options.Add(new(label, $"line|{first.Line.Id}|{first.DirectionCode}|{label}"));
        }

        return options;
    }

    //Godkänner bara kända format. Omvandlar det till separata värden som används till skapande av bevakning. 
    private static bool TryParseTarget(string? target, out string? mode, out int? lineId,
    out int? directionCode, out string label)
    {
        mode = null;
        lineId = null;
        directionCode = null;
        label = "Allt vid denna station";

        var parts = (target ?? "").Split('|');
        switch (parts[0])
        {
            case "all" when parts.Length == 1:
                return true;

            case "mode" when parts.Length == 2
                && parts[1].Length is > 0 and <= 20
                && parts[1].All(char.IsAsciiLetterUpper):
                mode = parts[1];
                label = $"Alla {SlModeNames.Plural(mode)}";
                return true;

            case "line" when parts.Length == 4
                && int.TryParse(parts[1], out var line)
                && int.TryParse(parts[2], out var direction):
                lineId = line;
                directionCode = direction;
                label = parts[3].Length > 100 ? parts[3][..100] : parts[3];
                return true;

            default:
                return false;
        }
    }

}
