using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GR24h_Controller.Pages.PersonalAreaManagement
{
    public class LogoutModel : PageModel
    {
        public void OnGet()
        {
        }

        public IActionResult OnPostLogout()
        {

            HttpContext.Session.Clear();

            TempData.Clear();

            return RedirectToPage("/Index");
            
        }
    }
}
