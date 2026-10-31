using CommuteAlert.Web.Data;
using CommuteAlert.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace CommuteAlert.Web.Controllers;


[Authorize]
public class AlertsController : Controller
{

    private readonly ApplicationDbContext _db;

    public AlertsController(ApplicationDbContext db)
    {
        _db = db;
    }

    private string GetUserId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier)!;
    }

    public async Task<IActionResult> Index()
    {
        var alerts = await _db.Alerts
            .Join(_db.Watches.Where(w => w.UserId == GetUserId()),
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
            .Take(50)
            .ToListAsync();

        return View(alerts);
    }
}
