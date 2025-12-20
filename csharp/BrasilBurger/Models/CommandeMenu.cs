namespace BrasilBurger.Models;

public class CommandeMenu
{
    public int Id { get; set; }
    public int CommandeId { get; set; }
    public int MenuId { get; set; }
    public int Quantity { get; set; } = 1;
    public decimal UnitPrice { get; set; }
}
