using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace GMCC.Pages
{
    public class AddDormOwner : PageModel
    {
        [BindProperty]
        public string DormitoryName { get; set; } = string.Empty;

        [BindProperty]
        public string AddressLocation { get; set; } = string.Empty;

        [BindProperty]
        public string Description { get; set; } = string.Empty;

        [BindProperty]
        public string MonthlyRent { get; set; } = string.Empty;

        public string? ErrorMessage { get; set; }

        public void OnGet()
        {
        }

        public IActionResult OnPostPublish()
        {
            if (string.IsNullOrWhiteSpace(DormitoryName) || string.IsNullOrWhiteSpace(AddressLocation))
            {
                ErrorMessage = "Please fill in the dormitory name and address before publishing.";
                return Page();
            }

            return RedirectToPage("/DashboardOwner");
        }

        public IActionResult OnPostDraft()
        {
            return RedirectToPage("/DashboardOwner");
        }
    }
}
