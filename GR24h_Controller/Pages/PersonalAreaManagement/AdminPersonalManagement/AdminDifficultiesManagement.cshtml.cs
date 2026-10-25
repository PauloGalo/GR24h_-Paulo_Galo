using GR24h_Models;
using GR24h_Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GR24h_Controller.Pages.PersonalAreaManagement.AdminPersonalManagement
{
    public class AdminDifficultiesManagementModel : PageModel
    {
        private readonly DifficultyService _difficultyService;


        public AdminDifficultiesManagementModel(DifficultyService difficultyService)
        {
            _difficultyService = difficultyService;
        }
        public List<Difficulty> Difficulties { get; set; } = new List<Difficulty>();


        [BindProperty]
        public string Name { get; set; } = string.Empty;

        public string ErrorMessage { get; set; } = string.Empty;

        public void OnGet()
        {
            Difficulties = _difficultyService.GetAllDifficulties();
        }

        public IActionResult OnPostAdd()
        {
            try
            {

                Difficulty newDifficulty = new Difficulty(0, Name);

                _difficultyService.CreateDifficultyByAdmin(newDifficulty);
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                Difficulties = _difficultyService.GetAllDifficulties();
                return Page();
            }
        }


        public IActionResult OnPostUpdate(int id, string name)
        {
            try
            {
                Difficulty updateDifficulty = new Difficulty(id, name);
                _difficultyService.UpdateDifficultyByAdmin(updateDifficulty);
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                Difficulties = _difficultyService.GetAllDifficulties();
                return Page();
            }
        }

        public IActionResult OnPostDelete(int id)
        {
            try
            {
                _difficultyService.DeleteDifficultyByAdmin(id);
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                Difficulties = _difficultyService.GetAllDifficulties();
                return Page();
            }
        }
    }
}
