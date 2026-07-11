using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GMCC.Pages
{
    public class RegisterStudent : PageModel
    {
        [BindProperty]
        public string FullName { get; set; }

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string ContactNumber { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public string ErrorMessage { get; set; }
        public string SuccessMessage { get; set; }

        public void OnGet()
        {
        }

        public IActionResult OnPostCreateAccount()
        {
            if (string.IsNullOrEmpty(FullName) || string.IsNullOrEmpty(Email) ||
                string.IsNullOrEmpty(ContactNumber) || string.IsNullOrEmpty(Password))
            {
                ErrorMessage = "Please fill in all fields.";
                return Page();
            }

            if (!IsValidEmail(Email))
            {
                ErrorMessage = "Please enter a valid email address.";
                return Page();
            }

            // insert Database code here to save the user information to the database

            SuccessMessage = "Account created successfully. Please log in.";
            return RedirectToPage("/LoginStudent");
        }

        public IActionResult OnPostLogin()
        {
            return RedirectToPage("/LoginStudent");
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
                return Regex.IsMatch(email, pattern);
            }
            catch
            {
                return false;
            }
        }
    }
}
