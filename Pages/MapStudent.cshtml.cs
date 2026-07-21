using System;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MongoDB.Driver;

namespace GMCC.Pages
{
    public class MapStudent : PageModel
    {
        private readonly MongoDBService _mongoService;

        public MapStudent(MongoDBService mongoService)
        {
            _mongoService = mongoService;
        }

        [BindProperty(SupportsGet = true)]
        public int DormId { get; set; }

        public bool ListingNotFound { get; set; }
        public string DormitoryName { get; set; } = "";
        public string Location { get; set; } = "";
        public string? NearestSchool { get; set; }
        public string EmbedMapUrl { get; set; } = "";
        public string OpenInGoogleMapsUrl { get; set; } = "";

        public void OnGet()
        {
            var listing = _mongoService.Listings.Find(l => l.Id == DormId).FirstOrDefault();

            if (listing == null)
            {
                ListingNotFound = true;
                return;
            }

            DormitoryName = listing.DormitoryName ?? "";
            Location = listing.AddressLocation ?? "";
            NearestSchool = (listing.NearSchools != null && listing.NearSchools.Count > 0)
                ? listing.NearSchools[0]
                : null;

            var query = WebUtility.UrlEncode($"{DormitoryName}, {Location}");
            EmbedMapUrl = $"https://www.google.com/maps?q={query}&output=embed";
            OpenInGoogleMapsUrl = $"https://www.google.com/maps?q={query}";
        }
    }
}