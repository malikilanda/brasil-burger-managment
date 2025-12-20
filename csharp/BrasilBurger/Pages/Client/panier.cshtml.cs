using BrasilBurger.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BrasilBurger.Pages.Client;

[Authorize]
public class PanierModel : PageModel
{
    private readonly AppDbContext _db;
    public PanierModel(AppDbContext db) => _db = db;

    public class CartLineVm
    {
        public string Key { get; set; } = ""; // ex: "B:12" / "M:4" / "C:7"
        public string Title { get; set; } = "";
        public string? Sub { get; set; }
        public string Image { get; set; } = "/images/burger.png";
        public int Qty { get; set; }
        public decimal UnitPrice { get; set; }
    }

    public List<CartLineVm> Lines { get; set; } = [];
    public decimal Total { get; set; }

    private int? GetUserId()
    {
        var s = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(s, out var id) ? id : null;
    }

    private async Task<int?> GetCommandeId()
    {
        var uid = GetUserId();
        if (uid is null) return null;

        var cmd = await _db.Commandes.FirstOrDefaultAsync(x => x.UserId == uid && x.Status == "EN_COURS");
        return cmd?.Id;
    }

    public async Task OnGet() => await LoadCart();

    private async Task LoadCart()
    {
        Lines = [];
        Total = 0;

        var cmdId = await GetCommandeId();
        if (cmdId is null) return;

        var burgers = await (
            from cb in _db.CommandeBurgers
            join b in _db.Burgers on cb.BurgerId equals b.Id
            where cb.CommandeId == cmdId
            select new CartLineVm
            {
                Key = "B:" + cb.Id,
                Title = b.Name,
                Sub = "Burger",
                Image = string.IsNullOrWhiteSpace(b.Image) ? "/images/burger.png" : (b.Image.StartsWith("/") ? b.Image : "/" + b.Image),
                Qty = cb.Quantity,
                UnitPrice = cb.UnitPrice
            }
        ).ToListAsync();

        var menus = await (
            from cm in _db.CommandeMenus
            join m in _db.Menus on cm.MenuId equals m.Id
            where cm.CommandeId == cmdId
            select new CartLineVm
            {
                Key = "M:" + cm.Id,
                Title = m.Name,
                Sub = "Menu",
                Image = string.IsNullOrWhiteSpace(m.Image) ? "/images/menu.png" : (m.Image.StartsWith("/") ? m.Image : "/" + m.Image),
                Qty = cm.Quantity,
                UnitPrice = cm.UnitPrice
            }
        ).ToListAsync();

        var comps = await (
            from cc in _db.CommandeComplements
            join c in _db.Complements on cc.ComplementId equals c.Id
            where cc.CommandeId == cmdId
            select new CartLineVm
            {
                Key = "C:" + cc.Id,
                Title = c.Name,
                Sub = c.Type,
                Image = string.IsNullOrWhiteSpace(c.Image)
                    ? (c.Type == "FRITES" ? "/images/frite.png" : "/images/boisson.png")
                    : (c.Image.StartsWith("/") ? c.Image : "/" + c.Image),
                Qty = cc.Quantity,
                UnitPrice = cc.UnitPrice
            }
        ).ToListAsync();

        Lines = burgers.Concat(menus).Concat(comps).ToList();
        Total = Lines.Sum(x => x.UnitPrice * x.Qty);
    }

    public async Task<IActionResult> OnPostClear()
    {
        var cmdId = await GetCommandeId();
        if (cmdId is null) return RedirectToPage();

        // delete lignes
        _db.CommandeBurgers.RemoveRange(_db.CommandeBurgers.Where(x => x.CommandeId == cmdId));
        _db.CommandeMenus.RemoveRange(_db.CommandeMenus.Where(x => x.CommandeId == cmdId));
        _db.CommandeComplements.RemoveRange(_db.CommandeComplements.Where(x => x.CommandeId == cmdId));

        await _db.SaveChangesAsync();
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostPlus(string key)
    {
        await UpdateQty(key, +1);
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostMinus(string key)
    {
        await UpdateQty(key, -1);
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostRemove(string key)
    {
        var cmdId = await GetCommandeId();
        if (cmdId is null) return RedirectToPage();

        if (key.StartsWith("B:") && int.TryParse(key[2..], out var idB))
        {
            var row = await _db.CommandeBurgers.FirstOrDefaultAsync(x => x.Id == idB && x.CommandeId == cmdId);
            if (row != null) _db.CommandeBurgers.Remove(row);
        }
        else if (key.StartsWith("M:") && int.TryParse(key[2..], out var idM))
        {
            var row = await _db.CommandeMenus.FirstOrDefaultAsync(x => x.Id == idM && x.CommandeId == cmdId);
            if (row != null) _db.CommandeMenus.Remove(row);
        }
        else if (key.StartsWith("C:") && int.TryParse(key[2..], out var idC))
        {
            var row = await _db.CommandeComplements.FirstOrDefaultAsync(x => x.Id == idC && x.CommandeId == cmdId);
            if (row != null) _db.CommandeComplements.Remove(row);
        }

        await _db.SaveChangesAsync();
        return RedirectToPage();
    }

    private async Task UpdateQty(string key, int delta)
    {
        var cmdId = await GetCommandeId();
        if (cmdId is null) return;

        if (key.StartsWith("B:") && int.TryParse(key[2..], out var idB))
        {
            var row = await _db.CommandeBurgers.FirstOrDefaultAsync(x => x.Id == idB && x.CommandeId == cmdId);
            if (row == null) return;
            row.Quantity = Math.Max(1, row.Quantity + delta);
        }
        else if (key.StartsWith("M:") && int.TryParse(key[2..], out var idM))
        {
            var row = await _db.CommandeMenus.FirstOrDefaultAsync(x => x.Id == idM && x.CommandeId == cmdId);
            if (row == null) return;
            row.Quantity = Math.Max(1, row.Quantity + delta);
        }
        else if (key.StartsWith("C:") && int.TryParse(key[2..], out var idC))
        {
            var row = await _db.CommandeComplements.FirstOrDefaultAsync(x => x.Id == idC && x.CommandeId == cmdId);
            if (row == null) return;
            row.Quantity = Math.Max(1, row.Quantity + delta);
        }

        await _db.SaveChangesAsync();
    }
}
