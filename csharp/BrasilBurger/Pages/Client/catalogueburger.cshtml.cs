using BrasilBurger.Data;
using BrasilBurger.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BrasilBurger.Pages.Client;

[Authorize]
public class CatalogueBurgerModel : PageModel
{
    private readonly AppDbContext _db;
    public CatalogueBurgerModel(AppDbContext db) => _db = db;

    public List<Burger> Items { get; set; } = [];

    public async Task OnGet()
    {
        Items = await _db.Burgers
            .Where(x => !x.Archived)
            .OrderBy(x => x.Id)
            .ToListAsync();
    }
}
