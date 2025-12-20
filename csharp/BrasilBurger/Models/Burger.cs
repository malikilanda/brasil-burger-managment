namespace BrasilBurger.Models;

public class Burger
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public decimal Price { get; set; }
    public string? Image { get; set; }
    public bool Archived { get; set; }
}
