using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MongoDB.Driver;
namespace GMCC.Pages
{
    public class LoginStudent : PageModel
    {
        private readonly MongoDBService _mongoService;

        public LoginStudent(MongoDBService mongoService)
        {
            _mongoService = mongoService;
        }

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
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password)) //IF EITHER EMAIL OR PASSWORD IS EMPTY, RETURN ERROR MESSAGE
            {
                ErrorMessage = "Please enter both email and password.";
                return Page();
            }

            var student = _mongoService.Students
                .Find(s => s.Email == Email)
                .FirstOrDefault();

            if (student == null || student.Password != Password) //IF STUDENT IS NULL or PASSWORD DOES NOT MATCH, RETURN ERROR MESSAGE
            {
                ErrorMessage = "Invalid email or password.";
                return Page();
            }

            else //SUCCESSFUL LOGIN, STAY IN PAGE (TEMPORARY)
            {
                ErrorMessage = "User Verified (test).";
                return Page();
            }

            //return RedirectToPage("/BrowseDormStudent");
        }

        public IActionResult OnPostForgotPassword()
        {
            return Page();
        }

        public IActionResult OnPostCreateAccount()
        {
            return RedirectToPage("/RegisterStudent");
        }
    }
}