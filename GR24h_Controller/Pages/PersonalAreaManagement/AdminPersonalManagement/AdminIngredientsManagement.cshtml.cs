using GR24h_Models;
using GR24h_Repos;
using GR24h_Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GR24h_Controller.Pages.PersonalAreaManagement.AdminPersonalManagement
{
    public class AdminIngredientsManagementModel : PageModel
    {
        private readonly IngredientService _ingredientService;

        public AdminIngredientsManagementModel(IngredientService ingredientService)
        {
            _ingredientService = ingredientService;

        }

        public List<Ingredient> Ingredients { get; set; } = new List<Ingredient>();

        [BindProperty]
        public string Name { get; set; } = string.Empty;

        public string ErrorMessage { get; set; } = string.Empty;

        public void OnGet()
        {
            Ingredients = _ingredientService.GetAllIngredients();
        }

        public IActionResult OnPostAdd()
        {
            try
            {

                Ingredient newIngredient = new Ingredient(0, Name);
                _ingredientService.CreateIngredientByAdmin(newIngredient);
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                Ingredients = _ingredientService.GetAllIngredients();
                return Page();
            }
        }

        public IActionResult OnPostUpdate(int id, string name)
        {
            try
            {
                Ingredient updateIngredient = new Ingredient(id, name);
                _ingredientService.UpdateIngredientByAdmin(updateIngredient);
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                Ingredients = _ingredientService.GetAllIngredients();
                return Page();
            }
        }

        public IActionResult OnPostDelete(int id)
        {
            try
            {
                _ingredientService.DeleteIngredientByAdmin(id);
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                Ingredients = _ingredientService.GetAllIngredients();
                return Page();
            }
        }

    }
}
