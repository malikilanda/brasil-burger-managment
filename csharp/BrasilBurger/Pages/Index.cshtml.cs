using BrasilBurger.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BrasilBurger.Pages;

public class IndexModel : PageModel
{
    private readonly AppDbContext _db;
    public IndexModel(AppDbContext db) => _db = db;

    [BindProperty] public string Phone { get; set; } = "";
    [BindProperty] public string Password { get; set; } = "";

    public string ErrorMessage { get; set; } = "";

    public async Task<IActionResult> OnGet()
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToPage("/Client/acceuil");

        return Page();
    }

    public async Task<IActionResult> OnPost()
    {
        var phone = (Phone ?? "").Trim();
        var pass = (Password ?? "").Trim();

        if (string.IsNullOrWhiteSpace(phone) || string.IsNullOrWhiteSpace(pass))
        {
            ErrorMessage = "Veuillez remplir tous les champs.";
            return Page();
        }

        // LOGIN: phone + password (plus tard on fera du hash)
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Phone == phone);

        if (user is null || user.Password != pass)
        {
            ErrorMessage = "Téléphone ou mot de passe incorrect.";
            return Page();
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, $"{user.Firstname} {user.Lastname}"),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity),
            new AuthenticationProperties
            {
                IsPersistent = false,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1)
            });

        return RedirectToPage("/Client/acceuil");
    }
}
