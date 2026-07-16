using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Tesseract;

namespace GMCC.Pages
{
    public class VerifyStudent : PageModel
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<VerifyStudent> _logger;

        private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png" };
        private static readonly string[] AllowedContentTypes = { "image/jpeg", "image/png" };
        private const long MaxFileSizeBytes = 10 * 1024 * 1024;

        public VerifyStudent(IWebHostEnvironment env, ILogger<VerifyStudent> logger)
        {
            _env = env;
            _logger = logger;
        }

        [BindProperty]
        public string School { get; set; } = string.Empty;

        [BindProperty]
        public string? SchoolEmail { get; set; }

        [BindProperty]
        public IFormFile? StudentIdFile { get; set; }

        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }
        public bool IsPendingReview { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostSubmit()
        {
            if (string.IsNullOrWhiteSpace(School))
            {
                ErrorMessage = "Please enter your school/university name.";
                return Page();
            }

            if (StudentIdFile == null || StudentIdFile.Length == 0)
            {
                ErrorMessage = "Please upload your student ID.";
                return Page();
            }

            if (StudentIdFile.Length > MaxFileSizeBytes)
            {
                ErrorMessage = "File is too large. Max size is 10 MB.";
                return Page();
            }

            var ext = Path.GetExtension(StudentIdFile.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(ext) || !AllowedContentTypes.Contains(StudentIdFile.ContentType))
            {
                ErrorMessage = "Invalid file type. Please upload a JPG or PNG image.";
                return Page();
            }

            if (!string.IsNullOrWhiteSpace(SchoolEmail) && !SchoolEmail.Contains('@'))
            {
                ErrorMessage = "Please enter a valid school email, or leave it blank.";
                return Page();
            }

            byte[] imageBytes;
            using (var memoryStream = new MemoryStream())
            {
                await StudentIdFile.CopyToAsync(memoryStream);
                imageBytes = memoryStream.ToArray();
            }

            var ocrResult = RunLocalOcrScan(imageBytes, School);
            if (!ocrResult.IsSuccess)
            {
                ErrorMessage = ocrResult.Message;
                return Page();
            }

            var storageFolder = Path.Combine(_env.ContentRootPath, "App_Data", "StudentVerifications");
            Directory.CreateDirectory(storageFolder);

            var storedFileName = $"{Guid.NewGuid()}{ext}";
            var fullPath = Path.Combine(storageFolder, storedFileName);

            try
            {
                await System.IO.File.WriteAllBytesAsync(fullPath, imageBytes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save student verification file.");
                ErrorMessage = "Something went wrong saving your file. Please try again.";
                return Page();
            }

            var verification = new StudentVerificationRequest
            {
                School = School,
                SchoolEmail = SchoolEmail,
                StoredFileName = storedFileName,
                Status = VerificationStatus.Approved, 
                SubmittedAtUtc = DateTime.UtcNow,
                UserId = User.Identity?.Name
            };// put this in database

            return RedirectToPage("/RenterVerify");
        }

        private (bool IsSuccess, string Message) RunLocalOcrScan(byte[] imageBytes, string userInputSchool)
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

                if (confidence < 0.55f)
                {
                    return (false, "The uploaded image is too blurry. Please try again with a clearer, brighter photo.");
                }

                string normalizedExtracted = extractedText.Replace(" ", "").ToLowerInvariant();

                string[] inputWords = userInputSchool.ToLowerInvariant()
                    .Split(new[] { ' ', '-', ',', '.' }, StringSplitOptions.RemoveEmptyEntries);

                int matchCount = inputWords.Count(word => normalizedExtracted.Contains(word));

                if (matchCount < 2)
                {
                    return (false, $"Verification failed. We couldn't match '{userInputSchool}' with the text read on this ID card.");
                }

                var idMatch = Regex.Match(extractedText, @"\b\d{2}-\d{4}-\d{3}\b");
                if (!idMatch.Success)
                {
                    return (false, "Verification failed. We could not read a valid Student ID number pattern on your card.");
                }
                string detectedIdNumber = idMatch.Value; 

                string registeredName = User.Identity?.Name ?? ""; 
                if (!string.IsNullOrEmpty(registeredName))
                {
                    string cleanRegisteredName = registeredName.Replace(" ", "").ToLowerInvariant();
                    if (!normalizedExtracted.Contains(cleanRegisteredName))
                    {
                        return (false, $"Verification failed. The name on this ID doesn't match your profile name ({registeredName}).");
                    }
                }

                return (true, "Success");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "OCR Engine Exception occurred.");
                return (false, "Failed to analyze ID card. Ensure you uploaded a clear, uncorrupted image.");
            }
        }

        public IActionResult OnPostSkip()
        {
            return RedirectToPage("/RenterVerify");
        }
    }

    public enum VerificationStatus
    {
        PendingReview,
        Approved,
        Rejected
    }

    public class StudentVerificationRequest
    {
        public int Id { get; set; }
        public string School { get; set; } = string.Empty;
        public string? SchoolEmail { get; set; }
        public string StoredFileName { get; set; } = string.Empty;
        public VerificationStatus Status { get; set; }
        public DateTime SubmittedAtUtc { get; set; }
        public string? UserId { get; set; }
    }
}