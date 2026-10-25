using GR24h_Models;
using GR24h_Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GR24h_Controller.Pages.PersonalAreaManagement.AdminPersonalManagement
{
    public class AdminUserManagementModel : PageModel
    {
        private readonly UsersService _userService;

        public AdminUserManagementModel(UsersService userService)
        {
            _userService = userService;
        }

        public List<Users> Users { get; set; } = new List<Users>();

        [TempData]
        public string StatusMessage { get; set; } = string.Empty;

        public void OnGet()
        {
            Users = _userService.GetAllUsers();
        }

        public IActionResult OnPostToggleLock(int targetId)
        {

            int adminId = HttpContext.Session.GetInt32("UserId") ?? 0;

            try
            {
                _userService.ToggleUserLock(adminId, targetId);
                StatusMessage = "Estado de bloqueio atualizado.";
            }
            catch (Exception ex)
            {
                StatusMessage = "Erro: " + ex.Message;
            }
            return RedirectToPage();
        }

        public IActionResult OnPostDeleteUser(int targetId)
        {

            int adminId = HttpContext.Session.GetInt32("UserId") ?? 0;

            try
            {
                _userService.AdminDeleteUser(adminId, targetId);
                StatusMessage = "Utilizador removido com sucesso.";
            }
            catch (Exception ex)
            {
                StatusMessage = "Erro: " + ex.Message;
            }
            return RedirectToPage();
        }

    }
}
