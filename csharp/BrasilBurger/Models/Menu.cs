namespace BrasilBurger.Models;

public class Menu
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string? Image { get; set; }

    public decimal price { get; set; }   
    public bool Archived { get; set; }
}
