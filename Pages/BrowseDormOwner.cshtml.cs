using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MongoDB.Driver;

namespace GMCC.Pages
{
    public class BrowseDormOwner : PageModel
    {
        private readonly MongoDBService _mongoService;

        public BrowseDormOwner(MongoDBService mongoService)
        {
            _mongoService = mongoService;
        }

        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }

        [BindProperty]
        public string DormitoryName { get; set; } = string.Empty;

        [BindProperty]
        public List<string> RoomTypeNames { get; set; } = new List<string>();

        [BindProperty]
        public List<string> RoomPrices { get; set; } = new List<string>();

        [BindProperty]
        public List<string> RoomAvailabilities { get; set; } = new List<string>();

        public string? ErrorMessage { get; set; }

        public IActionResult OnGet()
        {
            var ownerId = HttpContext.Session.GetInt32("OwnerId");
            if (ownerId == null)
            {
                return RedirectToPage("/LoginOwner");
            }

            var listing = _mongoService.Listings.Find(l => l.Id == Id).FirstOrDefault();

            if (listing == null || listing.OwnerId != ownerId.Value)
            {
                return RedirectToPage("/DashboardOwner");
            }

            DormitoryName = listing.DormitoryName;
            RoomTypeNames = listing.Rooms.Select(r => r.RoomTypeName).ToList();
            RoomPrices = listing.Rooms.Select(r => r.PricePerMonth).ToList();
            RoomAvailabilities = listing.Rooms.Select(r => r.Availability).ToList();

            return Page();
        }

        public IActionResult OnPostSaveChanges()
        {
            var ownerId = HttpContext.Session.GetInt32("OwnerId");
            if (ownerId == null)
            {
                return RedirectToPage("/LoginOwner");
            }

            var listing = _mongoService.Listings.Find(l => l.Id == Id).FirstOrDefault();
            if (listing == null || listing.OwnerId != ownerId.Value)
            {
                return RedirectToPage("/DashboardOwner");
            }

            var rooms = new List<roomTypeEntry>();
            for (int i = 0; i < RoomTypeNames.Count; i++)
            {
                if (string.IsNullOrWhiteSpace(RoomTypeNames[i])) continue;

                rooms.Add(new roomTypeEntry
                {
                    RoomTypeName = RoomTypeNames[i],
                    PricePerMonth = i < RoomPrices.Count ? RoomPrices[i] : "",
                    Availability = i < RoomAvailabilities.Count ? RoomAvailabilities[i] : "Available"
                });
            }

            var update = Builders<dormListing>.Update
                .Set(l => l.Rooms, rooms)
                .Set(l => l.Status, rooms.Any(r => r.Availability == "Available") ? "Vacant" : "Occupied");

            _mongoService.Listings.UpdateOne(l => l.Id == Id, update);

            return RedirectToPage("/BrowseDormOwner", new { id = Id });
        }

        public IActionResult OnPostDeleteListing()
        {
            var ownerId = HttpContext.Session.GetInt32("OwnerId");
            if (ownerId == null)
            {
                return RedirectToPage("/LoginOwner");
            }

            var listing = _mongoService.Listings.Find(l => l.Id == Id).FirstOrDefault();
            if (listing == null || listing.OwnerId != ownerId.Value)
            {
                return RedirectToPage("/DashboardOwner");
            }

            _mongoService.Listings.DeleteOne(l => l.Id == Id);

            return RedirectToPage("/DashboardOwner");
        }
    }
}