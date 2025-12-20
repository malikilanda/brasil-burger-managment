namespace BrasilBurger.Models;

public class Commande
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Type { get; set; } = "SUR_PLACE"; // SUR_PLACE / A_EMPORTER / LIVRAISON
    public int? QuartierId { get; set; }
    public string Status { get; set; } = "EN_COURS"; // EN_COURS / VALIDEE / TERMINEE / ANNULEE
    public decimal? TotalAmount { get; set; }
    public DateTime CreatedAt { get; set; }
}
public static class CommandeStatus
{
    public const string EN_COURS = "EN_COURS";
    public const string VALIDEE = "VALIDEE";
    public const string TERMINEE = "TERMINEE";
    public const string ANNULEE = "ANNULEE";
}
