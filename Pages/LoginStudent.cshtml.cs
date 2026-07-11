using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GMCC.Pages
{
    public class LoginStudent : PageModel
    {
        [BindProperty]
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [BindProperty]
        [Required]
        public string Password { get; set; }

        public string ErrorMessage { get; set; }

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            return RedirectToPage("/BrowseDormStudent");
        }

        public IActionResult OnPostForgotPassword()
        {
            // forgot password function can be implemented here
            return Page();
        }

        public IActionResult OnPostCreateAccount()
        {
            return RedirectToPage("/RegisterStudent");
        }
    }
}