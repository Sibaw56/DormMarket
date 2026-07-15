using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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

            if (Students.EmailExists(Email))
            {
                ErrorMessage = "An account with this email already exists.";
                return Page();
            }

            Students.Add(new StudentAccount
            {
                FullName = FullName,
                Email = Email,
                ContactNumber = ContactNumber,
                Password = Password
            });

            SuccessMessage = "Account created successfully.";
            return RedirectToPage("/VerifyStudent");
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

    public static class Students
    {
        private static readonly List<StudentAccount> _students = new();

        public static bool EmailExists(string email) =>
            _students.Any(s => s.Email.Equals(email, StringComparison.OrdinalIgnoreCase));

        public static void Add(StudentAccount student) => _students.Add(student);

        public static StudentAccount? FindByEmail(string email) =>
            _students.FirstOrDefault(s => s.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
    }

    public class StudentAccount
    {
        public string FullName { get; set; } = "";
        public string Email { get; set; } = "";
        public string ContactNumber { get; set; } = "";
        public string Password { get; set; } = "";
    }
}