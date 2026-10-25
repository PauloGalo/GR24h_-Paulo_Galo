using GR24h_Models;
using GR24h_Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GR24h_Controller.Pages.PersonalAreaManagement
{
    public class PersonalDataModel : PageModel
    {
        private readonly UsersService _userService;

        public PersonalDataModel(UsersService userService)
        {
            _userService = userService;
        }

        [BindProperty]
        public int Id { get; set; }
        [BindProperty]
        public string FirstName { get; set; } = string.Empty;
        [BindProperty]
        public string LastName { get; set; } = string.Empty;
        [BindProperty]
        public string Email { get; set; } = string.Empty;
        [BindProperty]
        public string Username { get; set; } = string.Empty;
        [BindProperty]
        public string? NewPassword { get; set; } = string.Empty;

        [BindProperty]
        public string PasswordConfirm { get; set; } = string.Empty;
        public bool IsAdmin { get; set; }

        [TempData]
        public string Message { get; set; } = string.Empty;

        public IActionResult OnGet()
        {

            int? userId = HttpContext.Session.GetInt32("UserId");


            if (userId == null) return RedirectToPage("/LoginRegister");

            Users? user = _userService.GetUserById(userId.Value);
            if (user == null) return RedirectToPage("/LoginRegister");


            Id = user.Id;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Email = user.Email;
            Username = user.Username;
            IsAdmin = user.IsAdmin;

            return Page();

        }

        public IActionResult OnPost()
        {
            try
            {

                _userService.UpdateProfile(Id, FirstName, LastName, Username, Email, NewPassword);

                Message = "Dados atualizados com sucesso!";
            }
            catch (Exception ex)
            {
                Message = "Erro: " + ex.Message;
            }

            return RedirectToPage();
        }

        public IActionResult OnPostDeleteAccount()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToPage("/LoginRegister");

            try
            {
                _userService.UserDeleteAccount(userId.Value,PasswordConfirm);

               
                HttpContext.Session.Clear();

                TempData["Message"] = "A sua conta foi eliminada com sucesso.";
                return RedirectToPage("/LoginRegister");
            }
            catch (Exception ex)
            {
                Message = "Erro: " + ex.Message;
                return RedirectToPage();
            }
        }
    }


}
