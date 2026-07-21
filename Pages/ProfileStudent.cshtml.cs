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
    public class ProfileStudent : PageModel
    {
        private readonly MongoDBService _mongoService;
        private readonly IWebHostEnvironment _env;

        public ProfileStudent(MongoDBService mongoService, IWebHostEnvironment env)
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
        public IFormFile? ProfilePhoto { get; set; }

        [BindProperty]
        public IFormFile? ProofOfStayFile { get; set; }

        [BindProperty]
        public string CurrentPassword { get; set; } = string.Empty;

        [BindProperty]
        public string NewPassword { get; set; } = string.Empty;

        [BindProperty]
        public string ConfirmNewPassword { get; set; } = string.Empty;

        public string? VerifiedSchool { get; set; }
        public string? VerifiedRentalDorm { get; set; }

        public List<VerificationDocItem> StudentVerification { get; set; } = new();
        public List<VerificationDocItem> RentalVerification { get; set; } = new();

        public IActionResult OnGet()
        {
            var studentId = HttpContext.Session.GetInt32("StudentId");
            if (studentId == null)
            {
                return RedirectToPage("/LoginStudent");
            }

            var student = _mongoService.Students.Find(s => s.Id == studentId.Value).FirstOrDefault();
            if (student == null)
            {
                return RedirectToPage("/LoginStudent");
            }

            LoadFromStudent(student);

            return Page();
        }

        public IActionResult OnPostBrowse()
        {
            return RedirectToPage("/BrowseDormStudent");
        }

        public async Task<IActionResult> OnPostChangePhoto()
        {
            var studentId = HttpContext.Session.GetInt32("StudentId");
            if (studentId == null)
            {
                return RedirectToPage("/LoginStudent");
            }

            var student = _mongoService.Students.Find(s => s.Id == studentId.Value).FirstOrDefault();
            if (student == null)
            {
                return RedirectToPage("/LoginStudent");
            }

            if (ProfilePhoto != null && ProfilePhoto.Length > 0)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "students", studentId.Value.ToString());
                Directory.CreateDirectory(uploadsFolder);

                var fileName = $"profile_{Guid.NewGuid()}{Path.GetExtension(ProfilePhoto.FileName)}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ProfilePhoto.CopyToAsync(stream);
                }

                var relativePath = $"/uploads/students/{studentId.Value}/{fileName}";

                var update = Builders<studentUser>.Update.Set(s => s.ProfilePhotoPath, relativePath);
                _mongoService.Students.UpdateOne(s => s.Id == studentId.Value, update);
            }

            return RedirectToPage("/ProfileStudent");
        }

        public async Task<IActionResult> OnPostUploadProof()
        {
            var studentId = HttpContext.Session.GetInt32("StudentId");
            if (studentId == null)
            {
                return RedirectToPage("/LoginStudent");
            }

            var student = _mongoService.Students.Find(s => s.Id == studentId.Value).FirstOrDefault();
            if (student == null)
            {
                return RedirectToPage("/LoginStudent");
            }

            if (ProofOfStayFile == null || ProofOfStayFile.Length == 0)
            {
                ErrorMessage = "Please choose an image to upload.";
                LoadFromStudent(student);
                return Page();
            }

            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "students", studentId.Value.ToString(), "proof-of-stay");
            Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(ProofOfStayFile.FileName)}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await ProofOfStayFile.CopyToAsync(stream);
            }

            var relativePath = $"/uploads/students/{studentId.Value}/proof-of-stay/{fileName}";

            var entry = new proofOfStayEntry
            {
                FilePath = relativePath,
                UploadedAtUtc = DateTime.UtcNow
            };

            var update = Builders<studentUser>.Update.Push(s => s.ProofOfStayUploads, entry);
            _mongoService.Students.UpdateOne(s => s.Id == studentId.Value, update);

            return RedirectToPage("/ProfileStudent");
        }

        public IActionResult OnPostChangePassword()
        {
            var studentId = HttpContext.Session.GetInt32("StudentId");
            if (studentId == null)
            {
                return RedirectToPage("/LoginStudent");
            }

            var student = _mongoService.Students.Find(s => s.Id == studentId.Value).FirstOrDefault();
            if (student == null)
            {
                return RedirectToPage("/LoginStudent");
            }

            LoadFromStudent(student);

            if (string.IsNullOrWhiteSpace(CurrentPassword) || string.IsNullOrWhiteSpace(NewPassword) || string.IsNullOrWhiteSpace(ConfirmNewPassword))
            {
                ErrorMessage = "Please fill in all password fields.";
                return Page();
            }

            if (student.Password != CurrentPassword)
            {
                ErrorMessage = "Current password is incorrect.";
                return Page();
            }

            if (NewPassword != ConfirmNewPassword)
            {
                ErrorMessage = "New password and confirmation do not match.";
                return Page();
            }

            if (NewPassword.Length < 6)
            {
                ErrorMessage = "New password must be at least 6 characters.";
                return Page();
            }

            var update = Builders<studentUser>.Update.Set(s => s.Password, NewPassword);
            _mongoService.Students.UpdateOne(s => s.Id == studentId.Value, update);

            StatusMessage = "Password updated.";
            return Page();
        }

        public IActionResult OnPostSaveChanges()
        {
            var studentId = HttpContext.Session.GetInt32("StudentId");
            if (studentId == null)
            {
                return RedirectToPage("/LoginStudent");
            }

            var student = _mongoService.Students.Find(s => s.Id == studentId.Value).FirstOrDefault();
            if (student == null)
            {
                return RedirectToPage("/LoginStudent");
            }

            if (string.IsNullOrWhiteSpace(FullName) || string.IsNullOrWhiteSpace(Email))
            {
                ErrorMessage = "Full name and email are required.";
                LoadFromStudent(student);
                return Page();
            }

            var emailTaken = _mongoService.Students
                .Find(s => s.Email == Email && s.Id != studentId.Value)
                .FirstOrDefault();

            if (emailTaken != null)
            {
                ErrorMessage = "That email is already used by another account.";
                LoadFromStudent(student);
                return Page();
            }

            var nameParts = FullName.Trim().Split(' ', 2);
            var firstName = nameParts[0];
            var lastName = nameParts.Length > 1 ? nameParts[1] : "";

            var update = Builders<studentUser>.Update
                .Set(s => s.FirstName, firstName)
                .Set(s => s.LastName, lastName)
                .Set(s => s.Email, Email)
                .Set(s => s.Phone, ContactNumber)
                .Set(s => s.Address, Address);

            _mongoService.Students.UpdateOne(s => s.Id == studentId.Value, update);

            StatusMessage = "Profile updated.";

            var refreshed = _mongoService.Students.Find(s => s.Id == studentId.Value).FirstOrDefault();
            if (refreshed != null)
            {
                LoadFromStudent(refreshed);
            }

            return Page();
        }

        public IActionResult OnPostLogout()
        {
            HttpContext.Session.Clear();
            return RedirectToPage("/LoginStudent");
        }

        private void LoadFromStudent(studentUser student)
        {
            FullName = $"{student.FirstName} {student.LastName}".Trim();
            Email = student.Email ?? "";
            ContactNumber = student.Phone ?? "";
            Address = student.Address ?? "";
            ProfilePhotoPath = student.ProfilePhotoPath;

            var isVerified = student.VerificationStatus == "Approved";
            VerifiedSchool = isVerified ? student.VerifiedSchool : null;

            var latestRental = student.RentalVerifications?
                .OrderByDescending(r => r.VerifiedAtUtc)
                .FirstOrDefault();
            VerifiedRentalDorm = latestRental?.DormitoryName;

            StudentVerification = new List<VerificationDocItem>
            {
                new VerificationDocItem
                {
                    Title = isVerified ? (student.VerifiedSchool ?? "School") : "Student ID Verification",
                    FileName = isVerified && student.VerifiedAtUtc.HasValue
                        ? $"Student ID / COR - {student.VerifiedAtUtc.Value:MMM d, yyyy}"
                        : "No student ID uploaded yet",
                    Verified = isVerified
                }
            };

            var rentalItems = (student.RentalVerifications ?? new List<rentalVerificationEntry>())
                .Select(r => new VerificationDocItem
                {
                    Title = r.DormitoryName,
                    FileName = $"Proof of stay uploaded - {r.VerifiedAtUtc:MMM d, yyyy}",
                    Verified = true
                });

            var proofItems = (student.ProofOfStayUploads ?? new List<proofOfStayEntry>())
                .Select((p, i) => new VerificationDocItem
                {
                    Title = $"Proof of Stay #{i + 1}",
                    FileName = $"Uploaded - {p.UploadedAtUtc:MMM d, yyyy}",
                    Verified = true
                });

            RentalVerification = rentalItems.Concat(proofItems).ToList();

            if (RentalVerification.Count == 0)
            {
                RentalVerification.Add(new VerificationDocItem
                {
                    Title = "No rentals verified yet",
                    FileName = "Upload a proof of stay to add one",
                    Verified = false
                });
            }
        }
    }

    public class VerificationDocItem
    {
        public string Title { get; set; } = "";
        public string FileName { get; set; } = "";
        public bool Verified { get; set; }
    }
}