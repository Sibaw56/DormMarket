using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MongoDB.Driver;

namespace GMCC.Pages
{
    public class DashboardOwner : PageModel
    {
        private readonly MongoDBService _mongoService;

        public DashboardOwner(MongoDBService mongoService)
        {
            _mongoService = mongoService;
        }

        public string? ListingStatus { get; set; }
        public int ActiveListingsCount { get; set; }
        public List<OwnerListingItem> Listings { get; set; } = new();

        public IActionResult OnGet()
        {
            var ownerId = HttpContext.Session.GetInt32("OwnerId");
            if (ownerId == null)
            {
                return RedirectToPage("/LoginOwner");
            }

            var myListings = _mongoService.Listings
                .Find(l => l.OwnerId == ownerId.Value)
                .SortByDescending(l => l.DateCreated)
                .ToList();

            Listings = myListings.Select(l => new OwnerListingItem
            {
                Id = l.Id.ToString(),
                Status = l.Status,
                ThumbnailPath = l.PhotoPaths != null && l.PhotoPaths.Count > 0 ? l.PhotoPaths[0] : null,
                DormitoryName = l.DormitoryName
            }).ToList();

            ActiveListingsCount = myListings.Count(l => l.Status == "Vacant" || l.Status == "Occupied");

            return Page();
        }

        public IActionResult OnPostProfile()
        {
            return RedirectToPage("/ProfileOwner");
        }
    }

    public class OwnerListingItem
    {
        public string Id { get; set; } = "";
        public string Status { get; set; } = "";
        public string? ThumbnailPath { get; set; }
        public string? DormitoryName { get; set; }
    }
}