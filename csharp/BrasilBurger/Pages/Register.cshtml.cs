using BrasilBurger.Data;
using BrasilBurger.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BrasilBurger.Pages;

public class RegisterModel : PageModel
{
    private readonly AppDbContext _db;
    public RegisterModel(AppDbContext db) => _db = db;

    [BindProperty] public string Firstname { get; set; } = "";
    [BindProperty] public string Lastname { get; set; } = "";
    [BindProperty] public string Phone { get; set; } = "";
    [BindProperty] public string Password { get; set; } = "";

    public string ErrorMessage { get; set; } = "";
    public string SuccessMessage { get; set; } = "";

    public void OnGet() { }

    public async Task<IActionResult> OnPost()
    {
        var firstname = (Firstname ?? "").Trim();
        var lastname = (Lastname ?? "").Trim();
        var phone = (Phone ?? "").Trim();
        var password = (Password ?? "").Trim();

        if (string.IsNullOrWhiteSpace(firstname) ||
            string.IsNullOrWhiteSpace(lastname) ||
            string.IsNullOrWhiteSpace(phone) ||
            string.IsNullOrWhiteSpace(password))
        {
            ErrorMessage = "Veuillez remplir tous les champs.";
            return Page();
        }

        // Vérifier téléphone unique
        var exists = await _db.Users.AnyAsync(u => u.Phone == phone);
        if (exists)
        {
            ErrorMessage = "Ce numéro existe déjà. Connecte-toi.";
            return Page();
        }

        var user = new User
        {
            Firstname = firstname,
            Lastname = lastname,
            Phone = phone,
            Password = password,   // plus tard: hash
            Role = "CLIENT"
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        // Après inscription -> aller login
        return RedirectToPage("/Index");
    }
}
