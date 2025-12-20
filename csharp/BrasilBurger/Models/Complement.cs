namespace BrasilBurger.Models;

public class Complement
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public decimal Price { get; set; }
    public string? Image { get; set; }
    public string Type { get; set; } = ""; // "FRITE" ou "BOISSON" (ou autre)
    public bool Archived { get; set; }
}
