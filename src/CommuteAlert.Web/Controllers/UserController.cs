using CommuteAlert.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CommuteAlert.Web.Controllers;

[Authorize(Policy = "Admin")]
public class UsersController : Controller
{
    private readonly UserManager<IdentityUser> _users;

    public UsersController(UserManager<IdentityUser> users)
    {
        _users = users;
    }

    public async Task<IActionResult> Index()
    {
        var model = new UsersViewModel();
        await Fill(model);
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(UsersViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = new IdentityUser { UserName = model.Email, Email = model.Email, EmailConfirmed = true };
            var result = await _users.CreateAsync(user, model.Password!);

            if (result.Succeeded)
            {
                TempData["Message"] = $"User {model.Email} created. Share the temporary password privately and ask them to change it.";
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);
        }

        model.Password = null;
        await Fill(model);
        return View("Index", model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string id)
    {
        if (id == _users.GetUserId(User)) return BadRequest();

        var user = await _users.FindByIdAsync(id);
        if (user == null) return NotFound();

        await _users.DeleteAsync(user);
        return RedirectToAction(nameof(Index));
    }

    private async Task Fill(UsersViewModel model)
    {
        model.Users = await _users.Users.OrderBy(u => u.Email).ToListAsync();
        model.CurrentUserId = _users.GetUserId(User);
    }
}