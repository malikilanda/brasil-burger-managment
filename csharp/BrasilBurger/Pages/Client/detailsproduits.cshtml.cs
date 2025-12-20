using BrasilBurger.Data;
using BrasilBurger.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BrasilBurger.Pages.Client;

[Authorize]
public class DetailsProduitsModel : PageModel
{
    [BindProperty]
    public string OrderType { get; set; } = "LIVRAISON";

    private readonly AppDbContext _db;
    public DetailsProduitsModel(AppDbContext db) => _db = db;

    [BindProperty(SupportsGet = true)] public string Type { get; set; } = "BURGER"; // BURGER / MENU
    [BindProperty(SupportsGet = true)] public int Id { get; set; }

    // UI
    public string Title { get; set; } = "";
    public string Desc { get; set; } = "";
    public string Image { get; set; } = "/images/burger.png";
    public decimal BasePrice { get; set; }

    public List<Complement> Frites { get; set; } = [];
    public List<Complement> Boissons { get; set; } = [];

    // POST
    [BindProperty] public int Qty { get; set; } = 1;
    [BindProperty] public int? FritesId { get; set; }
    [BindProperty] public int? BoissonId { get; set; }

    public async Task<IActionResult> OnGet()
    {
        // ✅ compléments DB
        Frites = await _db.Complements
            .Where(c => !c.Archived && c.Type == "FRITES")
            .OrderBy(c => c.Id)
            .ToListAsync();

        Boissons = await _db.Complements
            .Where(c => !c.Archived && c.Type == "BOISSON")
            .OrderBy(c => c.Id)
            .ToListAsync();

        if (Type.ToUpper() == "MENU")
        {
            var m = await _db.Menus.FirstOrDefaultAsync(x => x.Id == Id && !x.Archived);
            if (m is null) return RedirectToPage("/Client/CatalogueMenu");

            Title = m.Name;
            Desc = "Menu prédéfini (Burger + Frites + Boisson).";

            Image = string.IsNullOrWhiteSpace(m.Image)
                ? "/images/menu.png"
                : (m.Image.StartsWith("/") ? m.Image : "/" + m.Image);

            // ✅ prix FIXE : menus.price
            BasePrice = m.Price;
        }
        else
        {
            var b = await _db.Burgers.FirstOrDefaultAsync(x => x.Id == Id && !x.Archived);
            if (b is null) return RedirectToPage("/Client/CatalogueBurger");

            Title = b.Name;
            Desc = "Choisis tes options et ajoute au panier.";

            Image = string.IsNullOrWhiteSpace(b.Image)
                ? "/images/burger.png"
                : (b.Image.StartsWith("/") ? b.Image : "/" + b.Image);

            BasePrice = b.Price;
        }

        return Page();
    }

    // ✅ bouton Ajouter (form post)
    public async Task<IActionResult> OnPostAdd()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdStr, out var userId)) return RedirectToPage("/Index");

        Qty = Math.Max(1, Qty);

        // ✅ récupérer/créer commande EN_COURS (panier)
        var cmd = await _db.Commandes
            .FirstOrDefaultAsync(x => x.UserId == userId && x.Status == "EN_COURS");

        if (cmd is null)
        {
            cmd = new Commande
            {
                UserId = userId,
                Status = "EN_COURS", // ✅ conforme au CHECK
                Type = OrderType, 
                CreatedAt = DateTime.UtcNow
            };
            _db.Commandes.Add(cmd);
            await _db.SaveChangesAsync();
        }

        // ✅ ajouter ligne produit
        if (Type.ToUpper() == "MENU")
        {
            var m = await _db.Menus.FirstOrDefaultAsync(x => x.Id == Id && !x.Archived);
            if (m is null) return RedirectToPage("/Client/CatalogueMenu");

            // ✅ IMPORTANT: on sauvegarde le prix FIXE du menu
            _db.CommandeMenus.Add(new CommandeMenu
            {
                CommandeId = cmd.Id,
                MenuId = m.Id,
                Quantity = Qty,
                UnitPrice = m.Price
            });
        }
        else
        {
            var b = await _db.Burgers.FirstOrDefaultAsync(x => x.Id == Id && !x.Archived);
            if (b is null) return RedirectToPage("/Client/CatalogueBurger");

            _db.CommandeBurgers.Add(new CommandeBurger
            {
                CommandeId = cmd.Id,
                BurgerId = b.Id,
                Quantity = Qty,
                UnitPrice = b.Price
            });

            // ✅ extras sélectionnés
            if (FritesId.HasValue)
                await AddComplement(cmd.Id, FritesId.Value, Qty);

            if (BoissonId.HasValue)
                await AddComplement(cmd.Id, BoissonId.Value, Qty);
        }

        await _db.SaveChangesAsync();

        return RedirectToPage("/Client/Panier");
    }

    private async Task AddComplement(int commandeId, int complementId, int qty)
    {
        var c = await _db.Complements.FirstOrDefaultAsync(x => x.Id == complementId && !x.Archived);
        if (c is null) return;

        _db.CommandeComplements.Add(new CommandeComplement
        {
            CommandeId = commandeId,
            ComplementId = c.Id,
            Quantity = qty,
            UnitPrice = c.Price
        });
    }
}
