namespace BrasilBurger.Models;

public class CommandeComplement
{
    public int Id { get; set; }
    public int CommandeId { get; set; }

    // ✅ lien vers le burger choisi dans DetailsProduits
    public int? CommandeBurgerId { get; set; }

    public int ComplementId { get; set; }
    public int Quantity { get; set; } = 1;
    public decimal UnitPrice { get; set; }
}
