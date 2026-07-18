using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace GMCC.Pages
{
    public class ProfileStudent : PageModel
    {
        public string? StatusMessage { get; set; }

        [BindProperty]
        public string FullName { get; set; } = string.Empty;

        [BindProperty]
        public string Email { get; set; } = string.Empty;

        [BindProperty]
        public string ContactNumber { get; set; } = string.Empty;

        [BindProperty]
        public string Address { get; set; } = string.Empty;

        public string? VerifiedSchool { get; set; }
        public string? VerifiedRentalDorm { get; set; }

        public List<VerificationDocItem> StudentVerification { get; set; } = new();
        public List<VerificationDocItem> RentalVerification { get; set; } = new();

        public void OnGet()
        {
           //load all student info here
        }

        public IActionResult OnPostBrowse()
        {
            return RedirectToPage("/BrowseDormStudent");
        }

        public IActionResult OnPostSaveChanges()
        {
            StatusMessage = "Profile updated.";
            return Page();
        }

        public IActionResult OnPostLogout()
        {
            return RedirectToPage("/LoginStudent");
        }
    }

    public class VerificationDocItem
    {
        public string Title { get; set; } = "";
        public string FileName { get; set; } = "";
        public bool Verified { get; set; }
    }
}
