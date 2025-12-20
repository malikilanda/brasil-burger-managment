using BrasilBurger.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BrasilBurger.Pages
{
    public class TestDbModel : PageModel
    {
        private readonly AppDbContext _db;
        public string Message { get; set; } = "Test...";

        public TestDbModel(AppDbContext db)
        {
            _db = db;
        }

        public void OnGet()
        {
            try
            {
                var count = _db.Users.Count();
                Message = $"✅ DB OK - Users = {count}";
            }
            catch (Exception ex)
            {
                Message = "❌ DB ERROR : " + ex.Message;
            }
        }
    }
}
