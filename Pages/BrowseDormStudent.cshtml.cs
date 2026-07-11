using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace GMCC.Pages
{
    public class BrowseDormStudent : PageModel
    {


        public void OnGet()
        {
        }
        public IActionResult OnPostProfile()
        {
            return RedirectToPage("/ProfileStudent");
        }
    }
}