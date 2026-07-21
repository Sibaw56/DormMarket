using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MongoDB.Driver;

namespace GMCC.Pages
{
    public class DormDetailsStudent : PageModel
    {
        private readonly MongoDBService _mongoService;

        public DormDetailsStudent(MongoDBService mongoService)
        {
            _mongoService = mongoService;
        }

        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }

        public bool ListingNotFound { get; set; }

        public string DormitoryName { get; set; } = "";
        public string Location { get; set; } = "";
        public string Description { get; set; } = "";
        public string Status { get; set; } = "";
        public string? MainPhotoPath { get; set; }
        public List<string> GalleryPhotoPaths { get; set; } = new();
        public List<string> Amenities { get; set; } = new();
        public List<string> NearSchools { get; set; } = new();
        public string Curfew { get; set; } = "";
        public string ComfortRoom { get; set; } = "";
        public List<DormDetailRoomInfo> Rooms { get; set; } = new();
        public string StartingPrice { get; set; } = "";
        public int OwnerId { get; set; }

        public IActionResult OnGet()
        {
            var listing = _mongoService.Listings.Find(l => l.Id == Id).FirstOrDefault();

            if (listing == null)
            {
                ListingNotFound = true;
                return Page();
            }

            DormitoryName = listing.DormitoryName ?? "";
            Location = listing.AddressLocation ?? "";
            Description = listing.Description ?? "";
            Status = listing.Status ?? "";
            Amenities = listing.Amenities ?? new List<string>();
            NearSchools = listing.NearSchools ?? new List<string>();
            Curfew = listing.Curfew ?? "";
            ComfortRoom = listing.ComfortRoom ?? "";
            OwnerId = listing.OwnerId;

            if (listing.PhotoPaths != null && listing.PhotoPaths.Count > 0)
            {
                MainPhotoPath = listing.PhotoPaths[0];
                GalleryPhotoPaths = listing.PhotoPaths.Skip(1).ToList();
            }

            Rooms = BuildRoomInfo(listing);
            StartingPrice = Rooms.Count > 0 ? Rooms[0].Price : FormatPrice(listing.MonthlyRent);

            return Page();
        }

        private List<DormDetailRoomInfo> BuildRoomInfo(dormListing l)
        {
            if (l.Rooms != null && l.Rooms.Count > 0)
            {
                return l.Rooms.Select(r => new DormDetailRoomInfo
                {
                    RoomTypeName = r.RoomTypeName ?? "",
                    Price = FormatPrice(r.PricePerMonth),
                    Availability = string.IsNullOrWhiteSpace(r.Availability) ? "Available" : r.Availability
                }).ToList();
            }

            if (!string.IsNullOrWhiteSpace(l.MonthlyRent))
            {
                return new List<DormDetailRoomInfo>
                {
                    new DormDetailRoomInfo
                    {
                        RoomTypeName = l.RoomType ?? "",
                        Price = FormatPrice(l.MonthlyRent),
                        Availability = string.IsNullOrWhiteSpace(l.Status) ? "Available" : l.Status
                    }
                };
            }

            return new List<DormDetailRoomInfo>();
        }

        private string FormatPrice(string? raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return "";

            var cleaned = new string(raw.Where(c => char.IsDigit(c) || c == '.').ToArray());
            if (decimal.TryParse(cleaned, NumberStyles.Any, CultureInfo.InvariantCulture, out var value))
            {
                return $"₱{value:N0}";
            }

            return raw;
        }
    }

    public class DormDetailRoomInfo
    {
        public string RoomTypeName { get; set; } = "";
        public string Price { get; set; } = "";
        public string Availability { get; set; } = "Available";
    }
}