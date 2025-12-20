using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
namespace BrasilBurger.Pages.Client
{
    [Authorize]
    public class AcceuilModel : PageModel
    {
        public void OnGet() { }
    }
}
