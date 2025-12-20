namespace BrasilBurger.Models;

public class MenuComposition
{
    public int Id { get; set; }

    // colonnes DB : menu_id, burger_id, boisson_id, frites_id
    public int MenuId { get; set; }
    public int BurgerId { get; set; }
    public int BoissonId { get; set; }
    public int FritesId { get; set; }

    // Navigations (optionnel mais pratique)
    public Menu? Menu { get; set; }
    public Burger? Burger { get; set; }
    public Complement? Boisson { get; set; }
    public Complement? Frites { get; set; }
}
