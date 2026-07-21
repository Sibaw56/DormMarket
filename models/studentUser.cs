using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

[BsonIgnoreExtraElements]
public class studentUser
{
    [BsonId]
    public int Id { get; set; }

    [BsonElement("first_Name")]
    public string FirstName { get; set; }

    [BsonElement("last_Name")]
    public string LastName { get; set; }

    [BsonElement("email")]
    public string Email { get; set; }

    [BsonElement("password")]
    public string Password { get; set; }

    [BsonElement("phone")]
    public string Phone { get; set; }

    [BsonElement("date_Joined")]
    public DateTime DateJoined { get; set; }

    [BsonElement("id_image_path")]
    public string? IdImagePath { get; set; }

    [BsonElement("verification_status")]
    public string? VerificationStatus { get; set; }

    [BsonElement("verified_at_utc")]
    public DateTime? VerifiedAtUtc { get; set; }

    [BsonElement("address")]
    public string? Address { get; set; }

    [BsonElement("verified_School")]
    public string? VerifiedSchool { get; set; }

    [BsonElement("profile_Photo_Path")]
    public string? ProfilePhotoPath { get; set; }

    [BsonElement("rental_Verifications")]
    public List<rentalVerificationEntry> RentalVerifications { get; set; } = new List<rentalVerificationEntry>();

    [BsonElement("proof_Of_Stay_Uploads")]
    public List<proofOfStayEntry> ProofOfStayUploads { get; set; } = new List<proofOfStayEntry>();
}

public class rentalVerificationEntry
{
    [BsonElement("dormitory_Name")]
    public string DormitoryName { get; set; }

    [BsonElement("verified_At_Utc")]
    public DateTime VerifiedAtUtc { get; set; }
}

public class proofOfStayEntry
{
    [BsonElement("file_Path")]
    public string FilePath { get; set; }

    [BsonElement("uploaded_At_Utc")]
    public DateTime UploadedAtUtc { get; set; }
}