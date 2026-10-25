using GR24h_Models;
using GR24h_Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GR24h_Controller.Pages.PersonalAreaManagement.AdminPersonalManagement
{
    public class AdminPendingRecipesModel : PageModel
    {

        private readonly RecipeService _recipeService;
       

        public AdminPendingRecipesModel(RecipeService recipeService)
        {
            _recipeService = recipeService;
          
        }

        public List<RecipeDetailsDTO> PendingRecipes { get; set; } = new List<RecipeDetailsDTO>();

        [TempData]
        public string StatusMessage { get; set; } = string.Empty;

        public void OnGet()
        {
            PendingRecipes = _recipeService.GetPendingRecipesDetails();
        }

        public IActionResult OnPostApprove(int id)
        {

            int adminId = HttpContext.Session.GetInt32("UserId") ?? 0;

            try
            {
                _recipeService.ApproveRecipe(id,adminId);
                StatusMessage = "Receita aprovada com sucesso!";
               
            }
            catch (Exception ex)
            {
                StatusMessage = "Erro: " + ex.Message;
            }
            return RedirectToPage();
        }

        public IActionResult OnPostReject(int id)
        {

            int adminId = HttpContext.Session.GetInt32("UserId") ?? 0;

            try
            {
                _recipeService.AdminDeletePendingRecipes(adminId, id);
                StatusMessage = "Receita rejeitada e removida.";
            }
            catch (Exception ex)
            {
                StatusMessage = "Erro: " + ex.Message;
            }
            return RedirectToPage();
        }
    }
}
