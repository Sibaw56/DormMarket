using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace GMCC.Pages
{
    public class DashboardOwner : PageModel
    {
        public void OnGet()
        {
        }
         public IActionResult OnPostListing()
        {
            return RedirectToPage("/BrowseDormOwner");
        }
         public IActionResult OnPostProfile()
        {
            return RedirectToPage("/ProfileOwner");
        }
    }
}