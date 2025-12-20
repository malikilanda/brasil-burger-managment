namespace BrasilBurger.Models;

public class User
{
    public int Id { get; set; }
    public string Firstname { get; set; } = "";
    public string Lastname { get; set; } = "";
    public string Phone { get; set; } = "";
    public string? Email { get; set; }
    public string Password { get; set; } = ""; // (plus tard: hash)
    public string Role { get; set; } = "CLIENT";
    public DateTime CreatedAt { get; set; }
}
