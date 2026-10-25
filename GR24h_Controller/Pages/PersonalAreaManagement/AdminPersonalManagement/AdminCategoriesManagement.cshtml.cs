using GR24h_Models;
using GR24h_Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GR24h_Controller.Pages.PersonalAreaManagement.AdminPersonalManagement
{
    public class AdminCategoriesManagementModel : PageModel
    {
        private readonly CategoryService _categoryService;
        
        public AdminCategoriesManagementModel(CategoryService categoryService)
        {
            _categoryService = categoryService;
           
        }

        public List<Category> Categories { get; set; } = new List<Category>();

        [BindProperty]
        public string Name { get; set; } = string.Empty;

        public string ErrorMessage { get; set; } = string.Empty;

        public void OnGet()
        {
            Categories = _categoryService.GetAllCategories();
        }

        public IActionResult OnPostAdd()
        {
            try
            {
                Category newCategory = new Category(0, Name);

                _categoryService.CreateCategoryByAdmin(newCategory);
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                Categories = _categoryService.GetAllCategories();
                return Page();
            }

        }

        public IActionResult OnPostUpdate(int id, string name)
        {
            try
            {
                Category editCategory = new Category(id, name);
                _categoryService.UpdateCategoryByAdmin(editCategory);
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                Categories = _categoryService.GetAllCategories();
                return Page();
            }
        }

        public IActionResult OnPostDelete(int id)
        {
            try
            {
                _categoryService.DeleteCategoryByAdmin(id);
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                Categories = _categoryService.GetAllCategories();
                return Page();
            }
        }
    }
}
