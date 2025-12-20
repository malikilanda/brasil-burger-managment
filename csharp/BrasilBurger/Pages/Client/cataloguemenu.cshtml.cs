using BrasilBurger.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BrasilBurger.Pages.Client;

[Authorize]
public class CatalogueMenuModel : PageModel
{
    private readonly AppDbContext _db;
    public CatalogueMenuModel(AppDbContext db) => _db = db;

    public class MenuCardVm
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string? Image { get; set; }
        public decimal Price { get; set; }
    }

    public List<MenuCardVm> Items { get; set; } = [];

    public async Task OnGet()
    {
        Items = await _db.Menus
            .Where(m => !m.Archived)
            .OrderBy(m => m.Id)
            .Select(m => new MenuCardVm
            {
                Id = m.Id,
                Name = m.Name,
                Image = m.Image,
                Price = m.Price
            })
            .ToListAsync();
    }
}
