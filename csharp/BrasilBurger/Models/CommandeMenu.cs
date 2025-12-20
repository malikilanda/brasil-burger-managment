namespace BrasilBurger.Models;

public class CommandeMenu
{
    public int Id { get; set; }
    public int CommandeId { get; set; }
    public Commande Commande { get; set; } = null!;

    public int MenuId { get; set; }
    public Menu Menu { get; set; } = null!;

    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; } // prix calculé du menu
}
