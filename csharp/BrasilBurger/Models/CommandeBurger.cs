namespace BrasilBurger.Models;

public class CommandeBurger
{
    public int Id { get; set; }
    public int CommandeId { get; set; }
    public int BurgerId { get; set; }
    public int Quantity { get; set; } = 1;
    public decimal UnitPrice { get; set; }
}
