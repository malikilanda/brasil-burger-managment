using BrasilBurger.Models;
using Microsoft.EntityFrameworkCore;

namespace BrasilBurger.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Burger> Burgers => Set<Burger>();
    public DbSet<Complement> Complements => Set<Complement>();
    public DbSet<Menu> Menus => Set<Menu>();
    public DbSet<MenuComposition> MenuCompositions => Set<MenuComposition>();

    public DbSet<Commande> Commandes => Set<Commande>();
    public DbSet<CommandeBurger> CommandeBurgers => Set<CommandeBurger>();
    public DbSet<CommandeMenu> CommandeMenus => Set<CommandeMenu>();
    public DbSet<CommandeComplement> CommandeComplements => Set<CommandeComplement>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        base.OnModelCreating(mb);

        mb.Entity<User>(e =>
        {
            e.ToTable("users");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.Firstname).HasColumnName("firstname");
            e.Property(x => x.Lastname).HasColumnName("lastname");
            e.Property(x => x.Phone).HasColumnName("phone");
            e.Property(x => x.Email).HasColumnName("email");
            e.Property(x => x.Password).HasColumnName("password");
            e.Property(x => x.Role).HasColumnName("role");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
        });

        mb.Entity<Burger>(e =>
        {
            e.ToTable("burgers");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.Name).HasColumnName("name");
            e.Property(x => x.Price).HasColumnName("price");
            e.Property(x => x.Image).HasColumnName("image");
            e.Property(x => x.Archived).HasColumnName("archived");
        });

        mb.Entity<Complement>(e =>
        {
            e.ToTable("complements");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.Name).HasColumnName("name");
            e.Property(x => x.Price).HasColumnName("price");
            e.Property(x => x.Type).HasColumnName("type");
            e.Property(x => x.Image).HasColumnName("image");
            e.Property(x => x.Archived).HasColumnName("archived");
        });

        mb.Entity<Menu>(e =>
        {
            e.ToTable("menus");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.Name).HasColumnName("name");
            e.Property(x => x.Image).HasColumnName("image");
            e.Property(x => x.Archived).HasColumnName("archived");
            e.Property(x => x.Price).HasColumnName("price");
        });

mb.Entity<MenuComposition>(e =>
{
    e.ToTable("menu_composition");
    e.HasKey(x => x.Id);

    e.Property(x => x.Id).HasColumnName("id");
    e.Property(x => x.MenuId).HasColumnName("menu_id");
    e.Property(x => x.BurgerId).HasColumnName("burger_id");
    e.Property(x => x.BoissonId).HasColumnName("boisson_id");
    e.Property(x => x.FritesId).HasColumnName("frites_id");

    // relations (optionnelles mais bien)
    e.HasOne(x => x.Menu).WithMany().HasForeignKey(x => x.MenuId);
    e.HasOne(x => x.Burger).WithMany().HasForeignKey(x => x.BurgerId);
    e.HasOne(x => x.Boisson).WithMany().HasForeignKey(x => x.BoissonId);
    e.HasOne(x => x.Frites).WithMany().HasForeignKey(x => x.FritesId);
});



        mb.Entity<Commande>(e =>
        {
            e.ToTable("commandes");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.UserId).HasColumnName("user_id");
            e.Property(x => x.Type).HasColumnName("type");
            e.Property(x => x.QuartierId).HasColumnName("quartier_id");
            e.Property(x => x.Status).HasColumnName("status");
            e.Property(x => x.TotalAmount).HasColumnName("total_amount");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
        });

        mb.Entity<CommandeBurger>(e =>
        {
            e.ToTable("commande_burgers");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.CommandeId).HasColumnName("commande_id");
            e.Property(x => x.BurgerId).HasColumnName("burger_id");
            e.Property(x => x.Quantity).HasColumnName("quantity");
            e.Property(x => x.UnitPrice).HasColumnName("unit_price");
        });

        mb.Entity<CommandeMenu>(e =>
        {
            e.ToTable("commande_menus");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.CommandeId).HasColumnName("commande_id");
            e.Property(x => x.MenuId).HasColumnName("menu_id");
            e.Property(x => x.Quantity).HasColumnName("quantity");
            e.Property(x => x.UnitPrice).HasColumnName("unit_price");
        });

        mb.Entity<CommandeComplement>(e =>
        {
            e.ToTable("commande_complements");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.CommandeId).HasColumnName("commande_id");
            e.Property(x => x.ComplementId).HasColumnName("complement_id");
            e.Property(x => x.Quantity).HasColumnName("quantity");
            e.Property(x => x.UnitPrice).HasColumnName("unit_price");
        });
    }
}
