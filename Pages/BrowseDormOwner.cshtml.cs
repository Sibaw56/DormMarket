using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace GMCC.Pages
{
    public class BrowseDormOwner : PageModel
    {

        public void OnGet()
        {
        }
         public IActionResult OnPostDashboard()
        {
            return RedirectToPage("/DashboardOwner");
        }
         public IActionResult OnPostProfile()
        {
            return RedirectToPage("/ProfileOwner");
        }
    }
}