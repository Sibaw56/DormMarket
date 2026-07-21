using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MongoDB.Driver;

namespace GMCC.Pages
{
    public class ProfileOwner : PageModel
    {
        private readonly MongoDBService _mongoService;
        private readonly IWebHostEnvironment _env;

        public ProfileOwner(MongoDBService mongoService, IWebHostEnvironment env)
        {
            _mongoService = mongoService;
            _env = env;
        }

        public string? StatusMessage { get; set; }
        public string? ErrorMessage { get; set; }
        public string? ProfilePhotoPath { get; set; }

        [BindProperty]
        public string FullName { get; set; } = string.Empty;

        [BindProperty]
        public string Email { get; set; } = string.Empty;

        [BindProperty]
        public string ContactNumber { get; set; } = string.Empty;

        [BindProperty]
        public string Address { get; set; } = string.Empty;

        [BindProperty]
        public string MessengerLink { get; set; } = string.Empty;

        [BindProperty]
        public string OtherContactLink { get; set; } = string.Empty;

        [BindProperty]
        public IFormFile? ProfilePhoto { get; set; }

        public List<OwnerVerificationDocItem> Documents { get; set; } = new();

        public IActionResult OnGet()
        {
            var ownerId = HttpContext.Session.GetInt32("OwnerId");
            if (ownerId == null)
            {
                return RedirectToPage("/LoginOwner");
            }

            var owner = _mongoService.Owners.Find(o => o.Id == ownerId.Value).FirstOrDefault();
            if (owner == null)
            {
                return RedirectToPage("/LoginOwner");
            }

            LoadFromOwner(owner);

            return Page();
        }

        public IActionResult OnPostDashboard()
        {
            return RedirectToPage("/DashboardOwner");
        }

        public IActionResult OnPostListing()
        {
            return RedirectToPage("/BrowseDormOwner");
        }

        public IActionResult OnPostUploadDocuments()
        {
            var ownerId = HttpContext.Session.GetInt32("OwnerId");
            if (ownerId == null)
            {
                return RedirectToPage("/LoginOwner");
            }

            return RedirectToPage("/VerifyOwner", new { ownerId = ownerId.Value });
        }

        public async Task<IActionResult> OnPostChangePhoto()
        {
            var ownerId = HttpContext.Session.GetInt32("OwnerId");
            if (ownerId == null)
            {
                return RedirectToPage("/LoginOwner");
            }

            var owner = _mongoService.Owners.Find(o => o.Id == ownerId.Value).FirstOrDefault();
            if (owner == null)
            {
                return RedirectToPage("/LoginOwner");
            }

            if (ProfilePhoto != null && ProfilePhoto.Length > 0)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "owners", ownerId.Value.ToString());
                Directory.CreateDirectory(uploadsFolder);

                var fileName = $"profile_{Guid.NewGuid()}{Path.GetExtension(ProfilePhoto.FileName)}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ProfilePhoto.CopyToAsync(stream);
                }

                var relativePath = $"/uploads/owners/{ownerId.Value}/{fileName}";

                var update = Builders<ownerUser>.Update.Set(o => o.ProfilePhotoPath, relativePath);
                _mongoService.Owners.UpdateOne(o => o.Id == ownerId.Value, update);
            }

            return RedirectToPage("/ProfileOwner");
        }

        public IActionResult OnPostSaveChanges()
        {
            var ownerId = HttpContext.Session.GetInt32("OwnerId");
            if (ownerId == null)
            {
                return RedirectToPage("/LoginOwner");
            }

            var owner = _mongoService.Owners.Find(o => o.Id == ownerId.Value).FirstOrDefault();
            if (owner == null)
            {
                return RedirectToPage("/LoginOwner");
            }

            if (string.IsNullOrWhiteSpace(FullName) || string.IsNullOrWhiteSpace(Email))
            {
                ErrorMessage = "Full name and email are required.";
                LoadFromOwner(owner);
                return Page();
            }

            var emailTaken = _mongoService.Owners
                .Find(o => o.Email == Email && o.Id != ownerId.Value)
                .FirstOrDefault();

            if (emailTaken != null)
            {
                ErrorMessage = "That email is already used by another account.";
                LoadFromOwner(owner);
                return Page();
            }

            var update = Builders<ownerUser>.Update
                .Set(o => o.FullName, FullName)
                .Set(o => o.Email, Email)
                .Set(o => o.ContactNumber, ContactNumber)
                .Set(o => o.Address, Address)
                .Set(o => o.MessengerLink, MessengerLink)
                .Set(o => o.OtherContactLink, OtherContactLink);

            _mongoService.Owners.UpdateOne(o => o.Id == ownerId.Value, update);

            HttpContext.Session.SetString("OwnerName", FullName ?? "");

            StatusMessage = "Profile updated.";

            var refreshed = _mongoService.Owners.Find(o => o.Id == ownerId.Value).FirstOrDefault();
            if (refreshed != null)
            {
                LoadFromOwner(refreshed);
            }

            return Page();
        }

        public IActionResult OnPostLogout()
        {
            HttpContext.Session.Clear();
            return RedirectToPage("/LoginOwner");
        }

        private void LoadFromOwner(ownerUser owner)
        {
            FullName = owner.FullName ?? "";
            Email = owner.Email ?? "";
            ContactNumber = owner.ContactNumber ?? "";
            Address = owner.Address ?? "";
            MessengerLink = owner.MessengerLink ?? "";
            OtherContactLink = owner.OtherContactLink ?? "";
            ProfilePhotoPath = owner.ProfilePhotoPath;

            Documents = new List<OwnerVerificationDocItem>
            {
                new OwnerVerificationDocItem { Title = "Business Permit / Mayor's Permit", FileName = FileLabel(owner.BusinessPermitPath), Verified = !string.IsNullOrEmpty(owner.BusinessPermitPath) },
                new OwnerVerificationDocItem { Title = "Government-Issued ID", FileName = FileLabel(owner.GovernmentIdPath), Verified = !string.IsNullOrEmpty(owner.GovernmentIdPath) },
                new OwnerVerificationDocItem { Title = "Certificate of Property Ownership / Land Title", FileName = FileLabel(owner.PropertyOwnershipPath), Verified = !string.IsNullOrEmpty(owner.PropertyOwnershipPath) },
                new OwnerVerificationDocItem { Title = "Lease / Management Authorization", FileName = FileLabel(owner.LeaseAuthorizationPath), Verified = !string.IsNullOrEmpty(owner.LeaseAuthorizationPath) },
            };
        }

        private string FileLabel(string? path)
        {
            return string.IsNullOrEmpty(path) ? "No file uploaded" : Path.GetFileName(path);
        }
    }

    public class OwnerVerificationDocItem
    {
        public string Title { get; set; } = "";
        public string FileName { get; set; } = "";
        public bool Verified { get; set; }
    }
}