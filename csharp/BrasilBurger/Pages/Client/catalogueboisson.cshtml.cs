using BrasilBurger.Data;
using BrasilBurger.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BrasilBurger.Pages.Client;

[Authorize]
public class CatalogueBoissonModel : PageModel
{
    private readonly AppDbContext _db;
    public CatalogueBoissonModel(AppDbContext db) => _db = db;

    public List<Complement> Items { get; set; } = [];

    public async Task OnGet()
    {
        Items = await _db.Complements
            .Where(x => !x.Archived && x.Type == "BOISSON")
            .OrderBy(x => x.Id)
            .ToListAsync();
    }
}
