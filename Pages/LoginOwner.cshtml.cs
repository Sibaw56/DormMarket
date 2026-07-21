using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace GMCC.Pages
{
    public class LoginOwner : PageModel
    {
        private readonly MongoDBService _mongoService;

        public LoginOwner(MongoDBService mongoService)
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
            if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password))
            {
                ErrorMessage = "Please enter both email and password.";
                return Page();
            }
            
            var owner = _mongoService.Owners.Find(o => o.Email == Email).FirstOrDefault();
            if (owner == null || owner.Password != Password)
            {
                ErrorMessage = "Invalid email or password.";
                return Page();
            }

            HttpContext.Session.SetInt32("OwnerId", owner.Id);
            HttpContext.Session.SetString("OwnerName", owner.FullName ?? "");

            return RedirectToPage("/DashboardOwner");
        }

        public IActionResult OnPostForgotPassword()
        {
            // forgot password function can be implemented here
            return Page();
        }

        public IActionResult OnPostCreateAccount()
        {
            return RedirectToPage("/RegisterOwner");
        }
    }
}