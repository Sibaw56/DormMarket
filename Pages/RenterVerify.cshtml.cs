using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace DormMarket-main.Pages
{
    public class RenterVerify : PageModel
    {
        public void OnGet()
        {
        }
        public IActionResult OnPostRentedBefore()
        {
            //if user pick rented before they can upload proof of stay, choose rented dorm,stay in and stay out
        }
        public IActionResult OnPostProofofStay()
        {
            //verify uploaded image of proof of stay (Verified/pending/not added)
        }
        public IActionResult OnPostNeverRented()
        {
            //Proceed to login if user pick never rented before
            return RedirectToPage("/LoginStudent");
        }
        public IActionResult OnPostSubmit()
        {
            // check if the image og proof of stay is verified then proceed to Login
            // if not verified, show error message
            return RedirectToPage("/LoginStudent");
        }
    }
}