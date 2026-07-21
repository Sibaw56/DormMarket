using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MongoDB.Driver;

namespace GMCC.Pages
{
    public class BrowseDormStudent : PageModel
    {
        private readonly MongoDBService _mongoService;

        public BrowseDormStudent(MongoDBService mongoService)
        {
            _mongoService = mongoService;
        }

        [BindProperty(SupportsGet = true)]
        public int MaxPrice { get; set; } = 10000;

        [BindProperty(SupportsGet = true)]
        public List<string> NearSchools { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public List<string> RoomTypes { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public List<string> Curfew { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public List<string> ComfortRoom { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public List<string> Wifi { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string Search { get; set; } = string.Empty;

        public List<DormListingItem> Results { get; set; } = new();

        public void OnGet()
        {
            var allListings = _mongoService.Listings
                .Find(l => l.Status == "Vacant" || l.Status == "Occupied")
                .ToList();

            var filtered = allListings.Where(MatchesFilters).ToList();

            Results = filtered.Select(l => new DormListingItem
            {
                Id = l.Id.ToString(),
                Name = l.DormitoryName ?? "",
                Location = l.AddressLocation ?? "",
                ThumbnailPath = l.PhotoPaths != null && l.PhotoPaths.Count > 0 ? l.PhotoPaths[0] : null,
                Rooms = BuildRoomInfo(l)
            }).ToList();
        }

        public IActionResult OnPostProfile()
        {
            return RedirectToPage("/ProfileStudent");
        }

        private bool MatchesFilters(dormListing l)
        {
            if (!string.IsNullOrWhiteSpace(Search))
            {
                var term = Search.Trim();
                var nameMatch = (l.DormitoryName ?? "").Contains(term, StringComparison.OrdinalIgnoreCase);
                var locationMatch = (l.AddressLocation ?? "").Contains(term, StringComparison.OrdinalIgnoreCase);
                if (!nameMatch && !locationMatch) return false;
            }

            if (NearSchools.Count > 0)
            {
                var listingSchools = l.NearSchools ?? new List<string>();
                if (!NearSchools.Any(s => listingSchools.Contains(s))) return false;
            }

            if (RoomTypes.Count > 0)
            {
                if (string.IsNullOrEmpty(l.RoomType) || !RoomTypes.Contains(l.RoomType)) return false;
            }

            if (Curfew.Count > 0)
            {
                if (string.IsNullOrEmpty(l.Curfew) || !Curfew.Contains(l.Curfew)) return false;
            }

            if (ComfortRoom.Count > 0)
            {
                if (string.IsNullOrEmpty(l.ComfortRoom) || !ComfortRoom.Contains(l.ComfortRoom)) return false;
            }

            var wantsWifi = Wifi.Contains("HasWifi");
            var wantsNoWifi = Wifi.Contains("NoWifi");
            if (wantsWifi && !wantsNoWifi)
            {
                var amenities = l.Amenities ?? new List<string>();
                if (!amenities.Contains("WiFi")) return false;
            }
            else if (wantsNoWifi && !wantsWifi)
            {
                var amenities = l.Amenities ?? new List<string>();
                if (amenities.Contains("WiFi")) return false;
            }

            var lowestPrice = GetLowestPrice(l);
            if (lowestPrice.HasValue && lowestPrice.Value > MaxPrice) return false;

            return true;
        }

        private decimal? GetLowestPrice(dormListing l)
        {
            var prices = new List<decimal>();

            if (l.Rooms != null)
            {
                foreach (var room in l.Rooms)
                {
                    var parsed = ParsePrice(room.PricePerMonth);
                    if (parsed.HasValue) prices.Add(parsed.Value);
                }
            }

            if (prices.Count == 0)
            {
                var fallback = ParsePrice(l.MonthlyRent);
                if (fallback.HasValue) prices.Add(fallback.Value);
            }

            return prices.Count > 0 ? prices.Min() : (decimal?)null;
        }

        private decimal? ParsePrice(string? raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return null;

            var cleaned = new string(raw.Where(c => char.IsDigit(c) || c == '.').ToArray());
            if (decimal.TryParse(cleaned, NumberStyles.Any, CultureInfo.InvariantCulture, out var value))
            {
                return value;
            }

            return null;
        }

        private List<DormRoomInfo> BuildRoomInfo(dormListing l)
        {
            if (l.Rooms != null && l.Rooms.Count > 0)
            {
                return l.Rooms
                    .Select(r => new DormRoomInfo { Price = FormatPrice(r.PricePerMonth) })
                    .ToList();
            }

            if (!string.IsNullOrWhiteSpace(l.MonthlyRent))
            {
                return new List<DormRoomInfo> { new DormRoomInfo { Price = FormatPrice(l.MonthlyRent) } };
            }

            return new List<DormRoomInfo>();
        }

        private string FormatPrice(string? raw)
        {
            var parsed = ParsePrice(raw);
            return parsed.HasValue ? $"P {parsed.Value:N0}" : (raw ?? "");
        }
    }

    public class DormRoomInfo
    {
        public string Price { get; set; } = "";
    }

    public class DormListingItem
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string Location { get; set; } = "";
        public string? ThumbnailPath { get; set; }
        public List<DormRoomInfo> Rooms { get; set; } = new();
    }
}