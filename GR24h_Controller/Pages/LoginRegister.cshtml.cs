using GR24h_Models;
using GR24h_Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GR24h_Controller.Pages
{
    public class LoginRegisterModel : PageModel
    {
        private readonly UsersService _userService;

        public LoginRegisterModel(UsersService userService)
        {
            _userService = userService;
        }


        [BindProperty]
        public string LoginUsername { get; set; } = null!;
        [BindProperty]
        public string LoginPassword { get; set; } = null!;


        [BindProperty]
        public string FirstName { get; set; } = null!;
        [BindProperty]
        public string LastName { get; set; } = null!;
        [BindProperty]
        public string Email { get; set; } = null!;
        [BindProperty]
        public string Username { get; set; } = null!;
        [BindProperty]
        public string Password { get; set; } = null!;

        public bool IsAdmin { get; set; }

        public string? ErrorMessage { get; set; }

        public void OnGet()
        {
            HttpContext.Session.Clear();
        }


        public IActionResult OnPostLogin()
        {
            HttpContext.Session.Clear();

            try
            {
                LoginRequest reqLogin = new LoginRequest(LoginUsername, LoginPassword);

                Users? user = _userService.Login(reqLogin);

                if (user == null)
                {
                    ErrorMessage = "Utilizador inválido ou Palavra-Passe inválida.";
                    return Page();
                }


                HttpContext.Session.SetInt32("UserId", user.Id);
                HttpContext.Session.SetString("UserName", user.Username);
                HttpContext.Session.SetString("UserRole", user.IsAdmin ? "Admin" : "User");


                if (user.IsAdmin)
                {
                    return RedirectToPage("/PersonalAreaManagement/AdminPersonalManagement/AdminProfile", new { id = user.Id });
                }
                else
                {
                    return RedirectToPage("/PersonalAreaManagement/UserPersonalManagement/UserProfile", new { id = user.Id });
                }
            }
            catch (Exception ex)
            {
                HttpContext.Session.Clear();
                ErrorMessage = ex.Message;
                return Page();
            }
        }

        public IActionResult OnPostRegister()
        {
            HttpContext.Session.Clear();

            try
            {
                RegisterRequest reqRegister = new RegisterRequest(
                    FirstName, LastName, Username, Email, Password, IsAdmin);

                
                _userService.Register(reqRegister);

                TempData["SuccessMessage"] = "Registo efetuado com sucesso! Por favor faça login.";

                
                return RedirectToPage("/LoginRegister");
            }
            catch (Exception ex)
            {
                HttpContext.Session.Clear();
                ErrorMessage = ex.Message;
                return Page();
            }


        }
    }
}
