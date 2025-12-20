using BrasilBurger.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BrasilBurger.Pages.Client;

[Authorize]
public class MesCommandesModel : PageModel
{
    private readonly AppDbContext _db;
    public MesCommandesModel(AppDbContext db) => _db = db;

    public class OrderVm
    {
        public int Id { get; set; }
        public string StatusLabel { get; set; } = "";
        public string DeliveryLabel { get; set; } = "";
        public string Note { get; set; } = "";
    }

    public List<OrderVm> Items { get; set; } = [];

    public async Task OnGet()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdStr, out var userId)) return;

        // On n'affiche pas le panier EN_COURS ici (c'est le panier)
        var orders = await _db.Commandes
            .Where(x => x.UserId == userId && x.Status != "EN_COURS")
            .OrderByDescending(x => x.Id)
            .ToListAsync();

        Items = orders.Select(o => new OrderVm
        {
            Id = o.Id,
            StatusLabel = StatusToLabel(o.Status),
            DeliveryLabel = TypeToLabel(o.Type),
            Note = StatusToNote(o.Status)
        }).ToList();
    }

    private static string StatusToLabel(string? s)
    {
        s = (s ?? "").ToUpper();
        return s switch
        {
            "EN_COURS" => "En cours",
            "VALIDEE" => "Validée",
            "TERMINEE" => "Terminée",
            "ANNULEE" => "Annulée",
            _ => s
        };
    }

    private static string StatusToNote(string? s)
    {
        s = (s ?? "").ToUpper();
        return s switch
        {
            "VALIDEE" => "commande validée",
            "TERMINEE" => "commande livrée",
            "ANNULEE" => "commande annulée",
            "EN_COURS" => "commande en cours",
            _ => "commande"
        };
    }

    private static string TypeToLabel(string? t)
    {
        t = (t ?? "").ToUpper();
        return t switch
        {
            "LIVRAISON" => "Livraison",
            "SUR_PLACE" => "Sur place",
            "EMPORTER" => "Emporter",
            "" => "Livraison",
            _ => "Livraison"
        };
    }
}
