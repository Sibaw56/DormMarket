using System;
using MongoDB.Bson.Serialization.Attributes;

[BsonIgnoreExtraElements]
public class Reviews
{
    [BsonId]
    public int Id { get; set; }

    [BsonElement("dorm_Id")]
    public int DormId { get; set; }

    [BsonElement("student_Id")]
    public int StudentId { get; set; }

    [BsonElement("student_Name")]
    public string StudentName { get; set; }

    [BsonElement("rating")]
    public int Rating { get; set; }

    [BsonElement("review_Text")]
    public string ReviewText { get; set; }

    [BsonElement("photo_Path")]
    public string? PhotoPath { get; set; }

    [BsonElement("proof_Path")]
    public string? ProofPath { get; set; }

    [BsonElement("is_Verified_Renter")]
    public bool IsVerifiedRenter { get; set; }

    [BsonElement("created_At_Utc")]
    public DateTime CreatedAtUtc { get; set; }
}