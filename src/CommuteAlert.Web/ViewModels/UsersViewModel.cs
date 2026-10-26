using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace CommuteAlert.Web.ViewModels;

public class UsersViewModel
{
    [Required, EmailAddress]
    public string? Email { get; set; }

    [Required]
    public string? Password { get; set; }

    public List<IdentityUser> Users { get; set; } = new();
    public string? CurrentUserId { get; set; }
}
