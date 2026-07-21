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
    public class AddDormOwner : PageModel
    {
        private readonly MongoDBService _mongoService;
        private readonly IWebHostEnvironment _env;

        public AddDormOwner(MongoDBService mongoService, IWebHostEnvironment env)
        {
            _mongoService = mongoService;
            _env = env;
        }

        [BindProperty]
        public string DormitoryName { get; set; } = string.Empty;

        [BindProperty]
        public string AddressLocation { get; set; } = string.Empty;

        [BindProperty]
        public string Description { get; set; } = string.Empty;

        [BindProperty]
        public string MonthlyRent { get; set; } = string.Empty;

        [BindProperty]
        public List<string> NearSchools { get; set; } = new List<string>();

        [BindProperty]
        public List<string> Amenities { get; set; } = new List<string>();

        [BindProperty]
        public string Curfew { get; set; } = string.Empty;

        [BindProperty]
        public string ComfortRoom { get; set; } = string.Empty;

        [BindProperty]
        public string RoomType { get; set; } = string.Empty;

        [BindProperty]
        public List<IFormFile> Photos { get; set; } = new List<IFormFile>();

        public string? ErrorMessage { get; set; }

        public IActionResult OnGet()
        {
            if (GetOwnerId() == null)
            {
                return RedirectToPage("/LoginOwner");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostPublish()
        {
            return await SaveListing(publish: true);
        }

        public async Task<IActionResult> OnPostDraft()
        {
            return await SaveListing(publish: false);
        }

        private async Task<IActionResult> SaveListing(bool publish)
        {
            var ownerId = GetOwnerId();
            if (ownerId == null)
            {
                return RedirectToPage("/LoginOwner");
            }

            if (publish)
            {
                if (string.IsNullOrWhiteSpace(DormitoryName) ||
                    string.IsNullOrWhiteSpace(AddressLocation) ||
                    string.IsNullOrWhiteSpace(MonthlyRent) ||
                    Photos == null || Photos.Count(p => p != null && p.Length > 0) == 0)
                {
                    ErrorMessage = "To publish, please provide the dormitory name, address, monthly rent, and at least one photo.";
                    return Page();
                }
            }

            if (Photos != null && Photos.Count > 8)
            {
                ErrorMessage = "You can upload up to 8 photos only.";
                return Page();
            }

            var maxId = _mongoService.Listings
                .Find(FilterDefinition<dormListing>.Empty)
                .SortByDescending(l => l.Id)
                .Limit(1)
                .FirstOrDefault();
            var nextId = (maxId?.Id ?? 0) + 1;

            var photoPaths = new List<string>();
            if (Photos != null && Photos.Count > 0)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "listings", ownerId.Value.ToString(), nextId.ToString());
                Directory.CreateDirectory(uploadsFolder);

                foreach (var photo in Photos)
                {
                    if (photo == null || photo.Length == 0) continue;

                    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(photo.FileName)}";
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await photo.CopyToAsync(stream);
                    }

                    photoPaths.Add($"/uploads/listings/{ownerId}/{nextId}/{fileName}");
                }
            }

            var listing = new dormListing
            {
                Id = nextId,
                OwnerId = ownerId.Value,
                DormitoryName = DormitoryName,
                AddressLocation = AddressLocation,
                Description = Description,
                NearSchools = NearSchools ?? new List<string>(),
                PhotoPaths = photoPaths,
                Amenities = Amenities ?? new List<string>(),
                Curfew = Curfew,
                ComfortRoom = ComfortRoom,
                RoomType = RoomType,
                MonthlyRent = MonthlyRent,
                Status = publish ? "Vacant" : "Draft",
                DateCreated = DateTime.UtcNow
            };

            _mongoService.Listings.InsertOne(listing);

            return RedirectToPage("/DashboardOwner");
        }

        private int? GetOwnerId()
        {
            return HttpContext.Session.GetInt32("OwnerId");
        }
    }
}