using BrasilBurger.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BrasilBurger.Pages.Client;

[Authorize]
public class PaiementModel : PageModel
{
    private readonly AppDbContext _db;
    public PaiementModel(AppDbContext db) => _db = db;

    public string LeftTitle { get; set; } = "Votre commande";
    public string LeftImage { get; set; } = "/images/burger.png";

    public decimal SubTotal { get; set; }
    public decimal DeliveryFee { get; set; } = 0m;
    public decimal TotalNet { get; set; }

    [BindProperty] public string PaymentMethod { get; set; } = "MAXIT";
    public string ErrorMessage { get; set; } = "";

    private async Task<(int userId, Models.Commande? cmd)> GetCartAsync()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdStr, out var userId))
            return (0, null);

        var cmd = await _db.Commandes.FirstOrDefaultAsync(x => x.UserId == userId && x.Status == "EN_COURS");
        return (userId, cmd);
    }

    public async Task<IActionResult> OnGet()
    {
        var (_, cmd) = await GetCartAsync();
        if (cmd == null)
            return RedirectToPage("/Client/Panier");

        // 👉 Image/Titre (on prend le 1er élément du panier si existe)
        var firstBurger = await (
            from cb in _db.CommandeBurgers
            join b in _db.Burgers on cb.BurgerId equals b.Id
            where cb.CommandeId == cmd.Id
            select new { b.Name, b.Image }
        ).FirstOrDefaultAsync();

        var firstMenu = await (
            from cm in _db.CommandeMenus
            join m in _db.Menus on cm.MenuId equals m.Id
            where cm.CommandeId == cmd.Id
            select new { m.Name, m.Image }
        ).FirstOrDefaultAsync();

        var item = firstBurger != null ? firstBurger : firstMenu;

        if (item != null)
        {
            LeftTitle = item.Name;
            LeftImage = string.IsNullOrWhiteSpace(item.Image)
                ? "/images/burger.png"
                : (item.Image.StartsWith("/") ? item.Image : "/" + item.Image);
        }
        else
        {
            LeftTitle = "Votre commande";
            LeftImage = "/images/burger.png";
        }

        // ✅ totals depuis DB
        var burgersTotal = await _db.CommandeBurgers
            .Where(x => x.CommandeId == cmd.Id)
            .SumAsync(x => (decimal?)x.UnitPrice * x.Quantity) ?? 0m;

        var menusTotal = await _db.CommandeMenus
            .Where(x => x.CommandeId == cmd.Id)
            .SumAsync(x => (decimal?)x.UnitPrice * x.Quantity) ?? 0m;

        var compsTotal = await _db.CommandeComplements
            .Where(x => x.CommandeId == cmd.Id)
            .SumAsync(x => (decimal?)x.UnitPrice * x.Quantity) ?? 0m;

        SubTotal = burgersTotal + menusTotal + compsTotal;
        TotalNet = SubTotal + DeliveryFee;

        return Page();
    }

    public async Task<IActionResult> OnPostPay()
    {
        var (_, cmd) = await GetCartAsync();
        if (cmd == null)
            return RedirectToPage("/Client/Panier");

        PaymentMethod = (PaymentMethod ?? "").Trim().ToUpper();

        if (PaymentMethod != "MAXIT" && PaymentMethod != "WAVE" && PaymentMethod != "ESPECES")
        {
            ErrorMessage = "Choisis un mode de paiement.";
            return await OnGet();
        }

        // recalcul total (sécurité)
        var burgersTotal = await _db.CommandeBurgers
            .Where(x => x.CommandeId == cmd.Id)
            .SumAsync(x => (decimal?)x.UnitPrice * x.Quantity) ?? 0m;

        var menusTotal = await _db.CommandeMenus
            .Where(x => x.CommandeId == cmd.Id)
            .SumAsync(x => (decimal?)x.UnitPrice * x.Quantity) ?? 0m;

        var compsTotal = await _db.CommandeComplements
            .Where(x => x.CommandeId == cmd.Id)
            .SumAsync(x => (decimal?)x.UnitPrice * x.Quantity) ?? 0m;

        SubTotal = burgersTotal + menusTotal + compsTotal;
        TotalNet = SubTotal + DeliveryFee;

        // ✅ ici c’est ton "paiement" (simulation)
        // On passe la commande en VALIDEE (respect du CHECK)
        cmd.TotalAmount = TotalNet;
        cmd.Status = "VALIDEE";

        await _db.SaveChangesAsync();

        // redirection vers commandes (ou accueil)
        return RedirectToPage("/Client/MesCommandes");
    }
}
