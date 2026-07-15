using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace DormMarket-main.Pages
{
    public class VerifyStudent : PageModel
    {
        public void OnGet()
        {
        }

        public IActionResult OnPOstVeryID()
        {
            //perform verify student ID after uploading (Verified/pending/not added)
        }
        public IActionResult OnPostSubmit()
        {
            // check if the student ID is verified then proceed to Login
            // if not verified, show error message
            return RedirectToPage("/RemterVerify");
        }
    }
}