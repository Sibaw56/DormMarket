using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace GMCC.Pages
{
    public class Message : PageModel
    {
        private readonly ILogger<Message> _logger;
        private readonly MongoDBService _mongoService;

        public Message(ILogger<Message> logger, MongoDBService mongoService)
        {
            _logger = logger;
            _mongoService = mongoService;
        }

        [BindProperty(SupportsGet = true)]
        public int OwnerId { get; set; }

        [BindProperty(SupportsGet = true)]
        public int DormId { get; set; }

        public bool OwnerNotFound { get; set; }

        public string DormitoryName { get; set; } = "";
        public string OwnerName { get; set; } = "";
        public string? OwnerProfilePhotoPath { get; set; }
        public string? MessengerLink { get; set; }
        public string? ContactNumber { get; set; }
        public string? OtherContactLink { get; set; }

        public string? BusinessPermitPath { get; set; }
        public string? PropertyOwnershipPath { get; set; }
        public string? LeaseAuthorizationPath { get; set; }
        public string? OwnerNotes { get; set; }
        public bool HasAnyDocuments =>
            !string.IsNullOrWhiteSpace(BusinessPermitPath) ||
            !string.IsNullOrWhiteSpace(PropertyOwnershipPath) ||
            !string.IsNullOrWhiteSpace(LeaseAuthorizationPath) ||
            !string.IsNullOrWhiteSpace(OwnerNotes);

        public void OnGet()
        {
            var owner = _mongoService.Owners.Find(o => o.Id == OwnerId).FirstOrDefault();

            if (owner == null)
            {
                OwnerNotFound = true;
                return;
            }

            OwnerName = owner.FullName ?? "";
            OwnerProfilePhotoPath = owner.ProfilePhotoPath;
            MessengerLink = owner.MessengerLink;
            ContactNumber = owner.ContactNumber;
            OtherContactLink = owner.OtherContactLink;

            // Government ID is intentionally excluded - it's for verification staff only,
            // not for students to view.
            BusinessPermitPath = owner.BusinessPermitPath;
            PropertyOwnershipPath = owner.PropertyOwnershipPath;
            LeaseAuthorizationPath = owner.LeaseAuthorizationPath;
            OwnerNotes = owner.ReviewerNotes;

            if (DormId > 0)
            {
                var listing = _mongoService.Listings.Find(l => l.Id == DormId).FirstOrDefault();
                DormitoryName = listing?.DormitoryName ?? "";
            }
        }
    }
}