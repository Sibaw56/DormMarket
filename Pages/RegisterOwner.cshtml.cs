using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
namespace GMCC.Pages
{
    public class RegisterOwner : PageModel
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

            // TODO (DB teammate): replace Owners store below with a real DB insert.
            if (Owners.EmailExists(Email))
            {
                ErrorMessage = "An account with this email already exists.";
                return Page();
            }

            Owners.Add(new OwnerAccount //temporary save owner info, change after Database implementation
            {
                FullName = FullName,
                Email = Email,
                ContactNumber = ContactNumber,
                Password = Password 
            });

            SuccessMessage = "Account created successfully. Please log in.";
            return RedirectToPage("/LoginOwner");
        }

        public IActionResult OnPostLogin()
        {
            return RedirectToPage("/LoginOwner");
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

    public static class Owners //temporary owner added into list, change after Database implementation
    {
        private static readonly List<OwnerAccount> _owners = new();

        public static bool EmailExists(string email) =>
            _owners.Any(o => o.Email.Equals(email, StringComparison.OrdinalIgnoreCase));

        public static void Add(OwnerAccount owner) => _owners.Add(owner);

        public static OwnerAccount? FindByEmail(string email) =>
            _owners.FirstOrDefault(o => o.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
    }
    public class OwnerAccount
    {
        public string FullName { get; set; } = "";
        public string Email { get; set; } = "";
        public string ContactNumber { get; set; } = "";
        public string Password { get; set; } = "";
    }
}