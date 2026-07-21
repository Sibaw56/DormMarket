using System;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MongoDB.Driver;

namespace GMCC.Pages
{
    public class RegisterOwner : PageModel
    {
        private readonly MongoDBService _mongoService;

        public RegisterOwner(MongoDBService mongoService)
        {
            _mongoService = mongoService;
        }

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

            var existing = _mongoService.Owners
                .Find(o => o.Email == Email)
                .FirstOrDefault();

            if (existing != null)
            {
                ErrorMessage = "An account with this email already exists.";
                return Page();
            }

            var maxId = _mongoService.Owners
                .Find(FilterDefinition<ownerUser>.Empty)
                .SortByDescending(o => o.Id)
                .Limit(1)
                .FirstOrDefault();
            var nextId = (maxId?.Id ?? 0) + 1;

            var newOwner = new ownerUser
            {
                Id = nextId,
                FullName = FullName,
                Email = Email,
                Password = Password,
                ContactNumber = ContactNumber,
                DateJoined = DateTime.UtcNow
            };

            _mongoService.Owners.InsertOne(newOwner);

            SuccessMessage = "Account created successfully. Please log in.";
            return RedirectToPage("/VerifyOwner", new { ownerId = nextId });
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
}