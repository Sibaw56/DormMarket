using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace GMCC.Pages
{
    public class ContactOwnerStudent : PageModel
    {
        public Dormitory Dorm { get; private set; } = new();

        public IActionResult OnGet(string id)
        {
            var dorm = DummyDormitories.All.FirstOrDefault(d => d.Id == id);
            if (dorm == null)
            {
                return RedirectToPage("/BrowseDormStudent");
            }

            Dorm = dorm;
            return Page();
        }
    }
}
