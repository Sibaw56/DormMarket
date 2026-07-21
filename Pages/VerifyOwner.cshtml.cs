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
    public class VerifyOwner : PageModel
    {
        private readonly MongoDBService _mongoService;
        private readonly IWebHostEnvironment _env;

        public VerifyOwner(MongoDBService mongoService, IWebHostEnvironment env)
        {
            _mongoService = mongoService;
            _env = env;
        }

        [BindProperty(SupportsGet = true)]
        public int OwnerId { get; set; }

        [BindProperty]
        public IFormFile BusinessPermit { get; set; }

        [BindProperty]
        public IFormFile GovernmentId { get; set; }

        [BindProperty]
        public IFormFile PropertyOwnership { get; set; }

        [BindProperty]
        public IFormFile LeaseAuthorization { get; set; }

        [BindProperty]
        public string Notes { get; set; }

        public string ErrorMessage { get; set; }
        public string SuccessMessage { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var owner = _mongoService.Owners.Find(o => o.Id == OwnerId).FirstOrDefault();
            if (owner == null)
            {
                ErrorMessage = "Owner account not found.";
                return Page();
            }

            if (BusinessPermit == null || GovernmentId == null ||
                (PropertyOwnership == null && LeaseAuthorization == null))
            {
                ErrorMessage = "Please upload the Business Permit, Government ID, and either a Property Ownership document or a Lease/Management Authorization.";
                return Page();
            }

            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "verification", OwnerId.ToString());
            Directory.CreateDirectory(uploadsFolder);

            var update = Builders<ownerUser>.Update.Set(o => o.ReviewerNotes, Notes);

            update = await AttachFile(update, BusinessPermit, uploadsFolder, "business_permit",
                (u, path) => u.Set(o => o.BusinessPermitPath, path));

            update = await AttachFile(update, GovernmentId, uploadsFolder, "government_id",
                (u, path) => u.Set(o => o.GovernmentIdPath, path));

            update = await AttachFile(update, PropertyOwnership, uploadsFolder, "property_ownership",
                (u, path) => u.Set(o => o.PropertyOwnershipPath, path));

            update = await AttachFile(update, LeaseAuthorization, uploadsFolder, "lease_authorization",
                (u, path) => u.Set(o => o.LeaseAuthorizationPath, path));

            _mongoService.Owners.UpdateOne(o => o.Id == OwnerId, update);

            return RedirectToPage("/LoginOwner");
        }

        private async Task<UpdateDefinition<ownerUser>> AttachFile(
            UpdateDefinition<ownerUser> update,
            IFormFile file,
            string uploadsFolder,
            string prefix,
            Func<UpdateDefinitionBuilder<ownerUser>, string, UpdateDefinition<ownerUser>> setField)
        {
            if (file == null || file.Length == 0)
            {
                return update;
            }

            var fileName = $"{prefix}_{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var relativePath = $"/uploads/verification/{OwnerId}/{fileName}";
            var fieldUpdate = setField(Builders<ownerUser>.Update, relativePath);

            return Builders<ownerUser>.Update.Combine(update, fieldUpdate);
        }
    }
}