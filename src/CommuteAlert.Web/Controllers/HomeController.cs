using CommuteAlert.Web.Data;
using CommuteAlert.Web.Models;
using CommuteAlert.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;

namespace CommuteAlert.Web.Controllers;

public class HomeController : Controller
{

    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _db;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext db)
    {
        _logger = logger;
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        var model = new HomeViewModel();

        if (User.Identity?.IsAuthenticated != true) return View(model);

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        model.SignedIn = true;
        model.ActiveWatches = await _db.Watches.CountAsync(w => w.UserId == userId && w.IsActive);
        model.PausedWatches = await _db.Watches.CountAsync(w => w.UserId == userId && !w.IsActive);
        model.HasWebhook = await _db.Webhooks.AnyAsync(w => w.UserId == userId);
        model.RecentAlerts = await _db.Alerts
        .Join(_db.Watches.Where(w => w.UserId == userId),
            a => a.WatchId, w => w.Id,
            (a, w) => new AlertHistoryItem
            {
                CreatedAt = a.CreatedAt,
                SiteName = w.SiteName,
                Type = a.Type,
                Message = a.Message,
                Sent = a.SentAt != null
            })
        .OrderByDescending(i => i.CreatedAt)
        .Take(3)
        .ToListAsync();

        return View(model);
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
}
