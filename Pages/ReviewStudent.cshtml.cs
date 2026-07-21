using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MongoDB.Driver;

namespace GMCC.Pages
{
    public class ReviewStudent : PageModel
    {
        private readonly MongoDBService _mongoService;
        private readonly IWebHostEnvironment _env;

        public ReviewStudent(MongoDBService mongoService, IWebHostEnvironment env)
        {
            _mongoService = mongoService;
            _env = env;
        }

        [BindProperty(SupportsGet = true)]
        public int DormId { get; set; }

        [BindProperty]
        public int Rating { get; set; }

        [BindProperty]
        public string ReviewText { get; set; } = string.Empty;

        [BindProperty]
        public IFormFile? Photo { get; set; }

        [BindProperty]
        public IFormFile? ProofFile { get; set; }

        public string DormitoryName { get; set; } = "";
        public bool ListingNotFound { get; set; }
        public string? ErrorMessage { get; set; }

        public IActionResult OnGet()
        {
            var studentId = HttpContext.Session.GetInt32("StudentId");
            if (studentId == null)
            {
                return RedirectToPage("/LoginStudent");
            }

            var listing = _mongoService.Listings.Find(l => l.Id == DormId).FirstOrDefault();
            if (listing == null)
            {
                ListingNotFound = true;
                return Page();
            }

            DormitoryName = listing.DormitoryName ?? "";
            return Page();
        }

        public async Task<IActionResult> OnPostSubmit()
        {
            var studentId = HttpContext.Session.GetInt32("StudentId");
            if (studentId == null)
            {
                return RedirectToPage("/LoginStudent");
            }

            var listing = _mongoService.Listings.Find(l => l.Id == DormId).FirstOrDefault();
            if (listing == null)
            {
                ListingNotFound = true;
                return Page();
            }
            DormitoryName = listing.DormitoryName ?? "";

            var student = _mongoService.Students.Find(s => s.Id == studentId.Value).FirstOrDefault();
            if (student == null)
            {
                return RedirectToPage("/LoginStudent");
            }

            if (Rating < 1 || Rating > 5)
            {
                ErrorMessage = "Please select a star rating from 1 to 5.";
                return Page();
            }

            if (string.IsNullOrWhiteSpace(ReviewText))
            {
                ErrorMessage = "Please write a short review before posting.";
                return Page();
            }

            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "reviews", DormId.ToString());
            Directory.CreateDirectory(uploadsFolder);

            string? photoPath = null;
            if (Photo != null && Photo.Length > 0)
            {
                var fileName = $"photo_{Guid.NewGuid()}{Path.GetExtension(Photo.FileName)}";
                var filePath = Path.Combine(uploadsFolder, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await Photo.CopyToAsync(stream);
                }
                photoPath = $"/uploads/reviews/{DormId}/{fileName}";
            }

            string? proofPath = null;
            if (ProofFile != null && ProofFile.Length > 0)
            {
                var fileName = $"proof_{Guid.NewGuid()}{Path.GetExtension(ProofFile.FileName)}";
                var filePath = Path.Combine(uploadsFolder, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ProofFile.CopyToAsync(stream);
                }
                proofPath = $"/uploads/reviews/{DormId}/{fileName}";
            }

            var isVerifiedRenter = (student.RentalVerifications ?? new())
                .Any(r => string.Equals(r.DormitoryName, DormitoryName, StringComparison.OrdinalIgnoreCase))
                || !string.IsNullOrEmpty(proofPath);

            var maxId = _mongoService.Reviews
                .Find(FilterDefinition<Reviews>.Empty)
                .SortByDescending(r => r.Id)
                .Limit(1)
                .FirstOrDefault();
            var nextId = (maxId?.Id ?? 0) + 1;

            var review = new Reviews
            {
                Id = nextId,
                DormId = DormId,
                StudentId = studentId.Value,
                StudentName = $"{student.FirstName} {student.LastName}".Trim(),
                Rating = Rating,
                ReviewText = ReviewText,
                PhotoPath = photoPath,
                ProofPath = proofPath,
                IsVerifiedRenter = isVerifiedRenter,
                CreatedAtUtc = DateTime.UtcNow
            };

            _mongoService.Reviews.InsertOne(review);

            return RedirectToPage("/DormDetailsStudent", new { id = DormId });
        }
    }
}