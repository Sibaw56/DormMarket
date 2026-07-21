using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Tesseract;

namespace GMCC.Pages
{
    public class RenterVerify : PageModel
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<RenterVerify> _logger;

        private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".pdf" };
        private static readonly string[] AllowedContentTypes = { "image/jpeg", "image/png", "application/pdf" };
        private const long MaxFileSizeBytes = 10 * 1024 * 1024; // 10 MB limit

        public RenterVerify(IWebHostEnvironment env, ILogger<RenterVerify> logger)
        {
            _env = env;
            _logger = logger;
        }

        // 1. Properties bound to your HTML elements (asp-for match)
        [BindProperty]
        public string Dormitory { get; set; } = string.Empty;

        [BindProperty]
        public DateTime? MoveInDate { get; set; }

        [BindProperty]
        public DateTime? MoveOutDate { get; set; }

        [BindProperty]
        public IFormFile? ProofFile { get; set; }

        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }

        public void OnGet()
        {
        }

        // Handles button: asp-page-handler="NeverRented"
        public IActionResult OnPostNeverRented()
        {
            // Proceed to login if user picks "Never Rented Before"
            return RedirectToPage("/LoginStudent");
        }

        // Handles button: asp-page-handler="SubmitProof"
        public async Task<IActionResult> OnPostSubmitProof()
        {
            // 1. Basic Form Validations
            if (string.IsNullOrWhiteSpace(Dormitory))
            {
                ErrorMessage = "Please select or type the name of the dormitory you rented.";
                return Page();
            }

            if (ProofFile == null || ProofFile.Length == 0)
            {
                ErrorMessage = "Please upload an image of your Proof of Stay.";
                return Page();
            }

            if (ProofFile.Length > MaxFileSizeBytes)
            {
                ErrorMessage = "File is too large. Max size is 10 MB.";
                return Page();
            }

            var ext = Path.GetExtension(ProofFile.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(ext) || !AllowedContentTypes.Contains(ProofFile.ContentType))
            {
                ErrorMessage = "Invalid file type. Please upload a JPG, PNG, or PDF.";
                return Page();
            }

            // 2. Read Image into Memory Bytes
            byte[] imageBytes;
            using (var memoryStream = new MemoryStream())
            {
                await ProofFile.CopyToAsync(memoryStream);
                imageBytes = memoryStream.ToArray();
            }

            // 3. Run OCR Scan matching the selected Dormitory and Student's Name
            var ocrResult = VerifyProofOfStayDocument(imageBytes, Dormitory);
            if (!ocrResult.IsSuccess)
            {
                ErrorMessage = ocrResult.Message;
                return Page();
            }

            // 4. Save file securely to Server Storage
            var storageFolder = Path.Combine(_env.ContentRootPath, "App_Data", "RenterVerifications");
            Directory.CreateDirectory(storageFolder);

            var storedFileName = $"{Guid.NewGuid()}{ext}";
            var fullPath = Path.Combine(storageFolder, storedFileName);

            try
            {
                await System.IO.File.WriteAllBytesAsync(fullPath, imageBytes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save verification file.");
                ErrorMessage = "Something went wrong saving your file. Please try again.";
                return Page();
            }

            // TODO: Update your database status here
            // e.g. User.IsRenterVerified = true;

            // 5. Success! Redirect
            return RedirectToPage("/LoginStudent", new { renterVerification = "approved" });
        }

        private (bool IsSuccess, string Message) VerifyProofOfStayDocument(byte[] imageBytes, string expectedDorm)
        {
            string tessdataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tessdata");

            if (!Directory.Exists(tessdataPath))
            {
                _logger.LogError($"Missing tessdata directory at: {tessdataPath}");
                return (false, "Verification error: Missing OCR language database assets.");
            }

            try
            {
                using var engine = new TesseractEngine(tessdataPath, "eng", EngineMode.Default);
                using var img = Pix.LoadFromMemory(imageBytes);
                using var page = engine.Process(img);

                string extractedText = page.GetText();
                float confidence = page.GetMeanConfidence();

                // Validation A: Ensure image isn't too blurry
                if (confidence < 0.55f)
                {
                    return (false, "The uploaded document is too blurry. Please upload a clearer image.");
                }

                // Standardize casing and spaces for comparisons
                string normalizedExtracted = extractedText.Replace(" ", "").ToLowerInvariant();
                string normalizedDorm = expectedDorm.Replace(" ", "").ToLowerInvariant();

                // Validation B: Check if selected Dorm name appears on document text
                if (!normalizedExtracted.Contains(normalizedDorm))
                {
                    return (false, $"Verification failed. We couldn't find your selected dormitory '{expectedDorm}' mentioned on this document.");
                }

                // Validation C: Verify document belongs to the logged-in student's name
                string registeredName = User.Identity?.Name ?? "";
                if (!string.IsNullOrEmpty(registeredName))
                {
                    string cleanRegisteredName = registeredName.Replace(" ", "").ToLowerInvariant();
                    if (!normalizedExtracted.Contains(cleanRegisteredName))
                    {
                        return (false, $"Verification failed. The name on this document doesn't match your profile name ({registeredName}).");
                    }
                }

                return (true, "Success");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "OCR Engine processing exception.");
                return (false, "Failed to analyze your proof of stay. Make sure it is a valid, uncorrupted image.");
            }
        }
    }
}