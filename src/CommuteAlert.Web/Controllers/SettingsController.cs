using CommuteAlert.Web.Data;
using CommuteAlert.Web.Models;
using CommuteAlert.Web.Notifications;
using CommuteAlert.Web.Security;
using CommuteAlert.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CommuteAlert.Web.Controllers;


[Authorize]
public class SettingsController : Controller
{

    private readonly ApplicationDbContext _db;
    private readonly WebhookProtector _webhookProtector;
    private readonly INotifier _notifier;

    public SettingsController(ApplicationDbContext db, WebhookProtector webhookProtector, INotifier notifier)
    {
        _db = db;
        _webhookProtector = webhookProtector;
        _notifier = notifier;
    }

    private string GetUserId()
    {
       return User.FindFirstValue(ClaimTypes.NameIdentifier)!;
    }

    public async Task<IActionResult> Index()
    {
        //Hämta användarens webhook från databasen och skicka den till vyn
        var hasWebhook = await _db.Webhooks.AnyAsync(u => u.UserId == GetUserId());
        return View(new WebhookViewModel { HasWebhook = hasWebhook });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Save(WebhookViewModel model)
    {
        if(!ModelState.IsValid || !WebhookValidator.IsValid(model.Url))
        {
            ModelState.AddModelError(nameof(model.Url), "Enter a valid Discord webhook address.");
            model.Url = null;
            model.HasWebhook = await _db.Webhooks.AnyAsync(u => u.UserId == GetUserId());
            return View("Index", model);
        }

        var encryptedUrl = _webhookProtector.Protect(model.Url!);
        var existingWebhook = await _db.Webhooks.SingleOrDefaultAsync(u => u.UserId == GetUserId());

        if(existingWebhook == null)
        {
            _db.Webhooks.Add(new Webhook
            {
                UserId = GetUserId(),
                EncryptedUrl = encryptedUrl,
                CreatedAt = DateTime.UtcNow
            });
        }
        else
        {
            existingWebhook.EncryptedUrl = encryptedUrl;
        }

        await _db.SaveChangesAsync();
        TempData["Message"] = "Webhook saved successfully.";
        return RedirectToAction("Index");
    }


    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> SendTest()
    {
        var webhook = await _db.Webhooks.SingleOrDefaultAsync(w => w.UserId == GetUserId());
        if (webhook == null)
        {
            TempData["Message"] = "Save a webhook first.";
            return RedirectToAction(nameof(Index));
        }

        var ok = await _notifier.SendAsync(_webhookProtector.Unprotect(webhook.EncryptedUrl),
            "Test message from CommuteAlert");
        TempData["Message"] = ok ? "Test message sent." : "Could not send the test message.";
        return RedirectToAction(nameof(Index));
    }

}
