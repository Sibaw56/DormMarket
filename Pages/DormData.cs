using System.Collections.Generic;

namespace GMCC.Pages
{
    public class RoomTypeRow
    {
        public string RoomType { get; set; } = string.Empty;
        public string Price { get; set; } = string.Empty;
        public string Availability { get; set; } = "Available";
    }

    public class Dormitory
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Status { get; set; } = "Vacant";
        public string Location { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<string> Amenities { get; set; } = new();
        public List<RoomTypeRow> Rooms { get; set; } = new();
        public string OwnerContactNumber { get; set; } = string.Empty;
        public string MessengerLink { get; set; } = string.Empty;
        public string OtherContactLink { get; set; } = string.Empty;
    }

    // TODO: replace with real data from the database
    public static class DummyDormitories
    {
        public static List<Dormitory> All = new()
        {
            new Dormitory
            {
                Id = "casa-verde",
                Name = "Casa-Verde Dormitory",
                Status = "Vacant",
                Location = "Near Cebu Institute of Technology-University (CIT-U)",
                Description = "Cozy, secure dormitory just a 5-minute walk from Cebu Institute of Technology - U. Rooms are shared (4 pax) with own CR, aircon, and free WiFi. CCTV-monitored premises, 10PM curfew, and on-site parking available. Perfect for students who want a safe, budget-friendly place close to campus.",
                Amenities = new List<string> { "WiFi", "Aircon", "CCTV", "Own CR", "Curfew: 10 PM", "Parking" },
                OwnerContactNumber = "0917 123 4567",
                MessengerLink = "https://m.me/casaverdedorm",
                OtherContactLink = "https://wa.me/639171234567",
                Rooms = new List<RoomTypeRow>
                {
                    new RoomTypeRow { RoomType = "Shared (4 pax)", Price = "P 3,500", Availability = "Available" },
                    new RoomTypeRow { RoomType = "Solo Room", Price = "P 5,800", Availability = "Available" },
                },
            },
            new Dormitory
            {
                Id = "blue-haven",
                Name = "Blue Haven Residence",
                Status = "Vacant",
                Location = "Near University of San Carlos (USC)",
                Description = "A quiet residence a short jeepney ride from USC. Shared rooms come with a common CR, while solo rooms have aircon. Laundry area and free WiFi included. No curfew, so it suits students with irregular class schedules.",
                Amenities = new List<string> { "WiFi", "Laundry", "Aircon", "Shared CR", "No Curfew" },
                OwnerContactNumber = "0918 234 5678",
                MessengerLink = "https://m.me/bluehavenresidence",
                OtherContactLink = "https://wa.me/639182345678",
                Rooms = new List<RoomTypeRow>
                {
                    new RoomTypeRow { RoomType = "Shared (4 pax)", Price = "P 3,200", Availability = "Full" },
                    new RoomTypeRow { RoomType = "Solo Room", Price = "P 6,000", Availability = "Available" },
                },
            },
            new Dormitory
            {
                Id = "sunset-suites",
                Name = "Sunset Suites",
                Status = "Vacant",
                Location = "Near University of San Jose-Recoletos (USJR)",
                Description = "Budget-friendly bedspace and solo rooms near USJR. Basic amenities with 24/7 security and a shared kitchen. A great pick for students looking to save on rent without sacrificing safety.",
                Amenities = new List<string> { "WiFi", "Kitchen", "CCTV", "Shared CR" },
                OwnerContactNumber = "0919 345 6789",
                MessengerLink = "https://m.me/sunsetsuites",
                OtherContactLink = "https://wa.me/639193456789",
                Rooms = new List<RoomTypeRow>
                {
                    new RoomTypeRow { RoomType = "Bedspace", Price = "P 2,000", Availability = "Available" },
                    new RoomTypeRow { RoomType = "Solo Room", Price = "P 5,500", Availability = "Full" },
                },
            },
            new Dormitory
            {
                Id = "green-court",
                Name = "Green Court Dorm",
                Status = "Vacant",
                Location = "Near University of Cebu (UC)",
                Description = "Spacious shared and bedspace rooms with a garden common area. Own CR available per unit, plus on-site parking for students with motorbikes. Strict 10PM curfew for a study-friendly environment.",
                Amenities = new List<string> { "WiFi", "Own CR", "Parking", "Curfew: 10 PM" },
                OwnerContactNumber = "0920 456 7890",
                MessengerLink = "https://m.me/greencourtdorm",
                OtherContactLink = "https://wa.me/639204567890",
                Rooms = new List<RoomTypeRow>
                {
                    new RoomTypeRow { RoomType = "Shared (4 pax)", Price = "P 3,000", Availability = "Available" },
                    new RoomTypeRow { RoomType = "Bedspace", Price = "P 1,800", Availability = "Available" },
                },
            },
        };
    }
}
